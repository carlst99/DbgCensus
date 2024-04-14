using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions;

/// <summary>
/// Represents a wrapper interface for the <see cref="IQueryBuilderFactory"/>
/// and <see cref="ICensusRestClient"/> services.
/// </summary>
public interface IQueryService
{
    /// <inheritdoc cref="IQueryBuilderFactory.Get(CensusQueryOptions?)"/>
    IQueryBuilder CreateQuery(CensusQueryOptions? options = null);

    /// <inheritdoc cref="ICensusRestClient.GetAsync{T}(IQueryBuilder, CancellationToken)"/>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(ICensusRestClient.SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(ICensusRestClient.SerializationUnreferencedCodeMessage)]
    Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.GetAsync{T}(IQueryBuilder, JsonTypeInfo{T}, CancellationToken)" />
    Task<T?> GetAsync<T>(IQueryBuilder query, JsonTypeInfo<T> typeInfo, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.CountAsync(string, CancellationToken)"/>
    Task<ulong> CountAsync(string collectionName, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.CountAsync(IQueryBuilder, CancellationToken)"/>
    Task<ulong> CountAsync(IQueryBuilder query, CancellationToken ct = default);

    /// <inheritdoc cref="ICensusRestClient.DistinctAsync{T}(string, string, int, CancellationToken)"/>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(ICensusRestClient.SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(ICensusRestClient.SerializationUnreferencedCodeMessage)]
    Task<IReadOnlyList<T>?> DistinctAsync<T>
    (
        string collectionName,
        string fieldName,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    );

    /// <inheritdoc cref="ICensusRestClient.DistinctAsync{T, TElement}(string, string, JsonTypeInfo{T}, int, CancellationToken)"/>
    Task<T?> DistinctAsync<T, TElement>
    (
        string collectionName,
        string fieldName,
        JsonTypeInfo<T> typeInfo,
        int limit = ICensusRestClient.DistinctLimit,
        CancellationToken ct = default
    ) where T : IEnumerable<TElement>;

    /// <inheritdoc cref="ICensusRestClient.GetPaginatedAsync{T}(IQueryBuilder, int, int, int, CancellationToken)"/>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(ICensusRestClient.SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(ICensusRestClient.SerializationUnreferencedCodeMessage)]
    IAsyncEnumerable<IEnumerable<T>> GetPaginatedAsync<T>
    (
        IQueryBuilder query,
        int pageSize,
        int pageCount = int.MaxValue,
        int start = 0,
        CancellationToken ct = default
    );

    /// <inheritdoc cref="ICensusRestClient.GetPaginatedAsync{T, TElement}(IQueryBuilder, int, JsonTypeInfo{T}, int, int, CancellationToken)"/>
    IAsyncEnumerable<T> GetPaginatedAsync<T, TElement>
    (
        IQueryBuilder query,
        int pageSize,
        JsonTypeInfo<T> typeInfo,
        int pageCount = int.MaxValue,
        int start = 0,
        CancellationToken ct = default
    ) where T : IEnumerable<TElement>;
}
