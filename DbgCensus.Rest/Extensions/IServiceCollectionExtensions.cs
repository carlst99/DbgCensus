using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;

namespace DbgCensus.Rest.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds required services for interacting with the Census REST API.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
    public static IServiceCollection AddCensusRestServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient<CensusRestClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

        serviceCollection.TryAddSingleton<ICensusRestClient, CensusRestClient>();

        serviceCollection.TryAddSingleton<IQueryBuilderFactory, QueryBuilderFactory>();

        serviceCollection.TryAddSingleton<IQueryService, QueryService>();

        return serviceCollection;
    }
}
