using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Net.Http;
using System.Text.Json;

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
        serviceCollection.Configure<JsonSerializerOptions>
        (
            Constants.JsonDeserializationOptionsName,
            o => o.AddCensusDeserializationOptions()
        );

        serviceCollection.AddHttpClient<ICensusRestClient, CensusRestClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
            .AddTransientHttpErrorPolicy
            (
                builder => builder.WaitAndRetryAsync
                (
                    Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 4)
                )
            )
            .AddTransientHttpErrorPolicy
            (
                builder => builder.CircuitBreakerAsync(4, TimeSpan.FromSeconds(30))
            );

        serviceCollection.TryAddSingleton<IQueryBuilderFactory, QueryBuilderFactory>();
        serviceCollection.TryAddTransient<IQueryService, QueryService>();

        return serviceCollection;
    }
}
