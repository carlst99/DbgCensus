using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandling;
using DbgCensus.EventStream.Objects.Event;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;

namespace DbgCensus.EventStream.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds required services for interacting with the Census REST API.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusEventStreamServices(this IServiceCollection serviceCollection)
            => AddCensusEventStreamServices(serviceCollection, (_) => new JsonSerializerOptions());

        /// <summary>
        /// Adds required services for interacting with the Census REST API.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="jsonOptions">JSON options to conform to.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusEventStreamServices(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, JsonSerializerOptions> jsonOptions)
        {
            serviceCollection.TryAddTransient<ClientWebSocket>();

            serviceCollection.TryAddSingleton<IEventHandlerRepository>(s => s.GetRequiredService<IOptions<EventHandlerRepository>>().Value);
            serviceCollection.TryAddSingleton<IServiceMessageTypeRepository>(s => s.GetRequiredService<IOptions<ServiceMessageTypeRepository>>().Value);

            serviceCollection.TryAddTransient<ICensusEventStreamClient>((s) =>
                new EventHandlingEventStreamClient(
                    s.GetRequiredService<ILogger<EventHandlingEventStreamClient>>(),
                    s.GetRequiredService<ClientWebSocket>(),
                    jsonOptions.Invoke(s),
                    s.GetRequiredService<IEventHandlerRepository>(),
                    s.GetRequiredService<IServiceMessageTypeRepository>(),
                    s.GetRequiredService<IServiceProvider>()));

            return serviceCollection;
        }

        /// <summary>
        /// Adds an <see cref="ICensusEventHandler{TEvent}"/> to the service collection.
        /// </summary>
        /// <typeparam name="THandler">The handler type.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that calls may be chained.</returns>
        public static IServiceCollection AddEventHandler<THandler>(this IServiceCollection serviceCollection) where THandler : ICensusEventHandler
        {
            Type handlerType = typeof(THandler);

            Type[] handlerTypeInterfaces = handlerType.GetInterfaces();
            IEnumerable<Type> handlerInterfaces = handlerTypeInterfaces.Where(
                r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(ICensusEventHandler<>));

            //IEnumerable<Type> serviceMessageHandlerInterfaces = handlerTypeInterfaces
            //    .SelectMany(r => r.GetGenericArguments())
            //    .Where(r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(ServiceMessage<>));

            foreach (Type handlerInterface in handlerInterfaces)
                serviceCollection.AddScoped(handlerInterface, handlerType);

            serviceCollection.AddScoped(handlerType);

            serviceCollection.Configure<EventHandlerRepository>(e => e.RegisterHandler<THandler>());

            return serviceCollection;
        }

        /// <summary>
        /// Adds an <see cref="ICensusEventHandler{TEvent}"/> that handles <see cref="ServiceMessage{T}"/> events.
        /// </summary>
        /// <typeparam name="THandler">The handler type.</typeparam>
        /// <typeparam name="TPayload">The type of payload that the <see cref="ServiceMessage{T}"/> carries.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="censusService">The service that the object is received from.</param>
        /// <param name="censusType">The type that the object represents.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that calls may be chained.</returns>
        public static IServiceCollection AddEventHandler<THandler, TPayload>(this IServiceCollection serviceCollection, string censusService, string censusType)
            where THandler : ICensusEventHandler<ServiceMessage<TPayload>>
        {
            return serviceCollection
                .AddEventHandler<THandler>()
                .Configure<ServiceMessageTypeRepository>(s => s.TryRegister<ServiceMessage<TPayload>, TPayload>(censusService, censusType));
        }

        /// <summary>
        /// Registers a payload type for a census <see cref="ServiceMessage{T}"/>.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="censusService">The service that the object is received from.</param>
        /// <param name="censusType">The type that the object represents.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that calls may be chained.</returns>
        [Obsolete("Use an overload " + nameof(AddEventHandler) + " instead.")]
        public static IServiceCollection AddServiceMessagePayload<TPayload>(this IServiceCollection serviceCollection, string censusService, string censusType)
            => serviceCollection.Configure<ServiceMessageTypeRepository>(s => s.TryRegister<ServiceMessage<TPayload>, TPayload>(censusService, censusType));

        /// <summary>
        /// Adds a <see cref="ICensusEventHandler"/> for a <see cref="ServiceMessage{T}"/> to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="THandler">The responder type.</typeparam>
        /// <returns>The <see cref="IServiceCollection"/> instance, so that calls may be chained.</returns>
        //public static IServiceCollection AddServiceMessageEventHandler<THandler, TPayload>(this IServiceCollection serviceCollection) where THandler : ICensusEventHandler
        //{
        //    Type handlerType = typeof(THandler);

        //    Type[] handlerTypeInterfaces = handlerType.GetInterfaces();
        //    IEnumerable<Type> handlerInterfaces = handlerTypeInterfaces.Where(
        //        r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(ICensusEventHandler<ServiceMessage<TPayload>>));

        //    foreach (Type handlerInterface in handlerInterfaces)
        //        serviceCollection.AddScoped(handlerInterface, handlerType);

        //    serviceCollection.AddScoped(handlerType);

        //    serviceCollection.Configure<EventHandlerRepository>(e => e.RegisterHandler<THandler>());

        //    return serviceCollection;
        //}
    }
}
