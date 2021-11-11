using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents an AchievementEarned event.
/// </summary>
public interface IAchievementEarned : IEvent
{
    /// <summary>
    /// Gets the ID of the achievement.
    /// </summary>
    uint AchievementID { get; }

    /// <summary>
    /// Gets the ID of the character that earned this achievement.
    /// </summary>
    ulong CharacterID { get; }

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
