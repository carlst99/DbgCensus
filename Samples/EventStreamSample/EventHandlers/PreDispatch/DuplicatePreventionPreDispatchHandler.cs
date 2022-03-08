using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.PreDispatch;

public class DuplicatePreventionPreDispatchHandler : IPreDispatchHandler
{
    private readonly ILogger<DuplicatePreventionPreDispatchHandler> _logger;
    private readonly Queue<int> _seenEvents;

    public DuplicatePreventionPreDispatchHandler(ILogger<DuplicatePreventionPreDispatchHandler> logger)
    {
        _logger = logger;
        _seenEvents = new Queue<int>();
    }

    /// <inheritdoc />
    public ValueTask<bool> HandlePayloadAsync<T>(T payload, CancellationToken ct = default)
        where T : IPayload
    {
        if (_seenEvents.Count > 50)
            _seenEvents.Dequeue();

        int hash = payload.GetHashCode();

        if (_seenEvents.Contains(hash))
        {
            _logger.LogWarning("Preventing dispatch of duplicate event {Event}", payload);
            return ValueTask.FromResult(true);
        }

        _seenEvents.Enqueue(hash);
        return ValueTask.FromResult(false);
    }
}
