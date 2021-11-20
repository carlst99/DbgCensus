using DbgCensus.EventStream.Abstractions.Objects.Control;

namespace DbgCensus.EventStream.Objects.Control;

/// <summary>
/// Initializes a new instance of the <see cref="ConnectionStateChanged"/> record.
/// </summary>
/// <param name="Connected">A value indicating whether or not the current connection is alive.</param>
/// <param name="Service">The websocket service that the payload was received from.</param>
/// <param name="Type">The Census type of the object.</param>
public record ConnectionStateChanged
(
    bool Connected,
    string Service,
    string Type
) : IConnectionStateChanged;
