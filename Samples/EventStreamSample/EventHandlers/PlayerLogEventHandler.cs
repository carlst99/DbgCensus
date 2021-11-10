using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects.Event;
using EventStreamSample.Objects;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers;

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
