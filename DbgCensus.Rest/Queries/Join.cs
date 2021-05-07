using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries
{
    public class Join : IJoin
    {
        private readonly string _toCollection;
        //private readonly List<string> _showHideFields; // Single quote (') delimited
        //private readonly List<QueryFilter> _filters;
        private readonly List<IJoin> _nestedJoins;

        private readonly QueryCommandFormatter<QueryFilter> _filterTerms;

        private QueryCommandFormatter<string> _showHideFields;
        private bool _isShowingFields; // Indicates whether, if present, fields in <see cref="_showHideFields"/> should be shown (or hidden).
        private string? _onField;
        private string? _toField;
        private bool _isList;
        private string? _injectAt;
        private bool _isOuter;

        public Join(string toCollection)
        {
            _toCollection = toCollection;

            //_showHideFields = new List<string>();
            _showHideFields = GetQueryCommandFormatter<string>("show", true);
            _filterTerms = GetQueryCommandFormatter<QueryFilter>("terms", true);
            _nestedJoins = new List<IJoin>();

            _isOuter = true;
        }

        /// <summary>
        /// Constructs a well-formed join string, without the join command (c:join=).
        /// </summary>
        /// <returns>A well-formed join string.</returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IJoin ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields = GetQueryCommandFormatter<string>("show", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public IJoin HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields = GetQueryCommandFormatter<string>("hide", true);

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public IJoin InjectAt(string name)
        {
            _injectAt = name;

            return this;
        }

        /// <inheritdoc />
        public IJoin IsList()
        {
            _isList = true;

            return this;
        }

        /// <inheritdoc />
        public IJoin IsInnerJoin()
        {
            _isOuter = false;

            return this;
        }

        /// <inheritdoc />
        public IJoin OnField(string fieldName)
        {
            _onField = fieldName;

            return this;
        }

        /// <inheritdoc />
        public IJoin ToField(string fieldName)
        {
            _toField = fieldName;

            return this;
        }

        /// <inheritdoc />
        public IJoin Where<T>(string field, T filterValue, SearchModifier modifier) where T : notnull
        {
            QueryFilter queryOperator = new(field, filterValue, modifier);
            _filterTerms.AddArgument(queryOperator);

            return this;
        }

        /// <inheritdoc/>
        public IJoin WithNestedJoin(string collection)
        {
            IJoin nested = new Join(collection);
            _nestedJoins.Add(nested);

            return nested;
        }

        private static QueryCommandFormatter<T> GetQueryCommandFormatter<T>(string command, bool allowsMultipleArguments) where T : notnull
        {
            if (allowsMultipleArguments)
                return new QueryCommandFormatter<T>(command, '\'', ':');
            else
                return new QueryCommandFormatter<T>(command, ':');
        }
    }
}
