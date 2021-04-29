using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a query string.
    /// </summary>
    public class Query : IQuery
    {
        private readonly string _rootEndpoint;
        private readonly string _serviceId;
        private readonly string _queryNamespace;

        private readonly List<QueryFilter> _filters;
        private readonly List<QueryResolve> _resolves; // Comma-separated
        private readonly List<string> _showHideFields; // Comma-separated
        private readonly List<string> _hasFields; // Comma-separated

        private bool _isShowingFields; // Indicates whether, if present, fields in <see cref="_showHideFields"/> should be shown (or hidden).
        private QueryType _verb;
        private string _onCollection;
        private uint? _limit;
        private uint? _limitPerDb;
        private bool _exactMatchesFirst;
        private string? _language;
        private bool _isCaseSensitive; // True by default
        private bool _withNullFields;
        private bool _withTimings;
        private bool _retry; // True by default
        private string? _distinctField;

        /// <summary>
        /// Provides functions to build a query string.
        /// </summary>
        /// <param name="serviceId">A Census service ID.</param>
        /// <param name="queryNamespace">The Census namespace to query.</param>
        /// <param name="rootEndpoint">The root endpoint of the Census REST API.</param>
        public Query(string serviceId, string queryNamespace, string rootEndpoint = "https://census.daybreakgames.com")
        {
            _rootEndpoint = rootEndpoint;
            _serviceId = serviceId;
            _queryNamespace = queryNamespace;

            _filters = new List<QueryFilter>();
            _resolves = new List<QueryResolve>();
            _showHideFields = new List<string>();
            _hasFields = new List<string>();

            _onCollection = string.Empty;
            _verb = QueryType.GET;
            _limit = null;
            _limitPerDb = null;
            _isCaseSensitive = true;
            _retry = true;
        }

        public Query(CensusQueryOptions options)
            : this(options.ServiceId, options.Namespace, options.RootEndpoint)
        {
            if (options.Language is not null)
                SetLanguage(options.Language);
        }

        /// <inheritdoc/>
        public Uri ConstructEndpoint()
        {
            UriBuilder builder = new(_rootEndpoint);

            builder.Path = $"s:{_serviceId}/{_verb.Value}/{_queryNamespace}/{_onCollection}";

            // No query can be made on the collection result.
            if (string.IsNullOrEmpty(_onCollection))
                return builder.Uri;

            if (_distinctField is not null)
            {
                builder.Query = $"?c:distinct={_distinctField}";
                return builder.Uri;
            }

            // Add filters
            foreach (QueryFilter filter in _filters)
                builder.Query += filter.GetFilterString() + "&";

            // Add 'has' fields
            if (_hasFields.Count > 0)
                builder.Query += $"c:has={ string.Join(',', _hasFields) }&";

            // Add show/hide operators
            if (_isShowingFields && _showHideFields.Count > 0)
                builder.Query += "c:show=";
            else if (_showHideFields.Count > 0)
                builder.Query += "c:hide=";
            builder.Query += string.Join(',', _showHideFields) + "&";

            // Add resolves
            if (_resolves.Count > 0)
                builder.Query += $"c:resolve={ string.Join(',', _resolves.Select(r => r.GetResolveString())) }&";

            // Add language, exact matching, case sensitivity and limits
            builder.Query += $"c:lang={_language}&c:exactMatchFirst={_exactMatchesFirst}&c:case={_isCaseSensitive}&c:includeNull={_withNullFields}&c:retry={_retry}&c:timing={_withTimings}";

            if (_limitPerDb is not null)
                builder.Query += $"&c:limitPerDb={_limitPerDb}";
            else if (_limit is not null)
                builder.Query += $"&c:limit={_limit}";

            return builder.Uri;
        }

        /// <inheritdoc/>
        public IQuery OfQueryType(QueryType type)
        {
            _verb = type;
            return this;
        }

        /// <inheritdoc/>
        public IQuery On(string collection)
        {
            _onCollection = collection;
            return this;
        }

        /// <inheritdoc/>
        public IQuery WithLimit(uint limit)
        {
            _limit = limit;

            return this;
        }

        /// <inheritdoc/>
        public IQuery WithLimitPerDatabase(uint limit)
        {
            _limitPerDb = limit;

            return this;
        }

        /// <inheritdoc/>
        public IQuery Where(string property, string filterValue, SearchModifier modifier)
        {
            QueryFilter queryOperator = new(property, filterValue, modifier);
            _filters.Add(queryOperator);

            return this;
        }

        /// <summary>
        /// Performs a search on the collection.
        /// </summary>
        /// <param name="filter">The filter to utilise.</param>
        /// <returns>An <see cref="IQuery"/> instance so that calls may be chained.</returns>
        public IQuery Where(QueryFilter filter)
        {
            _filters.Add(filter);

            return this;
        }

        /// <inheritdoc/>
        public IQuery WithExactMatchesFirst()
        {
            _exactMatchesFirst = true;

            return this;
        }

        /// <inheritdoc/>
        public IQuery WithResolve(string resolveTo, params string[] showFields)
        {
            _resolves.Add(new QueryResolve(resolveTo, showFields));

            return this;
        }

        /// <summary>
        /// Performs a pre-determined resolve. Multiple resolves can be made in the same query.
        /// </summary>
        /// <param name="resolve">The resolve to make.</param>
        public IQuery WithResolve(QueryResolve resolve)
        {
            _resolves.Add(resolve);

            return this;
        }

        /// <inheritdoc/>
        public IQuery ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields.Clear();

            _showHideFields.AddRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc/>
        public IQuery HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields.Clear();

            _showHideFields.AddRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc/>
        public IQuery SetLanguage(string languageCode)
        {
            _language = languageCode;

            return this;
        }

        /// <inheritdoc/>
        public IQuery HasFields(params string[] fieldNames)
        {
            _hasFields.AddRange(fieldNames);

            return this;
        }

        /// <inheritdoc/>
        public IQuery IsCaseInsensitive()
        {
            _isCaseSensitive = false;

            return this;
        }

        /// <inheritdoc/>
        public IQuery WithNullFields()
        {
            _withNullFields = true;

            return this;
        }

        /// <inheritdoc/>
        public IQuery WithTimings()
        {
            _withTimings = true;

            return this;
        }

        /// <inheritdoc/>
        public IQuery WithoutOneTimeRetry()
        {
            _retry = false;

            return this;
        }

        /// <inheritdoc/>
        public IQuery GetDistinctFieldValues(string fieldName)
        {
            if (string.IsNullOrEmpty(_onCollection))
                throw new InvalidOperationException("This operation can only be performed on a collection.");

            _distinctField = fieldName;

            return this;
        }
    }
}
