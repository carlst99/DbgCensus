using DbgCensus.EventStream.Abstractions.Objects.Commands;

namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="Echo{TPayload}"/> record.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
/// <param name="Payload">The payload to be echoed.</param>
public record Echo<TPayload>(TPayload Payload)
    : CommandBase("echo", "event"), IEcho<TPayload>;
