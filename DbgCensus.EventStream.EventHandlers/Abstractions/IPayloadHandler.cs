using DbgCensus.EventStream.Abstractions.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Represents a non-generic marker interface for a payload handler.
/// See <see cref="IPayloadHandler{TPayload}"/> for the generic implementation you should use instead.
/// </summary>
public interface IPayloadHandler
{
}

/// <summary>
/// Represents a marker interface for a payload handler.
/// </summary>
/// <typeparam name="TPayload"></typeparam>
public interface IPayloadHandler<TPayload> : IPayloadHandler where TPayload : IPayload
{
    /// <summary>
    /// Handles the event asynchronously.
    /// </summary>
    /// <param name="payload">The payload to respond to.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(TPayload payload, CancellationToken ct = default);
}
