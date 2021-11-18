using DbgCensus.EventStream.EventHandlers.Abstractions;

namespace DbgCensus.EventStream.EventHandlers;

/// <summary>
/// Initializes a new instance of the <see cref="EventContext"/> record.
/// </summary>
/// <param name="DispatchingClientName">The name of the client that dispatched the event.</param>
public record EventContext
(
    string DispatchingClientName
): IEventContext;
