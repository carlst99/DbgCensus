namespace DbgCensus.EventStream.EventHandlers.Abstractions.Objects;

/// <summary>
/// Represents the context of a dispatched payload.
/// </summary>
public interface IPayloadContext
{
    /// <summary>
    /// Gets the name of the <see cref="EventStream.Abstractions.IEventStreamClient"/> that dispatched this event.
    /// </summary>
    string DispatchingClientName { get; }
}
