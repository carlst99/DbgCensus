using DbgCensus.EventStream.Abstractions.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.Abstractions
{
    /// <summary>
    /// Allows connecting to, and receiving data from the Census event stream.
    /// </summary>
    public interface ICensusEventStreamClient : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if this <see cref="CensusEventStreamClient"/> instance has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets a value indicating if the client is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Connects to the census event stream and begins receiving events. This method will return when <see cref="StopAsync"/> is called or the operationj= is cancelled.
        /// </summary>
        /// <param name="options">The <see cref="CensusEventStreamOptions"/> to use when connecting to the event stream.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task StartAsync(CensusEventStreamOptions options, CancellationToken ct = default);

        /// <summary>
        /// Disconnects from the event stream and stops listening for events.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task StopAsync();

        /// <summary>
        /// Sends a <see cref="IEventStreamCommand"/> to the event stream.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IEventStreamCommand"/> to send.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand;
    }
}
