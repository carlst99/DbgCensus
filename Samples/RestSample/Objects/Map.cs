using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestSample.Objects
{
    /// <summary>
    /// The query model for https://census.daybreakgames.com/s:WVWCeOP4oeLb/get/ps2/map?world_id=1&zone_ids=2,4,6,8
    /// </summary>
    public record Map
    {
        public record RegionModel
        {
            public record RowModel
            {
                public record RowDataModel
                {
                    [JsonPropertyName("RegionId")]
                    public int RegionId { get; init; }

                    [JsonPropertyName("FactionId")]
                    public Faction FactionId { get; init; }
                }

                [JsonPropertyName("RowData")]
                public RowDataModel RowData { get; init; }

                public RowModel()
                {
                    RowData = new RowDataModel();
                }
            }

            [JsonPropertyName("IsList")]
            public bool IsList { get; init; }

            [JsonPropertyName("Row")]
            public List<RowModel> Row { get; init; }

            public RegionModel()
            {
                Row = new List<RowModel>();
            }
        }

        [JsonPropertyName("ZoneId")]
        public ZoneType ZoneId { get; init; }

        [JsonPropertyName("Regions")]
        public RegionModel Regions { get; init; }

        public Map()
        {
            Regions = new RegionModel();
        }
    }
}
