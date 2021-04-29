using DbgCensus.Rest.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DbgCensus.Rest
{
    public class CensusRestClient : ICensusRestClient
    {
        private readonly HttpClient _client;

        public CensusRestClient(HttpClient client)
        {
            _client = client;
        }

        public Task<T?> GetAsync<T>(string endPoint) where T : new()
        {
            throw new NotImplementedException();
        }
    }
}
