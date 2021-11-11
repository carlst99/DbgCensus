using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a Death event.
/// </summary>
public interface IDeath : IEvent
{
    /// <summary>
    /// Gets the ID of the attacking character.
    /// </summary>
    ulong AttackerCharacterID { get; }

    /// <summary>
    /// The fire mode of the attacking character.
    /// </summary>
    uint AttackerFireMode { get; }

    /// <summary>
    /// Gets the loadout ID of the attacking character.
    /// </summary>
    uint AttackerLoadoutID { get; }

    /// <summary>
    /// Gets the ID of the vehicle used by the attacking character, or zero if a vehicle was not used.
    /// </summary>
    uint AttackerVehicleID { get; }

    /// <summary>
    /// Gets the ID of the weapon used by the attacking character, or zero if a weapon was not used.
    /// </summary>
    uint AttackerWeaponID { get; }

    /// <summary>
    /// Gets the ID of the killed character.
    /// </summary>
    ulong CharacterID { get; }

    /// <summary>
    /// Gets the loadout ID of the killed character.
    /// </summary>
    uint CharacterLoadoutID { get; }

    /// <summary>
    /// Gets a value indicating whether or not the kill was a headshot.
    /// </summary>
    bool IsHeadshot { get; }

    /// <summary>
    /// Gets the time at which this event occured.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the world on which this event occured.
    /// </summary>
    WorldDefinition WorldID { get; }

    /// <summary>
    /// Gets the zone on which this event occured.
    /// </summary>
    ZoneID ZoneID { get; }
}
