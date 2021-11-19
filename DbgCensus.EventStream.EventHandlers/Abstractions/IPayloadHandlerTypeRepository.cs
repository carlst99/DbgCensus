using DbgCensus.EventStream.Abstractions.Objects;
using System;
using System.Collections.Generic;

namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Represents an interface for mapping types that implement <see cref="IPayloadHandler{T}"/>.
/// </summary>
public interface IPayloadHandlerTypeRepository
{
    /// <summary>
    /// Gets types implementing <see cref="IPayloadHandler{TEvent}"/> for the given payload type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the <see cref="IPayload"/>.</typeparam>
    /// <returns>A list of types.</returns>
    IReadOnlyList<Type> GetHandlerTypes<TEvent>() where TEvent : IPayload;

    /// <summary>
    /// Gets types implementing <see cref="IPayloadHandler{TEvent}"/> for the given event type.
    /// </summary>
    /// <param name="eventObjectType">The type of the <see cref="IPayload"/>.</param>
    /// <returns>A list of types.</returns>
    IReadOnlyList<Type> GetHandlerTypes(Type eventObjectType);

    /// <summary>
    /// Stores a handler in the repository, internally mapping it to each <see cref="IPayload"/> that it handles.
    /// </summary>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    void RegisterHandler<THandler>() where THandler : IPayloadHandler;
}
