namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// Contains constant fields for the various query types. Known as the Verb by the Census REST API.
/// </summary>
public sealed class QueryType
{
    /// <summary>
    /// A regular query.
    /// </summary>
    public static readonly QueryType Get = new("get");

    /// <summary>
    /// Returns the number of values matching the query.
    /// </summary>
    public static readonly QueryType Count = new("count");

    /// <summary>
    /// Gets the value of the verb.
    /// </summary>
    public string Value { get; }

    private QueryType(string modifier)
    {
        Value = modifier;
    }

    public static implicit operator string(QueryType t) => t.ToString();
    public static explicit operator QueryType(string s) => new(s);

    public override string ToString() => Value;

    public override bool Equals(object? obj)
        => obj is QueryType qt
        && qt.Value.Equals(Value);

    public override int GetHashCode() => Value.GetHashCode();
}
