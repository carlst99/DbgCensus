using DbgCensus.Core.Json;
using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.Commands;
using DbgCensus.EventStream.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.Buffers;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream
{
    /// <summary>
    /// <inheritdoc cref="IEventStreamClient"/>Reconnection is handled automatically.
    /// </summary>
    public abstract class BaseEventStreamClient : IEventStreamClient
    {
        /// <summary>
        /// Gets the size of the buffer used to send and receive data in chunks.
        /// </summary>
        protected const int SOCKET_BUFFER_SIZE = 4096;

        /// <summary>
        /// The keep-alive interval for the websocket.
        /// </summary>
        protected const int KEEPALIVE_INTERVAL_SEC = 20;

        /// <summary>
        /// Gets the constant used to delimit payloads in the receive pipe.
        /// Defined as an ASCII end-of-transmission character.
        /// </summary>
        public const byte RECEIVE_PAYLOAD_DELIMITER = 4;

        private readonly ILogger<BaseEventStreamClient> _logger;
        private readonly SemaphoreSlim _sendSemaphore;
        private readonly Utf8JsonWriter _sendJsonWriter;

        protected readonly IServiceProvider _services;
        protected readonly EventStreamOptions _options;
        protected readonly RecyclableMemoryStreamManager _memoryStreamPool;
        protected readonly JsonSerializerOptions _jsonDeserializerOptions;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// The constructed endpoint to connect to.
        /// </summary>
        protected readonly Uri _endpoint;

        private ArrayBufferWriter<byte> _sendBuffer;

        protected ClientWebSocket _webSocket;
        protected SubscribeCommand? _initialSubscription;

        /// <inheritdoc />
        public string Name { get; protected set; }

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
        /// <param name="memoryStreamPool">The memory stream pool.</param>
        /// <param name="options">The options used to configure the client.</param>
        protected BaseEventStreamClient(
            string name,
            ILogger<BaseEventStreamClient> logger,
            IServiceProvider services,
            RecyclableMemoryStreamManager memoryStreamPool,
            IOptions<EventStreamOptions> options)
        {
            if (options.Value.ReconnectionDelayMilliseconds < 1)
                throw new ArgumentException("Reconnection delay cannot be less than one millisecond.", nameof(options));

            Name = name;
            _logger = logger;
            _services = services;
            _memoryStreamPool = memoryStreamPool;
            _options = options.Value;
            _webSocket = services.GetRequiredService<ClientWebSocket>();

            _sendSemaphore = new SemaphoreSlim(1, 1);
            _sendBuffer = new ArrayBufferWriter<byte>(SOCKET_BUFFER_SIZE);

            _sendJsonWriter = new Utf8JsonWriter
            (
                _sendBuffer,
                new JsonWriterOptions { SkipValidation = true } // The JSON Serializer should handle everything correctly
            );

            _jsonDeserializerOptions = new JsonSerializerOptions(_options.SerializationOptions);
            _jsonDeserializerOptions.AddCensusDeserializationOptions();

            _jsonSerializerOptions = new JsonSerializerOptions(_options.SerializationOptions);
            if (_jsonSerializerOptions.PropertyNamingPolicy is null)
                _jsonSerializerOptions.PropertyNamingPolicy = new CamelCaseJsonNamingPolicy();

            UriBuilder builder = new(_options.RootEndpoint);
            builder.Path = "streaming";
            builder.Query = $"environment={ _options.Environment }&service-id=s:{ _options.ServiceId }";
            _endpoint = builder.Uri;
        }

        /// <inheritdoc />
        public virtual async Task StartAsync(SubscribeCommand? initialSubscription = null, CancellationToken ct = default)
        {
            DoDisposeChecks();
            _initialSubscription = initialSubscription;

            if (IsRunning || _webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
                throw new InvalidOperationException("Client has already been started.");

            await ConnectWebsocket(ct).ConfigureAwait(false);
            IsRunning = true;

            _logger.LogInformation("Listening for events...");
            await StartListeningAsync(ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task StopAsync()
        {
            DoDisposeChecks();

            if (!IsRunning)
                throw new InvalidOperationException("Client has already been stopped.");

            IsRunning = false;

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ConfigureAwait(false);
            _logger.LogInformation("Disconnected from the event stream websocket.");
            _webSocket.Dispose();
        }

        /// <inheritdoc />
        public virtual async Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand
        {
            DoDisposeChecks();

            if (_webSocket?.State is not WebSocketState.Open)
                throw new InvalidOperationException("Websocket connection is not open.");

            try
            {
                JsonSerializer.Serialize(_sendJsonWriter, command, _jsonSerializerOptions);

                ReadOnlyMemory<byte> data = _sendBuffer.WrittenMemory;

                bool entered = await _sendSemaphore.WaitAsync(1000, ct).ConfigureAwait(false);
                if (!entered)
                {
                    throw new OperationCanceledException("Could not enter semaphore.");
                }

                int pageCount = (int)Math.Ceiling((double)data.Length / SOCKET_BUFFER_SIZE);

                for (int i = 0; i< pageCount; i++)
                {
                    int offset = SOCKET_BUFFER_SIZE * i;
                    int count = SOCKET_BUFFER_SIZE * (i + 1) < data.Length ? SOCKET_BUFFER_SIZE : data.Length - offset;
                    bool isLastMessage = (i + 1) == pageCount;

                    await _webSocket.SendAsync(data.Slice(offset, count), WebSocketMessageType.Text, isLastMessage, ct).ConfigureAwait(false);
                }

                if (data.Length > SOCKET_BUFFER_SIZE * 8)
                {
                    // Reset the backing buffer so we don't hold on to more memory than necessary
                    _sendBuffer = new ArrayBufferWriter<byte>(SOCKET_BUFFER_SIZE);
                    _sendJsonWriter.Reset(_sendBuffer);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _sendSemaphore.Release();
                _sendBuffer.Clear();
                _sendJsonWriter.Reset();
            }
        }

        /// <inheritdoc />
        public virtual async Task ReconnectAsync(CancellationToken ct = default)
        {
            DoDisposeChecks();

            _logger.LogWarning(
                "Websocket was closed with status {code} and description {description}. Will attempt reconnection after cooldown...",
                _webSocket.CloseStatus,
                _webSocket.CloseStatusDescription);

            _webSocket.Dispose();

            await Task.Delay(_options.ReconnectionDelayMilliseconds, ct).ConfigureAwait(false);

            _logger.LogInformation("Attempting to reconnect websocket.");
            await ConnectWebsocket(ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Listens for messages from the websocket and automatically handles reconnection.
        /// </summary>
        /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task StartListeningAsync(CancellationToken ct = default)
        {
            IMemoryOwner<byte> buffer = MemoryPool<byte>.Shared.Rent(SOCKET_BUFFER_SIZE);
            ValueWebSocketReceiveResult result;

            try
            {
                while (IsRunning && !IsDisposed)
                {
                    if (ct.IsCancellationRequested)
                        throw new TaskCanceledException();

                    switch (_webSocket.State)
                    {
                        // The streaming API occasionally closes your connection. We'll helpfully restore that.
                        case WebSocketState.Aborted or WebSocketState.CloseReceived:
                            await ReconnectAsync(ct).ConfigureAwait(false);
                            continue;
                        // Give it a chance to connect
                        case WebSocketState.Connecting:
                            await Task.Delay(10, ct).ConfigureAwait(false);
                            continue;
                        // A graceful close, or request to close on our end, indicates things are wrapping up
                        case WebSocketState.Closed or WebSocketState.CloseSent:
                            return;
                    }

                    MemoryStream stream = _memoryStreamPool.GetStream();

                    do
                    {
                        result = await _webSocket.ReceiveAsync(buffer.Memory, ct).ConfigureAwait(false);

                        // The streaming API occasionally closes your connection. We'll helpfully restore that.
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            if (IsRunning)
                            {
                                await ReconnectAsync(ct).ConfigureAwait(false);
                                continue;
                            }
                            else
                            {
                                return;
                            }
                        }

                        await stream.WriteAsync(buffer.Memory.Slice(0, result.Count), ct).ConfigureAwait(false);
                    } while (!result.EndOfMessage);

                    stream.Seek(0, SeekOrigin.Begin);
                    await HandleEvent(stream, ct).ConfigureAwait(false);
                }
            }
            finally
            {
                buffer.Dispose();
            }
        }

        /// <summary>
        /// Gets a new <see cref="ClientWebSocket"/> instance and connects it to the <see cref="_endpoint"/>, along with sending an initial subscription if specified.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task ConnectWebsocket(CancellationToken ct = default)
        {
            _webSocket = _services.GetRequiredService<ClientWebSocket>();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(KEEPALIVE_INTERVAL_SEC);
            await _webSocket.ConnectAsync(_endpoint, ct).ConfigureAwait(false);

            _logger.LogInformation("Connected to event stream websocket.");

            if (_initialSubscription is not null)
            {
                _logger.LogInformation("Sending initial subscription...");
                await SendCommandAsync(_initialSubscription, ct).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Called when an event is received.
        /// </summary>
        /// <param name="eventStream">The event data. You must dispose of this stream when you are finished with it.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> used to stop the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default);

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    try
                    {
                        _webSocket.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                        // This is fine, we dispose websockets when reconnecting or stopping
                    }
                }

                IsDisposed = true;
            }
        }

        /// <summary>
        /// Checks if this object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        protected void DoDisposeChecks()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BaseEventStreamClient));
        }
    }
}
