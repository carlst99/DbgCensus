using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Commands;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using DbgCensus.EventStream.EventHandlers.Objects;
using DbgCensus.EventStream.EventHandlers.Services;
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
    private readonly EventHandlingClientOptions _handlingOptions;
    private readonly IPayloadHandlerTypeRepository _handlerTypeRepository;
    private readonly IPayloadTypeRepository _payloadTypeRepository;
    private readonly ConcurrentQueue<Task> _dispatchedPayloadHandlerQueue;

    private CancellationTokenSource _dispatchCts;
    private long _lastSubscriptionRefresh;

    /// <summary>
    /// Gets the current subscription of this client.
    /// </summary>
    public ISubscribe? CurrentSubscription { get; private set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="EventHandlingEventStreamClient"/> class.
    /// </summary>
    /// <param name="name">The identifying name of this client.</param>
    /// <param name="logger">The logging interface to use.</param>
    /// <param name="services">The service provider.</param>
    /// <param name="baseOptions">The options used to configure the client.</param>
    /// <param name="handlingOptions">The options used to configure the client.</param>
    /// <param name="jsonSerializerOptions">The JSON serializer options to use when de/serializing payloads.</param>
    /// <param name="memoryStreamPool">The memory stream pool.</param>
    /// <param name="handlerTypeRepository">The payload handler type repository.</param>
    /// <param name="payloadTypeRepository">The payload type repository types.</param>
    public EventHandlingEventStreamClient
    (
        string name,
        ILogger<EventHandlingEventStreamClient> logger,
        IServiceProvider services,
        IOptions<EventStreamOptions> baseOptions,
        IOptions<EventHandlingClientOptions> handlingOptions,
        IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
        RecyclableMemoryStreamManager memoryStreamPool,
        IPayloadHandlerTypeRepository handlerTypeRepository,
        IPayloadTypeRepository payloadTypeRepository
    )
        : base(name, logger, services, baseOptions, jsonSerializerOptions, memoryStreamPool)
    {
        _logger = logger;
        _handlingOptions = handlingOptions.Value;
        _handlerTypeRepository = handlerTypeRepository;
        _payloadTypeRepository = payloadTypeRepository;

        _dispatchedPayloadHandlerQueue = new ConcurrentQueue<Task>();
        _dispatchCts = new CancellationTokenSource();
        _lastSubscriptionRefresh = DateTimeOffset.UtcNow.Ticks;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
        _dispatchCts = new CancellationTokenSource();

        await base.StartAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc />
    /// Furthermore, finalises any event handlers that have not yet finished processing.
    /// </summary>
    /// <inheritdoc />
    public override async Task StopAsync()
    {
        await base.StopAsync().ConfigureAwait(false);

        _dispatchCts.Cancel();

        foreach (Task runningEvent in _dispatchedPayloadHandlerQueue)
            await FinaliseDispatchedPayloadHandler(runningEvent).ConfigureAwait(false);

        _dispatchCts.Dispose();
    }

    public override Task SendCommandAsync<T>(T command, CancellationToken ct = default)
    {
        if (command is ISubscribe subscribeCommand)
            CurrentSubscription = subscribeCommand;

        return base.SendCommandAsync(command, ct);
    }

    /// <inheritdoc />
    protected override async Task HandlePayloadAsync(MemoryStream eventStream, CancellationToken ct)
    {
        try
        {
            // Attempt to finalise one payload handler
            if (_dispatchedPayloadHandlerQueue.TryDequeue(out Task? handlerTask))
            {
                if (handlerTask.IsCompleted)
                    await FinaliseDispatchedPayloadHandler(handlerTask).ConfigureAwait(false);
                else
                    _dispatchedPayloadHandlerQueue.Enqueue(handlerTask);
            }

            using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

            if (jsonResponse.RootElement.TryGetProperty("type", out JsonElement typeElement))
            {
                string? censusType = typeElement.GetString();

                if (censusType is null)
                {
                    _logger.LogWarning($"A payload with a null type has been received. An {nameof(UnknownPayload)} will be dispatched.");
                    DispatchUnknownPayload(jsonResponse.RootElement.GetRawText(), _dispatchCts.Token);
                }
                else if (censusType == "serviceMessage") // Further parsing is need to dispatch the encapsulated event payload
                {
                    DispatchServiceMessage(jsonResponse.RootElement, _dispatchCts.Token);
                }
                else if (_payloadTypeRepository.TryGet(censusType, out (Type AbstractType, Type ImplementingType)? typeMap))
                {
                    DeserializeAndDispatchPayload(typeMap.Value.AbstractType, typeMap.Value.ImplementingType, jsonResponse.RootElement, _dispatchCts.Token);
                }
                else
                {
                    _logger.LogWarning($"A payload with an unknown type has been received. An {nameof(UnknownPayload)} will be dispatched.");
                    DispatchUnknownPayload(jsonResponse.RootElement.GetRawText(), _dispatchCts.Token);
                }
            }
            else if (jsonResponse.RootElement.TryGetProperty("subscription", out JsonElement subscriptionElement))
            {
                if (!_payloadTypeRepository.TryGet("subscription", out (Type AbstractType, Type ImplementingType)? typeMap))
                {
                    _logger.LogError("Types for the 'subscription' payload have not been registerd to the payload type repository. This is an internal library error.");
                    return;
                }

                DeserializeAndDispatchPayload(typeMap.Value.AbstractType, typeMap.Value.ImplementingType, subscriptionElement, _dispatchCts.Token);
            }
            else if (jsonResponse.RootElement.TryGetProperty("send this for help", out _))
            {
                // No need to process this
            }
            else
            {
                _logger.LogWarning($"An unknown payload has been received. An {nameof(UnknownPayload)} will be dispatched.");
                DispatchUnknownPayload(jsonResponse.RootElement.GetRawText(), _dispatchCts.Token);
            }
        }
        finally
        {
            await eventStream.DisposeAsync().ConfigureAwait(false);
        }

        AttemptSubscriptionRefresh(ct);
    }

    /// <summary>
    /// Refreshes the current subscription if necessary.
    /// </summary>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private void AttemptSubscriptionRefresh(CancellationToken ct)
    {
        if (!IsRunning || CurrentSubscription is null)
            return;

        long lastSubRefreshTicks = Interlocked.Read(ref _lastSubscriptionRefresh);
        DateTimeOffset lastSubRefresh = new(lastSubRefreshTicks, TimeSpan.Zero);

        if (lastSubRefresh.Add(_handlingOptions.SubscriptionRefreshIntervalMilliseconds) > DateTimeOffset.UtcNow)
            return;

        // A little naughty! We don't need to await this though
        SendCommandAsync(CurrentSubscription, ct).ConfigureAwait(false);
        Interlocked.Exchange(ref _lastSubscriptionRefresh, DateTimeOffset.UtcNow.Ticks);
        _logger.LogTrace("Subscription refreshed.");
    }

    /// <summary>
    /// Attempts to dispatch a service message event.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="ct"></param>
    private void DispatchServiceMessage(JsonElement element, CancellationToken ct)
    {
        // Attempt to get the payload element
        if (!element.TryGetProperty("payload", out JsonElement payloadElement))
        {
            _logger.LogWarning("A service message was received that did not contain a payload. An unknown event will be dispatched.");
            DispatchUnknownPayload(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the event name element
        if (!payloadElement.TryGetProperty("event_name", out JsonElement eventNameElement))
        {
            _logger.LogWarning("A service message was received with a malformed payload (Missing 'event_name'). An unknown event will be dispatched.");
            DispatchUnknownPayload(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the event name
        string? eventName = eventNameElement.GetString();
        if (eventName is null)
        {
            _logger.LogWarning("A service message was received with a malformed payload (NULL 'event_name'). An unknown event will be dispatched.");
            DispatchUnknownPayload(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the type of service message that represents this event
        if (!_payloadTypeRepository.TryGet(eventName, out (Type AbstractType, Type ImplementingType)? typeMap))
        {
            _logger.LogWarning("Types for the received {event} event have not been registered to the payload type repository.", eventName);
            return;
        }

        DeserializeAndDispatchPayload(typeMap.Value.AbstractType, typeMap.Value.ImplementingType, payloadElement, ct);
    }

    /// <summary>
    /// Dispatches an <see cref="UnknownPayload"/>.
    /// </summary>
    /// <param name="rawJson">The raw JSON that was received.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the dispatch handlers.</param>
    private void DispatchUnknownPayload(string rawJson, CancellationToken ct)
        => BeginPayloadDispatch
        (
            typeof(IUnknownPayload),
            new UnknownPayload(Name, rawJson),
            new PayloadContext(Name),
            ct
        );

    /// <summary>
    /// Deserializes a payload and dispatches it.
    /// </summary>
    /// <param name="abstractType">The abstract type used by payload handlers.</param>
    /// <param name="implementingType">The type that implements the payload.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop any handlers associated with this payload.</param>
    private void DeserializeAndDispatchPayload
    (
        Type abstractType,
        Type implementingType,
        JsonElement payload,
        CancellationToken ct
    )
    {
        try
        {
            object? deserialized = payload.Deserialize(implementingType, _jsonDeserializerOptions);
            if (deserialized is null)
            {
                _logger.LogError("Could not deserialise websocket payload. Raw response: {raw}", payload.GetRawText());
                return;
            }

            BeginPayloadDispatch
            (
                abstractType,
                deserialized,
                new PayloadContext(Name),
                ct
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to deserialize and dispatch event. An  {nameof(UnknownPayload)} will be dispatched.");
            DispatchUnknownPayload(payload.GetRawText(), ct);
        }
    }

    /// <summary>
    /// Creates an instance of <see cref="DispatchPayloadAsync{T}(T, IPayloadContext, CancellationToken)"/> and dispatches an event.
    /// </summary>
    /// <param name="abstractType">The abstract type used by payload handlers.</param>
    /// <param name="eventObject">The event object to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
    private void BeginPayloadDispatch
    (
        Type abstractType,
        object eventObject,
        IPayloadContext context,
        CancellationToken ct
    )
    {
        MethodInfo dispatchMethod = CreateDispatchMethod(abstractType);
        Task? dispatchTask = (Task?)dispatchMethod.Invoke(this, new object[] { eventObject, context, ct });

        if (dispatchTask is null)
        {
            _logger.LogError("Failed to dispatch an event.");
            return;
        }

        _dispatchedPayloadHandlerQueue.Enqueue(dispatchTask);
    }

    /// <summary>
    /// Constructs a <see cref="MethodInfo"/> instance of the <see cref="DispatchPayloadAsync{T}(T, IPayloadContext, CancellationToken)"/> method.
    /// </summary>
    /// <param name="abstractType">The abstract type used by payload handlers.</param>
    /// <returns>The method info.</returns>
    private MethodInfo CreateDispatchMethod(Type abstractType)
    {
        MethodInfo? dispatchMethod = GetType().GetMethod(nameof(DispatchPayloadAsync), BindingFlags.NonPublic | BindingFlags.Instance);
        if (dispatchMethod is null)
        {
            MissingMethodException ex = new(nameof(EventHandlingEventStreamClient), nameof(DispatchPayloadAsync));
            _logger.LogCritical(ex, "Failed to get the event dispatch method.");
            throw ex;
        }

        return dispatchMethod.MakeGenericMethod(abstractType);
    }

    /// <summary>
    /// Dispatches an event to all appropriate payload handlers. DO NOT call this directly.
    /// Use <see cref="CreateDispatchMethod(Type)"/> instead to ensure handlers do not block the receive queue.
    /// </summary>
    /// <typeparam name="T">The abstract type of the payload.</typeparam>
    /// <param name="payload">The payload to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the handlers.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task DispatchPayloadAsync<T>
    (
        T payload,
        IPayloadContext context,
        CancellationToken ct
    ) where T : IPayload
    {
        IReadOnlyList<Type> handlerTypes = _handlerTypeRepository.GetHandlerTypes<T>();
        if (handlerTypes.Count == 0)
            return;

        await Task.WhenAll
        (
            handlerTypes.Select
            (
                async h =>
                {
                    await using AsyncServiceScope scope = _services.CreateAsyncScope();

                    scope.ServiceProvider.GetRequiredService<PayloadContextInjectionService>().Context = context;
                    IPayloadHandler<T> handler = (IPayloadHandler<T>)scope.ServiceProvider.GetRequiredService(h);

                    try
                    {
                        await handler.HandleAsync(payload, ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to execute event handler");
                        throw;
                    }
                }
            )
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Logs any errors that occured while executing the payload handler.
    /// </summary>
    /// <param name="payloadTask">The task representing the payload handling operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    private async Task FinaliseDispatchedPayloadHandler(Task payloadTask)
    {
        try
        {
            await payloadTask.ConfigureAwait(false);
        }
        catch (AggregateException aex)
        {
            foreach (Exception ex in aex.InnerExceptions)
            {
                if (ex is TaskCanceledException)
                    continue;

                _logger.LogError(ex, "An exception occured in a payload handler");
            }
        }
        catch (Exception ex) when (ex is not TaskCanceledException)
        {
            _logger.LogError(ex, "An exception occured in a payload handler");
        }
    }
}
