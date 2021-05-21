using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CensusEndpointMonitor.Cli
{
    public class CensusCollection
    {
        public string Name { get; set; }

        [JsonPropertyName("hidden")]
        public bool IsHidden { get; set; }
        public ulong Count { get; set; }
        public List<string> ResolveList { get; set; }

        public CensusCollection()
        {
            Name = string.Empty;
            ResolveList = new List<string>();
        }
    }
}
