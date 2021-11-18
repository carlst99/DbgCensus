using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;
using System;

namespace DbgCensus.EventStream.Objects.Worlds;

/// <summary>
/// Initializes a new instance of the <see cref="MetagameEvent"/> record.
/// </summary>
/// <param name="ExperienceBonus">The XP awarded for playing the entirety of the metagame event.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="FactionNC">The amount of territory held by the NC at the time of the event.</param>
/// <param name="FactionTR">The amount of territory held by the TR at the time of the event.</param>
/// <param name="FactionVS">The amount of territory held by the VS at the time of the event.</param>
/// <param name="MetagameEventID">The ID of the metagame event.</param>
/// <param name="MetagameEventState">The state of the event.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
/// <param name="ZoneID">The zone on which the event occured.</param>
public record MetagameEvent
(
    double ExperienceBonus,
    string EventName,
    double FactionNC,
    double FactionTR,
    double FactionVS,
    MetagameEventDefinition MetagameEventID,
    MetagameEventState MetagameEventState,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID,
    ZoneID ZoneID
) : IMetagameEvent;
