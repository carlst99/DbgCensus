namespace DbgCensus.EventStream.Abstractions.Objects.Control;

/// <summary>
/// Represents a ConnectionStateChanged payload.
/// </summary>
public interface IConnectionStateChanged : IControl
{
    /// <summary>
    /// Gets a value indicating whether or not the current connection is alive.
    /// </summary>
    bool Connected { get; }
}
