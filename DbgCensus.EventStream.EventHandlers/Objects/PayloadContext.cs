using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;

namespace DbgCensus.EventStream.EventHandlers.Objects;

/// <summary>
/// Initializes a new instance of the <see cref="PayloadContext"/> record.
/// </summary>
/// <param name="DispatchingClientName">The name of the client that dispatched the event.</param>
public record PayloadContext
(
    string DispatchingClientName
) : IPayloadContext;
