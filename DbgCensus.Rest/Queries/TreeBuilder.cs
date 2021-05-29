using DbgCensus.Core.Utils;
using DbgCensus.Rest.Abstractions.Queries;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a tree command for the Census REST API.
    /// </summary>
    public class TreeBuilder : ITreeBuilder
    {
        private readonly SingleQueryCommandFormatter<string> _onField;
        private readonly SingleQueryCommandFormatter<char?> _isList; // No value by default, defaults to '0' in Census
        private readonly SingleQueryCommandFormatter<string> _prefix;
        private readonly SingleQueryCommandFormatter<string> _startOn;

        /// <summary>
        /// Initialises a new instance of the <see cref="TreeBuilder"/> class.
        /// </summary>
        /// <param name="onField">Sets the field to group data by. Will be removed from the data source.</param>
        public TreeBuilder(string onField)
        {
            _onField = GetSingleQCF<string>("field");
            _isList = GetSingleQCF<char?>("list");
            _prefix = GetSingleQCF<string>("prefix");
            _startOn = GetSingleQCF<string>("start");

            OnField(onField);
        }

        /// <inheritdoc />
        public virtual ITreeBuilder IsList()
        {
            _isList.SetArgument('1');

            return this;
        }

        /// <inheritdoc />
        public virtual ITreeBuilder OnField(string fieldName)
        {
            _onField.SetArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual ITreeBuilder StartOn(string fieldName)
        {
            _startOn.SetArgument(fieldName);

            return this;
        }

        /// <inheritdoc />
        public virtual ITreeBuilder WithPrefix(string prefix)
        {
            _prefix.SetArgument(prefix);

            return this;
        }

        public override string ToString() => StringUtils.JoinWithoutNullOrEmptyValues('^', _onField, _isList, _prefix, _startOn);

        public static implicit operator string(TreeBuilder t) => t.ToString();

        private static SingleQueryCommandFormatter<T> GetSingleQCF<T>(string command) => new(command, ':');
    }
}
