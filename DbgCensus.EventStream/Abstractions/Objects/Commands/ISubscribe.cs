using System.Collections.Generic;

namespace DbgCensus.EventStream.Abstractions.Objects.Commands;

/// <summary>
/// Represents a Subscribe command.
/// </summary>
public interface ISubscribe : ICommand
{
    /// <summary>
    /// Gets the characters to subscribe to.
    /// </summary>
    IEnumerable<string>? Characters { get; }

    /// <summary>
    /// Gets the events to subscribe to.
    /// </summary>
    IEnumerable<string>? EventNames { get; }

    /// <summary>
    /// Gets a value indicating whether or not events will only be sent if they belong to both the set of subscribed characters and the set of subscribed worlds.
    /// </summary>
    bool LogicalAndCharactersWithWorlds { get; }

    /// <summary>
    /// Gets the worlds to subscribe to.
    /// </summary>
    IEnumerable<string>? Worlds { get; }
}
