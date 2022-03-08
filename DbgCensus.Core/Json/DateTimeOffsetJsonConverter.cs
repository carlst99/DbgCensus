using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (ulong.TryParse(reader.GetString(), out ulong timestamp))
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddSeconds(timestamp);

        if (DateTimeOffset.TryParse(reader.GetString(), null, DateTimeStyles.AssumeUniversal, out DateTimeOffset time))
            return time;

        return DateTimeOffset.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new InvalidOperationException();
    }
}
