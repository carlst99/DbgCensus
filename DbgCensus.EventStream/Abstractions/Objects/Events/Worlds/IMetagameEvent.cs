using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;

/// <summary>
/// Represents a MetagameEvent event.
/// </summary>
public interface IMetagameEvent : IZoneEvent
{
    /// <summary>
    /// Gets the XP awarded for playing the entirety of this metagame event.
    /// </summary>
    double ExperienceBonus { get; }

    /// <summary>
    /// Gets the percentage of territory held by the NC at the time of this event.
    /// </summary>
    double FactionNC { get; }

    /// <summary>
    /// Gets the percentage of territory held by the TR at the time of this event.
    /// </summary>
    double FactionTR { get; }

    /// <summary>
    /// Gets the percentage of territory held by the VS at the time of this event.
    /// </summary>
    double FactionVS { get; }

    /// <summary>
    /// Gets the instance number of this event.
    /// </summary>
    uint InstanceID { get; }

    /// <summary>
    /// Gets the ID of this metagame event.
    /// </summary>
    MetagameEventDefinition MetagameEventID { get; }

    /// <summary>
    /// Gets the state of this event.
    /// </summary>
    MetagameEventState MetagameEventState { get; }

    /// <summary>
    /// Gets the string representation of the <see cref="MetagameEventState"/>.
    /// </summary>
    string MetagameEventStateName { get; }
}
