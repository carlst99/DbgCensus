﻿using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Options;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// A factory for <see cref="QueryBuilder"/> objects. Objects are constructed using the values of an <see cref="CensusQueryOptions"/> options instance retrieved from the IoC container.
    /// </summary>
    public class QueryFactory : IQueryFactory
    {
        private readonly CensusQueryOptions _defaultOptions;

        public QueryFactory(IOptions<CensusQueryOptions> queryOptions)
        {
            _defaultOptions = queryOptions.Value;
        }

        /// <inheritdoc />
        public IQueryBuilder Get() => new QueryBuilder(_defaultOptions);

        /// <inheritdoc />
        public IQueryBuilder Get(CensusQueryOptions options) => new QueryBuilder(_defaultOptions);
    }
}
