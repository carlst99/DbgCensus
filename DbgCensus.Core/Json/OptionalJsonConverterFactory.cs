using DbgCensus.Core.Objects;
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DbgCensus.Core.Json;

public class OptionalJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        TypeInfo typeInfo = typeToConvert.GetTypeInfo();
        if (!typeInfo.IsGenericType || typeInfo.IsGenericTypeDefinition)
            return false;

        Type genericType = typeInfo.GetGenericTypeDefinition();

        return genericType == typeof(Optional<>);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        TypeInfo typeInfo = typeToConvert.GetTypeInfo();
        Type optionalType = typeof(OptionalJsonConverter<>).MakeGenericType(typeInfo.GenericTypeArguments);

        if (Activator.CreateInstance(optionalType) is not JsonConverter createdConverter)
            throw new JsonException();

        return createdConverter;
    }
}
