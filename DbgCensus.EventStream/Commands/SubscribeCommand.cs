using System.Collections.Generic;

namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Creates a subscription to the Census event stream.
    /// </summary>
    public record SubscribeCommand : CensusCommandBase
    {
        /// <summary>
        /// Gets the characters to subscribe to.
        /// </summary>
        public IEnumerable<string>? Characters { get; init; }

        /// <summary>
        /// Gets the events to subscribe to.
        /// </summary>
        public IEnumerable<string>? EventNames { get; init; }

        /// <summary>
        /// Gets the worlds to subscribe to.
        /// </summary>
        public IEnumerable<string>? Worlds { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="SubscribeCommand"/> record.
        /// </summary>
        /// <param name="characters">The characters to subscribe to.</param>
        /// <param name="eventNames">The events to subscribe to.</param>
        /// <param name="worlds">The worlds to subscribe to.</param>
        public SubscribeCommand(IEnumerable<string>? characters = default, IEnumerable<string>? eventNames = default, IEnumerable<string>? worlds = default)
            : base("subscribe", "event")
        {
            Characters = characters;
            EventNames = eventNames;
            Worlds = worlds;
        }
    }
}
