using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers
{
    public class UnknownEventHandler : ICensusEventHandler<UnknownEvent>
    {
        private readonly ILogger<UnknownEventHandler> _logger;

        public UnknownEventHandler(ILogger<UnknownEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(UnknownEvent censusEvent, CancellationToken ct = default)
        {
            _logger.LogWarning("An unknown event was received from the Census event stream: {eventData}", censusEvent.RawData);
            return Task.CompletedTask;
        }
    }
}
