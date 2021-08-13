namespace DbgCensus.EventStream.Abstractions
{
    /// <summary>
    /// A factory to create or retrieve <see cref="IEventStreamClient"/> instances with a given name.
    /// </summary>
    public interface IEventStreamClientFactory
    {
        /// <summary>
        /// Gets an instance of an <see cref="IEventStreamClient"/> with the given name. If the instance is not cached, it will be created.
        /// </summary>
        /// <param name="name">The name of the instance to retrieve.</param>
        /// <returns>An <see cref="IEventStreamClient"/> instance.</returns>
        IEventStreamClient GetClient(string name, EventStreamOptions? options = default);

        /// <summary>
        /// Gets a new instance of an <see cref="IEventStreamClient"/>.
        /// </summary>
        /// <returns>An <see cref="IEventStreamClient"/> instance.</returns>
        IEventStreamClient GetClient(EventStreamOptions? options = default);

        /// <summary>
        /// Gets an instance of an <see cref="IEventStreamClient"/> dedicated for a given consumer.
        /// </summary>
        /// <typeparam name="T">The type that will consume the <see cref="IEventStreamClient"/> instance.</typeparam>
        /// <returns>An <see cref="IEventStreamClient"/> instance.</returns>
        IEventStreamClient GetClient<T>(EventStreamOptions? options = default);
    }
}
