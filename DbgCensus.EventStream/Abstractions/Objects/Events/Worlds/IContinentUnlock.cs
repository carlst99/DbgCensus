using DbgCensus.Core.Objects;

namespace DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;

public interface IContinentUnlock : IZoneEvent
{
    /// <summary>
    /// Gets the metagame event that this continent unlock was a result of.
    /// </summary>
    MetagameEventDefinition MetagameEventID { get; }

    /// <summary>
    /// Gets the NC population percentage on the continent at the time of unlocking.
    /// </summary>
    int NCPopulation { get; }

    /// <summary>
    /// Gets the previous faction to have locked the continent.
    /// </summary>
    FactionDefinition PreviousFaction { get; }

    /// <summary>
    /// Gets the TR population percentage on the continent at the time of unlocking.
    /// </summary>
    int TRPopulation { get; }

    /// <summary>
    /// Gets the faction that unlocked the continent.
    /// </summary>
    FactionDefinition TriggeringFaction { get; }

    /// <summary>
    /// Gets the VS population percentage on the continent at the time of unlocking.
    /// </summary>
    int VSPopulation { get; }
}
