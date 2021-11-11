using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects;
using System;

namespace DbgCensus.EventStream.Objects;

/// <summary>
/// Initializes a new instance of the <see cref="GainExperience"/> record.
/// </summary>
/// <param name="Amount">The amount of XP that was gained.</param>
/// <param name="CharacterID">The ID of the character that gained the experience.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="ExperienceID">The type of experience that caused the XP gain.</param>
/// <param name="LoadoutID">The loadout ID of the character.</param>
/// <param name="OtherID">The ID of the other entity/character involved in generating the XP gain.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record GainExperience
(
    uint Amount,
    ulong CharacterID,
    string EventName,
    uint ExperienceID,
    uint LoadoutID,
    ulong OtherID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IGainExperience;
