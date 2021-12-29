using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.ControlPayloads;

public class UnknownPayloadHandler : IPayloadHandler<IUnknownPayload>
{
    private readonly ILogger<UnknownPayloadHandler> _logger;

    public UnknownPayloadHandler(ILogger<UnknownPayloadHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IUnknownPayload censusEvent, CancellationToken ct = default)
    {
        _logger.LogWarning("An unknown event was received from the Census event stream: {eventData}", censusEvent.RawData);

        return Task.CompletedTask;
    }
}
