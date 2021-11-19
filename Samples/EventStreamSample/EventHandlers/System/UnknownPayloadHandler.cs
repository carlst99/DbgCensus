using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.System;

public class UnknownPayloadHandler : IPayloadHandler<UnknownPayload>
{
    private readonly ILogger<UnknownPayloadHandler> _logger;

    public UnknownPayloadHandler(ILogger<UnknownPayloadHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(UnknownPayload censusEvent, CancellationToken ct = default)
    {
        _logger.LogWarning("An unknown event was received from the Census event stream: {eventData}", censusEvent.RawData);
        return Task.CompletedTask;
    }
}
