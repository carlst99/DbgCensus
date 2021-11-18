namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Represents the context of an event.
/// </summary>
public interface IEventContext
{
    /// <summary>
    /// Gets the name of the <see cref="DbgCensus.EventStream.Abstractions.IEventStreamClient"/> that dispatched this event.
    /// </summary>
    string DispatchingClientName { get; }
}
