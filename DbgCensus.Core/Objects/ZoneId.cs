namespace DbgCensus.Core.Objects
{
    public record ZoneId
    {
        public static readonly ZoneId Default = new(0);

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
        /// Initialises a new instance of the <see cref="ZoneId"/> record.
        /// </summary>
        /// <param name="combinedId">The actual combined zone-id that was retrieved from Census.</param>
        public ZoneId(uint combinedId)
        {
            CombinedId = combinedId;
            Definition = (ZoneDefinition)(combinedId & 0xffff);
            Instance = (ushort)(combinedId >> 16);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ZoneId"/> record.
        /// </summary>
        /// <param name="combinedId">The actual combined zone-id that was retrieved from Census.</param>
        public ZoneId(int combinedId)
            : this((uint)combinedId)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ZoneId"/> record.
        /// </summary>
        /// <param name="definition">The definition of the zone.</param>
        /// <param name="instance">The instance of the zone.</param>
        public ZoneId(ZoneDefinition definition, ushort instance)
        {
            Definition = definition;
            Instance = instance;
            CombinedId = ((uint)instance << 16) | (uint)definition;
        }

        public override string ToString()
            => $"{ Definition } (instance { Instance })";

        public static explicit operator uint(ZoneId zoneId) => zoneId.CombinedId;
        public static explicit operator ZoneDefinition(ZoneId zoneId) => zoneId.Definition;

        public static explicit operator ZoneId(int combinedId) => new(combinedId);
        public static explicit operator ZoneId(uint combinedId) => new(combinedId);
    }
}
