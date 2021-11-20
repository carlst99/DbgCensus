using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;
using System;

namespace DbgCensus.EventStream.Objects.Events.Worlds;

/// <summary>
/// Initializes a new instance of the <see cref="FacilityControl"/> record.
/// </summary>
/// <param name="DurationHeld">The duration in seconds that the facility was last held for.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="FacilityID">The ID of the captured facility.</param>
/// <param name="NewFactionID">The ID of the new controlling faction.</param>
/// <param name="OldFactionID">The ID of the old controlling faction.</param>
/// <param name="OutfitID">The ID of the outfit that captured the facility, or zero if no outfit was involved in the capture.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record FacilityControl
(
    uint DurationHeld,
    string EventName,
    uint FacilityID,
    FactionDefinition NewFactionID,
    FactionDefinition OldFactionID,
    ulong OutfitID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IFacilityControl;
