namespace DbgCensus.EventStream.Objects
{
    /// <summary>
    /// Defines an object received from the Census event stream.
    /// </summary>
    public interface IEventStreamObject
    {
        /// <summary>
        /// The websocket service that this object has been received from.
        /// </summary>
        string Service { get; }

        /// <summary>
        /// The type of object.
        /// </summary>
        string Type { get; }
    }
}
