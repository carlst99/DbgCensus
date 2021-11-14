namespace DbgCensus.EventStream.Abstractions.Objects.Characters;

/// <summary>
/// Represents a BattleRankUp event.
/// </summary>
public interface IBattleRankUp : IZoneEvent
{
    /// <summary>
    /// Gets the achieved rank of the character.
    /// </summary>
    int BattleRank { get; }

    /// <summary>
    /// Gets the ID of the character that ranked up.
    /// </summary>
    ulong CharacterID { get; }
}
