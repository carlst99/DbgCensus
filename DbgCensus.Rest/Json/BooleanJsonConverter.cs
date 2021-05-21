using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Rest.Json
{
    public class BooleanJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (bool.TryParse(reader.GetString(), out bool result))
                return result;
            else
                return reader.GetString() == "1";
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
    }
}
