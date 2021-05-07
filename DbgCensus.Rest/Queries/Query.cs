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
        private readonly List<QuerySortKey> _sortKeys; // Comma-separated
        private readonly List<IJoin> _joins; // Comma-separated

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
        private uint _startIndex;

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
            _sortKeys = new List<QuerySortKey>();
            _joins = new List<IJoin>();

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

            if (options.Limit is not null)
                WithLimit((uint)options.Limit);
        }

        /// <inheritdoc />
        public Uri ConstructEndpoint()
        {
            UriBuilder builder = new(_rootEndpoint);

            builder.Path = $"s:{_serviceId}/{_verb.Value}/{_queryNamespace}/{_onCollection}";

            // A collection must be specified to perform a query
            if (string.IsNullOrEmpty(_onCollection))
                return builder.Uri;

            // Add distinct command
            if (_distinctField is not null)
            {
                builder.Query = $"?c:distinct={_distinctField}";
                return builder.Uri; // Querying doesn't work in tandem with the distinct command
            }

            // Add filters
            foreach (QueryFilter filter in _filters)
                builder.Query += filter.ToString() + "&";

            // Add has command
            if (_hasFields.Count > 0)
                builder.Query += $"c:has={ string.Join(',', _hasFields) }&";

            // Add show/hide command
            if (_showHideFields.Count > 0)
            {
                if (_isShowingFields)
                    builder.Query += "c:show=";
                else
                    builder.Query += "c:hide=";
                builder.Query += string.Join(',', _showHideFields) + "&";
            }

            // Add resolve command
            if (_resolves.Count > 0)
                builder.Query += $"c:resolve={ string.Join(',', _resolves.Select(r => r.ToString())) }&";

            // TODO: Check if resolves and joins are compatible
            if (_joins.Count > 0)
                builder.Query += $"c:join={ string.Join(',', _joins.Select(j => j.ToString())) }&";

            // Add sort command
            if (_sortKeys.Count > 0)
                builder.Query += $"c:sort={ string.Join(',', _sortKeys.Select(r => r.ToString())) }&";

            builder.Query += $"c:start={_startIndex}&c:lang={_language}&c:exactMatchFirst={_exactMatchesFirst}&c:case={_isCaseSensitive}&c:includeNull={_withNullFields}&c:retry={_retry}&c:timing={_withTimings}";

            // Add relevant limit command
            if (_limitPerDb is not null)
                builder.Query += $"&c:limitPerDb={_limitPerDb}";
            else if (_limit is not null)
                builder.Query += $"&c:limit={_limit}";

            return builder.Uri;
        }

        /// <inheritdoc />
        public IQuery OfQueryType(QueryType type)
        {
            _verb = type;
            return this;
        }

        /// <inheritdoc />
        public IQuery On(string collection)
        {
            _onCollection = collection;
            return this;
        }

        /// <inheritdoc />
        public IQuery WithLimit(uint limit)
        {
            _limit = limit;

            return this;
        }

        /// <inheritdoc />
        public IQuery WithLimitPerDatabase(uint limit)
        {
            _limitPerDb = limit;

            return this;
        }

        /// <inheritdoc />
        public IQuery WithStartIndex(uint index)
        {
            _startIndex = index;

            return this;
        }

        /// <inheritdoc />
        public IQuery Where<T>(string field, T filterValue, SearchModifier modifier) where T : notnull
        {
            QueryFilter queryFilter = new(field, filterValue, modifier);
            _filters.Add(queryFilter);

            return this;
        }

        /// <inheritdoc />
        public IQuery WithSortOrder(string fieldName, SortOrder order = SortOrder.Ascending)
        {
            _sortKeys.Add(new QuerySortKey(fieldName, order));

            return this;
        }

        /// <inheritdoc />
        public IQuery WithExactMatchesFirst()
        {
            _exactMatchesFirst = true;

            return this;
        }

        /// <inheritdoc />
        public IJoin WithJoin()
        {
            IJoin join = new Join();
            _joins.Add(join);

            return join;
        }

        /// <inheritdoc />
        public IQuery WithResolve(string resolveTo, params string[] showFields)
        {
            _resolves.Add(new QueryResolve(resolveTo, showFields));

            return this;
        }

        /// <inheritdoc />
        public IQuery ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields.Clear();

            _showHideFields.AddRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public IQuery HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields.Clear();

            _showHideFields.AddRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public IQuery HasFields(params string[] fieldNames)
        {
            _hasFields.AddRange(fieldNames);

            return this;
        }

        /// <inheritdoc />
        public IQuery SetLanguage(string languageCode)
        {
            _language = languageCode;

            return this;
        }

        /// <inheritdoc />
        public IQuery IsCaseInsensitive()
        {
            _isCaseSensitive = false;

            return this;
        }

        /// <inheritdoc />
        public IQuery WithNullFields()
        {
            _withNullFields = true;

            return this;
        }

        /// <inheritdoc />
        public IQuery WithTimings()
        {
            _withTimings = true;

            return this;
        }

        /// <inheritdoc />
        public IQuery WithoutOneTimeRetry()
        {
            _retry = false;

            return this;
        }

        /// <inheritdoc />
        public IQuery GetDistinctFieldValues(string fieldName)
        {
            if (string.IsNullOrEmpty(_onCollection))
                throw new InvalidOperationException("This operation can only be performed on a collection.");

            _distinctField = fieldName;

            return this;
        }

        public override string ToString() => ConstructEndpoint().ToString();
    }
}
