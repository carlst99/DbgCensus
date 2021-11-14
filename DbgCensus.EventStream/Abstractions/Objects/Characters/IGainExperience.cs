namespace DbgCensus.EventStream.Abstractions.Objects.Characters;

/// <summary>
/// Represents a GainExperience event.
/// </summary>
public interface IGainExperience : IZoneEvent
{
    /// <summary>
    /// Gets the amount of XP that was gained.
    /// </summary>
    uint Amount { get; }

    /// <summary>
    /// Gets the ID of the character that gained the experience.
    /// </summary>
    ulong CharacterID { get; }

    /// <summary>
    /// Gets the ID of the experience that caused the XP gain.
    /// </summary>
    uint ExperienceID { get; }

    /// <summary>
    /// Gets the loadout ID of the character.
    /// </summary>
    uint LoadoutID { get; }

    /// <summary>
    /// Gets the ID of the other entity/character involved in generating the XP gain.
    /// </summary>
    ulong OtherID { get; }
}
