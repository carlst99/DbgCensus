using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions;

/// <summary>
/// Represents a wrapper interface for the <see cref="IQueryBuilderFactory"/> and <see cref="ICensusRestClient"/> services.
/// </summary>
public interface IQueryService
{
    /// <inheritdoc cref="IQueryBuilderFactory.Get(CensusQueryOptions?)"/>
    IQueryBuilder CreateQuery(CensusQueryOptions? options = null);

    /// <inheritdoc cref="ICensusRestClient.GetAsync{T}(IQueryBuilder, CancellationToken)"/>
    Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.CountAsync(string, CancellationToken)"/>
    Task<ulong> CountAsync(string collectionName, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.DistinctAsync{T}(string, string, CancellationToken)"/>
    Task<IReadOnlyList<T>?> DistinctAsync<T>(string collectionName, string fieldName, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.GetPaginatedAsync{T}(IQueryBuilder, uint, uint, uint, CancellationToken)"/>
    IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>(IQueryBuilder query, uint pageSize, uint pageCount, uint start = 0, CancellationToken ct = default);
}
