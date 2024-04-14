using System.Text.Json.Serialization;

namespace DbgCensus.Rest;

[JsonSerializable(typeof(ulong))]
#if NET7_0_OR_GREATER
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
#else
[JsonSourceGenerationOptions]
#endif
internal partial class RestJsonContext : JsonSerializerContext
{
}
