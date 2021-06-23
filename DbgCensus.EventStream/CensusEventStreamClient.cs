using DbgCensus.Core.Exceptions;
using DbgCensus.Core.Json;
using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.Commands;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<CensusEventStreamClient> _logger;
        private readonly ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;

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
            UriBuilder builder = new(options.RootEndpoint);
            builder.Path = $"streaming?environment={ options.Environment }&service-id=s:{ options.ServiceId }";

            await _webSocket.ConnectAsync(builder.Uri, ct).ConfigureAwait(false);

            // Start listener
        }

        /// <inheritdoc />
        public virtual async Task StopAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand
        {
            if (_webSocket.State != WebSocketState.Open)
                throw new CensusException("Websocket connection is not open.");

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

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _webSocket.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                IsDisposed = true;
            }
        }
    }
}
