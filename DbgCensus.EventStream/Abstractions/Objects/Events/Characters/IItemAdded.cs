namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents an ItemAdded event.
/// </summary>
public interface IItemAdded : IZoneEvent
{
    /// <summary>
    /// Gets the character that the item was added to.
    /// </summary>
    ulong CharacterID { get; }

    /// <summary>
    /// The context under which the item was added. E.g. <c>SkillGrantItemLine</c>
    /// or <c>GuildBankWithdrawal</c>.
    /// </summary>
    string Context { get; }

    /// <summary>
    /// Gets the number of items that were added.
    /// </summary>
    int ItemCount { get; }

    /// <summary>
    /// Gets the ID of the item that was added.
    /// </summary>
    uint ItemID { get; }
}
