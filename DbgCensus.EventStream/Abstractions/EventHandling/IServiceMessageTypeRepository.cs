using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.Objects.Event;
using System;

namespace DbgCensus.EventStream.Abstractions.EventHandling
{
    /// <summary>
    /// Stores and maps <see cref="ServiceMessage"/> types to the census types they represent.
    /// </summary>
    public interface IServiceMessageTypeRepository
    {
        /// <summary>
        /// Attempts to get an <see cref="ServiceMessage{T}"/> type.
        /// </summary>
        /// <param name="censusService">The service that the object is received from.</param>
        /// <param name="censusType">The type that the object represents.</param>
        /// <param name="type">The retrieved object type.</param>
        /// <returns>A value indicating if the object type was found in the repository.</returns>
        bool TryGet(string censusService, string censusType, out Type? type);

        /// <summary>
        /// Registers an <see cref="IEventStreamObject"/> to the census type that it represents.
        /// </summary>
        /// <typeparam name="TPayload">The type of the payload.</typeparam>
        /// <param name="censusService">The service that the object is received from.</param>
        /// <param name="censusType">The type that the object represents.</param>
        /// <returns>A value indicating if the service message type was added to the repository. If false, the type has already been registered.</returns>
        bool TryRegister<TObject, TPayload>(string censusService, string censusType) where TObject : ServiceMessage<TPayload>;
    }
}
