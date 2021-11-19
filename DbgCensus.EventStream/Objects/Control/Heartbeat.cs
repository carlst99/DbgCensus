using DbgCensus.EventStream.Abstractions.Objects.Control;
using System.Collections.Generic;

namespace DbgCensus.EventStream.Objects.Control;

/// <summary>
/// Initializes a new instance of the <see cref="Heartbeat"/> record.
/// </summary>
/// <param name="Online">A map of endpoint names to their online status.</param>
/// <param name="Service">The websocket service that the payload was received from.</param>
/// <param name="Type">The Census type of the object.</param>
public record Heartbeat
(
    IReadOnlyDictionary<string, bool> Online,
    string Service,
    string Type
) : IHeartbeat;
