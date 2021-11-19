using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandlers.Abstractions;

/// <summary>
/// Stores and maps <see cref="IEvent"/> abstractions to the census objects they represent, and their implementing types.
/// </summary>
public interface IPayloadTypeRepository
{
    /// <summary>
    /// Attempts to get the type map for the given Census event name.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="typeMap">The retrieved abstract and implementing types.</param>
    /// <returns>A value indicating if the object types were found in the repository.</returns>
    bool TryGet(string name, [NotNullWhen(true)] out (Type abstractType, Type implementingType)? typeMap);

    /// <summary>
    /// Registers an event type used to deserialize socket events to.
    /// </summary>
    /// <typeparam name="TInterface">The abstracting type of the event.</typeparam>
    /// <typeparam name="TImplementation">The implementing type of the event.</typeparam>
    /// <param name="name">The Census name of the event, e.g. 'FacilityControl'.</param>
    void Register<TInterface, TImplementation>(string name) where TInterface : IPayload;
}
