namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Get a count of character ids for which events have been encountered recently.
/// </summary>
public record RecentCharacterIdsCountCommand()
    : EventStreamCommandBase("recentCharacterIdsCount", "event");
