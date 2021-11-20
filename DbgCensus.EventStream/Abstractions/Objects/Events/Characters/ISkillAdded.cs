namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents a SkillAdded event.
/// </summary>
public interface ISkillAdded : IZoneEvent
{
    /// <summary>
    /// Gets the ID of the character for which this event was generated.
    /// </summary>
    ulong CharacterID { get; }

    /// <summary>
    /// Gets the ID of the unlocked skill.
    /// </summary>
    uint SkillID { get; }
}
