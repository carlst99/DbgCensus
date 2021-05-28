namespace DbgCensus.Rest.Abstractions.Queries
{
    /// <summary>
    /// A factory for <see cref="IQueryBuilder"/> items.
    /// </summary>
    public interface IQueryBuilderFactory
    {
        /// <summary>
        /// Constructs an <see cref="IQueryBuilder"/> object
        /// </summary>
        /// <returns>An <see cref="IQueryBuilder"/> instance.</returns>
        IQueryBuilder Get();

        /// <summary>
        /// Constructs an <see cref="IQueryBuilder"/> object.
        /// </summary>
        /// <param name="options">The default options to use.</param>
        /// <returns>An <see cref="IQueryBuilder"/> instance.</returns>
        IQueryBuilder Get(CensusQueryOptions options);
    }
}
