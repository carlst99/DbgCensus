using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbgCensus.Rest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        // TODO: Add func to resolve CensusQueryOptions
        public static void AddCensusRestServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<CensusRestClient>();
            serviceCollection.TryAddSingleton<ICensusRestClient, CensusRestClient>();

            serviceCollection.TryAddSingleton<IQueryFactory, QueryFactory>();
        }
    }
}
