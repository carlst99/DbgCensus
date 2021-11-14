using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="BattleRankUp"/> record.
/// </summary>
/// <param name="BattleRank">The achieved rank of the character.</param>
/// <param name="CharacterID">The ID of the character that ranked up.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record BattleRankUp
(
    int BattleRank,
    ulong CharacterID,
    string EventName,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IBattleRankUp;
