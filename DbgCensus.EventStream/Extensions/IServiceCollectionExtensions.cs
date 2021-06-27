using DbgCensus.EventStream.Abstractions;
using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.EventHandling;
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

            serviceCollection.TryAddTransient<ICensusEventStreamClient>((s) =>
                new EventHandlingEventStreamClient(
                    s.GetRequiredService<ILogger<EventHandlingEventStreamClient>>(),
                    s.GetRequiredService<ClientWebSocket>(),
                    jsonOptions.Invoke(s),
                    s.GetRequiredService<IEventHandlerRepository>()));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a <see cref="ICensusEventHandler{TEvent}"/> to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <typeparam name="THandler">The responder type.</typeparam>
        /// <returns>The <see cref="IServiceCollection"/> instance, so that calls may be chained.</returns>
        public static IServiceCollection AddResponder<THandler>(this IServiceCollection serviceCollection) where THandler : ICensusEventHandler
        {
            Type handlerType = typeof(THandler);

            Type[] handlerTypeInterfaces = handlerType.GetInterfaces();
            IEnumerable<Type> handlerInterfaces = handlerTypeInterfaces.Where(
                r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(ICensusEventHandler<>));

            foreach (Type handlerInterface in handlerInterfaces)
                serviceCollection.AddScoped(handlerInterface, handlerType);

            serviceCollection.AddScoped(handlerType);

            serviceCollection.Configure<EventHandlerRepository>(e => e.RegisterHandler<THandler>());

            return serviceCollection;
        }
    }
}
