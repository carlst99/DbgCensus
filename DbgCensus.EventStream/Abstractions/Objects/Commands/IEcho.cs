namespace DbgCensus.EventStream.Abstractions.Objects.Commands;

/// <summary>
/// Represents an Echo command, a request for Census to echo the provided payload.
/// </summary>
/// <typeparam name="TPayload">The type of the payload.</typeparam>
public interface IEcho<TPayload> : ICommand
{
    /// <summary>
    /// Gets the payload to be echoed.
    /// </summary>
    TPayload Payload { get; }
}
