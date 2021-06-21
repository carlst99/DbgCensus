using System.Collections.Generic;

namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Clear subscriptions from the event stream.
    /// </summary>
    public record ClearSubscribeCommand : CensusCommandBase
    {
        /// <summary>
        /// Gets the characters to unsubscribe from.
        /// </summary>
        public IEnumerable<string>? Characters { get; init; }

        /// <summary>
        /// Gets the events to unsubscribe from.
        /// </summary>
        public IEnumerable<string>? EventNames { get; init; }

        /// <summary>
        /// Gets the worlds to unsubscribe from.
        /// </summary>
        public IEnumerable<string>? Worlds { get; init; }

        /// <summary>
        /// Gets a value indicating if every subscription will be cleared.
        /// </summary>
        public bool All { get; init; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ClearSubscribeCommand"/> record.
        /// </summary>
        /// <param name="characters">The characters to unsubscribe from.</param>
        /// <param name="eventNames">The events to unsubscribe from.</param>
        /// <param name="worlds">The worlds to unsubscribe from.</param>
        /// <param name="all">Set to true to unsubscribe from everything.</param>
        public ClearSubscribeCommand(IEnumerable<string>? characters = default, IEnumerable<string>? eventNames = default, IEnumerable<string>? worlds = default, bool all = false)
            : base("clearSubscribe", "event")
        {
            Characters = characters;
            EventNames = eventNames;
            Worlds = worlds;
            All = all;
        }
    }
}
