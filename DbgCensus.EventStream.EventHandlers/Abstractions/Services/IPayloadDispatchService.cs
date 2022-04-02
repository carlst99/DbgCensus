using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers.Abstractions.Services;

/// <summary>
/// Represents a service for dispatching payloads.
/// </summary>
public interface IPayloadDispatchService
{
    /// <summary>
    /// Gets a value indicating whether or not this <see cref="IPayloadDispatchService"/> is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Enqueues a payload for dispatch.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    /// <param name="payload">The payload.</param>
    /// <param name="context">The associated context.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the potentially asynchronous operation.</returns>
    ValueTask EnqueuePayloadAsync<TPayload>
    (
        TPayload payload,
        IPayloadContext context,
        CancellationToken ct = default
    ) where TPayload : IPayload;

    /// <summary>
    /// Runs the dispatch service, allowing payloads to be enqueued and dispatched.
    /// This method will not return until it is cancelled.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RunAsync(CancellationToken ct = default);
}
