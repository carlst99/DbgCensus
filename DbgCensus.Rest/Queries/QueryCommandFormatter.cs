using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a query command.
    /// </summary>
    internal sealed class QueryCommandFormatter
    {
        private readonly List<string> _arguments;

        /// <summary>
        /// Gets the command, i.e. c:join.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the value used to separate each argument.
        /// </summary>
        public char ArgumentSeparator { get; }

        /// <summary>
        /// Gets the value used to separate the command and its arguments.
        /// </summary>
        public char ComponentSeparator { get; }

        /// <summary>
        /// Gets a value indicating whether multiple arguments can be added.
        /// </summary>
        public bool AllowsMultipleArguments { get; }

        /// <summary>
        /// Gets the arguments used in the command.
        /// </summary>
        public IReadOnlyList<string> Arguments => _arguments.AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether any arguments have been added to this query command.
        /// </summary>
        public bool AnyValue => _arguments.Count > 0;

        /// <summary>
        /// Provides functions to build a query command with one argument.
        /// </summary>
        /// <param name="command">The command, i.e. c:join.</param>
        /// <param name="componentSeparator">The value used to separate each argument.</param>
        public QueryCommandFormatter(string command, char componentSeparator, string? defaultArgument = null)
            : this(command, componentSeparator, char.MinValue, defaultArgument)
        {
            AllowsMultipleArguments = false;
        }

        /// <summary>
        /// Provides functions to build a query command.
        /// </summary>
        /// <param name="command">The command, i.e. c:join.</param>
        /// <param name="componentSeparator">The value used to separate the command and its arguments.</param>
        /// <param name="argumentSeparator">The value used to separate each argument.</param>
        public QueryCommandFormatter(string command, char componentSeparator, char argumentSeparator, string? defaultArgument = null)
        {
            _arguments = new List<string>();
            AllowsMultipleArguments = true;

            Command = command;
            ComponentSeparator = componentSeparator;
            ArgumentSeparator = argumentSeparator;

            if (defaultArgument is not null)
                AddArgument(defaultArgument);
        }

        public void AddArgument(string argument)
        {
            if (AllowsMultipleArguments || _arguments.Count == 0)
                _arguments.Add(argument);
            else
                _arguments[0] = argument;
        }

        /// <summary>
        /// Adds an argument to the command.
        /// </summary>
        /// <param name="arguments">The argument/s to add.</param>
        public void AddArgumentRange(IEnumerable<string> arguments)
        {
            if (!AllowsMultipleArguments)
                throw new InvalidOperationException("Multiple arguments are not allowed.");

            foreach (string argument in arguments)
                _arguments.Add(argument);
        }

        public static implicit operator string(QueryCommandFormatter f) => f.ToString();

        /// <summary>
        /// Constructs a well-formed query command string.
        /// </summary>
        public override string ToString() => _arguments.Count > 0
            ? Command + ComponentSeparator + string.Join(ArgumentSeparator, _arguments)
            : string.Empty;
    }
}
