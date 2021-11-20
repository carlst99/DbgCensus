using DbgCensus.EventStream.Abstractions.Objects.Commands;

namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="RecentCharacterIDsCount"/> record.
/// </summary>
public record RecentCharacterIDsCount()
    : CommandBase("recentCharacterIdsCount", "event"), IRecentCharacterIDsCount;
