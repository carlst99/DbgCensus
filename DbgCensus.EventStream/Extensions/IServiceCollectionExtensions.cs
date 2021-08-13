using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        /// <param name="deserializationOptions">The JSON options to use when deserializing events.</param>
        /// <param name="serializationOptions">The JSON options to use when serializing commands.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusEventStreamServices(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, JsonSerializerOptions> deserializationOptions,
            Func<IServiceProvider, JsonSerializerOptions> serializationOptions)
        {
            serviceCollection.TryAddTransient<ClientWebSocket>();

            serviceCollection.TryAddSingleton<ICensusEventStreamClientFactory>
            (
                services => new CensusEventStreamClientFactory<CensusEventStreamClient>
                (
                    services.GetRequiredService<IOptions<CensusEventStreamOptions>>(),
                    services.GetRequiredService<IServiceProvider>(),
                    (s, name) => new CensusEventStreamClient
                    (
                        name,
                        s.GetRequiredService<ILogger<CensusEventStreamClient>>(),
                        s.GetRequiredService<IServiceProvider>(),
                        deserializationOptions.Invoke(s),
                        serializationOptions.Invoke(s)
                    ),
                    deserializationOptions,
                    serializationOptions
                )
            );
            serviceCollection.TryAddTransient<ICensusEventStreamClient>(s => s.GetRequiredService<ICensusEventStreamClientFactory>().GetClient());

            return serviceCollection;
        }
    }
}
