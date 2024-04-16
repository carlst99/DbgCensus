using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.Objects.Commands;
using DbgCensus.EventStream.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream;

/// <summary>
/// <inheritdoc cref="IEventStreamClient"/>
/// Reconnection in the case of a failure is handled automatically.
/// </summary>
public abstract class BaseEventStreamClient : IEventStreamClient, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the size of the buffer used to send and receive data in chunks.
    /// </summary>
    protected const int SOCKET_BUFFER_SIZE = 4096;

    /// <summary>
    /// Gets the keep-alive interval for the websocket.
    /// </summary>
    protected static readonly TimeSpan KeepAliveInterval = TimeSpan.FromSeconds(20);

    private readonly ILogger<BaseEventStreamClient> _logger;
    private readonly SemaphoreSlim _sendSemaphore;
    private readonly Utf8JsonWriter _sendJsonWriter;

    protected readonly IServiceProvider _services;
    protected readonly EventStreamOptions _options;
    protected readonly JsonSerializerOptions _jsonDeserializerOptions;
    protected readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// The constructed endpoint to connect to.
    /// </summary>
    protected readonly Uri _endpoint;

    private ArrayBufferWriter<byte> _sendBuffer;

    protected ClientWebSocket _webSocket;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsDisposed { get; protected set; }

    /// <inheritdoc />
    public bool IsRunning { get; protected set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="BaseEventStreamClient"/> class.
    /// </summary>
    /// <param name="name">The identifying name of this client.</param>
    /// <param name="logger">The logging interface to use.</param>
    /// <param name="services">The service provider, used to retrieve <see cref="ClientWebSocket"/> instances.</param>
    /// <param name="options">The options used to configure the client.</param>
    /// <param name="jsonSerializerOptions">The JSON serializer options to use when de/serializing payloads.</param>
    protected BaseEventStreamClient
    (
        string name,
        ILogger<BaseEventStreamClient> logger,
        IServiceProvider services,
        IOptions<EventStreamOptions> options,
        IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions
    )
    {
        if (string.IsNullOrEmpty(options.Value.ServiceId))
            throw new ArgumentNullException(nameof(options), "The provided service ID cannot be null or empty");

        if (options.Value.ReconnectionDelayMilliseconds < 1)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(options),
                options.Value.ReconnectionDelayMilliseconds,
                "Reconnection delay cannot be less than one millisecond"
            );
        }

        Name = name;
        _logger = logger;
        _services = services;
        _options = options.Value;
        _webSocket = services.GetRequiredService<ClientWebSocket>();

        _sendSemaphore = new SemaphoreSlim(1, 1);
        _sendBuffer = new ArrayBufferWriter<byte>(SOCKET_BUFFER_SIZE);

        _sendJsonWriter = new Utf8JsonWriter
        (
            _sendBuffer,
            new JsonWriterOptions { SkipValidation = true } // The JSON Serializer should handle everything correctly
        );

        _jsonDeserializerOptions = jsonSerializerOptions.Get(Constants.JsonDeserializationOptionsName);
        _jsonSerializerOptions = jsonSerializerOptions.Get(Constants.JsonSerializationOptionsName);

        UriBuilder builder = new(_options.RootEndpoint)
        {
            Path = "streaming",
            Query = $"environment={_options.Environment}&service-id=s:{_options.ServiceId}"
        };

        _endpoint = builder.Uri;
    }

    /// <inheritdoc />
    public virtual async Task StartAsync(CancellationToken ct = default)
    {
        DoDisposeChecks();

        if (IsRunning)
            return;

        try
        {
            await ConnectWebsocket(ct).ConfigureAwait(false);

            IsRunning = true;
            _logger.LogInformation("Listening for events...");

            await StartListeningAsync(ct).ConfigureAwait(false);
        }
        catch
        {
            await StopAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public virtual async Task StopAsync()
    {
        DoDisposeChecks();
        IsRunning = false;

        try
        {
            if (_webSocket.State is WebSocketState.Open)
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to gracefully close websocket connection");
        }

        _webSocket.Dispose();

        _logger.LogInformation("Disconnected from the event stream websocket");
    }

    /// <inheritdoc />
    public virtual async Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : ICommand
    {
        DoDisposeChecks();

        if (_webSocket.State is not WebSocketState.Open)
            throw new InvalidOperationException("Websocket connection is not open.");

        try
        {
            bool entered = await _sendSemaphore.WaitAsync(1000, ct).ConfigureAwait(false);
            if (!entered)
                throw new OperationCanceledException("Could not enter semaphore.");

            JsonSerializer.Serialize(_sendJsonWriter, command, command.GetType(), _jsonSerializerOptions);
            ReadOnlyMemory<byte> data = _sendBuffer.WrittenMemory;

            int pageCount = (int)Math.Ceiling((double)data.Length / SOCKET_BUFFER_SIZE);

            for (int i = 0; i < pageCount; i++)
            {
                int offset = SOCKET_BUFFER_SIZE * i;
                int count = SOCKET_BUFFER_SIZE * (i + 1) < data.Length ? SOCKET_BUFFER_SIZE : data.Length - offset;
                bool isLastMessage = (i + 1) == pageCount;

                await _webSocket.SendAsync(data.Slice(offset, count), WebSocketMessageType.Text, isLastMessage, ct).ConfigureAwait(false);
            }

            if (data.Length > SOCKET_BUFFER_SIZE * 8)
            {
                // Reset the backing buffer, so we don't hold on to more memory than necessary
                _sendBuffer = new ArrayBufferWriter<byte>(SOCKET_BUFFER_SIZE);
                _sendJsonWriter.Reset(_sendBuffer);
            }
        }
        finally
        {
            _sendBuffer.Clear();
            _sendJsonWriter.Reset();
            _sendSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public virtual async Task ReconnectAsync(CancellationToken ct = default)
    {
        DoDisposeChecks();

        _logger.LogWarning
        (
            "Websocket was closed with status {Code} and description {Description}. Will attempt reconnection after cooldown...",
            _webSocket.CloseStatus,
            _webSocket.CloseStatusDescription
        );

        _webSocket.Dispose();

        await Task.Delay(_options.ReconnectionDelayMilliseconds, ct).ConfigureAwait(false);

        _logger.LogInformation("Attempting to reconnect websocket");
        await ConnectWebsocket(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Listens for messages from the websocket and automatically handles reconnection.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected virtual async Task StartListeningAsync(CancellationToken ct)
    {
        JsonReadOnlySequenceSegment? startSeg = null;

        try
        {
            while (IsRunning && !ct.IsCancellationRequested)
            {
                DoDisposeChecks();

                switch (_webSocket.State)
                {
                    // The streaming API occasionally closes your connection. We'll helpfully restore that.
                    case WebSocketState.Aborted or WebSocketState.CloseReceived or WebSocketState.Closed:
                        await ReconnectAsync(ct).ConfigureAwait(false);
                        continue;
                    // Give it a chance to connect
                    case WebSocketState.Connecting:
                        await Task.Delay(10, ct).ConfigureAwait(false);
                        continue;
                    // A graceful close, or request to close on our end, indicates things are wrapping up
                    case WebSocketState.CloseSent:
                        return;
                }

                ValueWebSocketReceiveResult result;
                JsonReadOnlySequenceSegment? endSeg = null;
                int endSegIndex = 0;

                do
                {
                    IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent(SOCKET_BUFFER_SIZE);
                    result = await _webSocket.ReceiveAsync(buffer.Memory, ct).ConfigureAwait(false);

                    // The streaming API occasionally closes your connection. We'll helpfully restore that.
                    if (result.MessageType is WebSocketMessageType.Close)
                    {
                        // Perhaps we've shutdown
                        if (!IsRunning)
                            return;

                        await ReconnectAsync(ct).ConfigureAwait(false);
                        continue;
                    }

                    endSegIndex = result.Count;
                    if (startSeg is null)
                        startSeg = new JsonReadOnlySequenceSegment(buffer, result.Count);
                    else if (endSeg is null)
                        endSeg = new JsonReadOnlySequenceSegment(startSeg, buffer, result.Count);
                    else
                        endSeg = new JsonReadOnlySequenceSegment(endSeg, buffer, result.Count);
                } while (!result.EndOfMessage);

                if (startSeg is null)
                    return;
                ReadOnlySequence<byte> sequence = endSeg is null
                    ? new ReadOnlySequence<byte>(startSeg.Memory)
                    : new ReadOnlySequence<byte>(startSeg, 0, endSeg, endSegIndex);

                await HandlePayloadAsync(sequence, ct).ConfigureAwait(false);

                startSeg.Dispose();
                startSeg = null;
            }
        }
        finally
        {
            startSeg?.Dispose();
        }
    }

    /// <summary>
    /// Gets a new <see cref="ClientWebSocket"/> instance and connects it to the <see cref="_endpoint"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task ConnectWebsocket(CancellationToken ct)
    {
        _webSocket = _services.GetRequiredService<ClientWebSocket>();
        if (!OperatingSystem.IsBrowser())
            _webSocket.Options.KeepAliveInterval = KeepAliveInterval;
        await _webSocket.ConnectAsync(_endpoint, ct).ConfigureAwait(false);

        _logger.LogInformation("Connected to event stream websocket");
    }

    /// <summary>
    /// Called when an event is received.
    /// </summary>
    /// <param name="data">
    /// The event data. The underlying segments will be disposed by the <see cref="BaseEventStreamClient"/>.
    /// </param>
    /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract ValueTask HandlePayloadAsync(ReadOnlySequence<byte> data, CancellationToken ct);

    /// <summary>
    /// Checks if this object has been disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void DoDisposeChecks()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(BaseEventStreamClient));
    }

    /// <summary>
    /// Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposeManaged">A value indicating whether to dispose of managed resources.</param>
    protected virtual void Dispose(bool disposeManaged)
    {
        if (IsDisposed)
            return;

        if (disposeManaged)
        {
            _sendJsonWriter.Dispose();
            _sendSemaphore.Dispose();
            _webSocket.Dispose();
        }

        IsDisposed = true;
    }

    /// <summary>
    /// Asynchronously disposes of managed resources.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (IsDisposed)
            return;

        await _sendJsonWriter.DisposeAsync().ConfigureAwait(false);
        _sendSemaphore.Dispose();
        _webSocket.Dispose();

        IsDisposed = true;
    }
}
