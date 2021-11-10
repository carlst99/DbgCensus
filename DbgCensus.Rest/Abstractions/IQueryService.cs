using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions;

/// <summary>
/// Defines a wrapper interface for the <see cref="IQueryBuilderFactory"/> and <see cref="ICensusRestClient"/> services.
/// </summary>
public interface IQueryService
{
    /// <inheritdoc cref="IQueryBuilderFactory.Get(CensusQueryOptions?)"/>
    IQueryBuilder CreateQuery(CensusQueryOptions? options = null);

    /// <inheritdoc cref="ICensusRestClient.GetAsync{T}(IQueryBuilder, CancellationToken)"/>
    Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.GetPaginatedAsync{T}(IQueryBuilder, uint, uint, uint, CancellationToken)"/>
    IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>(IQueryBuilder query, uint pageSize, uint pageCount, uint start = 0, CancellationToken ct = default);
}
