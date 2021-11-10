using DbgCensus.Core.Objects;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

public class ZoneIdJsonConverter : JsonConverter<ZoneID>
{
    public override ZoneID? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.String && uint.TryParse(reader.GetString(), out uint value))
            return new ZoneID(value);
        else if (reader.TokenType is JsonTokenType.Number && reader.TryGetUInt32(out value))
            return new ZoneID(value);
        else
            return null;
    }

    public override void Write(Utf8JsonWriter writer, ZoneID value, JsonSerializerOptions options)
    {
        writer.WriteNumber("zone_id", value.CombinedId);
    }
}
