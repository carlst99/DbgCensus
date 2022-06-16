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

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    VSMeltdownAmerish = 148,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    NCMeltdownAmerish = 149,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    TRMeltdownEsamir = 150,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    VSMeltdownEsamir = 151,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    NCMeltdownEsamir = 152,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    TRMeltdownHossin = 153,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    VSMeltdownHossin = 154,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    NCMeltdownHossin = 155,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    TRMeltdownIndar = 156,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    VSMeltdownIndar = 157,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    NCMeltdownIndar = 158,

    /// <summary>
    /// Triggered when a fully-open continent collapses into
    /// an unstable alert due to low population.
    /// </summary>
    LowPopCollapseAmerish = 159,

    /// <summary>
    /// Triggered when a fully-open continent collapses into
    /// an unstable alert due to low population.
    /// </summary>
    LowPopCollapseEsamir = 160,

    /// <summary>
    /// Triggered when a fully-open continent collapses into
    /// an unstable alert due to low population.
    /// </summary>
    LowPopCollapseHossin = 161,

    /// <summary>
    /// Triggered when a fully-open continent collapses into
    /// an unstable alert due to low population.
    /// </summary>
    LowPopCollapseIndar = 162,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    NCUnstableMeltdownEsamir = 176,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    NCUnstableMeltdownHossin = 177,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    NCUnstableMeltdownAmerish = 178,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    NCUnstableMeltdownIndar = 179,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    VSUnstableMeltdownEsamir = 186,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    VSUnstableMeltdownHossin = 187,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    VSUnstableMeltdownAmerish = 188,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    VSUnstableMeltdownIndar = 189,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    TRUnstableMeltdownEsamir = 190,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    TRUnstableMeltdownHossin = 191,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    TRUnstableMeltdownAmerish = 192,

    /// <summary>
    /// An unstable meltdown alert triggered on a low-population
    /// continent by a faction achieving the required number of
    /// alert points, or the continent having been open for too long.
    /// </summary>
    TRUnstableMeltdownIndar = 193,

    OutfitWarsCaptureRelics = 204,
    OutfitWarsPreMatch = 205,
    OutfitWarsRelicsChanging = 206,
    OutfitWarsMatchStart = 207,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    NCMeltdownKoltyr = 208,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    TRMeltdownKoltyr = 209,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    VSMeltdownKoltyr = 210,

    /// <summary>
    /// Triggered immediately when more than 900 players
    /// are present on a continent.
    /// </summary>
    ConquestAmerish = 211,

    /// <summary>
    /// Triggered immediately when more than 900 players
    /// are present on a continent.
    /// </summary>
    ConquestEsamir = 212,

    /// <summary>
    /// Triggered immediately when more than 900 players
    /// are present on a continent.
    /// </summary>
    ConquestHossin = 213,

    /// <summary>
    /// Triggered immediately when more than 900 players
    /// are present on a continent.
    /// </summary>
    ConquestIndar = 214,

    /// <summary>
    /// Triggered immediately when a large number of players
    /// are present on a continent. Might be 450 for Koltyr?
    /// </summary>
    ConquestKoltyr = 215,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    NCMeltdownOshur = 222,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    TRMeltdownOshur = 223,

    /// <summary>
    /// Standard meltdown alert triggered by a faction
    /// achieving the required number of victory points.
    /// </summary>
    VSMeltdownOshur = 224,

    /// <summary>
    /// Triggered immediately when more than 900 players
    /// are present on a continent.
    /// </summary>
    ConquestOshur = 226,

    AirAnomalyIndar = 228,
    AirAnomalyHossin = 229,
    AirAnomalyAmerish = 230,
    AirAnomalyEsamir = 231,
    AirAnomalyOshur = 232
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
            _ => MELTDOWN_DURATION
        };
}
