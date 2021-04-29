using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbgCensus.Rest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCensusRestServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IQueryFactory, QueryFactory>();
            serviceCollection.TryAddSingleton<ICensusRestClient, CensusRestClient>();
        }
    }
}
