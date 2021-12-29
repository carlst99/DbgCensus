﻿using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers;

public class PlayerLogEventHandler : IPayloadHandler<IPlayerLogin>, IPayloadHandler<IPlayerLogout>
{
    private readonly ILogger<PlayerLogEventHandler> _logger;

    public PlayerLogEventHandler(ILogger<PlayerLogEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IPlayerLogin payload, CancellationToken ct = default)
    {
        _logger.LogInformation
        (
            "Player {playerId} logged in on {world} at {timestamp}",
            payload.CharacterID,
            payload.WorldID,
            payload.Timestamp
        );

        return Task.CompletedTask;
    }

    public Task HandleAsync(IPlayerLogout payload, CancellationToken ct = default)
    {
        _logger.LogInformation
        (
            "Player {playerId} logged out on {world} at {timestamp}",
            payload.CharacterID,
            payload.WorldID,
            payload.Timestamp
        );

        return Task.CompletedTask;
    }
}
