using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a tree command for the Census REST API.
    /// </summary>
    public class TreeBuilder : ITreeBuilder
    {
        private readonly QueryCommandFormatter _onField;
        private readonly QueryCommandFormatter _isList;
        private readonly QueryCommandFormatter _prefix;
        private readonly QueryCommandFormatter _startOn;

        /// <summary>
        /// Initialises a new instance of the <see cref="TreeBuilder"/> class.
        /// </summary>
        /// <param name="onField">Sets the field to group data by. Will be removed from the data source.</param>
        public TreeBuilder(string onField)
        {
            _onField = GetQueryCommandFormatter("field");
            _isList = GetQueryCommandFormatter("list", "0");
            _prefix = GetQueryCommandFormatter("prefix");
            _startOn = GetQueryCommandFormatter("start");

            OnField(onField);
        }

        /// <inheritdoc />
        public ITreeBuilder IsList()
        {
            _isList.AddArgument("1");

            return this;
        }

        /// <inheritdoc />
        public ITreeBuilder OnField(string fieldName)
        {
            _onField.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public ITreeBuilder StartOn(string fieldName)
        {
            _startOn.AddArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public ITreeBuilder WithPrefix(string prefix)
        {
            _prefix.AddArgument(prefix);

            return this;
        }

        public override string ToString() => StringUtils.JoinWithoutNullOrEmptyValues('^', _onField, _isList, _prefix, _startOn);

        public static implicit operator string(TreeBuilder t) => t.ToString();

        private static QueryCommandFormatter GetQueryCommandFormatter(string command, string? defaultArgument = null) => new QueryCommandFormatter(command, ':', defaultArgument);
    }
}
