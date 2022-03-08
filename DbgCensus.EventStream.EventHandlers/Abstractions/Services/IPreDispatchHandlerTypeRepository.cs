using System;
using System.Collections.Generic;

namespace DbgCensus.EventStream.EventHandlers.Abstractions.Services;

/// <summary>
/// Represents an interface for storing types that implement <see cref="IPreDispatchHandler"/>.
/// </summary>
public interface IPreDispatchHandlerTypeRepository
{
    /// <summary>
    /// Adds an <see cref="IPreDispatchHandler"/> implementation to the repository.
    /// </summary>
    /// <typeparam name="T">The type of the implementation.</typeparam>
    void Register<T>() where T : IPreDispatchHandler;

    /// <summary>
    /// Gets all of the types in the repository.
    /// </summary>
    /// <returns>A list of <see cref="IPreDispatchHandler"/> types.</returns>
    IReadOnlyList<Type> GetAll();
}
