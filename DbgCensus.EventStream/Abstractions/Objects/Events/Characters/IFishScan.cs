using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents a FishScan event.
/// </summary>
public interface IFishScan : IZoneEvent
{
    /// <summary>
    /// The ID of the character that scanned the fish.
    /// </summary>
    ulong CharacterID { get; }

    /// <summary>
    /// The ID of the fish that was scanned.
    /// </summary>
    uint FishID { get; }

    /// <summary>
    /// The loadout ID of the character that scanned the fish.
    /// </summary>
    uint LoadoutID { get; }

    /// <summary>
    /// Gets the currently-assigned faction of the character that scanned the fish.
    /// </summary>
    FactionDefinition TeamID { get; }
}
