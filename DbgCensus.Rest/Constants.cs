namespace DbgCensus.Rest;

/// <summary>
/// Defines constants for the Rest project.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the name of the configured <see cref="System.Text.Json.JsonSerializerOptions"/>
    /// that is used for deserializing payloads in an <see cref="CensusRestClient"/>.
    /// </summary>
    public const string JsonDeserializationOptionsName = "RestClientDeserialize";
}
