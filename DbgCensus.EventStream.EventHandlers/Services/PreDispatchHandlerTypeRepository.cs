using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using System;
using System.Collections.Generic;

namespace DbgCensus.EventStream.EventHandlers.Services;

/// <inheritdoc cref="IPreDispatchHandlerTypeRepository"/>
public class PreDispatchHandlerTypeRepository : IPreDispatchHandlerTypeRepository
{
    private readonly List<Type> _repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreDispatchHandlerTypeRepository"/>.
    /// </summary>
    public PreDispatchHandlerTypeRepository()
    {
        _repo = new List<Type>();
    }

    /// <inheritdoc />
    public void Register<T>() where T : IPreDispatchHandler
    {
        Type type = typeof(T);

        if (!_repo.Contains(type))
            _repo.Add(type);
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> GetAll()
        => _repo;
}
