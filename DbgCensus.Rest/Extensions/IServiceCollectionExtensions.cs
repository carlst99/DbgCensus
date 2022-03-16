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

namespace DbgCensus.Rest.Extensions;

public static class IServiceCollectionExtensions
{
    private static int _serviceIDIndex;

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
            .AddPolicyHandler
            (
                (services, _) => HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync
                    (
                        Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 4),
                        onRetry: (_, _, retryAttempt, _) =>
                        {
                            if (retryAttempt != 4)
                                return;

                            CensusQueryOptions qOptions = services.GetRequiredService<IOptionsMonitor<CensusQueryOptions>>().CurrentValue;
                            if (qOptions.ServiceIDs.Count == 0)
                                return;

                            _serviceIDIndex++;
                            if (_serviceIDIndex >= qOptions.ServiceIDs.Count)
                                _serviceIDIndex = 0;

                            qOptions.ServiceId = qOptions.ServiceIDs[_serviceIDIndex];
                        }
                    )
            )
            .AddTransientHttpErrorPolicy
            (
                builder => builder.CircuitBreakerAsync(4, TimeSpan.FromSeconds(15))
            );

        serviceCollection.TryAddSingleton<IQueryBuilderFactory, QueryBuilderFactory>();
        serviceCollection.TryAddTransient<IQueryService, QueryService>();

        return serviceCollection;
    }
}
