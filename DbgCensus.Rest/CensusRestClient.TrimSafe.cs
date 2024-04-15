using DbgCensus.Core.Exceptions;
using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest;

public partial class CensusRestClient
{
    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>
    (
        IQueryBuilder query,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct = default
    )
    {
        return await GetAsync(query.ConstructEndpoint().AbsoluteUri, query.CollectionName, typeInfo, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>
    (
        IQueryBuilder query,
        JsonSerializerContext jsonContext,
        CancellationToken ct = default
    )
    {
        return await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri, query.CollectionName, jsonContext, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>
    (
        string query,
        string? collectionName,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct = default
    )
    {
        _logger.LogTrace("Performing Census GET request with query: {Query}", query);

        using HttpResponseMessage response = await PerformQueryAsync(query, ct).ConfigureAwait(false);

        return await DeserializeResponseContentAsync(response.Content, collectionName, typeInfo, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>
    (
        string query,
        string? collectionName,
        JsonSerializerContext jsonContext,
        CancellationToken ct = default
    )
    {
        _logger.LogTrace("Performing Census GET request with query: {Query}", query);

        using HttpResponseMessage response = await PerformQueryAsync(query, ct).ConfigureAwait(false);

        return await DeserializeResponseContentAsync<T>(response.Content, collectionName, jsonContext, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<T?> DistinctAsync<T, TElement>
    (
        string collectionName,
        string fieldName,
        JsonTypeInfo<T> typeInfo,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    ) where T : IEnumerable<TElement>
    {
        if (limit is > ICensusRestClient.DistinctLimit or < 0)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(limit),
                limit,
                $"The limit may not be larger than {ICensusRestClient.DistinctLimit} or smaller than 0."
            );
        }

        IQueryBuilder query = _queryFactory.Get()
            .OnCollection(collectionName)
            .WithLimit(limit)
            .WithDistinctFieldValues(fieldName);

        using HttpResponseMessage response = await PerformQueryAsync(query.ConstructEndpoint().AbsoluteUri, ct).ConfigureAwait(false);
        using JsonDocument data = await InitialParseAsync(response.Content, ct).ConfigureAwait(false);
        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);

        if (collectionElement[0].TryGetProperty(fieldName, out JsonElement fieldElement))
            return fieldElement.Deserialize(typeInfo);

        string rawJson = data.RootElement.GetRawText();

        _logger.LogWarning("Returned data was not in the expected format for a distinct query: {RawJson}", rawJson);
        throw new CensusInvalidDataException("Returned data was not in the expected format for a distinct query.", rawJson);
    }

    /// <inheritdoc />
    public virtual async IAsyncEnumerable<T> GetPaginatedAsync<T, TElement>
    (
        IQueryBuilder query,
        int pageSize,
        JsonTypeInfo<T> typeInfo,
        int pageCount = int.MaxValue,
        int start = 0,
        [EnumeratorCancellation] CancellationToken ct = default
    ) where T : IEnumerable<TElement>
    {
        for (int i = 0; i < pageCount; i++)
        {
            if (ct.IsCancellationRequested)
                throw new TaskCanceledException();

            query.WithStartIndex(start);
            query.WithLimitPerDatabase(pageSize);

            T? results = await GetAsync(query, typeInfo, ct).ConfigureAwait(false);
            if (results is null)
                break;

            yield return results;

            if (results.Count() < pageSize)
                break;

            start += pageSize;
        }
    }

    /// <summary>
    /// Performs checks on the returned data and deserializes it.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="content">The result of the REST request.</param>
    /// <param name="collectionName">The name of the collection that was queried.</param>
    /// <param name="typeInfo">The JSON type info to deserialize into.</param>
    /// <param name="ct">A token which can be used to cancel asynchronous logic.</param>
    /// <returns>The deserialized value.</returns>
    protected virtual async Task<T?> DeserializeResponseContentAsync<T>
    (
        HttpContent content,
        string? collectionName,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct
    )
    {
        using JsonDocument data = await InitialParseAsync(content, ct).ConfigureAwait(false);
        collectionName ??= "datatype";

        // Shortcut early for count, as it is not nested in the collection array
        if (data.RootElement.TryGetProperty("count", out JsonElement countElement))
            return countElement.Deserialize(typeInfo);

        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);
        int length = collectionElement.GetArrayLength();

        if (typeof(T) == typeof(JsonDocument))
            return collectionElement.Deserialize(typeInfo);

        return length switch
        {
            0 => default,
            1 when !typeof(T).IsAssignableTo(typeof(IEnumerable)) => collectionElement[0].Deserialize(typeInfo),
            _ => collectionElement.Deserialize(typeInfo)
        };
    }

    /// <summary>
    /// Performs checks on the returned data and deserializes it.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="content">The result of the REST request.</param>
    /// <param name="collectionName">The name of the collection that was queried.</param>
    /// <param name="jsonContext">The JSON serializer context use.</param>
    /// <param name="ct">A token which can be used to cancel asynchronous logic.</param>
    /// <returns>The deserialized value.</returns>
    protected virtual async Task<T?> DeserializeResponseContentAsync<T>
    (
        HttpContent content,
        string? collectionName,
        JsonSerializerContext jsonContext,
        CancellationToken ct
    )
    {
        using JsonDocument data = await InitialParseAsync(content, ct).ConfigureAwait(false);
        collectionName ??= "datatype";

        // Shortcut early for count, as it is not nested in the collection array
        if (data.RootElement.TryGetProperty("count", out JsonElement countElement))
            return (T?)countElement.Deserialize(typeof(T), jsonContext);

        JsonElement collectionElement = GetCollectionArrayElement(data.RootElement, collectionName);
        int length = collectionElement.GetArrayLength();

        if (typeof(T) == typeof(JsonDocument))
            return (T?)collectionElement.Deserialize(typeof(JsonDocument), jsonContext);

        return length switch
        {
            0 => default,
            1 when !typeof(T).IsAssignableTo(typeof(IEnumerable))
                => (T?)collectionElement[0].Deserialize(typeof(T), jsonContext),
            _ => (T?)collectionElement.Deserialize(typeof(T), jsonContext)
        };
    }
}
