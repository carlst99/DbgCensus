using DbgCensus.Core.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.Objects;

/// <summary>
/// Represents a MetagameEvent event.
/// </summary>
public interface IMetagameEvent : IEvent
{
    /// <summary>
    /// Gets the XP awarded for playing the entirety of this metagame event.
    /// </summary>
    double ExperienceBonus { get; }

    /// <summary>
    /// Gets the amount of territory held by the NC at the time of this event.
    /// </summary>
    double FactionNC { get; }

    /// <summary>
    /// Gets the amount of territory held by the TR at the time of this event.
    /// </summary>
    double FactionTR { get; }

    /// <summary>
    /// Gets the amount of territory held by the VS at the time of this event.
    /// </summary>
    double FactionVS { get; }

    /// <summary>
    /// Gets the ID of this metagame event.
    /// </summary>
    MetagameEventDefinition MetagameEventID { get; }

    /// <summary>
    /// Gets the state of this event.
    /// </summary>
    MetagameEventState MetagameEventState { get; }

    /// <summary>
    /// Gets the time at which this event occured.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the world on which this event occured.
    /// </summary>
    WorldDefinition WorldID { get; }

    /// <summary>
    /// Gets the zone on which this event occured.
    /// </summary>
    ZoneID ZoneID { get; }
}
