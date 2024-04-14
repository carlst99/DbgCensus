using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest;

/// <inheritdoc cref="IQueryService"/>
public class QueryService : IQueryService
{
    protected readonly ICensusRestClient _client;
    protected readonly IQueryBuilderFactory _queryFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryService"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="queryFactory">The query factory.</param>
    public QueryService(ICensusRestClient client, IQueryBuilderFactory queryFactory)
    {
        _client = client;
        _queryFactory = queryFactory;
    }

    /// <inheritdoc />
    public virtual IQueryBuilder CreateQuery(CensusQueryOptions? options = null)
        => _queryFactory.Get(options);

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(ICensusRestClient.SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(ICensusRestClient.SerializationUnreferencedCodeMessage)]
    public virtual async Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default)
        => await _client.GetAsync<T>(query, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<T?> GetAsync<T>
    (
        IQueryBuilder query,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct = default
    )
    {
        return await _client.GetAsync(query, typeInfo, ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<ulong> CountAsync(string collectionName, CancellationToken ct = default)
        => await _client.CountAsync(collectionName, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<ulong> CountAsync(IQueryBuilder query, CancellationToken ct = default)
        => await _client.CountAsync(query, ct).ConfigureAwait(false);

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(ICensusRestClient.SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(ICensusRestClient.SerializationUnreferencedCodeMessage)]
    public virtual async Task<IReadOnlyList<T>?> DistinctAsync<T>
    (
        string collectionName,
        string fieldName,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    ) => await _client.DistinctAsync<T>(collectionName, fieldName, limit, ct).ConfigureAwait(false);

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
        return await _client.DistinctAsync<T, TElement>(collectionName, fieldName, typeInfo, limit, ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(ICensusRestClient.SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(ICensusRestClient.SerializationUnreferencedCodeMessage)]
    public virtual IAsyncEnumerable<IEnumerable<T>> GetPaginatedAsync<T>
    (
        IQueryBuilder query,
        int pageSize,
        int pageCount = int.MaxValue,
        int start = 0,
        CancellationToken ct = default
    ) => _client.GetPaginatedAsync<T>(query, pageSize, pageCount, start, ct);

    /// <inheritdoc />
    public virtual IAsyncEnumerable<T> GetPaginatedAsync<T, TElement>
    (
        IQueryBuilder query,
        int pageSize,
        JsonTypeInfo<T> typeInfo,
        int pageCount = int.MaxValue,
        int start = 0,
        CancellationToken ct = default
    ) where T : IEnumerable<TElement>
    {
        return _client.GetPaginatedAsync<T, TElement>(query, pageSize, typeInfo, pageCount, start, ct);
    }
}
