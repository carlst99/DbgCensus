namespace DbgCensus.Core.Objects;

/// <summary>
/// Represents Census' special zone ID format.
/// A zone ID is a <see cref="uint"/> where the upper two bytes represent the instance of the zone
/// and the lower two bytes represent the <see cref="ZoneDefinition"/>.
/// </summary>
public readonly struct ZoneID
{
    /// <summary>
    /// Gets the actual Census ID, a combination of the <see cref="Instance"/> and <see cref="Definition"/>
    /// </summary>
    public uint CombinedId { get; }

    /// <summary>
    /// Gets the instance of this zone.
    /// </summary>
    public ushort Instance => (ushort)(CombinedId >> 16);

    /// <summary>
    /// Gets the definition of this zone.
    /// </summary>
    public ZoneDefinition Definition => (ZoneDefinition)(CombinedId & 0xffff);

    /// <summary>
    /// Initialises a new instance of the <see cref="ZoneID"/> struct.
    /// </summary>
    /// <param name="combinedId">The actual combined zone-id that was retrieved from Census.</param>
    public ZoneID(uint combinedId)
    {
        CombinedId = combinedId;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ZoneID"/> struct.
    /// </summary>
    /// <param name="definition">The definition of the zone.</param>
    /// <param name="instance">The instance of the zone.</param>
    public ZoneID(ZoneDefinition definition, ushort instance)
    {
        CombinedId = ((uint)instance << 16) | (uint)definition;
    }

    public override string ToString()
        => $"{ Definition } (instance { Instance })";

    public override bool Equals(object? obj)
        => obj is ZoneID zoneId
           && zoneId.CombinedId == CombinedId;

    public override int GetHashCode()
        => CombinedId.GetHashCode();

    public static explicit operator uint(ZoneID zoneId)
        => zoneId.CombinedId;

    public static explicit operator ZoneDefinition(ZoneID zoneId)
        => zoneId.Definition;

    public static explicit operator ZoneID(uint combinedId)
        => new(combinedId);

    public static bool operator ==(ZoneID z1, ZoneID z2)
        => z1.Equals(z2);

    public static bool operator !=(ZoneID z1, ZoneID z2)
        => !z1.Equals(z2);
}
