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
            IEventHandlerRepository eventHandlerRepository)
            : base(logger, webSocket, jsonOptions)
        {
            _eventHandlerRepository = eventHandlerRepository;
            // TODO: Investigate period subscription refresh
            // TODO: Investigate CensusEventStreamClientFactory, similar to HttpClientFactory. So specific instances can be retrieved.
        }

        public override async Task StartAsync(CensusEventStreamOptions options, CancellationToken ct = default)
        {
            await base.StartAsync(options, ct).ConfigureAwait(false);

            Attribute[] attrs = Attribute.GetCustomAttributes(Assembly.GetExecutingAssembly(), typeof(EventStreamObjectAttribute));

            foreach (Attribute attr in attrs)
            {
                attr.
            }
            IEnumerable<EventStreamObjectAttribute> attributes = Assembly.GetExecutingAssembly().GetCustomAttributes<EventStreamObjectAttribute>();

            foreach (EventStreamObjectAttribute attr in attributes)
            {
            }
        }

        protected override async Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default)
        {
            using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

            // TODO: Discover type, build responder system
        }

        private IEnumerable<Tuple<EventStreamObjectAttribute, Type>> GetEventStreamObjectTypes()
        {
            string? assemblyOfAttribute = typeof(EventStreamObjectAttribute).Assembly.GetName().Name;
            if (string.IsNullOrEmpty(assemblyOfAttribute))
                return Array.Empty<Tuple<EventStreamObjectAttribute, Type>>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name != assemblyOfAttribute || !assembly.GetR)
            }
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
