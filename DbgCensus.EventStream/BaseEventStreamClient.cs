using DbgCensus.Core.Json;
using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.Commands;
using DbgCensus.EventStream.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        protected const int SOCKET_BUFFER_SIZE = 8192;

        /// <summary>
        /// The delay to use in between reconnection attempts.
        /// </summary>
        protected const int RECONNECT_DELAY = 5000;

        /// <summary>
        /// The keep-alive interval for the websocket.
        /// </summary>
        protected const int KEEPALIVE_INTERVAL_SEC = 20;

        protected static readonly RecyclableMemoryStreamManager _memoryStreamPool = new();

        protected readonly ILogger<BaseEventStreamClient> _logger;
        protected readonly IServiceProvider _services;
        protected readonly EventStreamOptions _options;
        protected readonly JsonSerializerOptions _jsonDeserializerOptions;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;

        protected Uri? _endpoint;
        protected ClientWebSocket _webSocket;

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
        /// <param name="services">The service provider.</param>
        /// <param name="options">The options used to configure the client.</param>
        protected BaseEventStreamClient(
            string name,
            ILogger<BaseEventStreamClient> logger,
            IServiceProvider services,
            EventStreamOptions options)
        {
            Name = name;
            _logger = logger;
            _services = services; // TODO: Is this necessary? It might make more sense to have a ClientWebSocketFactory
            _options = options;
            _webSocket = services.GetRequiredService<ClientWebSocket>();

            _jsonDeserializerOptions = new JsonSerializerOptions(_options.DeserializationOptions)
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            if (_jsonDeserializerOptions.PropertyNamingPolicy is null)
                _jsonDeserializerOptions.PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy();

            _jsonDeserializerOptions.Converters.Add(new BooleanJsonConverter());
            _jsonDeserializerOptions.Converters.Add(new JsonStringEnumConverter());

            _jsonDeserializerOptions.Converters.Add(new DateTimeJsonConverter());
            _jsonDeserializerOptions.Converters.Add(new DateTimeOffsetJsonConverter());

            _jsonSerializerOptions = new JsonSerializerOptions(_options.SerializationOptions);
            if (_jsonSerializerOptions.PropertyNamingPolicy is null)
                _jsonSerializerOptions.PropertyNamingPolicy = new CamelCaseJsonNamingPolicy();
        }

        /// <inheritdoc />
        public virtual async Task StartAsync(SubscribeCommand? initialSubscription = null, CancellationToken ct = default)
        {
            if (IsRunning || _webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
                throw new InvalidOperationException("Client has already been started.");

            UriBuilder builder = new(_options.RootEndpoint);
            builder.Path = "streaming";
            builder.Query = $"environment={ _options.Environment }&service-id=s:{ _options.ServiceId }";
            _endpoint = builder.Uri;

            await ConnectWebsocket(ct).ConfigureAwait(false);
            IsRunning = true;
            _logger.LogInformation("Connected to event stream websocket.");

            if (initialSubscription is not null)
            {
                _logger.LogInformation("Sending initial subscription...");
                await SendCommandAsync(initialSubscription, ct).ConfigureAwait(false);
            }

            _logger.LogInformation("Listening for events...");
            await StartListeningAsync(ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task StopAsync()
        {
            if (!IsRunning)
                return;

            IsRunning = false;

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ConfigureAwait(false);
            _webSocket.Dispose();
            _logger.LogInformation("Disconnected from the event stream websocket.");
        }

        /// <inheritdoc />
        public virtual async Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("Websocket connection is not open.");

            using MemoryStream stream = new();
            await JsonSerializer.SerializeAsync(stream, command, _jsonSerializerOptions, ct).ConfigureAwait(false);

            if (!stream.TryGetBuffer(out ArraySegment<byte> serialisedBuffer))
                throw new JsonException("Could not serialise command.");

            _logger.LogInformation("Sending census command: {command}", Encoding.UTF8.GetString(serialisedBuffer));

            int pageCount = (int)Math.Ceiling((double)serialisedBuffer.Count / SOCKET_BUFFER_SIZE);

            for (int i = 0; i < pageCount; i++)
            {
                int offset = SOCKET_BUFFER_SIZE * i;
                int count = SOCKET_BUFFER_SIZE * (i + 1) < serialisedBuffer.Count ? SOCKET_BUFFER_SIZE : serialisedBuffer.Count - offset;
                bool isLastMessage = (i + 1) == pageCount;

                await _webSocket.SendAsync(serialisedBuffer.Slice(offset, count), WebSocketMessageType.Text, isLastMessage, ct).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public virtual async Task ReconnectAsync(CancellationToken ct = default)
        {
            await StopAsync().ConfigureAwait(false);
            _logger.LogWarning("Websocket was closed with status {code} and description {description}.", _webSocket.CloseStatus, _webSocket.CloseStatusDescription);

            await Task.Delay(RECONNECT_DELAY, ct).ConfigureAwait(false);

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
            byte[] buffer = new byte[SOCKET_BUFFER_SIZE];

            while (IsRunning)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                if (_webSocket.State != WebSocketState.Open)
                {
                    await ReconnectAsync(ct).ConfigureAwait(false);
                    continue;
                }

                MemoryStream stream = _memoryStreamPool.GetStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _webSocket.ReceiveAsync(buffer, ct).ConfigureAwait(false);

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

                    stream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                stream.Seek(0, SeekOrigin.Begin);
                await HandleEvent(stream, ct).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets a new <see cref="ClientWebSocket"/> instance and connects it to the <see cref="_endpoint"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected virtual async Task ConnectWebsocket(CancellationToken ct = default)
        {
            _webSocket = _services.GetRequiredService<ClientWebSocket>();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(KEEPALIVE_INTERVAL_SEC);
            await _webSocket.ConnectAsync(_endpoint!, ct).ConfigureAwait(false);
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
    }
}
