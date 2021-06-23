using System.Collections.Generic;

namespace DbgCensus.EventStream.Objects.Event
{
    /// <summary>
    /// A heartbeat object sent by the event stream, to keep the connection alive.
    /// </summary>
    public record Heartbeat : EventStreamObjectBase
    {
        /// <summary>
        /// Gets the event server endpoints (i.e. worlds) that are online.
        /// </summary>
        public IEnumerable<string> Online { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Heartbeat"/> record.
        /// </summary>
        /// <param name="online">The event server endpoints (i.e. worlds) that are online.</param>
        public Heartbeat(IEnumerable<string> online)
            : base("event", "heartbeat")
        {
            Online = online;
        }
    }
}
