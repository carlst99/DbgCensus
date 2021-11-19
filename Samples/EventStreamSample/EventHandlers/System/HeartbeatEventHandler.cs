using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.Objects.Control;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.System;

public class HeartbeatEventHandler : IPayloadHandler<Heartbeat>
{
    private readonly ILogger<HeartbeatEventHandler> _logger;

    public HeartbeatEventHandler(ILogger<HeartbeatEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(Heartbeat censusEvent, CancellationToken ct = default)
    {
        string message = string.Empty;
        foreach (KeyValuePair<string, bool> element in censusEvent.Online)
            message += $"\t- {element.Key}: {element.Value}\n";

        _logger.LogInformation("Received heartbeat:\n{heartbeat}", message);
        return Task.CompletedTask;
    }
}
