using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a query string for the Census REST API.
    /// </summary>
    public class Query : IQuery
    {
        private readonly string _rootEndpoint;
        private readonly string _serviceId;
        private readonly string _queryNamespace;

        private readonly List<QueryFilter> _filters;
        private readonly QueryCommandFormatter _resolves;
        private readonly QueryCommandFormatter _hasFields;
        private readonly QueryCommandFormatter _sortKeys;
        private readonly QueryCommandFormatter _joins;
        private readonly QueryCommandFormatter _trees;
        private readonly QueryCommandFormatter _limit;
        private readonly QueryCommandFormatter _limitPerDb;
        private readonly QueryCommandFormatter _exactMatchesFirst;
        private readonly QueryCommandFormatter _language;
        private readonly QueryCommandFormatter _isCaseSensitive; // True by default
        private readonly QueryCommandFormatter _withNullFields;
        private readonly QueryCommandFormatter _withTimings;
        private readonly QueryCommandFormatter _retry; // True by default
        private readonly QueryCommandFormatter _distinctField;
        private readonly QueryCommandFormatter _startIndex;

        private QueryType _verb;
        private QueryCommandFormatter _showHideFields;
        private bool _isShowingFields; // Indicates whether, if present, fields in <see cref="_showHideFields"/> should be shown (or hidden).

        public string? CollectionName { get; protected set; }

        /// <summary>
        /// Provides functions to build a query string for the Census REST API.
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
            _resolves = GetQueryCommandFormatter("c:resolve", true);
            _hasFields = GetQueryCommandFormatter("c:has", true);
            _sortKeys = GetQueryCommandFormatter("c:sort", true);
            _joins = GetQueryCommandFormatter("c:join", true);
            _trees = GetQueryCommandFormatter("c:tree", false);
            _limit = GetQueryCommandFormatter("c:limit", false, 100.ToString());
            _limitPerDb = GetQueryCommandFormatter("c:limitPerDB", false);
            _exactMatchesFirst = GetQueryCommandFormatter("c:exactMatchFirst", false);
            _language = GetQueryCommandFormatter("c:lang", false);
            _isCaseSensitive = GetQueryCommandFormatter("c:case", false);
            _withNullFields = GetQueryCommandFormatter("c:includeNull", false);
            _withTimings = GetQueryCommandFormatter("c:timing", false);
            _retry = GetQueryCommandFormatter("c:retry", false);
            _distinctField = GetQueryCommandFormatter("c:distinct", false);
            _startIndex = GetQueryCommandFormatter("c:start", false);

            CollectionName = null;
            _verb = QueryType.GET;
            _showHideFields = GetQueryCommandFormatter("c:show", true);
        }

        /// <summary>
        /// Provides functions to build a query string for the Census REST API.
        /// </summary>
        /// <param name="options">Default configuration for the query.</param>
        public Query(CensusQueryOptions options)
            : this(options.ServiceId, options.Namespace, options.RootEndpoint)
        {
            if (options.LanguageCode is not null)
                WithLanguage((CensusLanguage)options.LanguageCode);

            if (options.Limit is not null)
                WithLimit((uint)options.Limit);
        }

        /// <inheritdoc />
        public Uri ConstructEndpoint()
        {
            UriBuilder builder = new(_rootEndpoint);

            builder.Path = $"s:{_serviceId}/{_verb.Value}/{_queryNamespace}";
            if (CollectionName is not null)
                builder.Path += $"/{CollectionName}";

            // A collection must be specified to perform a query
            if (CollectionName is null)
                return builder.Uri;

            // Add distinct command
            if (_distinctField.AnyValue)
            {
                builder.Query = _distinctField;
                return builder.Uri; // Querying doesn't work in tandem with the distinct command
            }

            // Add filters
            foreach (QueryFilter filter in _filters)
                builder.Query += filter.ToString() + "&";

            builder.Query += JoinWithoutNullOrEmptyValues('&', _hasFields, _showHideFields, _resolves, _joins, _sortKeys, _startIndex, _language, _exactMatchesFirst, _isCaseSensitive, _withNullFields, _withTimings, _retry);

            // Add relevant limit command
            if (_limitPerDb.AnyValue)
                builder.Query += '&' + _limitPerDb;
            else if (_limit is not null)
                builder.Query += '&' + _limit;

            return builder.Uri;
        }

        /// <inheritdoc />
        public IQuery OfQueryType(QueryType type)
        {
            _verb = type;
            return this;
        }

        /// <inheritdoc />
        public IQuery OnCollection(string collection)
        {
            if (string.IsNullOrEmpty(collection))
                throw new ArgumentNullException(nameof(collection));

            CollectionName = collection;
            return this;
        }

        /// <inheritdoc />
        public IQuery WithLimit(uint limit)
        {
            _limit.AddArgument(limit.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery WithLimitPerDatabase(uint limit)
        {
            _limitPerDb.AddArgument(limit.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery WithStartIndex(uint index)
        {
            _startIndex.AddArgument(index.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery Where<T>(string field, T filterValue, SearchModifier modifier) where T : notnull
        {
            string? filterValueString = filterValue.ToString();
            if (string.IsNullOrEmpty(filterValueString) || filterValueString.Equals(typeof(T).FullName))
                throw new ArgumentException("The type of " + nameof(filterValue) + " must have properly implemented ToString()", nameof(filterValue));

            QueryFilter queryFilter = new(field, filterValueString, modifier);
            _filters.Add(queryFilter);

            return this;
        }

        /// <inheritdoc />
        public IQuery WithSortOrder(string fieldName, SortOrder order = SortOrder.Ascending)
        {
            _sortKeys.AddArgument(new QuerySortKey(fieldName, order));

            return this;
        }

        /// <inheritdoc />
        public IQuery WithExactMatchesFirst()
        {
            _exactMatchesFirst.AddArgument(true.ToString());

            return this;
        }

        /// <inheritdoc />
        public IJoin WithJoin(string toCollection)
        {
            Join join = new(toCollection);
            _joins.AddArgument(join);

            return join;
        }

        /// <inheritdoc />
        public IQuery WithJoin(string collectionName, Action<IJoin> configureJoin)
        {
            Join join = new(collectionName);
            configureJoin(join);
            _joins.AddArgument(join);

            return this;
        }

        /// <inheritdoc />
        public IQuery WithResolve(string resolveTo, params string[] showFields)
        {
            _resolves.AddArgument(new QueryResolve(resolveTo, showFields));

            return this;
        }

        /// <inheritdoc />
        public IQuery ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields = GetQueryCommandFormatter("show", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public IQuery HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                GetQueryCommandFormatter("hide", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public IQuery HasFields(params string[] fieldNames)
        {
            _hasFields.AddArgumentRange(fieldNames);

            return this;
        }

        /// <inheritdoc />
        public IQuery WithLanguage(CensusLanguage languageCode)
        {
            _language.AddArgument(languageCode);

            return this;
        }

        /// <inheritdoc />
        public IQuery IsCaseInsensitive()
        {
            _isCaseSensitive.AddArgument(false.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery WithNullFields()
        {
            _withNullFields.AddArgument(true.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery WithTimings()
        {
            _withTimings.AddArgument(true.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery WithoutOneTimeRetry()
        {
            _retry.AddArgument(false.ToString());

            return this;
        }

        /// <inheritdoc />
        public IQuery WithDistinctFieldValues(string fieldName)
        {
            if (string.IsNullOrEmpty(CollectionName))
                throw new InvalidOperationException("This operation can only be performed on a collection.");

            _distinctField.AddArgument(fieldName);

            return this;
        }

        public override string ToString() => ConstructEndpoint().ToString();

        private static QueryCommandFormatter GetQueryCommandFormatter(string command, bool allowsMultipleArguments, string? defaultArgument = null)
        {
            if (allowsMultipleArguments)
                return new QueryCommandFormatter(command, '=', ',', defaultArgument);
            else
                return new QueryCommandFormatter(command, '=', defaultArgument);
        }

        private static string JoinWithoutNullOrEmptyValues(char separator, params string[] value) => string.Join(separator, value.Where(str => !string.IsNullOrEmpty(str)));
    }
}
