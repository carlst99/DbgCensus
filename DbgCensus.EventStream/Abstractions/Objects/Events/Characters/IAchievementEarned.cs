namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents an AchievementEarned event.
/// </summary>
public interface IAchievementEarned : IZoneEvent
{
    /// <summary>
    /// Gets the ID of the achievement.
    /// </summary>
    uint AchievementID { get; }

    /// <summary>
    /// Gets the ID of the character that earned this achievement.
    /// </summary>
    ulong CharacterID { get; }
}
