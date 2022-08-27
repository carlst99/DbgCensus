using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="Death"/> record.
/// </summary>
/// <param name="AttackerCharacterID">The ID of the attacking character.</param>
/// <param name="AttackerFireMode">The fire mode of the attacking character.</param>
/// <param name="AttackerLoadoutID">The loadout ID of the attacking character.</param>
/// <param name="AttackerTeamID">Gets the current faction of the attacking character.</param>
/// <param name="AttackerVehicleID">The ID of the vehicle used by the attacking character.</param>
/// <param name="AttackerWeaponID">The ID of the weapon used by the attacking character.</param>
/// <param name="CharacterID">The ID of the killed character.</param>
/// <param name="CharacterLoadoutID">The loadout ID of the killed character.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="IsCritical">Unknown.</param>
/// <param name="IsHeadshot">A value indicating whether the kill was a headshot.</param>
/// <param name="TeamID">Gets the current faction of the killed character.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record Death
(
    ulong AttackerCharacterID,
    uint AttackerFireMode,
    uint AttackerLoadoutID,
    FactionDefinition AttackerTeamID,
    uint AttackerVehicleID,
    uint AttackerWeaponID,
    ulong CharacterID,
    uint CharacterLoadoutID,
    string EventName,
    bool IsCritical,
    bool IsHeadshot,
    FactionDefinition TeamID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IDeath;
