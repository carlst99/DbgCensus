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
    TRMeltdownIndar = 147,

    /// <inheritdoc cref="TRMeltdownIndar" />
    VSMeltdownIndar = 148,

    /// <inheritdoc cref="TRMeltdownIndar" />
    NCMeltdownIndar = 149,

    /// <inheritdoc cref="TRMeltdownIndar" />
    TRMeltdownEsamir = 150,

    /// <inheritdoc cref="TRMeltdownIndar" />
    VSMeltdownEsamir = 151,

    /// <inheritdoc cref="TRMeltdownIndar" />
    NCMeltdownEsamir = 152,

    /// <inheritdoc cref="TRMeltdownIndar" />
    TRMeltdownHossin = 153,

    /// <inheritdoc cref="TRMeltdownIndar" />
    VSMeltdownHossin = 154,

    /// <inheritdoc cref="TRMeltdownIndar" />
    NCMeltdownHossin = 155,

    /// <inheritdoc cref="TRMeltdownIndar" />
    TRMeltdownAmerish = 156,

    /// <inheritdoc cref="TRMeltdownIndar" />
    VSMeltdownAmerish = 157,

    /// <inheritdoc cref="TRMeltdownIndar" />
    NCMeltdownAmerish = 158,

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

    /// <inheritdoc cref="TRMeltdownIndar" />
    NCMeltdownKoltyr = 208,

    /// <inheritdoc cref="TRMeltdownIndar" />
    TRMeltdownKoltyr = 209,

    /// <inheritdoc cref="TRMeltdownIndar" />
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

    /// <inheritdoc cref="TRMeltdownIndar" />
    NCMeltdownOshur = 222,

    /// <inheritdoc cref="TRMeltdownIndar" />
    TRMeltdownOshur = 223,

    /// <inheritdoc cref="TRMeltdownIndar" />
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
    SuddenDeathOshur = 240,

    /// <summary>
    /// An unaligned Bastion Fleet Carrier descends, rendering regions of the
    /// map un-contestable until destroyed. Triggered on a regular timer,
    /// however destroying Halloween Space Pumpkins will shorten this time.
    /// </summary>
    /// <remarks>
    /// This event was first introduced with the 2022 Nanite of the Living Dead
    /// update, and ran for a limited time.
    /// </remarks>
    HauntedBastionIndar = 242,

    /// <inheritdoc cref="HauntedBastionIndar" />
    HauntedBastionHossin = 243,

    /// <inheritdoc cref="HauntedBastionIndar" />
    HauntedBastionAmerish = 244,

    /// <inheritdoc cref="HauntedBastionIndar" />
    HauntedBastionEsamir = 245,
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
            MED.NCMeltdownIndar or MED.TRMeltdownIndar or MED.VSMeltdownIndar => MELTDOWN_DURATION,
            MED.NCMeltdownEsamir or MED.TRMeltdownEsamir or MED.VSMeltdownEsamir => MELTDOWN_DURATION,
            MED.NCMeltdownHossin or MED.TRMeltdownHossin or MED.VSMeltdownHossin => MELTDOWN_DURATION,
            MED.NCMeltdownAmerish or MED.TRMeltdownAmerish or MED.VSMeltdownAmerish => MELTDOWN_DURATION,
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
            MED.HauntedBastionIndar or MED.HauntedBastionHossin or MED.HauntedBastionAmerish or MED.HauntedBastionEsamir => TimeSpan.FromMinutes(15),
            _ => MELTDOWN_DURATION
        };
}
