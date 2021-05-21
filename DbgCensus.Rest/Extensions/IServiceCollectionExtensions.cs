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
        public static void AddCensusRestServices(this IServiceCollection serviceCollection)
            => AddCensusRestServices(serviceCollection, (_) => new JsonSerializerOptions());

        public static void AddCensusRestServices(
            this IServiceCollection serviceCollection,
            Func<IServiceProvider, JsonSerializerOptions> serializerOptions)
        {
            serviceCollection.AddHttpClient<CensusRestClient>();

            serviceCollection.TryAddSingleton<ICensusRestClient>((s) =>
                new CensusRestClient(
                    s.GetRequiredService<ILogger<CensusRestClient>>(),
                    s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(CensusRestClient)),
                    serializerOptions.Invoke(s)));

            serviceCollection.TryAddSingleton<IQueryFactory, QueryFactory>();
        }
    }
}
