using DbgCensus.Core.Exceptions;
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
    protected readonly IQueryBuilderFactory _queryFactory;
    protected readonly JsonSerializerOptions _jsonOptions;

    public bool IsDisposed { get; protected set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="CensusRestClient"/> class.
    /// </summary>
    /// <param name="logger">The logging interface to use.</param>
    /// <param name="client">The <see cref="HttpClient"/> to send requests with.</param>
    /// <param name="jsonSerializerOptions">The JSON serializer options.</param>
    /// <param name="queryFactory">The query factory.</param>
    public CensusRestClient
    (
        ILogger<CensusRestClient> logger,
        HttpClient client,
        IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
        IQueryBuilderFactory queryFactory
    )
    {
        _logger = logger;
        _client = client;
        _queryFactory = queryFactory;
        _jsonOptions = jsonSerializerOptions.Get(Constants.JsonDeserializationOptionsName);
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default)
        => await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri, query.CollectionName, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>
    (
        string query,
        string? collectionName,
        CancellationToken ct = default
    )
    {
        _logger.LogTrace("Performing Census GET request with query: {Query}", query);

        using HttpResponseMessage response = await PerformQueryAsync(query, ct).ConfigureAwait(false);

        return await DeserializeResponseContentAsync<T>(response.Content, collectionName, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<ulong> CountAsync(string collectionName, CancellationToken ct = default)
    {
        IQueryBuilder query = _queryFactory.Get()
            .OnCollection(collectionName)
            .OfQueryType(QueryType.Count);

        return await CountAsync(query, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<ulong> CountAsync(IQueryBuilder query, CancellationToken ct = default)
    {
        query.OfQueryType(QueryType.Count);
        return await GetAsync<ulong>(query, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>?> DistinctAsync<T>
    (
        string collectionName,
        string fieldName,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    )
    {
        if (limit is > ICensusRestClient.DistinctLimit or < 0)
            throw new ArgumentOutOfRangeException(nameof(limit), limit, $"The limit may not be larger than {ICensusRestClient.DistinctLimit} or smaller than 0.");

        IQueryBuilder query = _queryFactory.Get()
            .OnCollection(collectionName)
            .WithLimit(limit)
            .WithDistinctFieldValues(fieldName);

        using HttpResponseMessage response = await PerformQueryAsync(query.ConstructEndpoint().AbsoluteUri, ct).ConfigureAwait(false);
        using JsonDocument data = await InitialParseAsync(response.Content, ct).ConfigureAwait(false);
        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);

        if (collectionElement[0].TryGetProperty(fieldName, out JsonElement fieldElement))
            return fieldElement.Deserialize<List<T>>(_jsonOptions);

        string rawJson = data.RootElement.GetRawText();

        _logger.LogWarning("Returned data was not in the expected format for a distinct query: {RawJson}", rawJson);
        throw new CensusInvalidDataException("Returned data was not in the expected format for a distinct query.", rawJson);
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<IEnumerable<T>> GetPaginatedAsync<T>
    (
        IQueryBuilder query,
        int pageSize,
        int pageCount = int.MaxValue,
        int start = 0,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        for (int i = 0; i < pageCount; i++)
        {
            if (ct.IsCancellationRequested)
                throw new TaskCanceledException();

            query.WithStartIndex(start);
            query.WithLimit(pageSize);

            List<T>? results = await GetAsync<List<T>>(query, ct).ConfigureAwait(false);
            if (results is null)
                break;

            yield return results;

            if (results.Count < pageSize)
                break;

            start += pageSize;
        }
    }

    protected virtual async Task<HttpResponseMessage> PerformQueryAsync(string query, CancellationToken ct)
    {
        _logger.LogTrace("Performing GET request with query: {Query}", query);

        HttpResponseMessage response = await _client.GetAsync(query, ct).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
            return response;

        _logger.LogError("Census GET request failed with status code {Status} and reason {Reason}", response.StatusCode, response.ReasonPhrase);
        throw new CensusServiceUnavailableException();
    }

    /// <summary>
    /// Performs checks on the returned data and deserializes it.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="content">The result of the REST request.</param>
    /// <param name="collectionName">The name of the collection that was queried.</param>
    /// <param name="ct">A token which can be used to cancel asynchronous logic.</param>
    /// <returns>The deserialized value.</returns>
    protected virtual async Task<T?> DeserializeResponseContentAsync<T>
    (
        HttpContent content,
        string? collectionName,
        CancellationToken ct
    )
    {
        using JsonDocument data = await InitialParseAsync(content, ct).ConfigureAwait(false);
        collectionName ??= "datatype";

        if (data.RootElement.TryGetProperty("count", out JsonElement countElement))
            return countElement.Deserialize<T>(_jsonOptions);

        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);
        int length = collectionElement.GetArrayLength();

        if (typeof(T) == typeof(JsonDocument))
            return collectionElement.Deserialize<T>(_jsonOptions);

        return length switch
        {
            0 => default,
            1 when !typeof(T).IsAssignableTo(typeof(System.Collections.IEnumerable)) => collectionElement[0].Deserialize<T>(_jsonOptions),
            _ => collectionElement.Deserialize<T>(_jsonOptions)
        };
    }

    /// <summary>
    /// Parses and then attempts to find an error in the Census response.
    /// </summary>
    /// <param name="responseContent">The response content.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    protected virtual async Task<JsonDocument> InitialParseAsync(HttpContent responseContent, CancellationToken ct)
    {
        JsonDocument data = await JsonDocument.ParseAsync
        (
            await responseContent.ReadAsStreamAsync(ct).ConfigureAwait(false),
            cancellationToken: ct
        ).ConfigureAwait(false);

        JsonValueKind dataKind = data.RootElement.ValueKind;
        if (dataKind is JsonValueKind.Null or JsonValueKind.Undefined)
            throw new CensusInvalidDataException("No data was returned.", null);

        if (data.RootElement.TryGetProperty("error", out JsonElement errorValue))
        {
            string errorValueText = errorValue.GetRawText();

            if (errorValueText == "service_unavailable")
            {
                _logger.LogWarning("Census service unavailable");
                throw new CensusServiceUnavailableException();
            }

            _logger.LogError("Census query failed with an error: {Error}", errorValueText);
            throw new CensusQueryErrorException(errorValueText);
        }

        if (data.RootElement.TryGetProperty("errorCode", out JsonElement errorCode))
        {
            string errorCodeText = errorCode.GetRawText();
            string errorValueText = string.Empty;

            if (data.RootElement.TryGetProperty("errorMessage", out errorValue))
                errorValueText = errorValue.GetRawText();

            _logger.LogError("Census query failed with error code {Code} and message {Message}", errorCodeText, errorValueText);

            throw new CensusQueryErrorException(errorValueText, errorCodeText);
        }

        return data;
    }

    protected JsonElement GetCollectionArrayElement(JsonElement rootElement, string collectionName)
    {
        if (!rootElement.TryGetProperty(collectionName + "_list", out JsonElement collectionElement))
        {
            string rawJson = rootElement.GetRawText();

            _logger.LogWarning("Returned data was not in the expected format: {Data}", rawJson);
            throw new CensusInvalidDataException("Returned data was not in the expected format.", rawJson);
        }

        if (collectionElement.ValueKind is not JsonValueKind.Array)
            throw new CensusInvalidDataException("The Census result was expected to contain an array of data.", rootElement.GetRawText());

        return collectionElement;
    }
}
