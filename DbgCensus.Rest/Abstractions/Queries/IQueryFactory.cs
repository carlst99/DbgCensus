namespace DbgCensus.Rest.Abstractions.Queries
{
    /// <summary>
    /// A factory for <see cref="IQuery"/> items.
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Constructs an <see cref="IQuery"/> object
        /// </summary>
        /// <returns>An <see cref="IQuery"/> instance.</returns>
        IQuery Get();

        /// <summary>
        /// Constructs an <see cref="IQuery"/> object.
        /// </summary>
        /// <param name="options">The default options to use.</param>
        /// <returns>An <see cref="IQuery"/> instance.</returns>
        IQuery Get(CensusQueryOptions options);
    }
}
