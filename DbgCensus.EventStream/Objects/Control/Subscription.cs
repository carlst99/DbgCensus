using DbgCensus.EventStream.Abstractions.Objects.Control;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DbgCensus.EventStream.Objects.Control;

/// <summary>
/// Initializes a new instance of the <see cref="Subscription"/> record.
/// </summary>
/// <param name="Characters">The characters that have been subscribed to.</param>
/// <param name="CharacterCount">The number of characters that have been subscribed to.</param>
/// <param name="EventNames">The events that have been subscribed to.</param>
/// <param name="LogicalAndCharactersWithWorlds">
/// A value indicating whether or not events will only be sent if they belong to both the set of subscribed characters and the set of subscribed worlds.
/// </param>
/// <param name="Worlds">The worlds that have been subscribed to.</param>
public record Subscription
(
    IReadOnlyList<string>? Characters,
    [property: JsonPropertyName("characterCount")]
    int? CharacterCount,
    [property: JsonPropertyName("eventNames")]
    IReadOnlyList<string> EventNames,
    [property: JsonPropertyName("logicalAndCharactersWithWorlds")]
    bool LogicalAndCharactersWithWorlds,
    IReadOnlyList<string> Worlds
) : ISubscription;
