namespace DbgCensus.Core.Objects;

/// <summary>
/// Represents Census' special zone ID format.
/// A zone ID is a <see cref="uint"/> where the upper two bytes represent the instance of the zone
/// and the lower two bytes represent the <see cref="ZoneDefinition"/>.
/// </summary>
public record ZoneID
{
    public static readonly ZoneID Default = new(0);

    /// <summary>
    /// Gets the instance of this zone.
    /// </summary>
    public ushort Instance { get; }

    /// <summary>
    /// Gets the definition of this zone.
    /// </summary>
    public ZoneDefinition Definition { get; }

    /// <summary>
    /// Gets the actual Census ID, a combination of the <see cref="Instance"/> and <see cref="Definition"/>
    /// </summary>
    public uint CombinedId { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="ZoneID"/> record.
    /// </summary>
    /// <param name="combinedId">The actual combined zone-id that was retrieved from Census.</param>
    public ZoneID(uint combinedId)
    {
        CombinedId = combinedId;
        Definition = (ZoneDefinition)(combinedId & 0xffff);
        Instance = (ushort)(combinedId >> 16);
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ZoneID"/> record.
    /// </summary>
    /// <param name="definition">The definition of the zone.</param>
    /// <param name="instance">The instance of the zone.</param>
    public ZoneID(ZoneDefinition definition, ushort instance)
    {
        Definition = definition;
        Instance = instance;
        CombinedId = ((uint)instance << 16) | (uint)definition;
    }

    public override string ToString()
        => $"{ Definition } (instance { Instance })";

    public static explicit operator uint(ZoneID zoneId) => zoneId.CombinedId;
    public static explicit operator ZoneDefinition(ZoneID zoneId) => zoneId.Definition;

    public static explicit operator ZoneID(uint combinedId) => new(combinedId);
}
