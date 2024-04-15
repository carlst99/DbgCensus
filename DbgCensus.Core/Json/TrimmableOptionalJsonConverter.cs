using DbgCensus.Core.Objects;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace DbgCensus.Core.Json;

/// <summary>
/// Converts an <see cref="Optional{TValue}"/> to/from JSON.
/// </summary>
/// <typeparam name="TValue">The value type of the optional.</typeparam>
public class TrimmableOptionalJsonConverter<TValue> : JsonConverter<Optional<TValue?>>
{
    private readonly JsonTypeInfo<TValue> _typeInfo;

    public TrimmableOptionalJsonConverter(JsonTypeInfo<TValue> typeInfo)
    {
        _typeInfo = typeInfo;
    }

    /// <inheritdoc />
    public override Optional<TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Optional<TValue?>(JsonSerializer.Deserialize(ref reader, _typeInfo));
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

        JsonSerializer.Serialize(writer, value.Value, _typeInfo);
    }
}
