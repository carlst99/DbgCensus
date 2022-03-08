using DbgCensus.EventStream.Abstractions.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Represents an interface for handling payloads before they are dispatched.
/// </summary>
public interface IPreDispatchHandler
{
    /// <summary>
    /// Handles the payload.
    /// </summary>
    /// <typeparam name="T">The type of the payload.</typeparam>
    /// <param name="payload">The payload.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>
    /// A tuple value containing an indicator on whether the payload should be
    /// prevented from being dispatched, and the potentially modified payload.
    /// </returns>
    ValueTask<(bool PreventDispatch, T Payload)> HandlePayloadAsync<T>(T payload, CancellationToken ct = default)
        where T : IPayload;
}
