using System.Text.Json;

namespace DbgCensus.Core.Json;

public class CamelCaseJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => char.ToUpperInvariant(name[0]) + name[1..^0];
}
