using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;

namespace DbgCensus.EventStream.EventHandlers.Objects;

/// <summary>
/// Initializes a new instance of the <see cref="UnknownPayload"/> record.
/// </summary>
/// <param name="DispatchingClientName">The name of the client that received the payload.</param>
/// <param name="RawData">The raw data of the payload.</param>
public record UnknownPayload
(
    string DispatchingClientName,
    string RawData
) : IUnknownPayload;
