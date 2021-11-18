namespace DbgCensus.EventStream.Abstractions.Objects.Commands;

/// <summary>
/// Represents a marker interface for a command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the action that the command will perform.
    /// </summary>
    string Action { get; }

    /// <summary>
    /// Gets the websocket service that the command is intended for.
    /// </summary>
    string Service { get; }
}
