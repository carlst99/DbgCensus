namespace DbgCensus.EventStream.Objects.Push
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
        /// <param name="connected">A value indicating if the event stream websocket is still open.</param>
        public ConnectionStateChanged(bool connected)
            : base("push", "connectionStateChanged")
        {
            Connected = connected;
        }
    }
}
