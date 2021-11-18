using System.Collections.Generic;

namespace DbgCensus.EventStream.Abstractions.Objects.Commands;

/// <summary>
/// Represents a ClearSubscribe command.
/// </summary>
public interface IClearSubscribe : ICommand
{
    /// <summary>
    /// Gets the character subscriptions to clear.
    /// </summary>
    IEnumerable<string>? Characters { get; }

    /// <summary>
    /// Gets the event subscriptions to clear.
    /// </summary>
    IEnumerable<string>? EventNames { get; }

    /// <summary>
    /// Gets the world subscriptions to clear.
    /// </summary>
    IEnumerable<string>? Worlds { get; }

    /// <summary>
    /// Gets a value indicating whether or not all subscriptions will be cleared.
    /// </summary>
    bool All { get; }
}
