using DbgCensus.EventStream.Abstractions.Objects.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.Abstractions;

/// <summary>
/// Allows connecting to, and receiving data from the Census event stream.
/// </summary>
public interface IEventStreamClient : IDisposable
{
    /// <summary>
    /// The unique name of the client.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating if this <see cref="BaseEventStreamClient"/> instance has been disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets a value indicating if the client is running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Connects to the census event stream and begins receiving events.
    /// This method will return when <see cref="StopAsync"/> is called or the operation is cancelled.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    /// Disconnects from the event stream and stops listening for events.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StopAsync();

    /// <summary>
    /// Sends a <see cref="ICommand"/> to the event stream.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ICommand"/> to send.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : ICommand;

    /// <summary>
    /// Closes and reconnects to the websocket. Can help in cases where Census stops pushing data for your subscription.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ReconnectAsync(CancellationToken ct = default);
}
