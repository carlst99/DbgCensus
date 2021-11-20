﻿using DbgCensus.Core.Exceptions;
using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest;

/// <inheritdoc cref="ICensusRestClient" />
public class CensusRestClient : ICensusRestClient
{
    protected readonly ILogger<CensusRestClient> _logger;
    protected readonly HttpClient _client;
    protected readonly CensusQueryOptions _queryOptions;
    protected readonly IQueryBuilderFactory _queryFactory;
    protected readonly JsonSerializerOptions _jsonOptions;

    public bool IsDisposed { get; protected set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="CensusRestClient"/> class.
    /// </summary>
    /// <param name="logger">The logging interface to use.</param>
    /// <param name="client">The <see cref="HttpClient"/> to send requests with.</param>
    /// <param name="options">The query options to conform to.</param>
    /// <param name="queryFactory">The query factory.</param>
    public CensusRestClient
    (
        ILogger<CensusRestClient> logger,
        HttpClient client,
        IOptions<CensusQueryOptions> options,
        IQueryBuilderFactory queryFactory
    )
    {
        _logger = logger;
        _client = client;
        _queryOptions = options.Value;
        _queryFactory = queryFactory;

        _jsonOptions = new JsonSerializerOptions(_queryOptions.DeserializationOptions);
        _jsonOptions.AddCensusDeserializationOptions();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default)
        => await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri, query.CollectionName, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>(string query, string? collectionName, CancellationToken ct = default)
    {
        _logger.LogTrace("Performing Census GET request with query: {query}", query);

        using HttpResponseMessage response = await PerformQueryAsync(query, ct).ConfigureAwait(false);

        return await DeserializeResponseContentAsync<T>(response.Content, collectionName, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<ulong> CountAsync(string collectionName, CancellationToken ct = default)
    {
        IQueryBuilder query = _queryFactory.Get()
            .OnCollection(collectionName)
            .OfQueryType(QueryType.Count);

        return await GetAsync<ulong>(query, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>?> DistinctAsync<T>(string collectionName, string fieldName, CancellationToken ct = default)
    {
        IQueryBuilder query = _queryFactory.Get()
            .OnCollection(collectionName)
            .WithDistinctFieldValues(fieldName);

        using HttpResponseMessage response = await PerformQueryAsync(query.ConstructEndpoint().AbsoluteUri, ct).ConfigureAwait(false);
        using JsonDocument data = await InitialiseParseAsync(response.Content, ct).ConfigureAwait(false);
        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);

        if (!collectionElement[0].TryGetProperty(fieldName, out JsonElement fieldElement))
        {
            _logger.LogWarning("Returned data was not in the expected format for a distinct query", data.RootElement.GetRawText());
            throw new CensusInvalidDataException("Returned data was not in the expected format for a distinct query.", data.RootElement.GetRawText());
        }

        return fieldElement.Deserialize<List<T>>(_jsonOptions);
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>(IQueryBuilder query, uint pageSize, uint pageCount, uint start = 0, [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 0; i < pageCount; i++)
        {
            if (ct.IsCancellationRequested)
                throw new TaskCanceledException();

            query.WithStartIndex(start);
            query.WithLimit(pageSize);

            yield return await GetAsync<List<T>>(query, ct).ConfigureAwait(false);

            start += pageSize;
        }
    }

    protected virtual async Task<HttpResponseMessage> PerformQueryAsync(string query, CancellationToken ct)
    {
        _logger.LogTrace("Performing GET request with query: {query}", query);

        HttpResponseMessage response = await _client.GetAsync(query, ct).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Census GET request failed with status code {status} and reason {reason}", response.StatusCode, response.ReasonPhrase);
            throw new CensusServiceUnavailableException();
        }

        return response;
    }

    /// <summary>
    /// Performs checks on the returned data and deserializes it.
    /// </summary>
    /// <typeparam name="T">The POCO type to deserialize to.</typeparam>
    /// <param name="content">The result of the REST request.</param>
    /// <param name="collectionName">The name of the collection that was queried.</param>
    /// <param name="ct">A token which can be used to cancel asynchronous logic.</param>
    /// <returns></returns>
    protected virtual async Task<T?> DeserializeResponseContentAsync<T>(HttpContent content, string? collectionName, CancellationToken ct)
    {
        using JsonDocument data = await InitialiseParseAsync(content, ct).ConfigureAwait(false);

        if (collectionName is null)
            collectionName = "datatype";

        if (data.RootElement.TryGetProperty("count", out JsonElement countElement))
            return countElement.Deserialize<T>(_jsonOptions);

        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);

        if (typeof(T).IsAssignableTo(typeof(System.Collections.IEnumerable)))
            return JsonSerializer.Deserialize<T>(collectionElement.GetRawText(), _jsonOptions);

        int length = collectionElement.GetArrayLength();
        if (length > 1)
            throw new JsonException("You are trying to deserialise to a single object, but the Census data contained more than one entity.");
        else if (length == 0)
            return default;
        else
            return JsonSerializer.Deserialize<T>(collectionElement[0].GetRawText(), _jsonOptions);
    }

    /// <summary>
    /// Parses and then attempts to find an error in the Census response.
    /// </summary>
    /// <param name="responseContent">The response content.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    protected virtual async Task<JsonDocument> InitialiseParseAsync(HttpContent responseContent, CancellationToken ct)
    {
        JsonDocument data = await JsonDocument.ParseAsync
        (
            await responseContent.ReadAsStreamAsync(ct).ConfigureAwait(false),
            cancellationToken: ct
        ).ConfigureAwait(false);

        JsonValueKind dataKind = data.RootElement.ValueKind;
        if (dataKind == JsonValueKind.Null || dataKind == JsonValueKind.Undefined)
            throw new CensusInvalidDataException("No data was returned.", null);

        if (data.RootElement.TryGetProperty("error", out JsonElement errorValue))
        {
            if (errorValue.GetRawText() == "service_unavailable")
            {
                _logger.LogWarning("Census service unavailable.");
                throw new CensusServiceUnavailableException();
            }
            else
            {
                _logger.LogError("Census query failed with an error: {error}", errorValue.GetRawText());
                throw new CensusQueryErrorException(errorValue.GetRawText());
            }
        }

        if (data.RootElement.TryGetProperty("errorCode", out JsonElement errorCode))
        {
            if (data.RootElement.TryGetProperty("errorMessage", out errorValue))
                _logger.LogError("Census query failed with error code {code} and message {message}", errorCode.GetRawText(), errorValue.GetRawText());
            else
                _logger.LogError("Census query failed with error code: {code}", errorCode.GetRawText());

            throw new CensusQueryErrorException(errorValue.GetRawText(), errorCode.GetRawText());
        }

        return data;
    }

    protected virtual JsonElement GetCollectionArrayElement(JsonElement rootElement, string collectionName)
    {
        if (!rootElement.TryGetProperty(collectionName + "_list", out JsonElement collectionElement))
        {
            _logger.LogWarning("Returned data was not in the expected format: {data}", rootElement.GetRawText());
            throw new CensusInvalidDataException("Returned data was not in the expected format.", rootElement.GetRawText());
        }

        if (collectionElement.ValueKind != JsonValueKind.Array)
            throw new CensusInvalidDataException("The Census result was expected to contain an array of data.", rootElement.GetRawText());

        return collectionElement;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                _client.Dispose();
            }

            IsDisposed = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
