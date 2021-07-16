using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a join command for the Census REST API.
    /// </summary>
    public class JoinBuilder : IJoinBuilder
    {
        private readonly List<IJoinBuilder> _nestedJoins;

        private readonly SingleQueryCommandFormatter<string> _toCollection;
        private readonly MultiQueryCommandFormatter<QueryFilter> _filterTerms;
        private readonly SingleQueryCommandFormatter<string> _onField;
        private readonly SingleQueryCommandFormatter<string> _toField;
        private readonly SingleQueryCommandFormatter<string> _injectAt;
        private readonly SingleQueryCommandFormatter<char?> _isList; // No value by default, defaults to '0' in Census
        private readonly SingleQueryCommandFormatter<char?> _isOuter; // No value by default, defaults to '1' in Census

        private MultiQueryCommandFormatter<string> _showHideFields;
        private bool _isShowingFields; // Indicates whether, if present, fields in "_showHideFields" should be shown (or hidden).

        /// <summary>
        /// Initialises a new instance of the <see cref="JoinBuilder"/> class.
        /// </summary>
        /// <param name="toCollection">The name of the collection to join to.</param>
        public JoinBuilder(string toCollection)
        {
            _nestedJoins = new List<IJoinBuilder>();

            _toCollection = GetSingleQCF<string>("type");
            _filterTerms = GetMultiQCF<QueryFilter>("terms");
            _onField = GetSingleQCF<string>("on");
            _toField = GetSingleQCF<string>("to");
            _injectAt = GetSingleQCF<string>("inject_at");
            _isList = GetSingleQCF<char?>("list");
            _isOuter = GetSingleQCF<char?>("outer");

            _showHideFields = GetMultiQCF<string>("show");

            ToCollection(toCollection);
        }

        /// <inheritdoc/>
        public virtual IJoinBuilder ToCollection(string collectionName)
        {
            _toCollection.SetArgument(collectionName);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder ShowFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (!_isShowingFields)
                _showHideFields = GetMultiQCF<string>("show");

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = true;

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder HideFields(params string[] fieldNames)
        {
            // Show and hide are incompatible
            if (_isShowingFields)
                _showHideFields = GetMultiQCF<string>("hide");

            _showHideFields.AddArgumentRange(fieldNames);
            _isShowingFields = false;

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder InjectAt(string name)
        {
            _injectAt.SetArgument(name);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder IsList()
        {
            _isList.SetArgument('1');

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder IsInnerJoin()
        {
            _isOuter.SetArgument('0');

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder OnField(string fieldName)
        {
            _onField.SetArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder ToField(string fieldName)
        {
            _toField.SetArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual IJoinBuilder Where(string field, SearchModifier modifier, string filterValue)
        {
            _filterTerms.AddArgument(new QueryFilter(field, modifier, filterValue));

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

            return this;
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

        private static MultiQueryCommandFormatter<T> GetMultiQCF<T>(string command) => new(command, ':', '\'');

        private static SingleQueryCommandFormatter<T> GetSingleQCF<T>(string command) => new(command, ':');
    }
}
