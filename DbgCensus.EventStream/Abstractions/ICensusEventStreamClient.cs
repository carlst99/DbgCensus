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

        Task StartAsync(CensusEventStreamOptions options, CancellationToken ct = default);
        Task StopAsync();
        Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand;
    }
}
