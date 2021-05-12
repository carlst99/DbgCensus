using DbgCensus.Rest.Abstractions.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions
{
    public interface ICensusRestClient
    {
        Task<T?> GetAsync<T>(IQuery query, CancellationToken ct) where T : new();
        Task<T?> GetAsync<T>(string query, CancellationToken ct) where T : new();
    }
}
