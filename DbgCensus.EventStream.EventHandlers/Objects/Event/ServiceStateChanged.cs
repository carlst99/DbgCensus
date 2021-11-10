using System.Text.Json.Serialization;

namespace DbgCensus.EventStream.EventHandlers.Objects.Event;

/// <summary>
/// An object sent by the event stream when the state of a service changes.
/// </summary>
public record ServiceStateChanged : EventStreamObjectBase
{
    /// <summary>
    /// Gets the identifier of the world for which the event streaming endpoint has changed state.
    /// </summary>
    [JsonPropertyName("detail")]
    public string EndpointIdentifier { get; init; }

    /// <summary>
    /// Gets a value indicating if the endpoint is online.
    /// </summary>
    [JsonPropertyName("online")]
    public bool IsOnline { get; init; }

    /// <summary>
    /// Initialises a new instance of the <see cref="ServiceStateChanged"/> record.
    /// </summary>
    public ServiceStateChanged()
    {
        EndpointIdentifier = string.Empty;
        IsOnline = false;
    }
}
