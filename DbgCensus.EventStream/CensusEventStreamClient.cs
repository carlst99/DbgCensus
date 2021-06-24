using DbgCensus.Core.Exceptions;
using DbgCensus.Core.Json;
using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream
{
    /// <inheritdoc cref="ICensusEventStreamClient"/>
    public abstract class CensusEventStreamClient : ICensusEventStreamClient
    {
        /// <summary>
        /// Gets the size of the buffer used to send and receive data in chunks.
        /// </summary>
        private const int SOCKET_BUFFER_SIZE = 8192;

        /// <summary>
        /// The delay to use in between reconnection attempts.
        /// </summary>
        private const int RECONNECT_DELAY = 5000;

        /// <summary>
        /// The keep-alive interval for the websocket.
        /// </summary>
        private const int KEEPALIVE_INTERVAL_SEC = 20;

        private static readonly RecyclableMemoryStreamManager _memoryStreamPool = new();

        private readonly ILogger<CensusEventStreamClient> _logger;
        private readonly ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;

        private Uri? _endpoint;

        /// <inheritdoc />
        public bool IsDisposed { get; protected set; }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        protected CensusEventStreamClient(ILogger<CensusEventStreamClient> logger, ClientWebSocket webSocket)
            : this(logger, webSocket, new JsonSerializerOptions())
        { }

        protected CensusEventStreamClient(ILogger<CensusEventStreamClient> logger, ClientWebSocket webSocket, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _webSocket = webSocket;
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(KEEPALIVE_INTERVAL_SEC);

            _jsonOptions = new JsonSerializerOptions(jsonOptions)
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            if (_jsonOptions.PropertyNamingPolicy is null)
                _jsonOptions.PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy();

            _jsonOptions.Converters.Add(new BooleanJsonConverter());

            _jsonOptions.Converters.Add(new DateTimeJsonConverter());
            _jsonOptions.Converters.Add(new DateTimeOffsetJsonConverter());

            _jsonOptions.Converters.Add(new Int16JsonConverter());
            _jsonOptions.Converters.Add(new Int32JsonConverter());
            _jsonOptions.Converters.Add(new Int64JsonConverter());

            _jsonOptions.Converters.Add(new UInt16JsonConverter());
            _jsonOptions.Converters.Add(new UInt32JsonConverter());
            _jsonOptions.Converters.Add(new UInt64JsonConverter());
        }

        /// <inheritdoc />
        public virtual async Task StartAsync(CensusEventStreamOptions options, CancellationToken ct = default)
        {
            if (IsRunning || _webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
                throw new InvalidOperationException("Client has already been started.");

            IsRunning = true;

            UriBuilder builder = new(options.RootEndpoint);
            builder.Path = $"streaming?environment={ options.Environment }&service-id=s:{ options.ServiceId }";
            _endpoint = builder.Uri;

            await _webSocket.ConnectAsync(_endpoint, ct).ConfigureAwait(false);
            await StartListeningAsync(ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task StopAsync()
        {
            if (!IsRunning)
                return;

            IsRunning = false;

            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("Websocket connection is not open.");

            using MemoryStream stream = new();
            await JsonSerializer.SerializeAsync(stream, command, _jsonOptions, ct).ConfigureAwait(false);

            if (!stream.TryGetBuffer(out ArraySegment<byte> serialisedBuffer))
                throw new JsonException("Could not serialise command.");

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
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

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

                using MemoryStream stream = _memoryStreamPool.GetStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _webSocket.ReceiveAsync(buffer, ct).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ReconnectAsync(ct).ConfigureAwait(false);
                        continue;
                    }

                    stream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                await HandleEvent(stream, ct).ConfigureAwait(false);
            }
        }

        protected virtual async Task ReconnectAsync(CancellationToken ct = default)
        {
            await StopAsync().ConfigureAwait(false);
            _logger.LogError("Websocket was closed with status {code} and description {description}.", _webSocket.CloseStatus, _webSocket.CloseStatusDescription);

            await Task.Delay(RECONNECT_DELAY, ct).ConfigureAwait(false);

            _logger.LogInformation("Attempting to reconnect websocket.");
            await _webSocket.ConnectAsync(_endpoint!, ct).ConfigureAwait(false);
        }

        protected abstract Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default);

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _webSocket.Dispose();
                }

                IsDisposed = true;
            }
        }
    }
}
