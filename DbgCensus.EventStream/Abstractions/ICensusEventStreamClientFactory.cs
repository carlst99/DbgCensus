namespace DbgCensus.EventStream.Abstractions
{
    /// <summary>
    /// A factory to create or retrieve <see cref="ICensusEventStreamClient"/> instances with a given name.
    /// </summary>
    public interface ICensusEventStreamClientFactory
    {
        /// <summary>
        /// Gets an instance of an <see cref="ICensusEventStreamClient"/> with the given name. If the instance is not cached, it will be created.
        /// </summary>
        /// <param name="name">The name of the instance to retrieve.</param>
        /// <returns>An <see cref="ICensusEventStreamClient"/> instance.</returns>
        ICensusEventStreamClient GetClient(string name, CensusEventStreamOptions? options = default);

        /// <summary>
        /// Gets a new instance of an <see cref="ICensusEventStreamClient"/>.
        /// </summary>
        /// <returns>An <see cref="ICensusEventStreamClient"/> instance.</returns>
        ICensusEventStreamClient GetClient(CensusEventStreamOptions? options = default);

        /// <summary>
        /// Gets an instance of an <see cref="ICensusEventStreamClient"/> dedicated for a given consumer.
        /// </summary>
        /// <typeparam name="T">The type that will consume the <see cref="ICensusEventStreamClient"/> instance.</typeparam>
        /// <returns>An <see cref="ICensusEventStreamClient"/> instance.</returns>
        ICensusEventStreamClient GetClient<T>(CensusEventStreamOptions? options = default);
    }
}
