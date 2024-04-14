using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions;

/// <summary>
/// Provides functions to perform queries on the Census REST API, and deserialize the responses.
/// </summary>
public interface ICensusRestClient
{
    // Copied these messages straight from source.dot.net
    internal const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    internal const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";

    /// <summary>
    /// Gets the maximum number of elements that may be returned in a query
    /// that uses the c:distinct command. This limit is enforced by Census.
    /// </summary>
    internal const int DistinctLimit = 20000;

    /// <summary>
    /// Performs a query on the Census REST API.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>The deserialized response, or null if no value was returned.</returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default);

    /// <summary>
    /// Performs a query on the Census REST API.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="typeInfo">The JSON type info to deserialize into.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>The deserialized response, or null if no value was returned.</returns>
    Task<T?> GetAsync<T>(IQueryBuilder query, JsonTypeInfo<T> typeInfo, CancellationToken ct = default);

    /// <summary>
    /// Performs a query on the Census REST API.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="collectionName">The collection that the query will be performed on.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>The deserialized response, or null if no value was returned.</returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    Task<T?> GetAsync<T>
    (
        string query,
        string? collectionName,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a query on the Census REST API.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="collectionName">The collection that the query will be performed on.</param>
    /// <param name="typeInfo">The JSON type info to deserialize into.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>The deserialized response, or null if no value was returned.</returns>
    Task<T?> GetAsync<T>
    (
        string query,
        string? collectionName,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a query using the COUNT verb to retrieve the number of elements in the given collection.
    /// </summary>
    /// <param name="collectionName">The name of the collection.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>The number of elements in the collection.</returns>
    Task<ulong> CountAsync(string collectionName, CancellationToken ct = default);

    /// <summary>
    /// Performs a query using the COUNT verb to retrieve the number of elements returned by the given query.
    /// </summary>
    /// <param name="query">The query to perform.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>The number of elements returned by the query.</returns>
    Task<ulong> CountAsync(IQueryBuilder query, CancellationToken ct = default);

    /// <summary>
    /// Gets all the distinct values of a collection field. Limited by Census to 20000 results.
    /// </summary>
    /// <typeparam name="T">The type of the field.</typeparam>
    /// <param name="collectionName">The collection.</param>
    /// <param name="fieldName">The field.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A list of all the distinct values.</returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    Task<IReadOnlyList<T>?> DistinctAsync<T>
    (
        string collectionName,
        string fieldName,
        int limit = DistinctLimit,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets all the distinct values of a collection field. Limited by Census to 20000 results.
    /// </summary>
    /// <typeparam name="T">The type of the enumerable.</typeparam>
    /// <typeparam name="TElement">The type of element of the enumerable (i.e. the distinct field).</typeparam>
    /// <param name="collectionName">The collection.</param>
    /// <param name="fieldName">The field.</param>
    /// <param name="typeInfo">
    /// The JSON type info to deserialize into. Must represent an <see cref="IEnumerable{T}"/> type.
    /// </param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A list of all the distinct values.</returns>
    Task<T?> DistinctAsync<T, TElement>
    (
        string collectionName,
        string fieldName,
        JsonTypeInfo<T> typeInfo,
        int limit = DistinctLimit,
        CancellationToken ct = default
    ) where T : IEnumerable<TElement>;

    /// <summary>
    /// Performs a query on the Census REST API, returning the results in pages.
    /// </summary>
    /// <typeparam name="T">The type to deserialise the response to.</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="pageSize">The number of objects to return with each page.</param>
    /// <param name="pageCount">The number of pages to get.</param>
    /// <param name="start">The index at which to start getting objects from.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields an enumerable for each page.</returns>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
#endif
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    IAsyncEnumerable<IEnumerable<T>> GetPaginatedAsync<T>
    (
        IQueryBuilder query,
        int pageSize,
        int pageCount = int.MaxValue,
        int start = 0,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a query on the Census REST API, returning the results in pages.
    /// </summary>
    /// <typeparam name="T">The enumerable type to deserialise the pages to.</typeparam>
    /// <typeparam name="TElement">The type of element of the enumerable (i.e. the collection type).</typeparam>
    /// <param name="query">The query to perform.</param>
    /// <param name="pageSize">The number of objects to return with each page.</param>
    /// <param name="typeInfo">The JSON type info to deserialize into.</param>
    /// <param name="pageCount">The number of pages to get.</param>
    /// <param name="start">The index at which to start getting objects from.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields an enumerable for each page.</returns>
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
