using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;

namespace DbgCensus.Rest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds required services for interacting with the Census REST API.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusRestServices(this IServiceCollection serviceCollection)
            => AddCensusRestServices(serviceCollection, (_) => new JsonSerializerOptions());

        /// <summary>
        /// Adds required services for interacting with the Census REST API.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="jsonOptions">JSON options to conform to when deserialising Census data.</param>
        /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
        public static IServiceCollection AddCensusRestServices(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, JsonSerializerOptions> jsonOptions)
        {
            serviceCollection.AddHttpClient<CensusRestClient>();

            serviceCollection.TryAddSingleton<ICensusRestClient>((s) =>
                new CensusRestClient(
                    s.GetRequiredService<ILogger<CensusRestClient>>(),
                    s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(CensusRestClient)),
                    jsonOptions.Invoke(s)));

            serviceCollection.TryAddSingleton<IQueryBuilderFactory, QueryBuilderFactory>();

            serviceCollection.TryAddSingleton<IQueryService, QueryService>();

            return serviceCollection;
        }
    }
}
