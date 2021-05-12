using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
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

        public async Task<T?> GetAsync<T>(IQuery query, CancellationToken ct = default) where T : new()
        {
            return await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri, ct).ConfigureAwait(false);
        }

        public async Task<T?> GetAsync<T>(string query, CancellationToken ct = default) where T : new()
        {
            //T? result = await _client.GetFromJsonAsync<T>(query, ct);
            HttpResponseMessage response = await _client.GetAsync(query, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Census REST request failed with status code {status} and reason {reason}", response.StatusCode, response.ReasonPhrase);
                return default;
            }

            // Check for failed query message
            // Un-nest from root object
            // Check for list vs single
            // Check that object count matches returned results count if list
            await response.Content.ReadFromJsonAsync<T>(new JsonSerializerOptions(), ct);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false); // TODO: Inject json options
        }
    }
}
