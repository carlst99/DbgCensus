using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;
using System;

namespace DbgCensus.EventStream.Objects.Worlds;

/// <summary>
/// Initializes a new instance of the <see cref="ContinentLock"/> record.
/// </summary>
/// <param name="EventName">The name of the event.</param>
/// <param name="MetagameEventID">The metagame event that triggered this lock.</param>
/// <param name="NCPopulation">The NC population percentage at the time of locking.</param>
/// <param name="PreviousFaction">The previous faction to have locked the continent.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="TRPopulation">The TR population percentage at the time of locking.</param>
/// <param name="TriggeringFaction">The faction that locked the continent.</param>
/// <param name="VSPopulation">The VS population percentage at the time of locking.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record ContinentLock
(
    string EventName,
    MetagameEventDefinition MetagameEventID,
    int NCPopulation,
    FactionDefinition PreviousFaction,
    DateTimeOffset Timestamp,
    int TRPopulation,
    FactionDefinition TriggeringFaction,
    int VSPopulation,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IContinentLock;
