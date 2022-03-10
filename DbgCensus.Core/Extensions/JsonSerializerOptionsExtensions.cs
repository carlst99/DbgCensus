using DbgCensus.Core.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds required options to deserialize Census JSON data.
    /// </summary>
    /// <param name="options">The options instance to modify.</param>
    public static void AddCensusDeserializationOptions(this JsonSerializerOptions options)
    {
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;

        options.PropertyNamingPolicy ??= new SnakeCaseJsonNamingPolicy();

        options.Converters.Add(new BooleanJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new ZoneIdJsonConverter());
        options.Converters.Add(new OptionalJsonConverterFactory());
        options.Converters.Add(new DateTimeJsonConverter());
        options.Converters.Add(new DateTimeOffsetJsonConverter());
    }
}
