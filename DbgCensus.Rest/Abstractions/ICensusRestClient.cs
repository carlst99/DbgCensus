using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions
{
    public interface ICensusRestClient : IDisposable
    {
        Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default) where T : new();
        Task<T?> GetAsync<T>(string query, string? collectionName, CancellationToken ct = default) where T : new();
    }
}
