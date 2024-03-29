﻿using DbgCensus.Rest.Abstractions.Queries;
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

    /// <inheritdoc cref="ICensusRestClient.CountAsync(IQueryBuilder, CancellationToken)"/>
    Task<ulong> CountAsync(IQueryBuilder query, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.DistinctAsync{T}(string, string, int, CancellationToken)"/>
    Task<IReadOnlyList<T>?> DistinctAsync<T>
    (
        string collectionName,
        string fieldName,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    );

    /// <inheritdoc cref="ICensusRestClient.GetPaginatedAsync{T}(IQueryBuilder, int, int, int, CancellationToken)"/>
    IAsyncEnumerable<IEnumerable<T>> GetPaginatedAsync<T>
    (
        IQueryBuilder query,
        int pageSize,
        int pageCount = int.MaxValue,
        int start = 0,
        CancellationToken ct = default
    );
}
