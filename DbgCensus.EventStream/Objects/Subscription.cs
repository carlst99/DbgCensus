using DbgCensus.EventStream.Abstractions.Objects;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DbgCensus.EventStream.Objects
{
    /// <summary>
    /// An object sent by the event stream to provide details of the current subscription.
    /// </summary>
    public record Subscription : IEventStreamObject
    {
        /// <inheritdoc />
        public string DispatchingClientName { get; set; }

        /// <summary>
        /// Gets the characters that have been subscribed to.
        /// </summary>
        public IEnumerable<string> Characters { get; init; }

        /// <summary>
        /// Gets the number of characters that have been subscribed to.
        /// </summary>
        [JsonPropertyName("characterCount")]
        public int CharacterCount { get; init; }

        /// <summary>
        /// Gets the events that have been subscribed to.
        /// </summary>
        [JsonPropertyName("eventNames")]
        public IEnumerable<string> EventNames { get; init; }

        /// <summary>
        /// Gets a value indicating if events will only be sent when they both a character AND a world that has been subscribed to. Useful when subscribing to 'all' values of a category.
        /// </summary>
        [JsonPropertyName("logicalAndCharactersWithWorlds")]
        public bool LogicalAndCharactersWithWorlds { get; init; }

        /// <summary>
        /// Gets the worlds that have been subscribed to.
        /// </summary>
        public IEnumerable<string> Worlds { get; init; }

        ///// <summary>
        ///// Initialises a new instance of the <see cref="Subscription"/> record.
        ///// </summary>
        ///// <param name="characterCount">The number of characters that have been subscribed to.</param>
        ///// <param name="eventNames">The events that have been subscribed to.</param>
        ///// <param name="logicalAndCharactersWithWorlds">A value indicating if events will only be sent when they both a character AND a world that has been subscribed to. Useful when subscribing to 'all' values of a category.</param>
        ///// <param name="worlds">The worlds that have been subscribed to.</param>
        //public Subscription(int characterCount, IEnumerable<string> eventNames, bool logicalAndCharactersWithWorlds, IEnumerable<string> worlds)
        //{
        //    CharacterCount = characterCount;
        //    EventNames = eventNames ?? new List<string>();
        //    LogicalAndCharactersWithWorlds = logicalAndCharactersWithWorlds;
        //    Worlds = worlds ?? new List<string>();
        //}
    
        /// <summary>
        /// Initialises a new instance of the <see cref="Subscription"/> record.
        /// </summary>
        public Subscription()
        {
            DispatchingClientName = string.Empty;
            Characters = new List<string>();
            EventNames = new List<string>();
            Worlds = new List<string>();
        }
    }
}
