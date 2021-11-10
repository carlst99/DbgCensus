using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects.Event;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.System;

public class ServiceStateChangedEventHandler : ICensusEventHandler<ServiceStateChanged>
{
    private readonly ILogger<ServiceStateChangedEventHandler> _logger;

    public ServiceStateChangedEventHandler(ILogger<ServiceStateChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ServiceStateChanged censusEvent, CancellationToken ct = default)
    {
        _logger.LogInformation("An event stream endpoint has changed state: {endpoint} is now {state}", censusEvent.EndpointIdentifier, censusEvent.IsOnline ? "online" : "offline");
        return Task.CompletedTask;
    }
}
