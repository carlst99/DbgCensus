using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a FacilityControl event.
/// </summary>
public interface IFacilityControl : IEvent
{
    /// <summary>
    /// Gets the duration in seconds that the facility was last held for.
    /// </summary>
    uint DurationHeld { get; }

    /// <summary>
    /// Gets the ID of the captured facility.
    /// </summary>
    uint FacilityID { get; }

    /// <summary>
    /// Gets the ID of the new controlling faction.
    /// </summary>
    FactionDefinition NewFactionID { get; }

    /// <summary>
    /// Gets the ID of the old controlling faction.
    /// </summary>
    FactionDefinition OldFactionID { get; }

    /// <summary>
    /// Gets the ID of the outfit that captured the facility, or zero if no outfit was involved in the capture.
    /// </summary>
    ulong OutfitID { get; }

    /// <summary>
    /// Gets the time at which this event occured.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the world on which this event occured.
    /// </summary>
    WorldDefinition WorldId { get; }

    /// <summary>
    /// Gets the zone on which this event occured.
    /// </summary>
    ZoneID ZoneId { get; }
}
