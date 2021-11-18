using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;

public interface IContinentLock : IZoneEvent
{
    /// <summary>
    /// Gets the metagame event that this continent lock was a result of.
    /// </summary>
    MetagameEventDefinition MetagameEventID { get; }

    /// <summary>
    /// Gets the NC population percentage on the continent at the time of locking.
    /// </summary>
    int NCPopulation { get; }

    /// <summary>
    /// Gets the previous faction to have locked the continent.
    /// </summary>
    FactionDefinition PreviousFaction { get; }

    /// <summary>
    /// Gets the TR population percentage on the continent at the time of locking.
    /// </summary>
    int TRPopulation { get; }

    /// <summary>
    /// Gets the faction that locked the continent.
    /// </summary>
    FactionDefinition TriggeringFaction { get; }

    /// <summary>
    /// Gets the VS population percentage on the continent at the time of locking.
    /// </summary>
    int VSPopulation { get; }
}
