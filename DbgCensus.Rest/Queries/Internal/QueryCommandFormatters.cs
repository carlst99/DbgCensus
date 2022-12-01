using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DbgCensus.Rest.Queries.Internal;

internal abstract class QueryCommandFormatterBase
{
    private static readonly Dictionary<Type, string> _typeStringNames = new();

    /// <summary>
    /// Gets the command.
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Gets the character used to separate the command from its arguments.
    /// </summary>
    public char ComponentSeparator { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="QueryCommandFormatterBase"/> class.
    /// </summary>
    /// <param name="command">A Census command.</param>
    /// <param name="componentSeparator">The value used to separate the command and its argument.</param>
    protected QueryCommandFormatterBase(string command, char componentSeparator)
    {
        Command = command;
        ComponentSeparator = componentSeparator;
    }

    /// <summary>
    /// Converts an object to a string, and verifies that the object's ToString() method has been properly implemented.
    /// </summary>
    /// <param name="value">The object to convert and verify.</param>
    /// <returns>The object's string representation.</returns>
    /// <exception cref="ArgumentException">Thrown if the object's ToString() method has not been properly implemented.</exception>
    protected static string ToStringVerified<T>(T value)
        where T : notnull
    {
        if (!_typeStringNames.TryGetValue(typeof(T), out string? typeName))
        {
            typeName = typeof(T).FullName ?? typeof(T).Name;
            _typeStringNames.Add(typeof(T), typeName);
        }
        string? valueString = value.ToString();

        if (string.IsNullOrEmpty(valueString) || valueString == typeName)
            throw new ArgumentException($"The type {typeName} must have properly implemented ToString()");

        return valueString;
    }

    /// <summary>
    /// Returns a well-formed query command string.
    /// </summary>
    public override string ToString()
        => Command + ComponentSeparator;

    public static implicit operator string(QueryCommandFormatterBase f)
        => f.ToString();
}

/// <summary>
/// Allows a query command string, that accepts multiple values, to be built.
/// </summary>
/// <typeparam name="T">The type of value to be used as the argument.</typeparam>
internal sealed class MultiQueryCommandFormatter<T> : QueryCommandFormatterBase
    where T : notnull
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

    /// <summary>
    /// Initialises a new instance of the <see cref="MultiQueryCommandFormatter{T}"/> class.
    /// </summary>
    /// <param name="command">A Census command.</param>
    /// <param name="componentSeparator">The value used to separate the command and its argument.</param>
    /// <param name="argumentSeparator">The value used to separate each argument.</param>
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
        if (arguments is null)
            throw new ArgumentNullException(nameof(arguments));

        foreach (T argument in arguments)
        {
            if (argument is null)
                throw new ArgumentNullException(nameof(arguments));

            _arguments.Add(argument);
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => AnyArguments
            ? base.ToString() + string.Join(ArgumentSeparator, Arguments.Select(ToStringVerified))
            : string.Empty;
}

/// <summary>
/// Allows a query command string, that only accepts a single value, to be built.
/// </summary>
/// <typeparam name="T">The type of value to be used as the argument.</typeparam>
internal sealed class SingleQueryCommandFormatter<T> : QueryCommandFormatterBase
    where T : notnull
{
    /// <summary>
    /// Gets the argument.
    /// </summary>
    public T? Argument { get; private set; }

    /// <summary>
    /// Gets a value indicating if an argument has been set on this <see cref="SingleQueryCommandFormatter{T}"/> instance.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Argument))]
    public bool HasArgument { get; private set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="SingleQueryCommandFormatter{T}"/> class.
    /// </summary>
    /// <param name="command">A Census command.</param>
    /// <param name="componentSeparator">The value used to separate the command and its argument.</param>
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
        HasArgument = true;
    }

    /// <inheritdoc />
    public override string ToString()
        => HasArgument
            ? base.ToString() + ToStringVerified(Argument)
            : string.Empty;
}
