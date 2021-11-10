using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Objects.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandlers;

/// <inheritdoc cref="IServiceMessageTypeRepository"/>
public class ServiceMessageTypeRepository : IServiceMessageTypeRepository
{
    private readonly Dictionary<string, Type> _repository;

    public ServiceMessageTypeRepository()
    {
        _repository = new Dictionary<string, Type>();
    }

    /// <inheritdoc />
    public bool TryGet(string eventName, [NotNullWhen(true)] out Type? type)
    {
        type = null;

        if (!_repository.ContainsKey(eventName))
            return false;

        type = _repository[eventName];
        return true;
    }

    /// <inheritdoc />
    public bool TryRegister<TObject, TPayload>(string eventName) where TObject : ServiceMessage<TPayload>
    {
        if (_repository.ContainsKey(eventName))
            return false;

        _repository.Add(eventName, typeof(TObject));
        return true;
    }
}
