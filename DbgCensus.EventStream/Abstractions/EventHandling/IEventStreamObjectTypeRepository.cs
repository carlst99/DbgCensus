using DbgCensus.EventStream.Abstractions.Objects;
using System;

namespace DbgCensus.EventStream.Abstractions.EventHandling
{
    /// <summary>
    /// Stores and maps <see cref="IEventStreamObject"/> types to the census types they represent.
    /// </summary>
    public interface IEventStreamObjectTypeRepository
    {
        /// <summary>
        /// Attempts to get an <see cref="IEventStreamObject"/> type.
        /// </summary>
        /// <param name="censusService">The service that the object is received from.</param>
        /// <param name="censusType">The type that the object represents.</param>
        /// <param name="type">The retrieved object type.</param>
        /// <returns>A value indicating if the object type was found in the repository.</returns>
        bool TryGet(string censusService, string censusType, out Type? type);

        /// <summary>
        /// Registers an <see cref="IEventStreamObject"/> to the census type that it represents.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IEventStreamObject"/></typeparam>
        /// <param name="censusService">The service that the object is received from.</param>
        /// <param name="censusType">The type that the object represents.</param>
        /// <returns>A value indicating if the object type was added to the repository. If false, the object has already been registered.</returns>
        bool TryRegister<T>(string censusService, string censusType) where T : IEventStreamObject;
    }
}
