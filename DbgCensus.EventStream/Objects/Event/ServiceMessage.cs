namespace DbgCensus.EventStream.Objects.Event
{
    /// <summary>
    /// An object sent by the event stream when an event occurs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record ServiceMessage<T> : EventStreamObjectBase
    {
        /// <summary>
        /// Gets the event object.
        /// </summary>
        public T Payload { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ServiceMessage{T}"/> record.
        /// </summary>
        /// <param name="payload">The event object.</param>
        public ServiceMessage(T payload)
            : base("event", "serviceMessage")
        {
            Payload = payload;
        }
    }
}
