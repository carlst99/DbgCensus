using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace DbgCensus.Rest.Extensions;

public static class IServiceCollectionExtensions
{
    private static volatile int _serviceIDIndex;

    /// <summary>
    /// Adds required services for interacting with the Census REST API.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="maxRetryAttempts">The maximum number of times that a query may be retried on failure.</param>
    /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
    public static IServiceCollection AddCensusRestServices
    (
        this IServiceCollection serviceCollection,
        int maxRetryAttempts = 3
    )
    {
        serviceCollection.Configure<JsonSerializerOptions>
        (
            Constants.JsonDeserializationOptionsName,
#pragma warning disable IL3050 // RequiresDynamicCode: This path should only be used when the AOT API is not in use
            o => o.AddCensusDeserializationOptions()
#pragma warning restore IL3050
        );

        serviceCollection.AddHttpClient<ICensusRestClient, CensusRestClient>()
            // Disallow redirects so that when Census is down, we don't head over to the HTML land of the DBG website
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
            .AddPolicyHandler
            (
                (services, _) => HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync
                    (
                        Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), maxRetryAttempts),
                        onRetry: (_, _, retryAttempt, _) =>
                        {
                            if (retryAttempt != maxRetryAttempts)
                                return;

                            CensusQueryOptions qOptions = services.GetRequiredService<IOptionsMonitor<CensusQueryOptions>>().CurrentValue;
                            if (qOptions.ServiceIDs.Count == 0)
                                return;

                            int serviceIDIndex = Interlocked.Increment(ref _serviceIDIndex);
                            if (serviceIDIndex >= qOptions.ServiceIDs.Count)
                            {
                                Interlocked.Exchange(ref _serviceIDIndex, 0);
                                serviceIDIndex = 0;
                            }

                            qOptions.ServiceId = qOptions.ServiceIDs[serviceIDIndex];
                        }
                    )
            );

        serviceCollection.TryAddSingleton<IQueryBuilderFactory, QueryBuilderFactory>();
        serviceCollection.TryAddTransient<IQueryService, QueryService>();

        return serviceCollection;
    }
}
