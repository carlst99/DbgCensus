using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a join command for the Census REST API.
    /// </summary>
    public class JoinBuilder : IJoinBuilder
    {
        private readonly List<IJoinBuilder> _nestedJoins;

        private readonly QueryCommandFormatter _toCollection;
        private readonly QueryCommandFormatter _filterTerms;
        private readonly QueryCommandFormatter _onField;
        private readonly QueryCommandFormatter _toField;
        private readonly QueryCommandFormatter _isList;
        private readonly QueryCommandFormatter _injectAt;
        private readonly QueryCommandFormatter _isOuter;

        private QueryCommandFormatter _showHideFields;
        private bool _isShowingFields; // Indicates whether, if present, fields in "_showHideFields" should be shown (or hidden).

        /// <summary>
        /// Initialises a new instance of the <see cref="JoinBuilder"/> class.
        /// </summary>
        /// <param name="toCollection">The name of the collection to join to.</param>
        public JoinBuilder(string toCollection)
        {
            _nestedJoins = new List<IJoinBuilder>();

            _toCollection = GetQueryCommandFormatter("type", false);
            _filterTerms = GetQueryCommandFormatter("terms", true);
            _onField = GetQueryCommandFormatter("on", false);
            _toField = GetQueryCommandFormatter("to", false);
            _isList = GetQueryCommandFormatter("list", false, "0");
            _injectAt = GetQueryCommandFormatter("inject_at", false);
            _isOuter = GetQueryCommandFormatter("outer", false, "1");
            _showHideFields = GetQueryCommandFormatter("show", true);

            ToCollection(toCollection);
        }

        /// <inheritdoc/>
        public virtual IJoinBuilder ToCollection(string collectionName)
        {
            _toCollection.AddArgument(collectionName);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields = GetQueryCommandFormatter("show", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields = GetQueryCommandFormatter("hide", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder InjectAt(string name)
        {
            _injectAt.AddArgument(name);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder IsList()
        {
            _isList.AddArgument("1");

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder IsInnerJoin()
        {
            _isOuter.AddArgument("0");

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder OnField(string fieldName)
        {
            _onField.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder ToField(string fieldName)
        {
            _toField.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder Where<T>(string field, SearchModifier modifier, params T[] filterValues) where T : notnull
        {
            string[] stringValues = new string[filterValues.Length];
            string? typeName = typeof(T).FullName;

            for (int i = 0; i < filterValues.Length; i++)
            {
                string? value = filterValues[i].ToString();
                if (string.IsNullOrEmpty(value) || value == typeName)
                    throw new ArgumentException("The type " + typeName + " must have properly implemented ToString()", nameof(filterValues));

                stringValues[i] = value;
            }

            QueryFilter queryFilter = new(field, modifier, stringValues);
            _filterTerms.AddArgument(queryFilter);

            return this;
        }

        /// <inheritdoc/>
        public virtual IJoinBuilder AddNestedJoin(string toCollection)
        {
            JoinBuilder nested = new(toCollection);
            _nestedJoins.Add(nested);

            return nested;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder AddNestedJoin(string toCollection, Action<IJoinBuilder> configureJoin)
        {
            JoinBuilder nested = new(toCollection);
            configureJoin(nested);
            _nestedJoins.Add(nested);

            return nested;
        }

        /// <summary>
        /// Constructs a well-formed join string, without the join command (c:join=).
        /// </summary>
        /// <returns>A well-formed join string.</returns>
        public override string ToString()
        {
            string join = StringUtils.JoinWithoutNullOrEmptyValues('^', _toCollection, _onField, _toField, _isList, _showHideFields, _injectAt, _filterTerms, _isOuter);

            if (_nestedJoins.Count > 0)
                join += $"({ string.Join(',', _nestedJoins) })";

            return join;
        }

        public static implicit operator string(JoinBuilder j) => j.ToString();

        private static QueryCommandFormatter GetQueryCommandFormatter(string command, bool allowsMultipleArguments, string? defaultArgument = null)
        {
            if (allowsMultipleArguments)
                return new QueryCommandFormatter(command, ':', '\'', defaultArgument);
            else
                return new QueryCommandFormatter(command, ':', defaultArgument);
        }
    }
}
