using System;

namespace DbgCensus.Core.Objects;

/// <summary>
/// Enumerates the IDs of every supported zone in the Census API.
/// </summary>
public enum ZoneDefinition : ushort
{
    Indar = 2,
    Hossin = 4,
    Amerish = 6,
    Esamir = 8,
    Nexus = 10,
    Koltyr = 14,

    /// <summary>
    /// The old tutorial zone. Since replaced by <see cref="Tutorial2"/>.
    /// </summary>
    [Obsolete("This zone has been replaced by " + nameof(Tutorial2))]
    Tutorial = 95,

    VRTrainingNC = 96,
    VRTrainingTR = 97,
    VRTrainingVS = 98,
    Oshur = 344,
    Desolation = 361,
    Sanctuary = 362,

    /// <summary>
    /// The new tutorial zone, introduced with the NPE update on 15/09/2021
    /// </summary>
    Tutorial2 = 364
}
