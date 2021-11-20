using DbgCensus.EventStream.Abstractions.Objects.Control;

namespace DbgCensus.EventStream.Objects.Control;

/// <summary>
/// Initializes a new instance of the <see cref="ServiceMessage{TEvent}"/> record.
/// </summary>
/// <typeparam name="TEvent">The type of the encapsulated event.</typeparam>
/// <param name="Payload">The encapsulated event payload.</param>
/// <param name="Service">The websocket service that the payload was received from.</param>
/// <param name="Type">The Census type of the object.</param>
public record ServiceMessage<TEvent>
(
    TEvent Payload,
    string Service,
    string Type
) : IServiceMessage<TEvent>;
