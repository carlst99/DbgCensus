namespace DbgCensus.Core.Exceptions;

/// <summary>
/// Stores information about invalid or unexpected data that was returned from the Census API.
/// </summary>
#pragma warning disable RCS1194 // Implement exception constructors.
public class CensusInvalidDataException : CensusException
#pragma warning restore RCS1194 // Implement exception constructors.
{
    /// <summary>
    /// The raw data that was returned.
    /// </summary>
    public string? JsonData { get; }

    /// <summary>
    /// Stores information about invalid or unexpected data that was returned from the Census API.
    /// </summary>
    /// <param name="message">The reason this exception was thrown.</param>
    /// <param name="jsonData">The raw data that was returned.</param>
    public CensusInvalidDataException(string message, string? jsonData)
        : base(message)
    {
        JsonData = jsonData;
    }
}
