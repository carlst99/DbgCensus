using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Commands;
using OneOf;
using System.Collections.Generic;

namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="Subscribe"/> record.
/// </summary>
/// <param name="Characters">The characters to subscribe to.</param>
/// <param name="EventNames">The events to subscribe to.</param>
/// <param name="LogicalAndCharactersWithWorlds">
/// A value indicating whether or not events will only be sent if they belong to both the set of subscribed characters and the set of subscribed worlds.
/// </param>
/// <param name="Worlds">The worlds to subscribe to.</param>
public record Subscribe
(
    OneOf<All, IEnumerable<ulong>>? Characters = default,
    IEnumerable<string>? EventNames = default,
    bool LogicalAndCharactersWithWorlds = false,
    OneOf<All, IEnumerable<WorldDefinition>>? Worlds = default
) : CommandBase("subscribe", "event"), ISubscribe;
