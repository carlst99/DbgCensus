using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.Objects.Control;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.System;

public class ServiceStateChangedEventHandler : IPayloadHandler<ServiceStateChanged>
{
    private readonly ILogger<ServiceStateChangedEventHandler> _logger;

    public ServiceStateChangedEventHandler(ILogger<ServiceStateChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ServiceStateChanged censusEvent, CancellationToken ct = default)
    {
        _logger.LogInformation("An event stream endpoint has changed state: {endpoint} is now {state}", censusEvent.Detail, censusEvent.Online ? "online" : "offline");
        return Task.CompletedTask;
    }
}
