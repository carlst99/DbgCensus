using System;
using MED = DbgCensus.Core.Objects.MetagameEventDefinition;

namespace DbgCensus.Core.Objects;

/// <summary>
/// Enumerates the various metagame event types.
/// </summary>
public enum MetagameEventDefinition : uint
{
    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    TRMeltdownAmerish = 147,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    VSMeltdownAmerish = 148,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    NCMeltdownAmerish = 149,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    TRMeltdownEsamir = 150,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    VSMeltdownEsamir = 151,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    NCMeltdownEsamir = 152,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    TRMeltdownHossin = 153,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    VSMeltdownHossin = 154,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    NCMeltdownHossin = 155,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    TRMeltdownIndar = 156,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    VSMeltdownIndar = 157,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    NCMeltdownIndar = 158,

    /// <summary>
    /// Triggered when a fully-open continent collapses into
    /// an unstable alert due to low population.
    /// </summary>
    LowPopCollapseAmerish = 159,

    /// <inheritdoc cref="LowPopCollapseAmerish" />
    LowPopCollapseEsamir = 160,

    /// <inheritdoc cref="LowPopCollapseAmerish" />
    LowPopCollapseHossin = 161,

    /// <inheritdoc cref="LowPopCollapseAmerish" />
    LowPopCollapseIndar = 162,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    NCUnstableMeltdownEsamir = 176,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    NCUnstableMeltdownHossin = 177,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    NCUnstableMeltdownAmerish = 178,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    NCUnstableMeltdownIndar = 179,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    VSUnstableMeltdownEsamir = 186,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    VSUnstableMeltdownHossin = 187,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    VSUnstableMeltdownAmerish = 188,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    VSUnstableMeltdownIndar = 189,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    TRUnstableMeltdownEsamir = 190,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    TRUnstableMeltdownHossin = 191,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    TRUnstableMeltdownAmerish = 192,

    /// <inheritdoc cref="NCUnstableMeltdownEsamir" />
    TRUnstableMeltdownIndar = 193,

    OutfitWarsCaptureRelics = 204,
    OutfitWarsPreMatch = 205,
    OutfitWarsRelicsChanging = 206,
    OutfitWarsMatchStart = 207,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    NCMeltdownKoltyr = 208,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    TRMeltdownKoltyr = 209,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    VSMeltdownKoltyr = 210,

    /// <summary>
    /// Triggered immediately when more than 900 players
    /// are present on a continent.
    /// </summary>
    ConquestAmerish = 211,

    /// <inheritdoc cref="ConquestAmerish" />
    ConquestEsamir = 212,

    /// <inheritdoc cref="ConquestAmerish" />
    ConquestHossin = 213,

    /// <inheritdoc cref="ConquestAmerish" />
    ConquestIndar = 214,

    /// <summary>
    /// Triggered immediately when a large number of players
    /// are present on a continent. Might be 450 for Koltyr?
    /// </summary>
    ConquestKoltyr = 215,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    NCMeltdownOshur = 222,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    TRMeltdownOshur = 223,

    /// <inheritdoc cref="TRMeltdownAmerish" />
    VSMeltdownOshur = 224,

    /// <inheritdoc cref="ConquestAmerish" />
    ConquestOshur = 226,

    AirAnomalyIndar = 228,
    AirAnomalyHossin = 229,
    AirAnomalyAmerish = 230,
    AirAnomalyEsamir = 231,
    AirAnomalyOshur = 232,

    /// <summary>
    /// Triggered upon a drawn alert, sudden death alerts
    /// last 15 minutes, or until a faction achieves 10 000 kills.
    /// The faction with the most kills at the end of the
    /// alert will lock the continent in their favour.
    /// </summary>
    SuddenDeathIndar = 236,

    /// <inheritdoc cref="SuddenDeathIndar" />
    SuddenDeathHossin = 237,

    /// <inheritdoc cref="SuddenDeathIndar" />
    SuddenDeathAmerish = 238,

    /// <inheritdoc cref="SuddenDeathIndar" />
    SuddenDeathEsamir = 239,

    /// <inheritdoc cref="SuddenDeathIndar" />
    SuddenDeathOshur = 240
}

public static class MetagameEventDefinitionExtensions
{
    private static readonly TimeSpan MELTDOWN_DURATION = TimeSpan.FromMinutes(90);
    private static readonly TimeSpan UNSTABLE_MELTDOWN_DURATION = TimeSpan.FromMinutes(45);
    private static readonly TimeSpan KOLTYR_MELTDOWN_DURATION = TimeSpan.FromMinutes(45);

    /// <summary>
    /// Gets the duration of the alert that this metagame event
    /// is associated with.
    /// </summary>
    /// <param name="definition">The metagame event.</param>
    /// <returns>The alert duration.</returns>
    public static TimeSpan GetAlertDuration(this MetagameEventDefinition definition)
        => definition switch
        {
            MED.NCMeltdownAmerish or MED.TRMeltdownAmerish or MED.VSMeltdownAmerish => MELTDOWN_DURATION,
            MED.NCMeltdownEsamir or MED.TRMeltdownEsamir or MED.VSMeltdownEsamir => MELTDOWN_DURATION,
            MED.NCMeltdownHossin or MED.TRMeltdownHossin or MED.VSMeltdownHossin => MELTDOWN_DURATION,
            MED.NCMeltdownIndar or MED.TRMeltdownIndar or MED.VSMeltdownIndar => MELTDOWN_DURATION,
            MED.NCMeltdownKoltyr or MED.TRMeltdownKoltyr or MED.VSMeltdownKoltyr => KOLTYR_MELTDOWN_DURATION,
            MED.NCMeltdownOshur or MED.TRMeltdownOshur or MED.VSMeltdownOshur => MELTDOWN_DURATION,
            MED.LowPopCollapseAmerish or MED.LowPopCollapseEsamir or MED.LowPopCollapseHossin or MED.LowPopCollapseIndar => UNSTABLE_MELTDOWN_DURATION,
            MED.NCUnstableMeltdownAmerish or MED.TRUnstableMeltdownAmerish or MED.VSUnstableMeltdownAmerish => UNSTABLE_MELTDOWN_DURATION,
            MED.NCUnstableMeltdownEsamir or MED.TRUnstableMeltdownEsamir or MED.VSUnstableMeltdownEsamir => UNSTABLE_MELTDOWN_DURATION,
            MED.NCUnstableMeltdownHossin or MED.TRUnstableMeltdownHossin or MED.VSUnstableMeltdownHossin => UNSTABLE_MELTDOWN_DURATION,
            MED.NCUnstableMeltdownIndar or MED.TRUnstableMeltdownIndar or MED.VSUnstableMeltdownIndar => UNSTABLE_MELTDOWN_DURATION,
            MED.ConquestAmerish or MED.ConquestEsamir or MED.ConquestHossin or MED.ConquestIndar or MED.ConquestOshur => MELTDOWN_DURATION,
            MED.ConquestKoltyr => KOLTYR_MELTDOWN_DURATION,
            MED.AirAnomalyIndar or MED.AirAnomalyHossin or MED.AirAnomalyAmerish or MED.AirAnomalyEsamir or MED.AirAnomalyOshur => TimeSpan.FromMinutes(30),
            MED.SuddenDeathIndar or MED.SuddenDeathHossin or MED.SuddenDeathAmerish or MED.SuddenDeathEsamir or MED.SuddenDeathOshur => TimeSpan.FromMinutes(15),
            _ => MELTDOWN_DURATION
        };
}
