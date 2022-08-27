using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents a Death event.
/// </summary>
public interface IDeath : IZoneEvent
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
    /// Gets the current faction of the attacking character.
    /// </summary>
    FactionDefinition AttackerTeamID { get; }

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
    /// Unknown.
    /// </summary>
    bool IsCritical { get; }

    /// <summary>
    /// Gets a value indicating whether or not the kill was a headshot.
    /// </summary>
    bool IsHeadshot { get; }

    /// <summary>
    /// Gets the current faction of the killed character.
    /// </summary>
    FactionDefinition TeamID { get; }
}
