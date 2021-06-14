using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (ulong.TryParse(reader.GetString(), out ulong timestamp))
                return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestamp);
            else if (DateTime.TryParse(reader.GetString(), null, DateTimeStyles.AssumeUniversal, out DateTime time))
                return time;
            else
                return DateTime.MinValue;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new InvalidOperationException();
        }
    }
}
