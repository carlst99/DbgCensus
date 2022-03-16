using System.Collections.Generic;

namespace DbgCensus.Rest;

public class CensusQueryOptions
{
    /// <summary>
    /// The root endpoint of the Census API.
    /// </summary>
    public string RootEndpoint { get; set; }

    /// <summary>
    /// The service ID used to authenticate with the Census API.
    /// </summary>
    public string ServiceId { get; set; }

    /// <summary>
    /// Gets a list of service IDs that can be used to authenticate with the Census API.
    /// </summary>
    public List<string> ServiceIDs { get; }

    /// <summary>
    /// The Census namespace to retrieve data from.
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// Optionally remove all translations by default from internationalized strings except the one specified.
    /// </summary>
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Optionally set a default limit for each query.
    /// </summary>
    public int? Limit { get; set; }

    public CensusQueryOptions()
    {
        RootEndpoint = "https://census.daybreakgames.com";
        ServiceId = "example";
        ServiceIDs = new List<string>();
        Namespace = "ps2";
    }
}
