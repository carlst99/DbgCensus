﻿using DbgCensus.EventStream.Abstractions.Objects.Control;
using DbgCensus.EventStream.Abstractions.Objects.Events;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using DbgCensus.EventStream.Abstractions.Objects.Events.Worlds;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using DbgCensus.EventStream.EventHandlers.Services;
using DbgCensus.EventStream.Extensions;
using DbgCensus.EventStream.Objects.Control;
using DbgCensus.EventStream.Objects.Events.Characters;
using DbgCensus.EventStream.Objects.Events.Worlds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DbgCensus.EventStream.EventHandlers.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds required services for interacting with the Census REST API.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
    public static IServiceCollection AddCensusEventHandlingServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IPayloadHandlerTypeRepository>
        (
            s => s.GetRequiredService<IOptions<PayloadHandlerTypeRepository>>().Value
        );

        serviceCollection.TryAddSingleton<IPreDispatchHandlerTypeRepository>
        (
            s => s.GetRequiredService<IOptions<PreDispatchHandlerTypeRepository>>().Value
        );

        serviceCollection.TryAddSingleton<IPayloadDispatchService, DefaultPayloadDispatchService>();
        serviceCollection.TryAddSingleton<IPayloadTypeRepository>(s => s.GetRequiredService<IOptions<PayloadTypeRepository>>().Value);
        serviceCollection.TryAddScoped<PayloadContextInjectionService>();
        serviceCollection.TryAddTransient<IPayloadContext>(s => s.GetRequiredService<PayloadContextInjectionService>().Context);

        serviceCollection.AddCensusEventStreamServices
        (
            (s, o, n) => new EventHandlingEventStreamClient
            (
                n,
                s.GetRequiredService<ILogger<EventHandlingEventStreamClient>>(),
                s,
                o,
                s.GetRequiredService<IOptions<EventHandlingClientOptions>>(),
                s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>(),
                s.GetRequiredService<IPayloadTypeRepository>(),
                s.GetRequiredService<IPayloadDispatchService>()
            )
        );

        serviceCollection.Configure<PayloadTypeRepository>
        (
            o =>
            {
                // Control payloads
                o.Register<IConnectionStateChanged, ConnectionStateChanged>(ControlPayloadNames.ConnectionStateChanged);
                o.Register<IHeartbeat, Heartbeat>(ControlPayloadNames.Heartbeat);
                o.Register<IServiceStateChanged, ServiceStateChanged>(ControlPayloadNames.ServiceStateChanged);
                o.Register<ISubscription, Subscription>(ControlPayloadNames.Subscription);

                // Character-level events
                o.Register<IAchievementEarned, AchievementEarned>(EventNames.AchievementEarned);
                o.Register<IBattleRankUp, BattleRankUp>(EventNames.BattleRankUp);
                o.Register<IDeath, Death>(EventNames.Death);
                o.Register<IFishScan, FishScan>(EventNames.FishScan);
                o.Register<IGainExperience, GainExperience>(EventNames.GainExperience);
                o.Register<IItemAdded, ItemAdded>(EventNames.ItemAdded);
                o.Register<IPlayerFacilityCapture, PlayerFacilityCapture>(EventNames.PlayerFacilityCapture);
                o.Register<IPlayerFacilityDefend, PlayerFacilityDefend>(EventNames.PlayerFacilityDefend);
                o.Register<IPlayerLogin, PlayerLogin>(EventNames.PlayerLogin);
                o.Register<IPlayerLogout, PlayerLogout>(EventNames.PlayerLogout);
                o.Register<ISkillAdded, SkillAdded>(EventNames.SkillAdded);
                o.Register<IVehicleDestroy, VehicleDestroy>(EventNames.VehicleDestroy);

                // World-level events
                o.Register<IContinentLock, ContinentLock>(EventNames.ContinentLock);
                o.Register<IFacilityControl, FacilityControl>(EventNames.FacilityControl);
                o.Register<IMetagameEvent, MetagameEvent>(EventNames.MetagameEvent);
            }
        );

        return serviceCollection;
    }

    /// <summary>
    /// Adds an <see cref="IPayloadHandler{TEvent}"/> to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="lifetime">The lifetime scope to register the payload handler under.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance so that calls may be chained.</returns>
    public static IServiceCollection AddPayloadHandler<THandler>
    (
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped
    ) where THandler : IPayloadHandler
    {
        Type handlerType = typeof(THandler);

        // Get every event handler interface
        Type[] handlerTypeInterfaces = handlerType.GetInterfaces();
        IEnumerable<Type> handlerInterfaces = handlerTypeInterfaces.Where(
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IPayloadHandler<>));

        // Register the handler interface to the implementing type
        foreach (Type handlerInterface in handlerInterfaces)
            serviceCollection.Add(new ServiceDescriptor(handlerInterface, handlerType, lifetime));

        serviceCollection.Add(new ServiceDescriptor(handlerType, handlerType, lifetime));

        serviceCollection.Configure<PayloadHandlerTypeRepository>(r => r.RegisterHandler<THandler>());

        return serviceCollection;
    }

    /// <summary>
    /// Adds an <see cref="IPreDispatchHandler"/> to the service collection.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="lifetime">The lifetime scope to register the payload handler under.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance so that calls may be chained.</returns>
    public static IServiceCollection RegisterPreDispatchHandler<THandler>
    (
        this IServiceCollection serviceCollection,
        ServiceLifetime lifetime = ServiceLifetime.Scoped
    ) where THandler : IPreDispatchHandler
    {
        Type handlerType = typeof(THandler);

        serviceCollection.Add(new ServiceDescriptor(handlerType, handlerType, lifetime));
        serviceCollection.Configure<PreDispatchHandlerTypeRepository>(r => r.Register<THandler>());

        return serviceCollection;
    }
}
