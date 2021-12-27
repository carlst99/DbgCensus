namespace DbgCensus.EventStream;

public static class Constants
{
    /// <summary>
    /// Gets the name of the configured <see cref="System.Text.Json.JsonSerializerOptions"/>
    /// that is used for deserializing payloads in an <see cref="BaseEventStreamClient"/>.
    /// </summary>
    public const string JsonDeserializationOptionsName = "EventStreamDeserialize";

    /// <summary>
    /// Gets the name of the configured <see cref="System.Text.Json.JsonSerializerOptions"/>
    /// that is used for serializing payloads in an <see cref="BaseEventStreamClient"/>.
    /// </summary>
    public const string JsonSerializationOptionsName = "EventStreamSerialize";
}
