using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions;

/// <summary>
/// Provides functions to perform queries on the Census REST API, and deserialise the responses.
/// </summary>
public interface ICensusRestClient : IDisposable
{
    /// <summary>
    /// Performs a query on the Census REST API.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>The deserialised response, or null if no value was returned.</returns>
    Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default);

    /// <summary>
    /// Performs a query on the Census REST API.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="collectionName">The collection that the query will be performed on.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>The deserialised response, or null if no value was returned.</returns>
    Task<T?> GetAsync<T>(string query, string? collectionName, CancellationToken ct = default);

    /// <summary>
    /// Performs a query on the Census REST API, returning the results in pages.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="pageSize">The number of objects to return with each page.</param>
    /// <param name="pageCount">The number of pages to get.</param>
    /// <param name="start">The index at which to start getting objects from.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields enumerables for each page.</returns>
    IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>(IQueryBuilder query, uint pageSize, uint pageCount, uint start = 0, CancellationToken ct = default);
}
