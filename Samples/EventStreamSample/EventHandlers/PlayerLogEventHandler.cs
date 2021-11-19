using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.Objects.Control;
using DbgCensus.EventStream.Objects.Events.Characters;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers;

public class PlayerLogEventHandler : IPayloadHandler<ServiceMessage<PlayerLogin>>, IPayloadHandler<ServiceMessage<PlayerLogout>>
{
    private readonly ILogger<PlayerLogEventHandler> _logger;

    public PlayerLogEventHandler(ILogger<PlayerLogEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ServiceMessage<PlayerLogin> censusEvent, CancellationToken ct = default)
    {
        _logger.LogInformation("Player {playerId} logged in on {world} at {timestamp}", censusEvent.Payload.CharacterID, censusEvent.Payload.WorldID, censusEvent.Payload.Timestamp);
        return Task.CompletedTask;
    }

    public Task HandleAsync(ServiceMessage<PlayerLogout> censusEvent, CancellationToken ct = default)
    {
        _logger.LogInformation("Player {playerId} logged out on {world} at {timestamp}", censusEvent.Payload.CharacterID, censusEvent.Payload.WorldID, censusEvent.Payload.Timestamp);
        return Task.CompletedTask;
    }
}
