namespace DbgCensus.Rest;

public class CensusQueryOptions
{
    /// <summary>
    /// A default key under which configuration for this object can be written.
    /// </summary>
    public const string CONFIG_KEY = "CensusQueryOptions";

    /// <summary>
    /// The root endpoint of the Census API.
    /// </summary>
    public string RootEndpoint { get; set; } = "https://census.daybreakgames.com";

    /// <summary>
    /// The service ID used to authenticate with the Census API.
    /// </summary>
    public string ServiceId { get; set; } = "example";

    /// <summary>
    /// The Census namespace to retrieve data from.
    /// </summary>
    public string Namespace { get; set; } = "ps2";

    /// <summary>
    /// Optionally remove all translations by default from internationalized strings except the one specified.
    /// </summary>
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Optionally set a default limit for each query.
    /// </summary>
    public int? Limit { get; set; }
}
