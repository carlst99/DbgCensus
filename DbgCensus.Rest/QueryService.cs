using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest
{
    public class QueryService : IQueryService
    {
        protected readonly ICensusRestClient _client;
        protected readonly IQueryBuilderFactory _queryFactory;

        public QueryService(ICensusRestClient client, IQueryBuilderFactory queryFactory)
        {
            _client = client;
            _queryFactory = queryFactory;
        }

        public virtual IQueryBuilder CreateQuery(CensusQueryOptions? options = null)
            => _queryFactory.Get(options);

        public virtual Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default)
            => _client.GetAsync<T>(query, ct);

        public virtual IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>(IQueryBuilder query, uint pageSize, uint pageCount, uint start = 0, CancellationToken ct = default)
            => _client.GetPaginatedAsync<T>(query, pageSize, pageCount, start, ct);
    }
}
