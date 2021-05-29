using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.Rest.Queries
{
    internal abstract class QueryCommandFormatterBase
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the character used to separate the command from its arguments.
        /// </summary>
        public char ComponentSeparator { get; }

        protected QueryCommandFormatterBase(string command, char componentSeparator)
        {
            Command = command;
            ComponentSeparator = componentSeparator;
        }

        public static string VerifyAndToString(object value)
        {
            string? typeName = value.GetType().FullName; // TODO: Verify this works
            string? valueString = value.ToString();

            if (string.IsNullOrEmpty(valueString) || valueString == typeName)
                throw new ArgumentException("The type " + typeName + " must have properly implemented ToString()");

            return valueString;
        }

        /// <summary>
        /// Returns a well-formed query command string.
        /// </summary>
        public override string ToString() => Command + ComponentSeparator;

        public static implicit operator string(QueryCommandFormatterBase f) => f.ToString(); // TODO: Check if this works with derived classes
    }

    internal sealed class MultiQueryCommandFormatter<T> : QueryCommandFormatterBase
    {
        private readonly List<T> _arguments;

        /// <summary>
        /// Gets the character used to separate each argument.
        /// </summary>
        public char ArgumentSeparator { get; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public IReadOnlyList<T> Arguments => _arguments.AsReadOnly();

        /// <summary>
        /// Gets a value indicating if any arguments have been added to this <see cref="MultiQueryCommandFormatter{T}"/>.
        /// </summary>
        public bool AnyArguments => _arguments.Count > 0;

        public MultiQueryCommandFormatter(string command, char componentSeparator, char argumentSeparator)
            : base(command, componentSeparator)
        {
            ArgumentSeparator = argumentSeparator;

            _arguments = new List<T>();
        }

        /// <summary>
        /// Adds an argument to the command.
        /// </summary>
        /// <param name="argument">The argument value to add.</param>
        public void AddArgument(T argument)
        {
            if (argument is null)
                throw new ArgumentNullException(nameof(argument));

            _arguments.Add(argument);
        }

        /// <summary>
        /// Adds a range of arguments to the command.
        /// </summary>
        /// <param name="arguments">The argument/s to add.</param>
        public void AddArgumentRange(IEnumerable<T> arguments)
        {
            foreach (T argument in arguments)
            {
                if (argument is null)
                    throw new ArgumentNullException(nameof(arguments));

                _arguments.Add(argument);
            }
        }

        /// <inheritdoc />
        public override string ToString() => AnyArguments
            ? base.ToString() + string.Join(ArgumentSeparator, Arguments.Select(a => VerifyAndToString(a)))
            : string.Empty;
    }

    internal sealed class SingleQueryCommandFormatter<T> : QueryCommandFormatterBase
    {
        /// <summary>
        /// Gets the argument.
        /// </summary>
        public T? Argument { get; private set; }

        public bool HasValue { get; private set; }

        public SingleQueryCommandFormatter(string command, char componentSeparator)
            : base(command, componentSeparator)
        {
        }

        /// <summary>
        /// Sets the argument of this command.
        /// </summary>
        /// <param name="argument">The argument value.</param>
        public void SetArgument(T argument)
        {
            if (argument is null)
                throw new ArgumentNullException(nameof(argument));

            Argument = argument;
            HasValue = true;
        }

        /// <inheritdoc />
        public override string ToString() => Argument is not null
            ? base.ToString() + VerifyAndToString(Argument)
            : string.Empty;
    }
}
