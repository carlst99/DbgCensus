namespace DbgCensus.Rest.Abstractions.Queries;

/// <summary>
/// Modifiers used to filter searches in the Census REST API.
/// </summary>
public enum SearchModifier : short
{
    /// <summary>
    /// Returns values that are equal to the value of the filter.
    /// </summary>
    Equals = 0,

    /// <summary>
    /// Returns values that are not equal to the value of the filter.
    /// </summary>
    NotEquals = (short)'!',

    /// <summary>
    /// Returns values that contain the value of the filter.
    /// </summary>
    Contains = (short)'*',

    /// <summary>
    /// Returns values that are less than the value of the filter.
    /// </summary>
    LessThan = (short)'<',

    /// <summary>
    /// Returns values that are greater than the value of the filter.
    /// </summary>
    GreaterThan = (short)'>',

    /// <summary>
    /// Returns values that are less than or equal to the value of the filter.
    /// </summary>
    LessThanOrEqual = (short)'[',

    /// <summary>
    /// Returns values that are greater than or equal to the value of the filter.
    /// </summary>
    GreaterThanOrEqual = (short)']',

    /// <summary>
    /// Returns values that start with the value of the filter.
    /// </summary>
    StartsWith = (short)'^'
}
