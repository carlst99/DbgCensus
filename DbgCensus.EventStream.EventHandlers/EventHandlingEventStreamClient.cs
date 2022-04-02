using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Commands;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using DbgCensus.EventStream.EventHandlers.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers;

/// <summary>
/// <inheritdoc cref="BaseEventStreamClient" />
/// Events are dispatched to registered instances of <see cref="IPayloadHandler{TPayload}"/>.
/// </summary>
public sealed class EventHandlingEventStreamClient : BaseEventStreamClient
{
    private readonly ILogger<EventHandlingEventStreamClient> _logger;
    private readonly EventHandlingClientOptions _handlingOptions;
    private readonly IPayloadTypeRepository _payloadTypeRepository;
    private readonly IPayloadDispatchService _dispatchService;

    private CancellationTokenSource _dispatchCts;
    private Task? _dispatchTask;
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
    /// <param name="payloadTypeRepository">The payload type repository.</param>
    /// <param name="dispatchService">The payload dispatch service.</param>
    public EventHandlingEventStreamClient
    (
        string name,
        ILogger<EventHandlingEventStreamClient> logger,
        IServiceProvider services,
        IOptions<EventStreamOptions> baseOptions,
        IOptions<EventHandlingClientOptions> handlingOptions,
        IOptionsMonitor<JsonSerializerOptions> jsonSerializerOptions,
        RecyclableMemoryStreamManager memoryStreamPool,
        IPayloadTypeRepository payloadTypeRepository,
        IPayloadDispatchService dispatchService
    )
        : base(name, logger, services, baseOptions, jsonSerializerOptions, memoryStreamPool)
    {
        _logger = logger;
        _handlingOptions = handlingOptions.Value;
        _payloadTypeRepository = payloadTypeRepository;
        _dispatchService = dispatchService;

        _dispatchCts = new CancellationTokenSource();
        _lastSubscriptionRefresh = DateTimeOffset.UtcNow.Ticks;
    }

    public override async Task StartAsync(CancellationToken ct = default)
    {
        _dispatchCts.Dispose();
        _dispatchCts = new CancellationTokenSource();

        _dispatchTask?.Dispose();
        _dispatchTask = _dispatchService.RunAsync(_dispatchCts.Token);

        await base.StartAsync(ct).ConfigureAwait(false);
    }

    public override async Task StopAsync()
    {
        _dispatchCts.Cancel();

        if (_dispatchTask is not null)
            await _dispatchTask;

        await base.StopAsync();
    }

    public override Task SendCommandAsync<T>(T command, CancellationToken ct = default)
    {
        if (command is ISubscribe subscribeCommand)
            CurrentSubscription = subscribeCommand;

        return base.SendCommandAsync(command, ct);
    }

    /// <inheritdoc />
    protected override async ValueTask HandlePayloadAsync(MemoryStream eventStream, CancellationToken ct)
    {
        // Check the health of the dispatch task
        if (_dispatchTask!.IsCompleted)
        {
            try
            {
                await _dispatchTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "The dispatch service stopped with an error. Restarting it...");
            }

            _dispatchTask.Dispose();
            _dispatchTask = _dispatchService.RunAsync(_dispatchCts.Token);
        }

        using JsonDocument jsonResponse = await JsonDocument.ParseAsync(eventStream, cancellationToken: ct).ConfigureAwait(false);

        if (jsonResponse.RootElement.TryGetProperty("type", out JsonElement typeElement))
        {
            string? censusType = typeElement.GetString();

            if (censusType is null)
            {
                _logger.LogWarning($"A payload with a null type has been received. An {nameof(UnknownPayload)} will be dispatched.");
                await DispatchUnknownPayload(jsonResponse.RootElement.GetRawText(), ct);
            }
            else if (censusType == "serviceMessage") // Further parsing is need to dispatch the encapsulated event payload
            {
                await DispatchServiceMessage(jsonResponse.RootElement, ct);
            }
            else if (_payloadTypeRepository.TryGet(censusType, out (Type AbstractType, Type ImplementingType)? typeMap))
            {
                await DeserializeAndDispatchPayload(typeMap.Value.ImplementingType, jsonResponse.RootElement, ct);
            }
            else
            {
                _logger.LogWarning($"A payload with an unknown type has been received. An {nameof(UnknownPayload)} will be dispatched.");
                await DispatchUnknownPayload(jsonResponse.RootElement.GetRawText(), ct);
            }
        }
        else if (jsonResponse.RootElement.TryGetProperty("subscription", out JsonElement subscriptionElement))
        {
            if (!_payloadTypeRepository.TryGet("subscription", out (Type AbstractType, Type ImplementingType)? typeMap))
            {
                _logger.LogError
                (
                    "Types for the 'subscription' payload have not been registered to the payload type repository. This is an internal library error"
                );

                return;
            }

            await DeserializeAndDispatchPayload(typeMap.Value.ImplementingType, subscriptionElement, ct);
        }
        else if (jsonResponse.RootElement.TryGetProperty("send this for help", out _))
        {
            // No need to process this
        }
        else
        {
            _logger.LogWarning($"An unknown payload has been received. An {nameof(UnknownPayload)} will be dispatched.");
            await DispatchUnknownPayload(jsonResponse.RootElement.GetRawText(), ct);
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
        _logger.LogTrace("Subscription refreshed");
    }

    /// <summary>
    /// Attempts to dispatch a service message event.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="ct"></param>
    private async ValueTask DispatchServiceMessage(JsonElement element, CancellationToken ct)
    {
        // Attempt to get the payload element
        if (!element.TryGetProperty("payload", out JsonElement payloadElement))
        {
            _logger.LogWarning("A service message was received that did not contain a payload. An unknown event will be dispatched");
            await DispatchUnknownPayload(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the event name element
        if (!payloadElement.TryGetProperty("event_name", out JsonElement eventNameElement))
        {
            _logger.LogWarning("A service message was received with a malformed payload (Missing 'event_name'). An unknown event will be dispatched");
            await DispatchUnknownPayload(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the event name
        string? eventName = eventNameElement.GetString();
        if (eventName is null)
        {
            _logger.LogWarning("A service message was received with a malformed payload (NULL 'event_name'). An unknown event will be dispatched");
            await DispatchUnknownPayload(element.GetRawText(), ct);
            return;
        }

        // Attempt to get the type of service message that represents this event
        if (!_payloadTypeRepository.TryGet(eventName, out (Type AbstractType, Type ImplementingType)? typeMap))
        {
            _logger.LogWarning("Types for the received {Event} event have not been registered to the payload type repository", eventName);
            return;
        }

        await DeserializeAndDispatchPayload(typeMap.Value.ImplementingType, payloadElement, ct);
    }

    /// <summary>
    /// Dispatches an <see cref="UnknownPayload"/>.
    /// </summary>
    /// <param name="rawJson">The raw JSON that was received.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the dispatch handlers.</param>
    private async ValueTask DispatchUnknownPayload(string rawJson, CancellationToken ct)
        => await _dispatchService.EnqueuePayloadAsync
        (
            new UnknownPayload(Name, rawJson),
            new PayloadContext(Name),
            ct
        );

    /// <summary>
    /// Deserializes a payload and dispatches it.
    /// </summary>
    /// <param name="implementingType">The type that implements the payload.</param>
    /// <param name="payload">The payload.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop any handlers associated with this payload.</param>
    private async ValueTask DeserializeAndDispatchPayload
    (
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
                _logger.LogError("Could not deserialise websocket payload. Raw response: {Raw}", payload.GetRawText());
                return;
            }

            await _dispatchService.EnqueuePayloadAsync
            (
                (IPayload)deserialized,
                new PayloadContext(Name),
                ct
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to deserialize and dispatch event. An  {nameof(UnknownPayload)} will be dispatched.");
            await DispatchUnknownPayload(payload.GetRawText(), ct);
        }
    }
}
