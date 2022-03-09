using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="PlayerFacilityDefend"/> record.
/// </summary>
/// <param name="CharacterID">The ID of the character for which this event was generated.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="FacilityID">The ID of the defended facility.</param>
/// <param name="OutfitID">The ID of the outfit that the <paramref name="CharacterID"/> belongs to.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record PlayerFacilityDefend
(
    ulong CharacterID,
    string EventName,
    uint FacilityID,
    ulong OutfitID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IPlayerFacilityDefend;
