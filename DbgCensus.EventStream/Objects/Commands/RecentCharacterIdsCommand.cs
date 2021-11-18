namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Get a list of character ids for which events have been encountered recently.
/// </summary>
public record RecentCharacterIdsCommand()
    : EventStreamCommandBase("recentCharacterIds", "event");
