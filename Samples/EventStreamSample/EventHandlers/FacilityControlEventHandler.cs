using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects.Event;
using DbgCensus.EventStream.Objects.Worlds;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers;

public class FacilityControlEventHandler : ICensusEventHandler<ServiceMessage<FacilityControl>>
{
    private readonly ILogger<FacilityControlEventHandler> _logger;

    public FacilityControlEventHandler(ILogger<FacilityControlEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ServiceMessage<FacilityControl> censusEvent, CancellationToken ct = default)
    {
        FacilityControl controlEvent = censusEvent.Payload;

        _logger.LogInformation(
            "The facility {facilityId} on {world} changed ownership, from {oldFaction} to {newFaction}. It was captured at {captureTime}, in the zone {zone}.",
            controlEvent.FacilityID,
            controlEvent.WorldID,
            controlEvent.OldFactionID,
            controlEvent.NewFactionID,
            controlEvent.Timestamp,
            controlEvent.ZoneID);
        return Task.CompletedTask;
    }
}
