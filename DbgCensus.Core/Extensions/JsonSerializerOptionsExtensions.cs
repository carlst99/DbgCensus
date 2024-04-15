using DbgCensus.Core;
using DbgCensus.Core.Json;
using DbgCensus.Core.Objects;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace System.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds required options to deserialize Census JSON data and DbgCensus-specific types.
    /// </summary>
    /// <param name="options">The options instance to modify.</param>
#if NET7_0_OR_GREATER
    [RequiresDynamicCode("This method registers JSON converters that may not be compatible with trimming and Native AOT.")]
#endif
    public static void AddCensusDeserializationOptions(this JsonSerializerOptions options)
    {
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;

        options.PropertyNamingPolicy ??= SnakeCaseJsonNamingPolicy.Default;

        options.Converters.Add(new BooleanJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new ZoneIdJsonConverter());
        options.Converters.Add(new OptionalJsonConverterFactory());
        options.Converters.Add(new DateTimeJsonConverter());
        options.Converters.Add(new DateTimeOffsetJsonConverter());
    }

    /// <summary>
    /// Adds required options to deserialize Census JSON data and DbgCensus-specific types.
    /// This method is a NativeAOT/trimming compatible variant of <see cref="AddCensusDeserializationOptions"/>.
    /// As such, only specific enum and <see cref="Optional{TValue}"/> converters are registered.
    /// </summary>
    /// <remarks>
    /// To use these options in a trimming-compatible manner, create a new instance of your
    /// <see cref="JsonSerializerContext"/> and provide the <paramref name="options"/> to the constructor.
    /// Use this created instance to retrieve JSON type infos.
    /// </remarks>
    /// <param name="options">The options instance to modify.</param>
#if NET7_0_OR_GREATER
    public static void AddDbgCensusOptions(this JsonSerializerOptions options)
    {
        options.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.PropertyNamingPolicy ??= SnakeCaseJsonNamingPolicy.Default;

        options.Converters.Add(new BooleanJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter<FactionDefinition>());
        options.Converters.Add(new JsonStringEnumConverter<MetagameEventDefinition>());
        options.Converters.Add(new JsonStringEnumConverter<MetagameEventState>());
        options.Converters.Add(new JsonStringEnumConverter<WorldDefinition>());
        options.Converters.Add(new JsonStringEnumConverter<ZoneDefinition>());
        options.Converters.Add(new ZoneIdJsonConverter());
        options.Converters.Add(new TrimmableOptionalJsonConverter<string>(CoreJsonContext.Default.String));
        options.Converters.Add(new DateTimeJsonConverter());
        options.Converters.Add(new DateTimeOffsetJsonConverter());
    }
#endif
}
