using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DbgCensus.Rest
{
    public class CensusRestClient : ICensusRestClient
    {
        private readonly ILogger<CensusRestClient> _logger;
        private readonly HttpClient _client;

        public CensusRestClient(ILogger<CensusRestClient> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<T?> GetAsync<T>(IQuery query) where T : new()
        {
            return await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri).ConfigureAwait(false);
        }

        public async Task<T?> GetAsync<T>(string query) where T : new()
        {
            HttpResponseMessage response = await _client.GetAsync(query).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Census REST request failed with status code {status} and reason {reason}", response.StatusCode, response.ReasonPhrase);
                return default;
            }

            // Check for failed query message
        }
    }
}
