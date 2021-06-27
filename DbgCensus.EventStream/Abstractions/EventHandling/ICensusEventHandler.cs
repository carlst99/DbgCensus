using DbgCensus.EventStream.Abstractions.Objects;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.Abstractions.EventHandling
{
    public interface ICensusEventHandler
    {
    }

    public interface ICensusEventHandler<TEvent> : ICensusEventHandler where TEvent : IEventStreamObject
    {
        /// <summary>
        /// Handles the event asynchronously.
        /// </summary>
        /// <param name="censusEvent">The event to respond to.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
        /// <returns></returns>
        Task HandleAsync(TEvent censusEvent, CancellationToken ct = default);
    }
}
