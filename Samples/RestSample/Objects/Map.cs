using DbgCensus.Core.Objects;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static RestSample.Objects.Map;

namespace RestSample.Objects;

/// <summary>
/// Initialises a new instance of the <see cref="Map"/> record.
/// </summary>
/// <param name="ZoneID">The ID of the zone that this map represents.</param>
/// <param name="Regions">The region model of the map.</param>
public record Map
(
    [property: JsonPropertyName("ZoneId")] ZoneID ZoneID,
    [property: JsonPropertyName("Regions")] RegionModel Regions
)
{
    public record RegionModel
    (
        [property: JsonPropertyName("IsList")] bool IsList,
        [property: JsonPropertyName("Row")] List<RowModel> Row
    );

    public record RowModel
    (
        [property: JsonPropertyName("RowData")] RowDataModel RowData
    );

    public record RowDataModel
    (
        [property: JsonPropertyName("RegionId")] int RegionID,
        [property: JsonPropertyName("FactionId")] FactionDefinition FactionID
    );
}
