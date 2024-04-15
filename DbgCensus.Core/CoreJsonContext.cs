using System.Text.Json.Serialization;

namespace DbgCensus.Core;

#if NET7_0_OR_GREATER
[JsonSerializable(typeof(string))]
internal partial class CoreJsonContext : JsonSerializerContext;
#endif
