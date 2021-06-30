namespace DbgCensus.EventStream.Abstractions.Objects
{
    /// <summary>
    /// Defines an object received from the Census event stream.
    /// </summary>
    public interface IEventStreamObject
    {
        /// <summary>
        /// The name of the <see cref="ICensusEventStreamClient"/> from which this event was dispatched.
        /// </summary>
        string DispatchingClientName { get; set; }
    }
}
