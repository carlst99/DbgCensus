using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
public class DefaultPayloadDispatchService : IPayloadDispatchService
{
    private readonly ILogger<DefaultPayloadDispatchService> _logger;
    private readonly IPayloadHandlerTypeRepository _handlerTypeRepository;
    private readonly IPreDispatchHandlerTypeRepository _preDispatchHandlerTypeRepository;
    private readonly IServiceProvider _services;

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
    }

    /// <inheritdoc />
    public async Task DispatchPayloadAsync<T>
    (
        T payload,
        IPayloadContext context,
        CancellationToken ct = default
    ) where T : IPayload
    {
        AsyncServiceScope scope = _services.CreateAsyncScope();

        try
        {
            scope.ServiceProvider.GetRequiredService<PayloadContextInjectionService>().Context = context;

            IReadOnlyList<Type> preDispatchHandlerTypes = _preDispatchHandlerTypeRepository.GetAll();
            foreach (Type preDType in preDispatchHandlerTypes)
            {
                IPreDispatchHandler preDispatchHandler = (IPreDispatchHandler)scope.ServiceProvider.GetRequiredService(preDType);

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
        finally
        {
            await scope.DisposeAsync();
        }
    }
}
