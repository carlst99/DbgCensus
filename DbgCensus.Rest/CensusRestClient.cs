using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Exceptions;
using DbgCensus.Rest.Json;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest
{
    public class CensusRestClient : ICensusRestClient
    {
        protected readonly ILogger<CensusRestClient> _logger;
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        public bool IsDisposed { get; protected set; }

        public CensusRestClient(ILogger<CensusRestClient> logger, HttpClient client, JsonSerializerOptions jsonOptions)
        {
            _logger = logger;
            _client = client;

            _jsonOptions = new JsonSerializerOptions(jsonOptions)
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy()
            };
            _jsonOptions.Converters.Add(new BooleanJsonConverter());
        }

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(IQuery query, CancellationToken ct = default) where T : new()
        {
            return await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri, query.CollectionName, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T?> GetAsync<T>(string query, string? collectionName, CancellationToken ct = default) where T : new()
        {
            _logger.LogTrace("Performing Census GET request with query: {query}", query);

            HttpResponseMessage response = await _client.GetAsync(query, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Census GET request failed with status code {status} and reason {reason}", response.StatusCode, response.ReasonPhrase);
                throw new CensusServiceUnavailableException();
            }

            return await DeserializeResponseContentAsync<T>(response.Content, collectionName, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Performs checks on the returned data and deserializes it.
        /// </summary>
        /// <typeparam name="T">The POCO type to deserialize to.</typeparam>
        /// <param name="content">The result of the REST request.</param>
        /// <param name="collectionName">The name of the collection that was queried.</param>
        /// <param name="ct">A token which can be used to cancel asynchronous logic.</param>
        /// <returns></returns>
        protected virtual async Task<T?> DeserializeResponseContentAsync<T>(HttpContent content, string? collectionName, CancellationToken ct = default)
        {
            JsonDocument data = await JsonDocument.ParseAsync(await content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);

            CheckForResponseError(data);

            if (collectionName is null)
                collectionName = "datatype";

            //if (!data.RootElement.TryGetProperty("returned", out JsonElement returnedElement) || !returnedElement.TryGetUInt64(out ulong returnedCount))
            //    throw new CensusInvalidDataException("Expected a valid 'returned' element.", data.RootElement.GetString());

            // TODO: Test with distinct and other commands that vastly modify the result
            // Check for list vs single?
            // Check that object count matches returned results count if list?
            if (!data.RootElement.TryGetProperty(collectionName + "_list", out JsonElement collectionElement))
            {
                _logger.LogWarning("Returned data was not in the expected format: {data}", data.RootElement.GetRawText());
                throw new CensusInvalidDataException("Returned data was not in the expected format.", data.RootElement.GetRawText());
            }

            return JsonSerializer.Deserialize<T>(collectionElement.GetRawText(), _jsonOptions);
        }

        /// <summary>
        /// Attempts to find an error displayed in the Census response (rather than an error in the returned data).
        /// </summary>
        /// <param name="data">The response.</param>
        protected virtual void CheckForResponseError(JsonDocument data)
        {
            JsonValueKind dataKind = data.RootElement.ValueKind;
            if (dataKind == JsonValueKind.Null || dataKind == JsonValueKind.Undefined)
                throw new CensusInvalidDataException("No data was returned.", null);

            if (data.RootElement.TryGetProperty("error", out JsonElement errorValue))
            {
                if (errorValue.GetRawText() == "service_unavailable")
                {
                    _logger.LogWarning("Census service unavailable.");
                    throw new CensusServiceUnavailableException();
                }
                else
                {
                    _logger.LogError("Census query failed with an error: {error}", errorValue.GetRawText());
                    throw new CensusQueryErrorException(errorValue.GetRawText());
                }
            }

            if (data.RootElement.TryGetProperty("errorCode", out JsonElement errorCode))
            {
                if (data.RootElement.TryGetProperty("errorMessage", out errorValue))
                    _logger.LogError("Census query failed with error code {code} and message {message}", errorCode.GetInt32(), errorValue.GetRawText());
                else
                    _logger.LogError("Census query failed with error code: {code}", errorCode.GetRawText());

                throw new CensusQueryErrorException(errorValue.GetRawText(), errorCode.GetInt32());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
