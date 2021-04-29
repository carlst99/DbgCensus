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
        IQuery Get();
    }
}
