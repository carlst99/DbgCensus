namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Get a list of character ids for which events have been encountered recently.
    /// </summary>
    public record RecentCharacterIdsCommand() : CensusCommandBase("recentCharacterIds", "event");
}
