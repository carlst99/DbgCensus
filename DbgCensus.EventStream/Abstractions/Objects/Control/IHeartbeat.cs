using System.Collections.Generic;

namespace DbgCensus.EventStream.Abstractions.Objects.Control;

/// <summary>
/// Represents a Heartbeat payload.
/// </summary>
public interface IHeartbeat : IControl
{
    /// <summary>
    /// Gets a map of endpoint names to their online status.
    /// </summary>
    public IReadOnlyDictionary<string, bool> Online { get; }
}
