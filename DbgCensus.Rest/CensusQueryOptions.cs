using System.Text.Json;

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
    public uint? Limit { get; set; }

    /// <summary>
    /// The JSON options to use when deserializing requests.
    /// </summary>
    public JsonSerializerOptions DeserializationOptions { get; set; }

    public CensusQueryOptions()
    {
        RootEndpoint = "https://census.daybreakgames.com";
        ServiceId = "example";
        Namespace = "ps2";
        DeserializationOptions = new JsonSerializerOptions();
        DeserializationOptions.AddCensusDeserializationOptions();
    }
}
