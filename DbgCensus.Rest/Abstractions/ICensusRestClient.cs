using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICensusRestClient : IDisposable
    {
        Task<T> GetAsync<T>(IQuery query, CancellationToken ct) where T : new();
        Task<T> GetAsync<T>(string query, string? collectionName, CancellationToken ct) where T : new();
    }
}
