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
        private readonly ILogger<CensusEventStreamClient> _logger;
        private readonly ClientWebSocket _webSocket;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Gets a value indicating if this <see cref="CensusEventStreamClient"/> instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        public CensusEventStreamClient(ILogger<CensusEventStreamClient> logger)
            : this(logger, new JsonSerializerOptions())
        { }

        public CensusEventStreamClient(ILogger<CensusEventStreamClient> logger, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _webSocket = new ClientWebSocket();

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
        public virtual async Task ConnectAsync(CensusEventStreamOptions options, CancellationToken ct = default)
        {
            UriBuilder builder = new(options.RootEndpoint);
            builder.Path = $"streaming?environment={ options.Environment }&service-id=s:{ options.ServiceId }";

            await _webSocket.ConnectAsync(builder.Uri, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task DisconnectAsync(CancellationToken ct = default)
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task SendCommandAsync<T>(T command, CancellationToken ct = default) where T : IEventStreamCommand
        {
            using MemoryStream stream = new();
            await JsonSerializer.SerializeAsync(stream, command, _jsonOptions, ct).ConfigureAwait(false);

            if (!stream.TryGetBuffer(out ArraySegment<byte> serialisedBuffer))
                throw new JsonException("Could not serialise command.");

            await _webSocket.SendAsync(serialisedBuffer, WebSocketMessageType.Text, true, ct).ConfigureAwait(false);
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
