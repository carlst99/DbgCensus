using DbgCensus.Core.Objects;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

/// <summary>
/// Converts an <see cref="Optional{TValue}"/> to/from JSON.
/// </summary>
/// <typeparam name="TValue">The value type of the optional.</typeparam>
#if NET7_0_OR_GREATER
[RequiresDynamicCode("Ensure the TValue of the optional is not trimmed, or use the TrimmableOptionalJsonConverter<TValue>")]
#endif
[RequiresUnreferencedCode("Ensure the TValue of the optional is not trimmed, or use the TrimmableOptionalJsonConverter<TValue>")]
public class OptionalJsonConverter<TValue> : JsonConverter<Optional<TValue?>>
{
    /// <inheritdoc />
    public override Optional<TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Optional<TValue?>(JsonSerializer.Deserialize<TValue>(ref reader, options));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Optional<TValue?> value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        if (value.Value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
