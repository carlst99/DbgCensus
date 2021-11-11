using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a GainExperience event.
/// </summary>
public interface IGainExperience : IEvent
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
