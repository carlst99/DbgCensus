using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Abstractions.Objects;
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
    /// <summary>
    /// <inheritdoc />Events are dispatched to registered instances of <see cref="ICensusEventHandler{TEvent}"/>.
    /// </summary>
    public sealed class EventHandlingEventStreamClient : CensusEventStreamClient
    {
        private readonly IEventHandlerTypeRepository _eventHandlerRepository;
        private readonly IServiceMessageTypeRepository _serviceMessageObjectRepository;
        private readonly ConcurrentQueue<Task> _dispatchedEventQueue;

        /// <summary>
        /// Initialises a new instance of the <see cref="EventHandlingEventStreamClient"/> class.
        /// </summary>
        /// <inheritdoc cref="CensusEventStreamClient(ILogger{CensusEventStreamClient}, ClientWebSocket, JsonSerializerOptions, JsonSerializerOptions)"/>
        /// <param name="eventHandlerTypeRepository">The repository of <see cref="ICensusEventHandler{TEvent}"/> types.</param>
        /// <param name="eventStreamObjectTypeRepository">The repository of <see cref="IEventStreamObject"/> types.</param>
        /// <param name="services">The <see cref="IServiceProvider"/>.</param>
        public EventHandlingEventStreamClient(
            ILogger<EventHandlingEventStreamClient> logger,
            IServiceProvider services,
            JsonSerializerOptions deserializerOptions,
            JsonSerializerOptions serializerOptions,
            IEventHandlerTypeRepository eventHandlerTypeRepository,
            IServiceMessageTypeRepository eventStreamObjectTypeRepository)
            : base(logger, services, deserializerOptions, serializerOptions)
        {
            _eventHandlerRepository = eventHandlerTypeRepository;
            _serviceMessageObjectRepository = eventStreamObjectTypeRepository;

            _dispatchedEventQueue = new ConcurrentQueue<Task>();
        }

        /// <summary>
        /// <inheritdoc />Furthermore, finalises any event handlers that have not yet finished processing.
        /// </summary>
        /// <inheritdoc />
        public override async Task StopAsync()
        {
            foreach (Task runningEvent in _dispatchedEventQueue)
                await FinaliseDispatchedEvent(runningEvent).ConfigureAwait(false);

            await base.StopAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
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
                    _logger.LogWarning("An event with an unspecified service and/or type has been received. An UnknownEvent object will be dispatched.");
                    BeginEventDispatch(new UnknownEvent(jsonResponse.RootElement.GetRawText()), ct);
                    return;
                }

                if (censusService == "event" && censusType == "serviceMessage")
                {
                    DispatchServiceMessage(jsonResponse.RootElement, ct);
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
                else
                {
                    // Get 
                }
            }
            else
            {
                _logger.LogWarning("An unknown event was received from the Census event stream. An UnknownEvent object will be dispatched.");
                BeginEventDispatch(new UnknownEvent(jsonResponse.RootElement.GetRawText()), ct);
            }

            eventStream.Dispose();
        }

        /// <summary>
        /// Attempts to dispatch a service message event.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <param name="ct"></param>
        private void DispatchServiceMessage(JsonElement rootElement, CancellationToken ct = default)
        {
            // Attempt to get the payload element
            if (!rootElement.TryGetProperty("payload", out JsonElement payloadElement))
            {
                _logger.LogWarning("A service message was received that did not contain a payload. An unknown event will be dispatched.");
                BeginEventDispatch(new UnknownEvent(rootElement.GetRawText()), ct);
                return;
            }

            // Attempt to get the event name element
            if (!payloadElement.TryGetProperty("event_name", out JsonElement eventNameElement))
            {
                _logger.LogWarning("A service message was received that did not contain a valid payload. An unknown event will be dispatched.");
                BeginEventDispatch(new UnknownEvent(rootElement.GetRawText()), ct);
                return;
            }

            // Attempt to get the event name
            string? eventName = eventNameElement.GetString();
            if (eventName is null)
            {
                _logger.LogWarning("An event with no name was received. An unknown event will be dispatched.");
                BeginEventDispatch(new UnknownEvent(rootElement.GetRawText()), ct);
                return;
            }

            // Attempt to get the type of service message that represents this event
            if (!_serviceMessageObjectRepository.TryGet(eventName, out Type? serviceMessageType))
            {
                _logger.LogWarning("A ServiceMessage object has not been registered for the census event {event}", eventName);
                return;
            }

            // Deserialise to the service message object and dispatch an event
            try
            {
                object? serviceMessageObject = JsonSerializer.Deserialize(rootElement.GetRawText(), serviceMessageType, _jsonDeserializerOptions);
                if (serviceMessageObject is null)
                {
                    _logger.LogError("Could not deserialise websocket event to the type {type}. Raw response: {raw}", serviceMessageType, rootElement.GetRawText());
                    return;
                }
                BeginEventDispatch(serviceMessageType, serviceMessageObject, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispatch event");
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="DispatchEventAsync{T}(T, CancellationToken)"/> and dispatches an event.
        /// </summary>
        /// <typeparam name="TEvent">The type of <see cref="IEventStreamObject"/> to dispatch.</typeparam>
        /// <param name="eventObject">The event object to dispatch.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
        private void BeginEventDispatch<TEvent>(TEvent eventObject, CancellationToken ct = default) where TEvent : IEventStreamObject
            => BeginEventDispatch(typeof(TEvent), eventObject, ct);

        /// <summary>
        /// Creates an instance of <see cref="DispatchEventAsync{T}(T, CancellationToken)"/> and dispatches an event.
        /// </summary>
        /// <param name="eventType">The type of <see cref="IEventStreamObject"/> to dispatch.</param>
        /// <param name="eventObject">The evnet object to dispatch.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
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

        /// <summary>
        /// Dispatches an event to all appropriate event handlers.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IEventStreamObject"/> to dispatch.</typeparam>
        /// <param name="eventObject">The event object to dispatch.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Constructs a <see cref="MethodInfo"/> instance of the <see cref="DispatchEventAsync{T}(T, CancellationToken)"/> method.
        /// </summary>
        /// <param name="eventType">The type of event that will be dispatched through the method.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Logs any errors that occured while executing the event handler.
        /// </summary>
        /// <param name="eventTask">The task representing the event handling operation.</param>
        /// <returns></returns>
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
