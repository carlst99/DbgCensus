using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

public class BooleanJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.True)
            return true;

        if (reader.TokenType is JsonTokenType.False)
            return false;

        if (reader.TokenType is not JsonTokenType.String)
            throw new JsonException("Could not convert token to boolean.");

        string? data = reader.GetString();

        return data switch
        {
            null => throw new JsonException("Could not convert token to boolean: token was null"),
            "0" => false,
            "1" => true,
            _ => bool.TryParse(data, out bool result)
                ? result
                : throw new JsonException("Could not convert token to boolean - invalid format: " + data)
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteBooleanValue(value);
}
