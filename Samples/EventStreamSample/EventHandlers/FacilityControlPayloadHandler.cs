using DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers;

public class FacilityControlPayloadHandler : IPayloadHandler<IFacilityControl>
{
    private readonly ILogger<FacilityControlPayloadHandler> _logger;

    public FacilityControlPayloadHandler(ILogger<FacilityControlPayloadHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IFacilityControl payload, CancellationToken ct = default)
    {
        _logger.LogInformation
        (
            "The facility {facilityId} on {world} changed ownership, from {oldFaction} to {newFaction}. It was captured at {captureTime}, in the zone {zone}.",
            payload.FacilityID,
            payload.WorldID,
            payload.OldFactionID,
            payload.NewFactionID,
            payload.Timestamp,
            payload.ZoneID
        );

        return Task.CompletedTask;
    }
}
