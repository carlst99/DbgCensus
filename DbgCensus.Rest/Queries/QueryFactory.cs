using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Options;
using System;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// A factory for <see cref="Query"/> objects. Objects are constructed using the values of an <see cref="CensusQueryOptions"/> options instance retrieved from the IoC container.
    /// </summary>
    public class QueryFactory : IQueryFactory
    {
        private readonly Func<IQuery> _getQuery;

        public QueryFactory(IOptions<CensusQueryOptions> queryOptions)
        {
            _getQuery = () => new Query(queryOptions.Value);
        }

        /// <inheritdoc/>
        public IQuery Get() => _getQuery.Invoke();
    }
}
