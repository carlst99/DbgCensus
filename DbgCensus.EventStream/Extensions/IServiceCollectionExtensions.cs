using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.WebSockets;

namespace DbgCensus.EventStream.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds required services for interacting with the Census REST API.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureClient">The delegate used to construct your <see cref="IEventStreamClient"/> implementation. Requires an options and name parameter for factory injection.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusEventStreamServices(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, EventStreamOptions, string, IEventStreamClient> configureClient)
        {
            serviceCollection.TryAddTransient<ClientWebSocket>();

            serviceCollection.TryAddSingleton<IEventStreamClientFactory>
            (
                s => new EventStreamClientFactory
                (
                    s.GetRequiredService<IServiceProvider>(),
                    s.GetRequiredService<IOptions<EventStreamOptions>>(),
                    configureClient
                )
            );

            serviceCollection.TryAddTransient<IEventStreamClient>(s => s.GetRequiredService<IEventStreamClientFactory>().GetClient());

            return serviceCollection;
        }
    }
}
