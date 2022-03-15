using DbgCensus.Core.Objects;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

// TODO: Fix. See BerryBrowser for error repro.

/// <summary>
/// Converts an <see cref="Optional{TValue}"/> to/from JSON.
/// </summary>
/// <typeparam name="TValue">The value type of the optional.</typeparam>
public class OptionalJsonConverter<TValue> : JsonConverter<Optional<TValue?>>
{
    /// <inheritdoc />
    public override Optional<TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(JsonSerializer.Deserialize<TValue>(ref reader, options));

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
