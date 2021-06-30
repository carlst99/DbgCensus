using DbgCensus.EventStream.Abstractions.Objects;

namespace DbgCensus.EventStream.Objects
{
    /// <summary>
    /// The base object of a majority of Census event stream objects.
    /// </summary>
    public abstract record EventStreamObjectBase : IEventStreamObject
    {
        /// <inheritdoc />
        public string DispatchingClientName { get; set; }

        /// <summary>
        /// The websocket service that this object has been received from.
        /// </summary>
        public string Service { get; init; }

        /// <summary>
        /// The type of object.
        /// </summary>
        public string Type { get; init; }

        protected EventStreamObjectBase()
        {
            DispatchingClientName = string.Empty;
            Service = string.Empty;
            Type = string.Empty;
        }
    }
}
