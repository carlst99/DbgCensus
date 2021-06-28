using DbgCensus.EventStream.Abstractions.Objects;
using System;
using System.Collections.Generic;

namespace DbgCensus.EventStream.Abstractions.EventHandling
{
    /// <summary>
    /// Stores and maps types that implement <see cref="ICensusEventHandler{T}"/>.
    /// </summary>
    public interface IEventHandlerRepository
    {
        /// <summary>
        /// Gets types implementing <see cref="ICensusEventHandler{TEvent}"/> for the given event type.
        /// </summary>
        /// <param name="eventObjectType">The type of the <see cref="IEventStreamObject"/> to retrieve handler types for.</param>
        /// <returns>A list of types.</returns>
        IReadOnlyList<Type> GetHandlerTypes(Type eventObjectType);

        /// <summary>
        /// Gets types implementing <see cref="ICensusEventHandler{TEvent}"/> for the given event type.
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="IEventStreamObject"/> to retrieve handler types for.</typeparam>
        /// <returns>A list of types.</returns>
        IReadOnlyList<Type> GetHandlerTypes<TEvent>() where TEvent : IEventStreamObject;

        /// <summary>
        /// Stores a handler in the repository, internally mapping it to each <see cref="IEventStreamObject"/> that it handles.
        /// </summary>
        /// <typeparam name="THandler">The type of handler to store in the repository.</typeparam>
        void RegisterHandler<THandler>() where THandler : ICensusEventHandler;
    }
}
