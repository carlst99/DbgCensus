using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Json;
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
        private readonly JsonSerializerOptions _jsonOptions;

        public CensusRestClient(ILogger<CensusRestClient> logger, HttpClient client, JsonSerializerOptions jsonOptions) // TODO: Figure out how this would be injected
        {
            _logger = logger;
            _client = client;

            _jsonOptions = new JsonSerializerOptions(jsonOptions)
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = new CamelToSnakeCaseJsonNamingPolicy()
            };
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
            await response.Content.ReadFromJsonAsync<T>(_jsonOptions, ct).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false); // TODO: Inject json options
        }
    }
}
