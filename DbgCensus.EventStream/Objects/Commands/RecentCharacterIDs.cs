using DbgCensus.EventStream.Abstractions.Objects.Commands;

namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="RecentCharacterIDs"/> record.
/// </summary>
public record RecentCharacterIDs()
    : CommandBase("recentCharacterIds", "event"), IRecentCharacterIDs;
