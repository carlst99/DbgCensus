using System;
using System.Collections.Generic;

namespace DbgCensus.Rest.Queries
{
    /// <summary>
    /// Provides functions to build a query command.
    /// </summary>
    /// <typeparam name="T">The type of object that acts as arguments of the command.</typeparam>
    internal sealed class QueryCommandFormatter<T> where T : notnull
    {
        private readonly Func<T, string?> _converter;
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
        /// Provides functions to build a query command with one argument.
        /// </summary>
        /// <param name="command">The command, i.e. c:join.</param>
        /// <param name="componentSeparator">The value used to separate each argument.</param>
        public QueryCommandFormatter(string command, char componentSeparator)
            : this(command, componentSeparator, char.MinValue)
        {
            AllowsMultipleArguments = false;
        }

        /// <summary>
        /// Provides functions to build a query command with one argument.
        /// </summary>
        /// <param name="command">The command, i.e. c:join.</param>
        /// <param name="componentSeparator">The value used to separate each argument.</param>
        /// <param name="converter">A function to convert an object of <see cref="T"/> into a string representation that is a valid argument.</param>
        public QueryCommandFormatter(string command, char componentSeparator, Func<T, string?> converter)
            : this(command, componentSeparator, char.MinValue, converter)
        {
            AllowsMultipleArguments = false;
        }

        /// <summary>
        /// Provides functions to build a query command with multiple arguments.
        /// </summary>
        /// <param name="command">The command, i.e. c:join.</param>
        /// <param name="componentSeparator">The value used to separate the command and its arguments.</param>
        /// <param name="argumentSeparator">The value used to separate each argument.</param>
        public QueryCommandFormatter(string command, char componentSeparator, char argumentSeparator)
            : this(command, argumentSeparator, componentSeparator, (t) => t.ToString())
        {
        }

        /// <summary>
        /// Provides functions to build a query command.
        /// </summary>
        /// <param name="command">The command, i.e. c:join.</param>
        /// <param name="componentSeparator">The value used to separate the command and its arguments.</param>
        /// <param name="argumentSeparator">The value used to separate each argument.</param>
        /// <param name="converter">A function to convert an object of <see cref="T"/> into a string representation that is a valid argument.</param>
        public QueryCommandFormatter(string command, char componentSeparator, char argumentSeparator, Func<T, string?> converter)
        {
            _arguments = new List<string>();
            AllowsMultipleArguments = true;

            Command = command;
            ComponentSeparator = componentSeparator;
            ArgumentSeparator = argumentSeparator;
            _converter = converter;
        }

        public void AddArgument(T argument)
        {
            string? argumentStringValue = _converter(argument);
            if (string.IsNullOrEmpty(argumentStringValue))
                throw new ArgumentNullException(nameof(argument), nameof(argument) + ".ToString() returned a null or empty value.");

            if (AllowsMultipleArguments || _arguments.Count == 0)
                _arguments.Add(argumentStringValue);
            else
                _arguments[0] = argumentStringValue;
        }

        /// <summary>
        /// Adds an argument to the command.
        /// </summary>
        /// <param name="arguments">The argument/s to add.</param>
        public void AddArgumentRange(IEnumerable<T> arguments)
        {
            if (!AllowsMultipleArguments)
                throw new InvalidOperationException("Multiple arguments are not allowed.");

            foreach (T argument in arguments)
            {
                string? argumentStringValue = _converter(argument);

                if (string.IsNullOrEmpty(argumentStringValue))
                    throw new ArgumentNullException(nameof(arguments), nameof(arguments) + ".ToString() returned a null or empty value.");

                _arguments.Add(argumentStringValue);
            }
        }

        /// <summary>
        /// Parses a <see cref="QueryCommandFormatter{T}"/> from a string.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="argumentSeparator">The character used to separate each argument.</param>
        /// <returns>A <see cref="QueryCommandFormatter{T}"/> instance.</returns>
        /// <exception cref="FormatException">Throw when an invalid value is submitted.</exception>
        public static QueryCommandFormatter<T> Parse(string value, char argumentSeparator)
        {
            string[] components = value.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (components.Length != 2)
                throw new FormatException("Value is not in the format <command>=<arguments>.");

            QueryCommandFormatter<T> formatter = new(components[0], argumentSeparator);
            formatter._arguments.AddRange(components[1].Split(argumentSeparator));

            return formatter;
        }

        /// <summary>
        /// Attempts to parse a <see cref="QueryCommandFormatter{T}"/> from a string.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="argumentSeparator">The character used to separate each argument.</param>
        /// <param name="queryCommand">The parsed <see cref="QueryCommandFormatter{T}"/></param>
        /// <returns>A boolean indicating whether the value was successfully parsed.</returns>
        public static bool TryParse(string value, char argumentSeparator, out QueryCommandFormatter<T>? queryCommand)
        {
            queryCommand = null;

            string[] components = value.Split('=');
            if (components.Length != 2)
                return false;

            queryCommand = new(components[0], argumentSeparator);
            queryCommand._arguments.AddRange(components[1].Split(argumentSeparator));

            return true;
        }

        /// <summary>
        /// Constructs a well-formed query command string.
        /// </summary>
        public override string ToString() => Command + ComponentSeparator + string.Join(ArgumentSeparator, _arguments);
    }
}
