using DbgCensus.Rest.Abstractions.Queries;
using System.Threading.Tasks;

namespace DbgCensus.Rest.Abstractions
{
    public interface ICensusRestClient
    {
        Task<T?> GetAsync<T>(IQuery query) where T : new();
        Task<T?> GetAsync<T>(string query) where T : new();
    }
}
