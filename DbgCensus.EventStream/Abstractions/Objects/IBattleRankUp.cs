using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a BattleRankUp event.
/// </summary>
public interface IBattleRankUp : IEvent
{
    /// <summary>
    /// Gets the achieved rank of the character.
    /// </summary>
    int BattleRank { get; }

    /// <summary>
    /// Gets the ID of the character that ranked up.
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
