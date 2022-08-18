using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// 
/// </summary>
/// <param name="CharacterID">The character that the item was added to.</param>
/// <param name="Context">
/// The context under which the item was added. E.g. <c>SkillGrantItemLine</c>
/// or <c>GuildBankWithdrawal</c>.
/// </param>
/// <param name="EventName">The name of the event.</param>
/// <param name="ItemCount">The number of items that were added.</param>
/// <param name="ItemID">The ID of the item that was added.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record ItemAdded
(
    ulong CharacterID,
    string Context,
    string EventName,
    int ItemCount,
    uint ItemID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IItemAdded;
