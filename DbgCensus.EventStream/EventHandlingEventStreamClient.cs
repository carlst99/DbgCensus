using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandling;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream
{
    /// <inheritdoc />
    public sealed class EventHandlingEventStreamClient : CensusEventStreamClient
    {
        private readonly IEventHandlerRepository _eventHandlerRepository;
        private readonly IServiceMessageTypeRepository _eventStreamObjectTypeRepository;

        /// <summary>
        /// Initialises a new instance of the <see cref="EventHandlingEventStreamClient"/> class.
        /// </summary>
        /// <param name="logger">The logging interface to use.</param>
        /// <param name="webSocket">The websocket to use.</param>
        /// <param name="jsonOptions">The JSON serialization options to use.</param>
        public EventHandlingEventStreamClient(
            ILogger<EventHandlingEventStreamClient> logger,
            ClientWebSocket webSocket,
            JsonSerializerOptions jsonOptions,
            IEventHandlerRepository eventHandlerRepository,
            IServiceMessageTypeRepository eventStreamObjectTypeRepository)
            : base(logger, webSocket, jsonOptions)
        {
            _eventHandlerRepository = eventHandlerRepository;
            _eventStreamObjectTypeRepository = eventStreamObjectTypeRepository;
            // TODO: Investigate period subscription refresh
            // TODO: Investigate CensusEventStreamClientFactory, similar to HttpClientFactory. So specific instances can be retrieved.
        }

        public override async Task StartAsync(CensusEventStreamOptions options, CancellationToken ct = default)
        {
            await base.StartAsync(options, ct).ConfigureAwait(false);
        }

        protected override async Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default)
        {
            using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

            // TODO: Discover type, build responder system

            // BIG DUM DUM
            // RECEIVED: {"payload":{"character_id":"5428011263437685377","event_name":"PlayerLogout","timestamp":"1624788175","world_id":"1"},"service":"event","type":"serviceMessage"}
        }

        private async Task DispatchEventAsync<T>(T eventObject, CancellationToken ct = default) where T : IEventStreamObject
        {
            IReadOnlyList<ICensusEventHandler<T>> handlers = _eventHandlerRepository.GetHandlers<T>();

            await Task.WhenAll
            (
                handlers.Select
                (
                    async h =>
                    {
                        try
                        {
                            await h.HandleAsync(eventObject, ct).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to execute event handler");
                            throw;
                        }
                        finally
                        {
                            if (h is IDisposable disposable)
                                disposable.Dispose();

                            if (h is IAsyncDisposable asyncDisposable)
                                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        }
                    }
                )
            ).ConfigureAwait(false);
        }
    }
}
