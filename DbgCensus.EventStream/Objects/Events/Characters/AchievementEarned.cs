﻿using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="AchievementEarned"/> record.
/// </summary>
/// <param name="AchievementID">The ID of the achievement.</param>
/// <param name="CharacterID">The ID of the character that earned the achievement.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record AchievementEarned
(
    uint AchievementID,
    ulong CharacterID,
    string EventName,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IAchievementEarned;
