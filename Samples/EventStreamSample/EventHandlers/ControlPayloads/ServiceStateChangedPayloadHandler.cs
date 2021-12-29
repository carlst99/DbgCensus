using DbgCensus.EventStream.Abstractions.Objects.Control;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.ControlPayloads;

public class ServiceStateChangedPayloadHandler : IPayloadHandler<IServiceStateChanged>
{
    private readonly ILogger<ServiceStateChangedPayloadHandler> _logger;

    public ServiceStateChangedPayloadHandler(ILogger<ServiceStateChangedPayloadHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IServiceStateChanged payload, CancellationToken ct = default)
    {
        _logger.LogInformation
        (
            "An event stream endpoint has changed state: {endpoint} is now {state}",
            payload.Detail,
            payload.Online ? "online" : "offline"
        );

        return Task.CompletedTask;
    }
}
