using DbgCensus.EventStream.Abstractions.Objects.Events;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Stores and maps <see cref="IEvent"/> abstractions to the census objects they represent, and their implementing types.
/// </summary>
public interface IEventTypeRepository
{
    /// <summary>
    /// Attempts to get the type map for the given Census event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="typeMap">The retrieved abstract and implementing types.</param>
    /// <returns>A value indicating if the object types were found in the repository.</returns>
    bool TryGet(string eventName, [NotNullWhen(true)] out (Type abstractEvent, Type implementingEvent)? typeMap);

    /// <summary>
    /// Registers an event type used to deserialize socket events to.
    /// </summary>
    /// <typeparam name="TEventInterface">The abstracting type of the event.</typeparam>
    /// <typeparam name="TEventImplementation">The implementing type of the event.</typeparam>
    /// <param name="eventName">The Census name of the event, e.g. 'FacilityControl'.</param>
    void Register<TEventInterface, TEventImplementation>(string eventName) where TEventInterface : IEvent;
}
