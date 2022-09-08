using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents a VehicleDestroy event.
/// </summary>
public interface IVehicleDestroy : IZoneEvent
{
    /// <summary>
    /// Gets the ID of the attacking character;
    /// </summary>
    ulong AttackerCharacterID { get; }

    /// <summary>
    /// Gets the loadout ID of the attacking character.
    /// </summary>
    uint AttackerLoadoutID { get; }

    /// <summary>
    /// Gets the currently assigned faction of the attacking character.
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
    /// This field is always set to zero by the event streaming API.
    /// </summary>
    uint FacilityID { get; }

    /// <summary>
    /// Gets the home faction of the killed character.
    /// </summary>
    FactionDefinition FactionID { get; }

    /// <summary>
    /// Gets the currently assigned faction of the killed character.
    /// </summary>
    FactionDefinition TeamID { get; }

    /// <summary>
    /// Gets the ID of the vehicle used by the killed character.
    /// </summary>
    uint VehicleID { get; }
}
