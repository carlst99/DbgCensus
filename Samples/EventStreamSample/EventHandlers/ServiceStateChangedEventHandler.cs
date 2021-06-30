using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Objects.Event;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers
{
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
}
