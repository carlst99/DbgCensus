using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a query URI for the Census REST API.
    /// </summary>
    public class QueryBuilder : IQueryBuilder
    {
        private readonly string _rootEndpoint;
        private readonly List<QueryFilter> _filters;
        private readonly QueryCommandFormatter _resolves;
        private readonly QueryCommandFormatter _hasFields;
        private readonly QueryCommandFormatter _sortKeys;
        private readonly QueryCommandFormatter _joins;
        private readonly QueryCommandFormatter _tree;
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

        private string _serviceId;
        private string _queryNamespace;
        private QueryType _verb;
        private QueryCommandFormatter _showHideFields;
        private bool _isShowingFields; // Indicates whether, if present, fields in <see cref="_showHideFields"/> should be shown (or hidden).

        public string? CollectionName { get; protected set; }

        /// <summary>
        /// Provides functions to build a query string for the Census REST API.
        /// </summary>
        /// <param name="serviceId">A Census service ID.</param>
        /// <param name="censusNamespace">The Census namespace to query.</param>
        /// <param name="rootEndpoint">The root endpoint of the Census REST API.</param>
        public QueryBuilder(string serviceId, string censusNamespace, string rootEndpoint = "https://census.daybreakgames.com")
        {
            _rootEndpoint = rootEndpoint;
            _serviceId = serviceId;
            _queryNamespace = censusNamespace;

            _filters = new List<QueryFilter>();
            _resolves = GetQueryCommandFormatter("c:resolve", true);
            _hasFields = GetQueryCommandFormatter("c:has", true);
            _sortKeys = GetQueryCommandFormatter("c:sort", true);
            _joins = GetQueryCommandFormatter("c:join", true);
            _tree = GetQueryCommandFormatter("c:tree", false);
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
        public QueryBuilder(CensusQueryOptions options)
            : this(options.ServiceId, options.Namespace, options.RootEndpoint)
        {
            if (options.LanguageCode is not null)
                WithLanguage((CensusLanguage)options.LanguageCode);

            if (options.Limit is not null)
                WithLimit((uint)options.Limit);
        }

        /// <inheritdoc />
        public virtual Uri ConstructEndpoint()
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

            builder.Query += StringUtils.JoinWithoutNullOrEmptyValues('&', _hasFields, _showHideFields, _resolves, _joins, _sortKeys, _startIndex, _language, _exactMatchesFirst, _isCaseSensitive, _withNullFields, _withTimings, _retry);

            // Add relevant limit command
            if (_limitPerDb.AnyValue)
                builder.Query += '&' + _limitPerDb;
            else if (_limit is not null)
                builder.Query += '&' + _limit;

            return builder.Uri;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder OfQueryType(QueryType type)
        {
            _verb = type;
            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder OnCollection(string collection)
        {
            if (string.IsNullOrEmpty(collection))
                throw new ArgumentNullException(nameof(collection));

            CollectionName = collection;
            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithLimit(uint limit)
        {
            _limit.AddArgument(limit.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithLimitPerDatabase(uint limit)
        {
            _limitPerDb.AddArgument(limit.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithStartIndex(uint index)
        {
            _startIndex.AddArgument(index.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder Where<T>(string field, T filterValue, SearchModifier modifier) where T : notnull
        {
            string? filterValueString = filterValue.ToString();
            if (string.IsNullOrEmpty(filterValueString) || filterValueString.Equals(typeof(T).FullName))
                throw new ArgumentException("The type of " + nameof(filterValue) + " must have properly implemented ToString()", nameof(filterValue));

            QueryFilter queryFilter = new(field, filterValueString, modifier);
            _filters.Add(queryFilter);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithSortOrder(string fieldName, SortOrder order = SortOrder.Ascending)
        {
            _sortKeys.AddArgument(new QuerySortKey(fieldName, order));

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithExactMatchesFirst()
        {
            _exactMatchesFirst.AddArgument(true.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder AddJoin(string toCollection)
        {
            JoinBuilder join = new(toCollection);
            _joins.AddArgument(join);

            return join;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder AddJoin(string collectionName, Action<IJoinBuilder> configureJoin)
        {
            JoinBuilder join = new(collectionName);
            configureJoin(join);
            _joins.AddArgument(join);

            return this;
        }

        /// <inheritdoc />
        public virtual ITreeBuilder WithTree(string onField)
        {
            TreeBuilder tree = new(onField);
            _tree.AddArgument(tree);

            return tree;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithTree(string onField, Action<ITreeBuilder> configureTree)
        {
            TreeBuilder tree = new(onField);
            configureTree(tree);
            _tree.AddArgument(tree);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder AddResolve(string resolveTo, params string[] showFields)
        {
            _resolves.AddArgument(new QueryResolve(resolveTo, showFields));

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields = GetQueryCommandFormatter("show", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                GetQueryCommandFormatter("hide", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder HasFields(params string[] fieldNames)
        {
            _hasFields.AddArgumentRange(fieldNames);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithLanguage(CensusLanguage languageCode)
        {
            _language.AddArgument(languageCode);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder IsCaseInsensitive()
        {
            _isCaseSensitive.AddArgument(false.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithNullFields()
        {
            _withNullFields.AddArgument(true.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithTimings()
        {
            _withTimings.AddArgument(true.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithoutOneTimeRetry()
        {
            _retry.AddArgument(false.ToString());

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithDistinctFieldValues(string fieldName)
        {
            if (string.IsNullOrEmpty(CollectionName))
                throw new InvalidOperationException("This operation can only be performed on a collection.");

            _distinctField.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithServiceId(string serviceId)
        {
            _serviceId = serviceId;

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder OnNamespace(string censusNamespace)
        {
            _queryNamespace = censusNamespace;

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
    }
}
