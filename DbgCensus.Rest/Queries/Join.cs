using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Functions to build a join string for the Census REST API.
    /// </summary>
    public class Join : IJoin
    {
        private readonly List<IJoin> _nestedJoins;

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
        /// Initialises a new instance of the <see cref="Join"/> class.
        /// </summary>
        /// <param name="toCollection">The name of the collection to join to.</param>
        public Join(string toCollection)
        {
            _nestedJoins = new List<IJoin>();

            _toCollection = GetQueryCommandFormatter("type", false, toCollection);
            _filterTerms = GetQueryCommandFormatter("terms", true);
            _onField = GetQueryCommandFormatter("on", false);
            _toField = GetQueryCommandFormatter("to", false);
            _isList = GetQueryCommandFormatter("list", false, "0");
            _injectAt = GetQueryCommandFormatter("inject_at", false);
            _isOuter = GetQueryCommandFormatter("outer", false, "1");
            _showHideFields = GetQueryCommandFormatter("show", true);
        }

        /// <inheritdoc />
        public IJoin ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields = GetQueryCommandFormatter("show", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public IJoin HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields = GetQueryCommandFormatter("hide", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public IJoin InjectAt(string name)
        {
            _injectAt.AddArgument(name);

            return this;
        }

        /// <inheritdoc />
        public IJoin IsList()
        {
            _isList.AddArgument("1");

            return this;
        }

        /// <inheritdoc />
        public IJoin IsInnerJoin()
        {
            _isOuter.AddArgument("0");

            return this;
        }

        /// <inheritdoc />
        public IJoin OnField(string fieldName)
        {
            _onField.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public IJoin ToField(string fieldName)
        {
            _toField.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public IJoin Where<T>(string field, T filterValue, SearchModifier modifier) where T : notnull
        {
            string? filterValueString = filterValue.ToString();
            if (string.IsNullOrEmpty(filterValueString) || filterValueString.Equals(typeof(T).FullName))
                throw new ArgumentException(nameof(filterValue) + " must have properly implemented ToString()", nameof(filterValue));

            QueryFilter queryFilter = new(field, filterValueString, modifier);
            _filterTerms.AddArgument(queryFilter);

            return this;
        }

        /// <inheritdoc/>
        public IJoin WithNestedJoin(string toCollection)
        {
            IJoin nested = new Join(toCollection);
            _nestedJoins.Add(nested);

            return nested;
        }

        public static implicit operator string(Join j) => j.ToString();

        /// <summary>
        /// Constructs a well-formed join string, without the join command (c:join=).
        /// </summary>
        /// <returns>A well-formed join string.</returns>
        public override string ToString()
        {
            string join = JoinWithoutNullOrEmptyValues('^', _toCollection, _onField, _toField, _isList, _showHideFields, _injectAt, _filterTerms, _isOuter);

            if (_nestedJoins.Count > 0)
                join += $"({ string.Join(',', _nestedJoins) })";

            return join;
        }

        private static QueryCommandFormatter GetQueryCommandFormatter(string command, bool allowsMultipleArguments, string? defaultArgument = null)
        {
            if (allowsMultipleArguments)
                return new QueryCommandFormatter(command, ':', '\'', defaultArgument);
            else
                return new QueryCommandFormatter(command, ':', defaultArgument);
        }

        private static string JoinWithoutNullOrEmptyValues(char separator, params string[] value) => string.Join(separator, value.Where(str => !string.IsNullOrEmpty(str)));
    }
}
