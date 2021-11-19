using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects;
using DbgCensus.EventStream.EventHandlers.Services;
using DbgCensus.EventStream.Objects.Control;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers;

/// <summary>
/// <inheritdoc />
/// Events are dispatched to registered instances of <see cref="IPayloadHandler{TEvent}"/>.
/// </summary>
public sealed class EventHandlingEventStreamClient : BaseEventStreamClient
{
    private readonly ILogger<EventHandlingEventStreamClient> _logger;
    private readonly IPayloadHandlerTypeRepository _eventHandlerRepository;
    private readonly IPayloadTypeRepository _serviceMessageObjectRepository;
    private readonly ConcurrentQueue<Task> _dispatchedEventQueue;

    /// <summary>
    /// Initialises a new instance of the <see cref="EventHandlingEventStreamClient"/> class.
    /// </summary>
    /// <param name="name">The identifying name of this client.</param>
    /// <param name="logger">The logging interface to use.</param>
    /// <param name="services">The service provider.</param>
    /// <param name="memoryStreamPool">The memory stream pool.</param>
    /// <param name="options">The options used to configure the client.</param>
    /// <param name="eventHandlerTypeRepository">The repository of <see cref="IPayloadHandler{TEvent}"/> types.</param>
    /// <param name="eventStreamObjectTypeRepository">The repository of <see cref="IEventStreamObject"/> types.</param>
    public EventHandlingEventStreamClient(
        string name,
        ILogger<EventHandlingEventStreamClient> logger,
        IServiceProvider services,
        RecyclableMemoryStreamManager memoryStreamPool,
        IOptions<EventStreamOptions> options,
        IPayloadHandlerTypeRepository eventHandlerTypeRepository,
        IPayloadTypeRepository eventStreamObjectTypeRepository)
        : base(name, logger, services, memoryStreamPool, options)
    {
        _logger = logger;
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
        await base.StopAsync().ConfigureAwait(false);

        foreach (Task runningEvent in _dispatchedEventQueue)
            await FinaliseDispatchedEvent(runningEvent).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task HandleEvent(MemoryStream eventStream, CancellationToken ct = default)
    {
        try
        {
            // Attempt to finalise one event handler
            if (_dispatchedEventQueue.TryDequeue(out Task? eventTask))
            {
                if (eventTask.IsCompleted)
                    await FinaliseDispatchedEvent(eventTask).ConfigureAwait(false);
                else
                    _dispatchedEventQueue.Enqueue(eventTask);
            }

            using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

            // Handle properly formed events
            if
            (
                jsonResponse.RootElement.TryGetProperty("service", out JsonElement serviceElement)
                && jsonResponse.RootElement.TryGetProperty("type", out JsonElement typeElement)
            )
            {
                string? censusService = serviceElement.GetString();
                string? censusType = typeElement.GetString();

                if (censusService is null || censusType is null)
                {
                    _logger.LogWarning("An event with an unspecified service and/or type has been received. An UnknownEvent object will be dispatched.");
                    DispatchUnknownEvent(jsonResponse.RootElement.GetRawText(), ct);
                }
                else if (censusService == "event" && censusType == "serviceMessage")
                {
                    DispatchServiceMessage(jsonResponse.RootElement, ct);
                }
                else if (censusService == "event" && censusType == "heartbeat")
                {
                    DeserializeAndBeginEventDispatch<Heartbeat>(jsonResponse.RootElement, ct);
                }
                else if (censusService == "event" && censusType == "serviceStateChanged")
                {
                    DeserializeAndBeginEventDispatch<ServiceStateChanged>(jsonResponse.RootElement, ct);
                }
                else if (censusService == "push" && censusType == "connectionStateChanged")
                {
                    DeserializeAndBeginEventDispatch<ConnectionStateChanged>(jsonResponse.RootElement, ct);
                }
            }
            else if (jsonResponse.RootElement.TryGetProperty("subscription", out JsonElement subscriptionElement)) // Handle subscription events
            {
                DeserializeAndBeginEventDispatch<Subscription>(subscriptionElement, ct);
            }
            else if (jsonResponse.RootElement.TryGetProperty("send this for help", out _)) // Ignore the 'send for help'
            {
                // No need to process this
            }
            else // Handle unknown events
            {
                _logger.LogWarning($"An unknown event was received from the Census event stream. An {nameof(UnknownPayload)} object will be dispatched.");
                DispatchUnknownEvent(jsonResponse.RootElement.GetRawText(), ct);
            }
        }
        finally
        {
            eventStream.Dispose();
        }
    }

    /// <summary>
    /// Attempts to dispatch a service message event.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="ct"></param>
    private void DispatchServiceMessage(JsonElement element, CancellationToken ct = default)
    {
        // Attempt to get the payload element
        if (!element.TryGetProperty("payload", out JsonElement payloadElement))
        {
            _logger.LogWarning("A service message was received that did not contain a payload. An unknown event will be dispatched.");
            DispatchUnknownEvent(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the event name element
        if (!payloadElement.TryGetProperty("event_name", out JsonElement eventNameElement))
        {
            _logger.LogWarning("A service message was received with a malformed payload (Missing 'event_name'). An unknown event will be dispatched.");
            DispatchUnknownEvent(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the event name
        string? eventName = eventNameElement.GetString();
        if (eventName is null)
        {
            _logger.LogWarning("A service message was received with a malformed payload (NULL 'event_name'). An unknown event will be dispatched.");
            DispatchUnknownEvent(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the type of service message that represents this event
        if (!_serviceMessageObjectRepository.TryGet(eventName, out (Type abstractEvent, Type implementingEvent)? typeMap))
        {
            _logger.LogWarning("A ServiceMessage object has not been registered for the received service message event {event}", eventName);
            return;
        }

        // Deserialise to the service message object and dispatch an event
        Type serviceMessageType = typeof(ServiceMessage<>);
        serviceMessageType = serviceMessageType.MakeGenericType(typeMap.Value.implementingEvent);

        DeserializeAndBeginEventDispatch(serviceMessageType, element, ct);
    }

    /// <summary>
    /// Dispatches an <see cref="UnknownPayload"/>.
    /// </summary>
    /// <param name="rawJson">The raw JSON that was received.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the dispatch handlers.</param>
    private void DispatchUnknownEvent(string rawJson, CancellationToken ct)
        => BeginEventDispatch
        (
            new UnknownPayload(Name, rawJson),
            new PayloadContext(Name),
            ct
        );

    private void DeserializeAndBeginEventDispatch<T>(JsonElement element, CancellationToken ct) where T : IPayload
    {
        try
        {
            T? deserialized = JsonSerializer.Deserialize<T>(element.GetRawText(), _jsonDeserializerOptions);
            if (deserialized is null)
            {
                _logger.LogError("Could not deserialise websocket event. Raw response: {raw}", element.GetRawText());
                return;
            }

            BeginEventDispatch
            (
                deserialized,
                new PayloadContext(Name),
                ct
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize and dispatch event");
        }
    }

    private void DeserializeAndBeginEventDispatch(Type eventType, JsonElement element, CancellationToken ct)
    {
        try
        {
            object? deserialized = JsonSerializer.Deserialize(element.GetRawText(), eventType, _jsonDeserializerOptions);
            if (deserialized is null)
            {
                _logger.LogError("Could not deserialise websocket event. Raw response: {raw}", element.GetRawText());
                return;
            }

            BeginEventDispatch
            (
                eventType,
                deserialized,
                new PayloadContext(Name),
                ct
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize and dispatch event.");
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="DispatchEventAsync{T}(T, CancellationToken)"/> and dispatches an event.
    /// </summary>
    /// <typeparam name="TEvent">The type of <see cref="IEventStreamObject"/> to dispatch.</typeparam>
    /// <param name="eventObject">The event object to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
    private void BeginEventDispatch<TEvent>
    (
        TEvent eventObject,
        IPayloadContext context,
        CancellationToken ct
    ) where TEvent : IPayload
        => BeginEventDispatch(typeof(TEvent), eventObject, context, ct);

    /// <summary>
    /// Creates an instance of <see cref="DispatchEventAsync{T}(T, CancellationToken)"/> and dispatches an event.
    /// </summary>
    /// <param name="eventType">The type of <see cref="IEventStreamObject"/> to dispatch.</param>
    /// <param name="eventObject">The event object to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
    private void BeginEventDispatch
    (
        Type eventType,
        object eventObject,
        IPayloadContext context,
        CancellationToken ct
    )
    {
        MethodInfo dispatchMethod = CreateDispatchMethod(eventType);
        Task? dispatchTask = (Task?)dispatchMethod.Invoke(this, new object[] { eventObject, context, ct });

        if (dispatchTask is null)
        {
            _logger.LogError("Failed to dispatch an event.");
            return;
        }

        _dispatchedEventQueue.Enqueue(dispatchTask);
    }

    /// <summary>
    /// Constructs a <see cref="MethodInfo"/> instance of the <see cref="DispatchEventAsync{T}(T, IPayloadContext, CancellationToken)"/> method.
    /// </summary>
    /// <param name="eventType">The type of event that will be dispatched through the method.</param>
    /// <returns>The method info.</returns>
    private MethodInfo CreateDispatchMethod(Type eventType)
    {
        MethodInfo? dispatchMethod = GetType().GetMethod(nameof(DispatchEventAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        if (dispatchMethod is null)
        {
            MissingMethodException ex = new(nameof(EventHandlingEventStreamClient), nameof(DispatchEventAsync));
            _logger.LogCritical(ex, "Failed to get the event dispatch method.");
            throw ex;
        }

        return dispatchMethod.MakeGenericMethod(eventType);
    }

    /// <summary>
    /// Dispatches an event to all appropriate event handlers. DO NOT call this directly.
    /// Use <see cref="CreateDispatchMethod(Type)"/> instead to ensure handlers do not block the receive queue.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IEventStreamObject"/> to dispatch.</typeparam>
    /// <param name="eventObject">The event object to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task DispatchEventAsync<T>
    (
        T eventObject,
        IPayloadContext context,
        CancellationToken ct = default
    ) where T : IPayload
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
                    await using AsyncServiceScope scope = _services.CreateAsyncScope();

                    scope.ServiceProvider.GetRequiredService<EventContextInjectionService>().Context = context;
                    IPayloadHandler<T> handler = (IPayloadHandler<T>)scope.ServiceProvider.GetRequiredService(h);

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
    /// Logs any errors that occured while executing the event handler.
    /// </summary>
    /// <param name="eventTask">The task representing the event handling operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
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
