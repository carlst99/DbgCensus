using DbgCensus.EventStream.Abstractions.Commands;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.Abstractions
{
    public interface ICensusEventStreamClient : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if this <see cref="CensusEventStreamClient"/> instance has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the state of the websocket.
        /// </summary>
        WebSocketState State { get; }

        Task StartAsync(CensusEventStreamOptions options, CancellationToken ct = default);
        Task StopAsync();
        Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand;
    }
}
