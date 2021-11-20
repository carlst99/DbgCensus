using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="VehicleDestroy"/> record.
/// </summary>
/// <param name="AttackerCharacterID">The ID of the attacking character.</param>
/// <param name="AttackerLoadoutID">The loadout ID of the attacking character.</param>
/// <param name="AttackerVehicleID">The ID of the vehicle used by the attacking character.</param>
/// <param name="AttackerWeaponID">The ID of the weapon used by the attacking character.</param>
/// <param name="CharacterID">The ID of the killed character.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="FacilityID">The facility ID.</param>
/// <param name="FactionID">The faction of the killed character.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="VehicleID">The ID of the vehicle used by the killed character.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record VehicleDestroy
(
    ulong AttackerCharacterID,
    uint AttackerLoadoutID,
    uint AttackerVehicleID,
    uint AttackerWeaponID,
    ulong CharacterID,
    string EventName,
    uint FacilityID,
    FactionDefinition FactionID,
    DateTimeOffset Timestamp,
    uint VehicleID,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IVehicleDestroy;
