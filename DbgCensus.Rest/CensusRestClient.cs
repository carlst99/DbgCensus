using DbgCensus.Core.Exceptions;
using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensus.Rest
{
    /// <inheritdoc cref="ICensusRestClient" />
    public class CensusRestClient : ICensusRestClient
    {
        protected readonly ILogger<CensusRestClient> _logger;
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly CensusQueryOptions _queryOptions;

        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="CensusRestClient"/> class.
        /// </summary>
        /// <param name="logger">The logging interface to use.</param>
        /// <param name="client">The <see cref="HttpClient"/> to send requests with.</param>
        /// <param name="options">The query options to conform to.</param>
        public CensusRestClient(ILogger<CensusRestClient> logger, HttpClient client, IOptions<CensusQueryOptions> options)
        {
            _logger = logger;
            _client = client;
            _queryOptions = options.Value;

            _jsonOptions = new JsonSerializerOptions(_queryOptions.DeserializationOptions);
            _jsonOptions.AddCensusDeserializationOptions();
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetAsync<T>(IQueryBuilder query, CancellationToken ct = default)
        {
            return await GetAsync<T>(query.ConstructEndpoint().AbsoluteUri, query.CollectionName, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetAsync<T>(string query, string? collectionName, CancellationToken ct = default)
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

        /// <inheritdoc />
        public virtual async IAsyncEnumerable<IEnumerable<T>?> GetPaginatedAsync<T>(IQueryBuilder query, uint pageSize, uint pageCount, uint start = 0, [EnumeratorCancellation] CancellationToken ct = default)
        {
            for (int i = 0; i < pageCount; i++)
            {
                if (ct.IsCancellationRequested)
                    throw new TaskCanceledException();

                query.WithStartIndex(start);
                query.WithLimit(pageSize);

                yield return await GetAsync<List<T>>(query, ct).ConfigureAwait(false);

                start += pageSize;
            }
        }

        /// <summary>
        /// Performs checks on the returned data and deserializes it.
        /// </summary>
        /// <typeparam name="T">The POCO type to deserialize to.</typeparam>
        /// <param name="content">The result of the REST request.</param>
        /// <param name="collectionName">The name of the collection that was queried.</param>
        /// <param name="ct">A token which can be used to cancel asynchronous logic.</param>
        /// <returns></returns>
        protected virtual async Task<T?> DeserializeResponseContentAsync<T>(HttpContent content, string? collectionName = null, CancellationToken ct = default)
        {
            using JsonDocument data = await JsonDocument.ParseAsync(await content.ReadAsStreamAsync(ct).ConfigureAwait(false), cancellationToken: ct).ConfigureAwait(false);

            CheckForResponseError(data);

            if (collectionName is null)
                collectionName = "datatype";

            // TODO: Test with distinct and other commands that vastly modify the result
            if (!data.RootElement.TryGetProperty(collectionName + "_list", out JsonElement collectionElement))
            {
                _logger.LogWarning("Returned data was not in the expected format: {data}", data.RootElement.GetRawText());
                throw new CensusInvalidDataException("Returned data was not in the expected format.", data.RootElement.GetRawText());
            }

            if (collectionElement.ValueKind != JsonValueKind.Array)
                throw new CensusInvalidDataException("The Census result was expected to contain an array of data.", data.RootElement.GetRawText());

            if (typeof(T).IsAssignableTo(typeof(System.Collections.IEnumerable)))
                return JsonSerializer.Deserialize<T>(collectionElement.GetRawText(), _jsonOptions);

            int length = collectionElement.GetArrayLength();
            if (length > 1)
                throw new JsonException("You are trying to deserialise to a single object, but the Census data contained more than one entity.");
            else if (length == 0)
                return default;
            else
                return JsonSerializer.Deserialize<T>(collectionElement[0].GetRawText(), _jsonOptions);
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
                    _logger.LogError("Census query failed with error code {code} and message {message}", errorCode.GetRawText(), errorValue.GetRawText());
                else
                    _logger.LogError("Census query failed with error code: {code}", errorCode.GetRawText());

                throw new CensusQueryErrorException(errorValue.GetRawText(), errorCode.GetRawText());
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

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
