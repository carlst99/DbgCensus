namespace DbgCensus.Rest.Abstractions.Queries
{
    /// <summary>
    /// A factory for <see cref="IQueryBuilder"/> items.
    /// </summary>
    public interface IQueryBuilderFactory
    {
        /// <summary>
        /// Constructs an <see cref="IQueryBuilder"/> object.
        /// </summary>
        /// <param name="options">Provide an <see cref="CensusQueryOptions"/> object to override the default injected options.</param>
        /// <returns>An <see cref="IQueryBuilder"/> instance.</returns>
        IQueryBuilder Get(CensusQueryOptions? options = null);
    }
}
