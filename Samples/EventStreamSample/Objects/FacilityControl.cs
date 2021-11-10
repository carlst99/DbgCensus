using DbgCensus.Core.Objects;
using System;

namespace EventStreamSample.Objects;

public record FacilityControl
{
    public string EventName { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public WorldDefinition WorldId { get; init; }
    public Faction OldFactionId { get; init; }
    public Faction NewFactionId { get; init; }
    public ulong OutfitId { get; init; }
    public uint FacilityId { get; init; }
    public ulong DurationHeld { get; init; }
    public ZoneId ZoneId { get; init; }

    public FacilityControl()
    {
        EventName = "FacilityControl";
        ZoneId = ZoneId.Default;
    }
}
