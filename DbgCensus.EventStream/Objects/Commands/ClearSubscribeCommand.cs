using System.Collections.Generic;

namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="ClearSubscribe"/> record.
/// </summary>
/// <param name="All">A value indicating whether or not all subscriptions will be cleared.</param>
/// <param name="Characters">The character subscriptions to clear.</param>
/// <param name="EventNames">The event subscriptions to clear.</param>
/// <param name="Worlds">The world subscriptions to clear.</param>
public record ClearSubscribe
(
    bool All = false,
    IEnumerable<string>? Characters = default,
    IEnumerable<string>? EventNames = default,
    IEnumerable<string>? Worlds = default
) : CommandBase("clearSubscribe", "event");
