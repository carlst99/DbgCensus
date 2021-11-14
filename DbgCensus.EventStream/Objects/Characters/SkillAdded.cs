using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="SkillAdded"/> record.
/// </summary>
/// <param name="CharacterID">The ID of the character for which the event was generated.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="SkillID">The ID of the unlocked skill.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record SkillAdded
(
    ulong CharacterID,
    string EventName,
    uint SkillID,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : ISkillAdded;
