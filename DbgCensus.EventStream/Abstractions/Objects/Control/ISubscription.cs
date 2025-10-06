using System.Collections.Generic;

namespace DbgCensus.EventStream.Abstractions.Objects.Control;

/// <summary>
/// Represents a Subscription payload. Note that this interface does not map 1:1 to the original payload.
/// </summary>
public interface ISubscription : IPayload
{
    /// <summary>
    /// Gets the characters that have been subscribed to. May contain character IDs and/or the 'all' special value.
    /// </summary>
    IReadOnlyList<string>? Characters { get; }

    /// <summary>
    /// Gets the number of characters subscribed to.
    /// Included when either no characters, or an explicit set of characters, is subscribed to.
    /// </summary>
    int? CharacterCount { get; }

    /// <summary>
    /// Gets the events that have been subscribed to.
    /// Can contain event names and/or the 'all' special value.
    /// </summary>
    IReadOnlyList<string> EventNames { get; }

    /// <summary>
    /// Gets a value indicating whether events will only be sent if they belong to both the set of subscribed characters
    /// and the set of subscribed worlds.
    /// </summary>
    bool LogicalAndCharactersWithWorlds { get; }

    /// <summary>
    /// Gets the worlds that have been subscribed to. May contain world  IDs and/or the 'all' special value.
    /// </summary>
    IReadOnlyList<string> Worlds { get; }
}
