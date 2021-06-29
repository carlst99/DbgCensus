using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Objects.Event;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers
{
    public class HeartbeatEventHandler : ICensusEventHandler<Heartbeat>
    {
        private readonly ILogger<HeartbeatEventHandler> _logger;

        public HeartbeatEventHandler(ILogger<HeartbeatEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(Heartbeat censusEvent, CancellationToken ct = default)
        {
            _logger.LogInformation($"Received heartbeat. { censusEvent }");
            return Task.CompletedTask;
        }
    }
}
