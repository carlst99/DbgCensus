using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Polly;

public class ServiceIDRotationPolicy<TResult> : AsyncPolicy<TResult>
{
    private readonly IOptionsMonitor<CensusQueryOptions> _queryOptions;

    private int _errorEventCount;
    private int _serviceIDIndex;


    public ServiceIDRotationPolicy
    (
        PolicyBuilder<TResult> policyBuilder,
        IOptionsMonitor<CensusQueryOptions> queryOptions
    ) : base(policyBuilder)
    {
        _queryOptions = queryOptions;
    }

    protected override async Task<TResult> ImplementationAsync
    (
        Func<Context, CancellationToken, Task<TResult>> action,
        Context context,
        CancellationToken cancellationToken,
        bool continueOnCapturedContext
    )
    {
        try
        {
            TResult result = await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);
            return result;
        }
        catch (Exception)
        {
            _errorEventCount++;

            if (_errorEventCount is < 5)
                throw;

            CensusQueryOptions options = _queryOptions.CurrentValue;

            _serviceIDIndex++;
            if (_serviceIDIndex >= options.ServiceIDs.Count)
                _serviceIDIndex = 0;

            options.ServiceId = options.ServiceIDs[_serviceIDIndex];
            _errorEventCount = 0;

            throw;
        }
    }
}

public static class ServiceIDRotationPolicySyntax
{
    public static ServiceIDRotationPolicy<TResult> ServiceIDRotation<TResult>
    (
        this PolicyBuilder<TResult> policyBuilder,
        IOptionsMonitor<CensusQueryOptions> queryOptions
    )
        => new ServiceIDRotationPolicy<TResult>(policyBuilder, queryOptions);
}
