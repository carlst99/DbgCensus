namespace DbgCensus.EventStream.EventHandlers.Abstractions.Objects
{
    /// <summary>
    /// Defines an object received from the Census event stream.
    /// </summary>
    public interface IEventStreamObject
    {
        /// <summary>
        /// The name of the <see cref="EventStream.Abstractions.ICensusEventStreamClient"/> from which this event was dispatched.
        /// </summary>
        string DispatchingClientName { get; set; }
    }
}
