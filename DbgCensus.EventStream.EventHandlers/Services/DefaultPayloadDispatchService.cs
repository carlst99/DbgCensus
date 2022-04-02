using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DbgCensus.EventStream.EventHandlers.Services;

/// <summary>
/// <para>
/// Implements the <see cref="IPayloadDispatchService"/> interface
/// to dispatch payloads to <see cref="IPayloadHandler{TPayload}"/>
/// types registered in the <see cref="IPayloadHandlerTypeRepository"/>.
/// </para>
/// <para>
/// This service also runs <see cref="IPreDispatchHandler"/> types
/// registered in the <see cref="IPreDispatchHandlerTypeRepository"/>.
/// </para>
/// </summary>
public class DefaultPayloadDispatchService : IPayloadDispatchService, IAsyncDisposable
{
    private readonly ILogger<DefaultPayloadDispatchService> _logger;
    private readonly IPayloadHandlerTypeRepository _handlerTypeRepository;
    private readonly IPreDispatchHandlerTypeRepository _preDispatchHandlerTypeRepository;
    private readonly IServiceProvider _services;

    private readonly Dictionary<Type, MethodInfo> _dispatchMethods;
    private readonly Dictionary<Type, Type> _payloadTypeInterfaces;
    private readonly Type _dispatchType;
    private readonly Channel<Task> _handlerFinalizationQueue;
    private readonly Channel<(IPayload, IPayloadContext)> _payloadDispatchQueue;

    private CancellationTokenSource _dispatchCts;
    private Task? _finalizerTask;
    private bool _isDisposed;

    /// <inheritdoc />
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="DefaultPayloadDispatchService"/> class.
    /// </summary>
    /// <param name="logger">The logging interface to use.</param>
    /// <param name="services">The service provider.</param>
    /// <param name="handlerTypeRepository">The payload handler type repository.</param>
    /// <param name="preDispatchHandlerTypeRepository">The pre-dispatch handler type repository.</param>
    public DefaultPayloadDispatchService
    (
        ILogger<DefaultPayloadDispatchService> logger,
        IPayloadHandlerTypeRepository handlerTypeRepository,
        IPreDispatchHandlerTypeRepository preDispatchHandlerTypeRepository,
        IServiceProvider services
    )
    {
        _logger = logger;
        _handlerTypeRepository = handlerTypeRepository;
        _preDispatchHandlerTypeRepository = preDispatchHandlerTypeRepository;
        _services = services;

        _dispatchMethods = new Dictionary<Type, MethodInfo>();
        _payloadTypeInterfaces = new Dictionary<Type, Type>();
        _dispatchType = GetType();
        _handlerFinalizationQueue = Channel.CreateUnbounded<Task>();
        _payloadDispatchQueue = Channel.CreateUnbounded<(IPayload, IPayloadContext)>();

        _dispatchCts = new CancellationTokenSource();
    }

    /// <inheritdoc />
    public ValueTask EnqueuePayloadAsync<TPayload>(TPayload payload, IPayloadContext context, CancellationToken ct = default)
        where TPayload : IPayload
        => _payloadDispatchQueue.Writer.WriteAsync((payload, context), ct);

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken ct = default)
    {
        if (IsRunning)
            throw new InvalidOperationException("This dispatch service is already running");

        try
        {
            await Task.Yield();

            _dispatchCts.Dispose();
            _dispatchCts = new CancellationTokenSource();

            _finalizerTask?.Dispose();
            _finalizerTask = FinalizerTaskAsync();

            IsRunning = true;

            await foreach ((IPayload payload, IPayloadContext context) in _payloadDispatchQueue.Reader.ReadAllAsync(ct))
            {
                if (_finalizerTask.IsCompleted)
                {
                    await _finalizerTask;
                    return;
                }

                await InvokeDispatchAsync(payload, context, ct).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            await StopAsync();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of managed resources.
    /// </summary>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            await StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop the dispatch service when disposing");
        }

        _dispatchCts.Dispose();
        _finalizerTask?.Dispose();

        _isDisposed = true;
    }

    /// <summary>
    /// Cancels any running dispatchers and stops the finalizer task.
    /// </summary>
    /// <returns>The result of the finalizer task.</returns>
    protected virtual async Task StopAsync()
    {
        if (!this.IsRunning)
            return;

        _dispatchCts.Cancel();
        IsRunning = false;

        if (_finalizerTask is not null)
            await _finalizerTask;
    }

    /// <summary>
    /// Dispatches a payload.
    /// </summary>
    /// <param name="payload">The event object to dispatch.</param>
    /// <param name="context">The context to inject.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> that can be used to stop the entire event chain.</param>
    private async ValueTask InvokeDispatchAsync
    (
        IPayload payload,
        IPayloadContext context,
        CancellationToken ct
    )
    {
        Type payloadType = payload.GetType();
        if (!_payloadTypeInterfaces.TryGetValue(payloadType, out Type? payloadInterfaceType))
        {
            Type? payloadInterfaceTypes = payloadType.GetInterfaces()
                .FirstOrDefault(t => t.GetInterface(nameof(IPayload)) is not null);

            payloadInterfaceType = payloadInterfaceTypes ?? throw new InvalidOperationException
            (
                $"Failed to find an interface deriving from {nameof(IPayload)} on the {payloadType.FullName} type."
            );

            _payloadTypeInterfaces[payloadType] = payloadInterfaceType;
        }

        MethodInfo dispatchMethod = CreateDispatchMethod(payloadInterfaceType);
        Task? dispatchTask = (Task?)dispatchMethod.Invoke(this, new object[] { payload, context, _dispatchCts.Token });

        if (dispatchTask is null)
        {
            _logger.LogError("Failed to dispatch an event");
            return;
        }

        await _handlerFinalizationQueue.Writer.WriteAsync(dispatchTask, ct);
    }

    /// <summary>
    /// Constructs a <see cref="MethodInfo"/> instance of the
    /// <see cref="DispatchPayloadAsync{T}(T, IPayloadContext, CancellationToken)"/> method.
    /// </summary>
    /// <param name="abstractType">The abstract type used by payload handlers.</param>
    /// <returns>The method info.</returns>
    private MethodInfo CreateDispatchMethod(Type abstractType)
    {
        if (_dispatchMethods.ContainsKey(abstractType))
            return _dispatchMethods[abstractType];

        MethodInfo? dispatchMethod = _dispatchType.GetMethod
        (
            nameof(DispatchPayloadAsync),
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (dispatchMethod is null)
        {
            MissingMethodException ex = new(nameof(EventHandlingEventStreamClient), nameof(DispatchPayloadAsync));
            _logger.LogCritical(ex, "Failed to get the event dispatch method");

            throw ex;
        }

        dispatchMethod = dispatchMethod.MakeGenericMethod(abstractType);
        _dispatchMethods[abstractType] = dispatchMethod;

        return dispatchMethod;
    }

    private async Task DispatchPayloadAsync<T>
    (
        T payload,
        IPayloadContext context,
        CancellationToken ct = default
    ) where T : IPayload
    {
        IReadOnlyList<Type> preDispatchHandlerTypes = _preDispatchHandlerTypeRepository.GetAll();
        await using AsyncServiceScope preScope = _services.CreateAsyncScope();
        preScope.ServiceProvider.GetRequiredService<PayloadContextInjectionService>().Context = context;

        foreach (Type preDType in preDispatchHandlerTypes)
        {
            IPreDispatchHandler preDispatchHandler = (IPreDispatchHandler)preScope.ServiceProvider.GetRequiredService(preDType);

            try
            {
                if(await preDispatchHandler.HandlePayloadAsync(payload, ct))
                    return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute pre-dispatch handler");
                throw;
            }
        }

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
                        await handler.HandleAsync(payload, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to execute event handler");
                        throw;
                    }
                }
            )
        );
    }

    private async Task FinalizerTaskAsync()
    {
        try
        {
            await foreach (Task handlerTask in _handlerFinalizationQueue.Reader.ReadAllAsync(_dispatchCts.Token).ConfigureAwait(false))
            {
                if (!handlerTask.IsCompleted)
                {
                    await _handlerFinalizationQueue.Writer.WriteAsync(handlerTask, _dispatchCts.Token);
                    continue;
                }

                await FinalizeResponderTask(handlerTask).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // This is fine, we must be stopping
        }

        // Finalize any remaining responders on the queue
        while (_handlerFinalizationQueue.Reader.TryRead(out Task? handlerTask))
            await FinalizeResponderTask(handlerTask).ConfigureAwait(false);
    }

    private async ValueTask FinalizeResponderTask(Task handlerTask)
    {
        try
        {
            await handlerTask.ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            // This is fine
        }
        catch (AggregateException aex)
        {
            foreach (Exception ex in aex.InnerExceptions)
            {
                if (ex is OperationCanceledException or TaskCanceledException)
                    continue;

                _logger.LogError(ex, "An exception occured in a payload handler");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occured in a payload handler");
        }
    }
}
