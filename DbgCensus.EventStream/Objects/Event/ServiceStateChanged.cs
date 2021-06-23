namespace DbgCensus.EventStream.Objects.Event
{
    /// <summary>
    /// An object sent by the event stream when the state of a service changes.
    /// </summary>
    public record ServiceStateChanged : EventStreamObjectBase
    {
        /// <summary>
        /// Gets the identifier of the particular component within the service that has changed state.
        /// </summary>
        public string Detail { get; init; }

        /// <summary>
        /// Gets a value indicating if this component of the service is online.
        /// </summary>
        public bool Online { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ServiceStateChanged"/> record.
        /// </summary>
        /// <param name="detail">The identifier of the particular component within the service that has changed state.</param>
        /// <param name="online">A value indicating if this component is online.</param>
        public ServiceStateChanged(string detail, bool online)
            : base("event", "serviceStateChanged")
        {
            Detail = detail;
            Online = online;
        }
    }
}
