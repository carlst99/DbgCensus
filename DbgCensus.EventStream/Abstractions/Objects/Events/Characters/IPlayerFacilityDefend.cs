namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents a PlayerFacilityDefend event.
/// </summary>
public interface IPlayerFacilityDefend : IZoneEvent
{
    /// <summary>
    /// Gets the ID of the character for which this event was generated.
    /// </summary>
    ulong CharacterID { get; }

    /// <summary>
    /// Gets the ID of the defended facility.
    /// </summary>
    public uint FacilityID { get; }

    /// <summary>
    /// Gets the ID of the outfit that the <see cref="CharacterID"/>
    /// belongs to, or zero if they are not in an outfit.
    /// </summary>
    public ulong OutfitID { get; }
}
