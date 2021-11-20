namespace DbgCensus.EventStream.Abstractions.Objects.Commands;

/// <summary>
/// Represents a RecentCharacterIDs command, a request for a list of character IDs
/// that have recently been sent through the event stream.
/// </summary>
public interface IRecentCharacterIDs : ICommand
{
}
