using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a query URI for the Census REST API.
    /// </summary>
    public class QueryBuilder : IQueryBuilder
    {
        private readonly string _rootEndpoint;
        private readonly List<QueryFilter> _filters;
        private readonly MultiQueryCommandFormatter<QueryResolve> _resolves;
        private readonly MultiQueryCommandFormatter<string> _hasFields;
        private readonly MultiQueryCommandFormatter<QuerySortKey> _sortKeys;
        private readonly MultiQueryCommandFormatter<IJoinBuilder> _joins;
        private readonly SingleQueryCommandFormatter<ITreeBuilder> _tree;
        private readonly SingleQueryCommandFormatter<uint?> _limit;
        private readonly SingleQueryCommandFormatter<uint?> _limitPerDb;
        private readonly SingleQueryCommandFormatter<bool?> _exactMatchesFirst; // False by default
        private readonly SingleQueryCommandFormatter<string> _language;
        private readonly SingleQueryCommandFormatter<bool?> _isCaseSensitive; // True by default
        private readonly SingleQueryCommandFormatter<bool?> _withNullFields; // False by default
        private readonly SingleQueryCommandFormatter<bool?> _withTimings; // False by default
        private readonly SingleQueryCommandFormatter<bool?> _retry; // True by default
        private readonly SingleQueryCommandFormatter<string> _distinctField;
        private readonly SingleQueryCommandFormatter<uint?> _startIndex;

        private string _serviceId;
        private string _queryNamespace;
        private QueryType _verb;
        private MultiQueryCommandFormatter<string> _showHideFields;
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
            _resolves = GetMultiQCF<QueryResolve>("c:resolve");
            _hasFields = GetMultiQCF<string>("c:has");
            _sortKeys = GetMultiQCF<QuerySortKey>("c:sort");
            _joins = GetMultiQCF<IJoinBuilder>("c:join");
            _tree = GetSingleQCF<ITreeBuilder>("c:tree");
            _limit = GetSingleQCF<uint?>("c:limit");
            _limitPerDb = GetSingleQCF<uint?>("c:limitPerDB");
            _exactMatchesFirst = GetSingleQCF<bool?>("c:exactMatchFirst");
            _language = GetSingleQCF<string>("c:lang");
            _isCaseSensitive = GetSingleQCF<bool?>("c:case");
            _withNullFields = GetSingleQCF<bool?>("c:includeNull");
            _withTimings = GetSingleQCF<bool?>("c:timing");
            _retry = GetSingleQCF<bool?>("c:retry");
            _distinctField = GetSingleQCF<string>("c:distinct");
            _startIndex = GetSingleQCF<uint?>("c:start");

            CollectionName = null;
            _verb = QueryType.GET;
            _showHideFields = GetMultiQCF<string>("c:show");

            WithLimit(100);
        }

        /// <summary>
        /// Provides functions to build a query string for the Census REST API.
        /// </summary>
        /// <param name="options">Default configuration for the query.</param>
        public QueryBuilder(CensusQueryOptions options)
            : this(options.ServiceId, options.Namespace, options.RootEndpoint)
        {
            if (options.LanguageCode is not null)
                WithLanguage(options.LanguageCode);

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
            if (_distinctField.HasArgument)
            {
                builder.Query = _distinctField;
                return builder.Uri; // Querying doesn't work in tandem with the distinct command
            }

            // Add filters
            foreach (QueryFilter filter in _filters)
                builder.Query += filter.ToString() + "&";

            builder.Query += StringUtils.JoinWithoutNullOrEmptyValues('&', _hasFields, _showHideFields, _resolves, _joins, _sortKeys, _startIndex, _language, _exactMatchesFirst, _isCaseSensitive, _withNullFields, _withTimings, _retry);

            // Add relevant limit command
            if (_limitPerDb.HasArgument)
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
            _limit.SetArgument(limit);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithLimitPerDatabase(uint limit)
        {
            _limitPerDb.SetArgument(limit);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithStartIndex(uint index)
        {
            _startIndex.SetArgument(index);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder Where<T>(string field, SearchModifier modifier, T filterValue) where T : notnull
        {
            string value = StringUtils.SafeToString(filterValue);
            _filters.Add(new QueryFilter(field, modifier, value));

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder Where<T>(string field, SearchModifier modifier, IEnumerable<T> filterValues) where T : notnull
        {
            if (!filterValues.Any())
                throw new ArgumentException("At least one value must be provided", nameof(filterValues));

            List<string> values = new();
            foreach (T element in filterValues)
                values.Add(StringUtils.SafeToString(element));

            _filters.Add(new QueryFilter(field, modifier, values));

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
            _exactMatchesFirst.SetArgument(true);

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
            _tree.SetArgument(tree);

            return tree;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithTree(string onField, Action<ITreeBuilder> configureTree)
        {
            TreeBuilder tree = new(onField);
            configureTree(tree);
            _tree.SetArgument(tree);

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
                _showHideFields = GetMultiQCF<string>("c:show");

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields = GetMultiQCF<string>("c:hide");

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
        public virtual IQueryBuilder WithLanguage(string languageCode)
        {
            _language.SetArgument(languageCode);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder IsCaseInsensitive()
        {
            _isCaseSensitive.SetArgument(false);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithNullFields()
        {
            _withNullFields.SetArgument(true);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithTimings()
        {
            _withTimings.SetArgument(true);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithoutOneTimeRetry()
        {
            _retry.SetArgument(false);

            return this;
        }

        /// <inheritdoc />
        public virtual IQueryBuilder WithDistinctFieldValues(string fieldName)
        {
            if (string.IsNullOrEmpty(CollectionName))
                throw new InvalidOperationException("This operation can only be performed on a Census collection.");

            _distinctField.SetArgument(fieldName);

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

        private static MultiQueryCommandFormatter<T> GetMultiQCF<T>(string command) => new(command, '=', ',');

        private static SingleQueryCommandFormatter<T> GetSingleQCF<T>(string command) => new(command, '=');
    }
}
