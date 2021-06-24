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
    public class CensusEventStreamClient : ICensusEventStreamClient
    {
        /// <summary>
        /// Gets the size of the buffer used to send and receive data in chunks.
        /// </summary>
        private const int SOCKET_BUFFER_SIZE = 8192;
        private static readonly RecyclableMemoryStreamManager _memoryStreamPool = new();

        private readonly ILogger<CensusEventStreamClient> _logger;
        private readonly ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;

        private CancellationTokenSource _listenerCancellationToken;

        /// <inheritdoc />
        public bool IsDisposed { get; protected set; }

        /// <inheritdoc />
        public WebSocketState State => _webSocket.State;

        public CensusEventStreamClient(ILogger<CensusEventStreamClient> logger)
            : this(logger, new JsonSerializerOptions())
        { }

        public CensusEventStreamClient(ILogger<CensusEventStreamClient> logger, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _webSocket = new ClientWebSocket();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _listenerCancellationToken = new CancellationTokenSource();

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
            if (_webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
                throw new InvalidOperationException("Client has already been started.");

            UriBuilder builder = new(options.RootEndpoint);
            builder.Path = $"streaming?environment={ options.Environment }&service-id=s:{ options.ServiceId }";

            await _webSocket.ConnectAsync(builder.Uri, ct).ConfigureAwait(false);

            _listenerCancellationToken = new CancellationTokenSource();
            ct.Register(() => _listenerCancellationToken.Cancel());
            await StartListening(_listenerCancellationToken.Token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task StopAsync()
        {
            if (_listenerCancellationToken.IsCancellationRequested)
                return;

            _listenerCancellationToken.Cancel();
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

        protected virtual async Task StartListening(CancellationToken ct = default)
        {
            byte[] buffer = new byte[SOCKET_BUFFER_SIZE];

            while (_webSocket.State == WebSocketState.Open)
            {
                using MemoryStream stream = _memoryStreamPool.GetStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _webSocket.ReceiveAsync(buffer, ct).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await StopAsync().ConfigureAwait(false);
                        // TODO: Notify of the closure
                        return;
                    }

                    stream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                JsonDocument jsonResponse = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);

                // TODO: Discover type, build responder system
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _webSocket.Dispose();
                    _listenerCancellationToken.Dispose();
                }

                IsDisposed = true;
            }
        }
    }
}
