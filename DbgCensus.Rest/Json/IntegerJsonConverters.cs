using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Rest.Json
{
    public class Int16JsonConverter : JsonConverter<Int16>
    {
        public override Int16 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt16();
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("The token could not be converted to an Int16");

            string? data = reader.GetString();
            if (data is null)
                return 0;

            if (Int16.TryParse(data, out Int16 result))
                return result;
            else if (data == "?" || data == "dynamic")
                return 0;
            else
                throw new JsonException("The token could not be converted to an Int16: " + data);
        }

        public override void Write(Utf8JsonWriter writer, Int16 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class Int32JsonConverter : JsonConverter<Int32>
    {
        public override Int32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt32();
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("The token could not be converted to an Int32");

            string? data = reader.GetString();
            if (data is null)
                return 0;

            if (Int32.TryParse(data, out Int32 result))
                return result;
            else if (data == "?" || data == "dynamic")
                return 0;
            else
                throw new JsonException("The token could not be converted to an Int32: " + data);
        }

        public override void Write(Utf8JsonWriter writer, Int32 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class Int64JsonConverter : JsonConverter<Int64>
    {
        public override Int64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetInt64();
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("The token could not be converted to an Int64");

            string? data = reader.GetString();
            if (data is null)
                return 0;

            if (Int64.TryParse(data, out Int64 result))
                return result;
            else if (data == "?" || data == "dynamic")
                return 0;
            else
                throw new JsonException("The token could not be converted to an Int64: " + data);
        }

        public override void Write(Utf8JsonWriter writer, Int64 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class UInt16JsonConverter : JsonConverter<UInt16>
    {
        public override UInt16 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetUInt16();
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("The token could not be converted to an UInt16");

            string? data = reader.GetString();
            if (data is null)
                return 0;

            if (UInt16.TryParse(data, out UInt16 result))
                return result;
            else if (data == "?" || data == "dynamic")
                return 0;
            else
                throw new JsonException("The token could not be converted to an UInt16: " + data);
        }

        public override void Write(Utf8JsonWriter writer, UInt16 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class UInt32JsonConverter : JsonConverter<UInt32>
    {
        public override UInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetUInt32();
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("The token could not be converted to an UInt32");

            string? data = reader.GetString();
            if (data is null)
                return 0;

            if (UInt32.TryParse(data, out UInt32 result))
                return result;
            else if (data == "?" || data == "dynamic")
                return 0;
            else
                throw new JsonException("The token could not be converted to an UInt32: " + data);
        }

        public override void Write(Utf8JsonWriter writer, UInt32 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class UInt64JsonConverter : JsonConverter<UInt64>
    {
        public override UInt64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetUInt64();
            else if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("The token could not be converted to an UInt64");

            string? data = reader.GetString();
            if (data is null)
                return 0;

            if (UInt64.TryParse(data, out UInt64 result))
                return result;
            else if (data == "?" || data == "dynamic")
                return 0;
            else
                throw new JsonException("The token could not be converted to an UInt64: " + data);
        }

        public override void Write(Utf8JsonWriter writer, UInt64 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

}