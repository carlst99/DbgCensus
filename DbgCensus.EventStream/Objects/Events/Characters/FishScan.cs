using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="FishScan"/> record.
/// </summary>
/// <param name="CharacterID">The ID of the character that scanned the fish.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="FishID">The ID of the fish that was scanned.</param>
/// <param name="LoadoutID">The loadout ID of the character that scanned the fish.</param>
/// <param name="TeamID">Gets the currently-assigned faction of the character that scanned the fish.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record FishScan
(
    ulong CharacterID,
    string EventName,
    uint FishID,
    uint LoadoutID,
    FactionDefinition TeamID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IFishScan;
