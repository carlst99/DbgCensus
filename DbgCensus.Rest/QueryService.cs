using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
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
    public virtual async Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default)
        => await _client.GetAsync<T>(query, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<ulong> CountAsync(string collectionName, CancellationToken ct = default)
        => await _client.CountAsync(collectionName, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>?> DistinctAsync<T>
    (
        string collectionName,
        string fieldName,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    )
        => await _client.DistinctAsync<T>(collectionName, fieldName, limit, ct).ConfigureAwait(false);

    /// <inheritdoc />
    public virtual IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>
    (
        IQueryBuilder query,
        int pageSize,
        int pageCount,
        int start = 0,
        CancellationToken ct = default
    )
        => _client.GetPaginatedAsync<T>(query, pageSize, pageCount, start, ct);
}
