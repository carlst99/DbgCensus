using DbgCensus.EventStream.Abstractions.Objects;
using OneOf;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.EventStream.Json;

public sealed class SubscribeUInt64ListJsonConverter : JsonConverter<OneOf<All, IEnumerable<ulong>>?>
{
    private readonly All _all = new();

    public override OneOf<All, IEnumerable<ulong>>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
            return null;

        if (reader.TokenType is not JsonTokenType.StartArray)
            throw new JsonException("Expected a StartArray token.");

        reader.Read();

        if (reader.TokenType is JsonTokenType.EndArray)
            return null;

        if (reader.TokenType is JsonTokenType.String && reader.GetString() == _all.ToString())
        {
            reader.Read(); // Read past the end of the array
            return new All();
        }

        List<ulong> list = new();
        while (reader.TokenType is not JsonTokenType.EndArray)
        {
            string? value = reader.GetString();
            if (value is not null)
                list.Add(ulong.Parse(value));

            reader.Read();
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, OneOf<All, IEnumerable<ulong>>? value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        if (value is null)
        {
            writer.WriteEndArray();
            return;
        }

        if (value.Value.IsT0)
        {
            writer.WriteStringValue(_all.ToString());
        }
        else
        {
            foreach (ulong item in value.Value.AsT1)
                writer.WriteStringValue(item.ToString());
        }

        writer.WriteEndArray();
    }
}
