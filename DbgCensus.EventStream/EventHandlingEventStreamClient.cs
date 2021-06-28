using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandling;
using DbgCensus.EventStream.Objects;
using DbgCensus.EventStream.Objects.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
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
        private readonly IServiceMessageTypeRepository _serviceMessageObjectRepository;
        private readonly IServiceProvider _services;
        private readonly ConcurrentQueue<Task> _dispatchedEventQueue;

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
            IServiceMessageTypeRepository eventStreamObjectTypeRepository,
            IServiceProvider services)
            : base(logger, webSocket, jsonOptions)
        {
            _eventHandlerRepository = eventHandlerRepository;
            _serviceMessageObjectRepository = eventStreamObjectTypeRepository;
            _services = services;

            _dispatchedEventQueue = new ConcurrentQueue<Task>();
        }

        /// <inheritdoc />
        public override async Task StopAsync()
        {
            foreach (Task runningEvent in _dispatchedEventQueue)
                await FinaliseDispatchedEvent(runningEvent).ConfigureAwait(false);

            await base.StopAsync().ConfigureAwait(false);
        }

        protected override async Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default)
        {
            if (_dispatchedEventQueue.TryDequeue(out Task? eventTask))
            {
                if (eventTask.IsCompleted)
                    await FinaliseDispatchedEvent(eventTask).ConfigureAwait(false);
                else
                    _dispatchedEventQueue.Enqueue(eventTask);
            }

            using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

            if (jsonResponse.RootElement.TryGetProperty("service", out JsonElement serviceElement) && jsonResponse.RootElement.TryGetProperty("type", out JsonElement typeElement))
            {
                string? censusService = serviceElement.GetString();
                string? censusType = typeElement.GetString();

                if (censusService is null || censusType is null)
                {
                    _logger.LogWarning("Received unknown event from service {service} of type {type}", censusService, censusType);
                    BeginEventDispatch(new UnknownEvent(jsonResponse.RootElement.GetRawText()), ct);
                    return;
                }

                if (censusService == "event" && censusType == "serviceMessage")
                {
                    if (!_serviceMessageObjectRepository.TryGet(censusService, censusType, out Type? serviceMessageType))
                    {
                        _logger.LogWarning("Received unknown event from service {service} of type {type}", censusService, censusType);
                        BeginEventDispatch(new UnknownEvent(jsonResponse.RootElement.GetRawText()), ct);
                        return;
                    }

                    object? serviceMessageObject = JsonSerializer.Deserialize(jsonResponse.RootElement.GetRawText(), serviceMessageType, _jsonOptions);
                    if (serviceMessageObject is null)
                    {
                        _logger.LogError("Could not deserialise websocket event. Raw response: {raw}", jsonResponse.RootElement.GetRawText());
                        return;
                    }
                    BeginEventDispatch(serviceMessageType, serviceMessageObject, ct);
                }
                else if (censusService == "event" && censusType == "heartbeat")
                {
                    Heartbeat? heartbeat = JsonSerializer.Deserialize<Heartbeat>(jsonResponse.RootElement.GetRawText());
                    if (heartbeat is null)
                    {
                        _logger.LogError("Could not deserialise websocket event. Raw response: {raw}", jsonResponse.RootElement.GetRawText());
                        return;
                    }
                    BeginEventDispatch(heartbeat, ct);
                }
            }

            // BIG DUM DUM
            // RECEIVED: {"payload":{"character_id":"5428011263437685377","event_name":"PlayerLogout","timestamp":"1624788175","world_id":"1"},"service":"event","type":"serviceMessage"}
        }

        private void BeginEventDispatch<TEvent>(TEvent eventObject, CancellationToken ct = default) where TEvent : notnull
            => BeginEventDispatch(typeof(TEvent), eventObject, ct);

        private void BeginEventDispatch(Type eventType, object eventObject, CancellationToken ct = default)
        {
            MethodInfo dispatchMethod = CreateDispatchMethod(eventType!);
            Task? dispatchTask = (Task?)dispatchMethod.Invoke(this, new object[] { eventObject!, ct });
            if (dispatchTask is null)
            {
                _logger.LogError("Failed to dispatch an event");
                return;
            }
            _dispatchedEventQueue.Enqueue(dispatchTask);
        }

        private async Task DispatchEventAsync<T>(T eventObject, CancellationToken ct = default) where T : IEventStreamObject
        {
            IReadOnlyList<Type> handlerTypes = _eventHandlerRepository.GetHandlerTypes<T>();
            if (handlerTypes.Count == 0)
                return;

            await Task.WhenAll
            (
                handlerTypes.Select
                (
                    async h =>
                    {
                        using IServiceScope scope = _services.CreateScope();
                        ICensusEventHandler<T> handler = (ICensusEventHandler<T>)scope.ServiceProvider.GetRequiredService(h);

                        try
                        {
                            await handler.HandleAsync(eventObject, ct).ConfigureAwait(false);
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

        private MethodInfo CreateDispatchMethod(Type eventType)
        {
            MethodInfo? dispatchMethod = GetType().GetMethod(nameof(DispatchEventAsync), BindingFlags.NonPublic | BindingFlags.Instance);
            if (dispatchMethod is null)
            {
                MissingMethodException ex = new(nameof(EventHandlingEventStreamClient), nameof(DispatchEventAsync));
                _logger.LogCritical(ex, "Failed to get the event dispatch method.");
                throw ex;
            }
            return dispatchMethod.MakeGenericMethod(new Type[] { eventType });
        }

        private async Task FinaliseDispatchedEvent(Task eventTask)
        {
            try
            {
                await eventTask.ConfigureAwait(false);
            }
            catch (AggregateException aex)
            {
                foreach (Exception ex in aex.InnerExceptions)
                {
                    if (ex is TaskCanceledException)
                        continue;

                    _logger.LogError(ex, "Error occured in an event handler");
                }
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, "Error occured in an event handler");
            }
        }
    }
}
