namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// Represents a factory for retrieving <see cref="IQueryBuilder"/> instances.
/// </summary>
public interface IQueryBuilderFactory
{
    /// <summary>
    /// Constructs an <see cref="IQueryBuilder"/> instance.
    /// </summary>
    /// <param name="options">Overriding options to apply instead of the factory defaults.</param>
    /// <returns>An <see cref="IQueryBuilder"/> instance.</returns>
    IQueryBuilder Get(CensusQueryOptions? options = null);

    /// <summary>
    /// Constructs an <see cref="IQueryBuilder"/> instance.
    /// </summary>
    /// <param name="onCollection">The collection to perform the query on.</param>
    /// <param name="options">Overriding options to apply instead of the factory defaults.</param>
    /// <returns>An <see cref="IQueryBuilder"/> instance.</returns>
    IQueryBuilder Get(string onCollection, CensusQueryOptions? options = null);
}
