using DbgCensus.EventStream.Abstractions.Objects;

namespace DbgCensus.EventStream.EventHandlers.Abstractions.Objects;

/// <summary>
/// Represents an unknown event.
/// </summary>
public interface IUnknownPayload : IPayload
{
    /// <summary>
    /// Gets the name of the client that received this payload.
    /// </summary>
    string DispatchingClientName { get; }

    /// <summary>
    /// Gets the raw data of the payload.
    /// </summary>
    string RawData { get; }
}
