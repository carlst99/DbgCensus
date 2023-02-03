namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// A container for a query type value. Known as the 'Verb' by the Census REST API.
/// </summary>
/// <param name="Value">The query type.</param>
public readonly record struct QueryType(string Value)
{
    /// <summary>
    /// A regular query.
    /// </summary>
    public static readonly QueryType Get = new("get");

    /// <summary>
    /// Returns the number of values matching the query.
    /// </summary>
    public static readonly QueryType Count = new("count");

    public override string ToString()
        => Value;

    public static implicit operator string(QueryType t)
        => t.ToString();

    public static explicit operator QueryType(string s)
        => new(s);
}
