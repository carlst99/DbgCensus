using DbgCensus.EventStream.Abstractions.Objects.Control;

namespace DbgCensus.EventStream.Objects.Control;

/// <summary>
/// Initializes a new instance of the <see cref="ServiceStateChanged"/> record.
/// </summary>
/// <param name="Detail">The name of the endpoint that has changed state.</param>
/// <param name="Online">A value indicating whether or not the endpoint is online.</param>
/// <param name="Service">The websocket service that the payload was received from.</param>
/// <param name="Type">The Census type of the object.</param>
public record ServiceStateChanged
(
    string Detail,
    bool Online,
    string Service,
    string Type
) : IServiceStateChanged;
