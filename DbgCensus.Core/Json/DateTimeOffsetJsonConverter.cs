using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Number)
            return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());

        if (long.TryParse(reader.GetString(), out long timestamp))
            return DateTimeOffset.FromUnixTimeSeconds(timestamp);

        if (DateTimeOffset.TryParse(reader.GetString(), null, DateTimeStyles.AssumeUniversal, out DateTimeOffset time))
            return time;

        throw new JsonException("Failed to read token as DateTimeOffset");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => throw new InvalidOperationException();
}
