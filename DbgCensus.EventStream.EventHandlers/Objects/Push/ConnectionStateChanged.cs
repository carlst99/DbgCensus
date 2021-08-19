namespace DbgCensus.EventStream.EventHandlers.Objects.Push
{
    /// <summary>
    /// An object sent by the event stream when the state of the connection changes.
    /// </summary>
    public record ConnectionStateChanged : EventStreamObjectBase
    {
        /// <summary>
        /// Gets a value indicating if the event stream websocket is still open.
        /// </summary>
        public bool Connected { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ConnectionStateChanged"/> record.
        /// </summary>
        public ConnectionStateChanged()
        {
            Connected = false;
        }
    }
}
