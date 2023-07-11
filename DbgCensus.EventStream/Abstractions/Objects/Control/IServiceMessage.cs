namespace DbgCensus.EventStream.Abstractions.Objects.Control;

/// <summary>
/// Represents a ServiceMessage payload.
/// </summary>
/// <typeparam name="TEvent">The type of the encapsulated event.</typeparam>
public interface IServiceMessage<out TEvent> : IControl
{
    /// <summary>
    /// Gets the encapsulated event payload.
    /// </summary>
    TEvent Payload { get; }
}
