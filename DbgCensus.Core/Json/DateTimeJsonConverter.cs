using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Number)
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(reader.GetInt64());

        if (long.TryParse(reader.GetString(), out long timestamp))
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);

        if (DateTime.TryParse(reader.GetString(), null, DateTimeStyles.AssumeUniversal, out DateTime time))
            return time;

        throw new JsonException("Failed to read token as DateTimeOffset");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => throw new InvalidOperationException();
}
