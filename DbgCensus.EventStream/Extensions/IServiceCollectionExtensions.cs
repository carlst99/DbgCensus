using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.EventHandling;
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
            => AddCensusEventStreamServices(serviceCollection, (_) => new JsonSerializerOptions(), (_) => new JsonSerializerOptions());

        /// <summary>
        /// Adds required services for interacting with the Census REST API.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="deserializationOptions">JSON options to conform to.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusEventStreamServices(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, JsonSerializerOptions> deserializationOptions,
            Func<IServiceProvider, JsonSerializerOptions> serializationOptions)
        {
            serviceCollection.TryAddTransient<ClientWebSocket>();

            serviceCollection.TryAddSingleton<ICensusEventStreamClientFactory>
            (
                services => new CensusEventStreamClientFactory<EventHandlingEventStreamClient>
                (
                    services.GetRequiredService<IOptions<CensusEventStreamOptions>>(),
                    services.GetRequiredService<IServiceProvider>(),
                    (s, name) => new EventHandlingEventStreamClient
                    (
                        name,
                        s.GetRequiredService<ILogger<EventHandlingEventStreamClient>>(),
                        s.GetRequiredService<IServiceProvider>(),
                        deserializationOptions.Invoke(s),
                        serializationOptions.Invoke(s),
                        s.GetRequiredService<IEventHandlerTypeRepository>(),
                        s.GetRequiredService<IServiceMessageTypeRepository>()
                    ),
                    deserializationOptions,
                    serializationOptions
                )
            );
            serviceCollection.TryAddTransient<ICensusEventStreamClient>(s => s.GetRequiredService<ICensusEventStreamClientFactory>().GetClient());

            serviceCollection.TryAddSingleton<IEventHandlerTypeRepository>(s => s.GetRequiredService<IOptions<EventHandlerTypeRepository>>().Value);
            serviceCollection.TryAddSingleton<IServiceMessageTypeRepository>(s => s.GetRequiredService<IOptions<ServiceMessageTypeRepository>>().Value);

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

            foreach (Type handlerInterface in handlerInterfaces)
                serviceCollection.AddScoped(handlerInterface, handlerType);

            serviceCollection.AddScoped(handlerType);

            serviceCollection.Configure<EventHandlerTypeRepository>(e => e.RegisterHandler<THandler>());

            return serviceCollection;
        }

        /// <summary>
        /// Adds an <see cref="ICensusEventHandler{TEvent}"/> that handles <see cref="ServiceMessage{T}"/> events.
        /// </summary>
        /// <typeparam name="THandler">The handler type.</typeparam>
        /// <typeparam name="TPayload">The type of payload that the <see cref="ServiceMessage{T}"/> carries.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="eventName">The name of the event that this payload is for.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that calls may be chained.</returns>
        public static IServiceCollection AddEventHandler<THandler, TPayload>(this IServiceCollection serviceCollection, string eventName)
            where THandler : ICensusEventHandler<ServiceMessage<TPayload>>
        {
            return serviceCollection
                .AddEventHandler<THandler>()
                .Configure<ServiceMessageTypeRepository>(s => s.TryRegister<ServiceMessage<TPayload>, TPayload>(eventName));
        }
    }
}
