namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a marker interface for an event.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the name of this event.
    /// </summary>
    string EventName { get; }
}
