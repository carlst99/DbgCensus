using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries;

/// <summary>
/// Provides functions to build a query URI for the Census REST API.
/// </summary>
public sealed class QueryBuilder : IQueryBuilder
{
    private readonly CensusQueryOptions _queryOptions;
    private readonly List<QueryFilter> _filters;
    private readonly MultiQueryCommandFormatter<QueryResolve> _resolves;
    private readonly MultiQueryCommandFormatter<string> _hasFields;
    private readonly MultiQueryCommandFormatter<QuerySortKey> _sortKeys;
    private readonly MultiQueryCommandFormatter<IJoinBuilder> _joins;
    private readonly SingleQueryCommandFormatter<ITreeBuilder> _tree;
    private readonly SingleQueryCommandFormatter<int> _limit;
    private readonly SingleQueryCommandFormatter<int> _limitPerDb;
    private readonly SingleQueryCommandFormatter<bool> _exactMatchesFirst; // False by default
    private readonly SingleQueryCommandFormatter<string> _language;
    private readonly SingleQueryCommandFormatter<bool> _isCaseSensitive; // True by default
    private readonly SingleQueryCommandFormatter<bool> _withNullFields; // False by default
    private readonly SingleQueryCommandFormatter<bool> _withTimings; // False by default
    private readonly SingleQueryCommandFormatter<bool> _retry; // True by default
    private readonly SingleQueryCommandFormatter<string> _distinctField;
    private readonly SingleQueryCommandFormatter<int> _startIndex;
    private readonly List<string> _customParameters;

    private string _serviceId;
    private string _queryNamespace;
    private QueryType _verb;
    private MultiQueryCommandFormatter<string>? _showHideFields;

    public string? CollectionName { get; private set; }

    /// <summary>
    /// Provides functions to build a query string for the Census REST API.
    /// </summary>
    /// <param name="queryOptions">The default configuration for the query.</param>
    public QueryBuilder(CensusQueryOptions queryOptions)
    {
        _queryOptions = queryOptions;
        _serviceId = queryOptions.ServiceId;
        _queryNamespace = queryOptions.Namespace;

        _filters = new List<QueryFilter>();
        _resolves = GetMultiQCF<QueryResolve>("c:resolve");
        _hasFields = GetMultiQCF<string>("c:has");
        _sortKeys = GetMultiQCF<QuerySortKey>("c:sort");
        _joins = GetMultiQCF<IJoinBuilder>("c:join");
        _tree = GetSingleQCF<ITreeBuilder>("c:tree");
        _limit = GetSingleQCF<int>("c:limit");
        _limitPerDb = GetSingleQCF<int>("c:limitPerDB");
        _exactMatchesFirst = GetSingleQCF<bool>("c:exactMatchFirst");
        _language = GetSingleQCF<string>("c:lang");
        _isCaseSensitive = GetSingleQCF<bool>("c:case");
        _withNullFields = GetSingleQCF<bool>("c:includeNull");
        _withTimings = GetSingleQCF<bool>("c:timing");
        _retry = GetSingleQCF<bool>("c:retry");
        _distinctField = GetSingleQCF<string>("c:distinct");
        _startIndex = GetSingleQCF<int>("c:start");
        _customParameters = new List<string>();

        CollectionName = null;
        _verb = QueryType.Get;

        if (queryOptions.LanguageCode is not null)
            WithLanguage(queryOptions.LanguageCode);

        if (queryOptions.Limit is not null)
            WithLimit((int)queryOptions.Limit);
    }

    /// <inheritdoc />
    public Uri ConstructEndpoint()
    {
        UriBuilder builder = new(_queryOptions.RootEndpoint);
        builder.Path += $"s:{_serviceId}/{_verb.Value}/{_queryNamespace}";

        // A collection must be specified to perform a query
        if (CollectionName is null)
            return builder.Uri;
        builder.Path += $"/{CollectionName}";

        // Add distinct command
        if (_distinctField.HasArgument)
        {
            builder.Query = _distinctField;
            return builder.Uri; // Querying doesn't work in tandem with the distinct command
        }

        // Add any custom parameters
        if (_customParameters.Count > 0)
            builder.Query += string.Join('&', _customParameters);

        // Add filters
        foreach (QueryFilter filter in _filters)
            builder.Query += $"{filter}&";

        // Add commands
        string commandsJoin = StringUtils.JoinWithoutNullOrEmptyValues
        (
            '&',
            _hasFields,
            _showHideFields?.ToString(),
            _resolves,
            _joins,
            _sortKeys,
            _startIndex,
            _language,
            _exactMatchesFirst,
            _isCaseSensitive,
            _withNullFields,
            _withTimings,
            _retry
        );

        if (!string.IsNullOrEmpty(commandsJoin))
            builder.Query += commandsJoin;

        // Add relevant limit command
        if (_limitPerDb.HasArgument)
            builder.Query += '&' + _limitPerDb;
        else if (_limit.HasArgument)
            builder.Query += '&' + _limit;

        return builder.Uri;
    }

    /// <inheritdoc />
    public IQueryBuilder OfQueryType(QueryType type)
    {
        _verb = type;
        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder OnCollection(string collection)
    {
        if (string.IsNullOrEmpty(collection))
            throw new ArgumentNullException(nameof(collection));

        CollectionName = collection;
        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithLimit(int limit)
    {
        _limit.SetArgument(limit);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithLimitPerDatabase(int limit)
    {
        _limitPerDb.SetArgument(limit);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithStartIndex(int index)
    {
        _startIndex.SetArgument(index);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder Where<T>(string field, SearchModifier modifier, T value) where T : notnull
    {
        _filters.Add(new QueryFilter(field, modifier, StringUtils.SafeToString(value)));

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WhereAll<T>(string field, SearchModifier modifier, IEnumerable<T> filterValues) where T : notnull
    {
        string value = string.Join
        (
            ',',
            filterValues.Select(StringUtils.SafeToString)
        );

        _filters.Add
        (
            new QueryFilter(field, modifier, value)
        );

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithSortOrder(string fieldName, SortOrder order = SortOrder.Ascending)
    {
        _sortKeys.AddArgument(new QuerySortKey(fieldName, order));

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithExactMatchesFirst()
    {
        _exactMatchesFirst.SetArgument(true);

        return this;
    }

    /// <inheritdoc />
    public IJoinBuilder AddJoin(string toCollection)
    {
        JoinBuilder join = new(toCollection);
        _joins.AddArgument(join);

        return join;
    }

    /// <inheritdoc />
    public IQueryBuilder AddJoin(string collectionName, Action<IJoinBuilder> configureJoin)
    {
        JoinBuilder join = new(collectionName);
        configureJoin(join);
        _joins.AddArgument(join);

        return this;
    }

    /// <inheritdoc />
    public ITreeBuilder WithTree(string onField)
    {
        TreeBuilder tree = new(onField);
        _tree.SetArgument(tree);

        return tree;
    }

    /// <inheritdoc />
    public IQueryBuilder WithTree(string onField, Action<ITreeBuilder> configureTree)
    {
        TreeBuilder tree = new(onField);
        configureTree(tree);
        _tree.SetArgument(tree);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder AddResolve(string resolveTo, params string[] showFields)
    {
        _resolves.AddArgument(new QueryResolve(resolveTo, showFields));

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder ShowFields(params string[] fieldNames)
    {
        const string showCommand = "c:show";

        if (_showHideFields is not null && _showHideFields.Command is not showCommand)
            throw new InvalidOperationException($"{nameof(ShowFields)} is not compatible with {nameof(HideFields)}");

        _showHideFields ??= GetMultiQCF<string>(showCommand);
        _showHideFields.AddArgumentRange(fieldNames);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder HideFields(params string[] fieldNames)
    {
        const string hideCommand = "c:hide";

        if (_showHideFields is not null && _showHideFields.Command is not hideCommand)
            throw new InvalidOperationException($"{nameof(HideFields)} is not compatible with {nameof(ShowFields)}");

        _showHideFields ??= GetMultiQCF<string>(hideCommand);
        _showHideFields.AddArgumentRange(fieldNames);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder HasFields(params string[] fieldNames)
    {
        _hasFields.AddArgumentRange(fieldNames);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithLanguage(string languageCode)
    {
        _language.SetArgument(languageCode);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder IsCaseInsensitive()
    {
        _isCaseSensitive.SetArgument(false);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithNullFields()
    {
        _withNullFields.SetArgument(true);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithTimings()
    {
        _withTimings.SetArgument(true);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithoutOneTimeRetry()
    {
        _retry.SetArgument(false);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithDistinctFieldValues(string fieldName)
    {
        if (string.IsNullOrEmpty(CollectionName))
            throw new InvalidOperationException("This operation can only be performed on a Census collection.");

        _distinctField.SetArgument(fieldName);

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder WithServiceId(string serviceId)
    {
        _serviceId = serviceId;

        return this;
    }

    /// <inheritdoc />
    public IQueryBuilder OnNamespace(string censusNamespace)
    {
        _queryNamespace = censusNamespace;

        return this;
    }

    public IQueryBuilder WithCustomParameter(string parameter)
    {
        parameter = parameter.Trim('?', '&');

        if (!string.IsNullOrEmpty(parameter))
            _customParameters.Add(parameter);

        return this;
    }

    public override string ToString() => ConstructEndpoint().ToString();

    private static MultiQueryCommandFormatter<T> GetMultiQCF<T>(string command)
        where T : notnull
        => new(command, '=', ',');

    private static SingleQueryCommandFormatter<T> GetSingleQCF<T>(string command)
        where T : notnull
        => new(command, '=');
}
