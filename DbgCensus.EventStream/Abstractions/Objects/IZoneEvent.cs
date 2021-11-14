using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a marker interface for an event that occurs on a zone.
/// </summary>
public interface IZoneEvent : IEvent
{
    /// <summary>
    /// Gets the zone on which this event occured.
    /// </summary>
    ZoneID ZoneID { get; }
}
