using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Options;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// A factory for <see cref="Query"/> objects. Objects are constructed using the values of an <see cref="CensusQueryOptions"/> options instance retrieved from the IoC container.
    /// </summary>
    public class QueryFactory : IQueryFactory
    {
        private readonly CensusQueryOptions _defaultOptions;

        public QueryFactory(IOptions<CensusQueryOptions> queryOptions)
        {
            _defaultOptions = queryOptions.Value;
        }

        /// <inheritdoc />
        public IQuery Get() => new Query(_defaultOptions);

        /// <inheritdoc />
        public IQuery Get(CensusQueryOptions options) => new Query(_defaultOptions);
    }
}
