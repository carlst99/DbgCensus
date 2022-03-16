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
    /// <param name="maxRetryAttempts">The maximum number of times a query may be retried on failure.</param>
    /// <param name="useCircuitBreakerPolicy">
    /// A value indicating whether or not the circuit breaker policy should be used.
    /// If true, more than <paramref name="maxRetryAttempts"/> failures will cause
    /// all query requests to be blocked for 15s.
    /// </param>
    /// <returns>A reference to this <see cref="IServiceCollection"/> so that calls may be chained.</returns>
    public static IServiceCollection AddCensusRestServices
    (
        this IServiceCollection serviceCollection,
        int maxRetryAttempts = 4,
        bool useCircuitBreakerPolicy = true
    )
    {
        serviceCollection.Configure<JsonSerializerOptions>
        (
            Constants.JsonDeserializationOptionsName,
            o => o.AddCensusDeserializationOptions()
        );

        IHttpClientBuilder httpBuilder = serviceCollection.AddHttpClient<ICensusRestClient, CensusRestClient>()
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

        if (useCircuitBreakerPolicy)
        {
            httpBuilder.AddTransientHttpErrorPolicy
            (
                builder => builder.CircuitBreakerAsync(4, TimeSpan.FromSeconds(15))
            );
        }

        serviceCollection.TryAddSingleton<IQueryBuilderFactory, QueryBuilderFactory>();
        serviceCollection.TryAddTransient<IQueryService, QueryService>();

        return serviceCollection;
    }
}
