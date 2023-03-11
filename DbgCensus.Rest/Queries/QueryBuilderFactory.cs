using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Options;

namespace DbgCensus.Rest.Queries;

/// <summary>
/// A factory for <see cref="QueryBuilder"/> objects. Objects are constructed using the values of an
/// <see cref="CensusQueryOptions"/> options instance retrieved from the IoC container.
/// </summary>
public sealed class QueryBuilderFactory : IQueryBuilderFactory
{
    private readonly IOptionsMonitor<CensusQueryOptions> _defaultOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryBuilderFactory"/> class.
    /// </summary>
    /// <param name="queryOptions">The default query options to use.</param>
    public QueryBuilderFactory(IOptionsMonitor<CensusQueryOptions> queryOptions)
    {
        _defaultOptions = queryOptions;
    }

    /// <inheritdoc />
    public IQueryBuilder Get(CensusQueryOptions? options = null)
        => new QueryBuilder(options ?? _defaultOptions.CurrentValue);
}
