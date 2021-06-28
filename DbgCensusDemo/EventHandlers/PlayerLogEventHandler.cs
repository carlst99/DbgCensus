using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Objects.Event;
using DbgCensusDemo.Objects.EventStream;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensusDemo.EventHandlers
{
    public class PlayerLogEventHandler : ICensusEventHandler<ServiceMessage<PlayerLogin>>, ICensusEventHandler<ServiceMessage<PlayerLogout>>
    {
        private readonly ILogger<PlayerLogEventHandler> _logger;

        public PlayerLogEventHandler(ILogger<PlayerLogEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(ServiceMessage<PlayerLogin> censusEvent, CancellationToken ct = default)
        {
            _logger.LogInformation("Player {playerId} logged in on {world} at {timestamp}", censusEvent.Payload.CharacterId, censusEvent.Payload.WorldId, censusEvent.Payload.Timestamp);
            return Task.CompletedTask;
        }

        public Task HandleAsync(ServiceMessage<PlayerLogout> censusEvent, CancellationToken ct = default)
        {
            _logger.LogInformation("Player {playerId} logged out on {world} at {timestamp}", censusEvent.Payload.CharacterId, censusEvent.Payload.WorldId, censusEvent.Payload.Timestamp);
            return Task.CompletedTask;
        }
    }
}
