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
    /// A value indicating whether or not to prevent the payload from being dispatched.
    /// </returns>
    ValueTask<bool> HandlePayloadAsync<T>(T payload, CancellationToken ct = default)
        where T : IPayload;
}
