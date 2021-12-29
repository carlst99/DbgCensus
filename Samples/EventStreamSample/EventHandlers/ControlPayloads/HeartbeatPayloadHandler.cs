using DbgCensus.EventStream.Abstractions.Objects.Control;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.ControlPayloads;

public class HeartbeatPayloadHandler : IPayloadHandler<IHeartbeat>
{
    private readonly ILogger<HeartbeatPayloadHandler> _logger;

    public HeartbeatPayloadHandler(ILogger<HeartbeatPayloadHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(IHeartbeat payload, CancellationToken ct = default)
    {
        string message = string.Empty;
        foreach (KeyValuePair<string, bool> element in payload.Online)
            message += $"\t- {element.Key}: {element.Value}\n";

        _logger.LogInformation("Received heartbeat:\n{heartbeat}", message);

        return Task.CompletedTask;
    }
}
