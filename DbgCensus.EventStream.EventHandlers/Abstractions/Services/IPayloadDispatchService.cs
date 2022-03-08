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
    /// Dispatches an event to all appropriate payload handlers.
    /// </summary>
    /// <typeparam name="T">The abstract type of the payload.</typeparam>
    /// <param name="payload">The payload to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the handlers.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DispatchPayloadAsync<T>
    (
        T payload,
        IPayloadContext context,
        CancellationToken ct = default
    ) where T : IPayload;
}
