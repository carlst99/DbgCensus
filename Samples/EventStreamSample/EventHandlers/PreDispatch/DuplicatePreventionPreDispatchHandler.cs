using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample.EventHandlers.PreDispatch;

public sealed class DuplicatePreventionPreDispatchHandler : IPreDispatchHandler, IDisposable
{
    private const int MAX_QUEUE_ELEMENTS = 15;

    private readonly ILogger<DuplicatePreventionPreDispatchHandler> _logger;

    private readonly Dictionary<Type, SemaphoreSlim> _payloadTypeLocks;
    private readonly Dictionary<Type, Queue<int>> _payloadEvents;

    public DuplicatePreventionPreDispatchHandler(ILogger<DuplicatePreventionPreDispatchHandler> logger)
    {
        _logger = logger;
        _payloadTypeLocks = new Dictionary<Type, SemaphoreSlim>();
        _payloadEvents = new Dictionary<Type, Queue<int>>();

        IEnumerable<Type> payloadTypes = typeof(IPayload).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IPayload)));

        foreach (Type payloadType in payloadTypes)
        {
            _payloadTypeLocks.Add(payloadType, new SemaphoreSlim(1));
            _payloadEvents.Add(payloadType, new Queue<int>(MAX_QUEUE_ELEMENTS));
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> HandlePayloadAsync<T>(T payload, CancellationToken ct = default)
        where T : IPayload
    {
        Type type = typeof(T);

        if (!_payloadTypeLocks.TryGetValue(type, out SemaphoreSlim? semaphore))
            throw new InvalidOperationException("Has not been initialized with type " + type);

        await semaphore.WaitAsync(ct);

        if (!_payloadEvents.TryGetValue(type, out Queue<int>? seenEvents))
            throw new InvalidOperationException("Has not been initialized with type " + type);

        if (seenEvents.Count >= MAX_QUEUE_ELEMENTS)
            seenEvents.Dequeue();

        int hash = payload.GetHashCode();
        bool result;

        if (seenEvents.Contains(hash))
        {
            _logger.LogWarning("Preventing dispatch of duplicate event {Event}", payload);
            result = true;
        }
        else
        {
            seenEvents.Enqueue(hash);
            result = false;
        }

        semaphore.Release();
        return result;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (SemaphoreSlim semaphore in _payloadTypeLocks.Values)
            semaphore.Dispose();
    }
}
