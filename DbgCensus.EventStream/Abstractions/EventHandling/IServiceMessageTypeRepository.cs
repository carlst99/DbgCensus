using DbgCensus.EventStream.Objects.Event;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.Abstractions.EventHandling
{
    /// <summary>
    /// Stores and maps <see cref="ServiceMessage{T}"/> types to the census types they represent.
    /// </summary>
    public interface IServiceMessageTypeRepository
    {
        /// <summary>
        /// Attempts to get an <see cref="ServiceMessage{T}"/> type.
        /// </summary>
        /// <param name="eventName">The name of the event that this service message contains a payload for.</param>
        /// <param name="type">The retrieved object type.</param>
        /// <returns>A value indicating if the object type was found in the repository.</returns>
        bool TryGet(string eventName, [NotNullWhen(true)] out Type? type);

        /// <summary>
        /// Registers a <see cref="ServiceMessage{T}"/> to the census event type that it carries a payload for.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <param name="eventName">The name of the event that this payload is for.</param>
        /// <returns>A value indicating if the service message type was added to the repository. If false, the type has already been registered.</returns>
        bool TryRegister<TObject, TPayload>(string eventName) where TObject : ServiceMessage<TPayload>;
    }
}
