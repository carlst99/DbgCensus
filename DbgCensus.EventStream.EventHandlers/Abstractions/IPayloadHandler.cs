using DbgCensus.EventStream.Abstractions.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Represents a non-generic marker interface for a payload handler.
/// See <see cref="IPayloadHandler{TEvent}"/> for the generic implementation you should use instead.
/// </summary>
public interface IPayloadHandler
{
}

/// <summary>
/// Represents a marker interface for a payload handler.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IPayloadHandler<TEvent> : IPayloadHandler where TEvent : IPayload
{
    /// <summary>
    /// Handles the event asynchronously.
    /// </summary>
    /// <param name="censusEvent">The event to respond to.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent censusEvent, CancellationToken ct = default);
}
