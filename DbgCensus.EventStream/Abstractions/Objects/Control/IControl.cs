namespace DbgCensus.EventStream.Abstractions.Objects.Control;

/// <summary>
/// Represents a marker interface for a control object.
/// </summary>
public interface IControl : IPayload
{
    /// <summary>
    /// Gets the websocket service that this object was received from.
    /// </summary>
    string Service { get; }

    /// <summary>
    /// Gets the Census type of this object.
    /// </summary>
    string Type { get; }
}
