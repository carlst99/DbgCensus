namespace DbgCensus.EventStream.Abstractions.Objects.Control;

/// <summary>
/// Represents a ServiceStateChanged payload.
/// </summary>
public interface IServiceStateChanged : IControl
{
    /// <summary>
    /// Gets the name of the endpoint that has changed state.
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// Gets a value indicating whether or not the endpoint is online.
    /// </summary>
    bool Online { get; }
}
