namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Represents the context of a dispatched payload.
/// </summary>
public interface IPayloadContext
{
    /// <summary>
    /// Gets the name of the <see cref="DbgCensus.EventStream.Abstractions.IEventStreamClient"/> that dispatched this event.
    /// </summary>
    string DispatchingClientName { get; }
}
