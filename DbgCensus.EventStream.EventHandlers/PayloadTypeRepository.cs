using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandlers;

/// <inheritdoc cref="IPayloadTypeRepository"/>
public class PayloadTypeRepository : IPayloadTypeRepository
{
    private readonly Dictionary<string, (Type abstractType, Type implementingType)> _eventMap;

    public IReadOnlyDictionary<string, (Type abstractType, Type implementingType)> EventMap => _eventMap;

    public PayloadTypeRepository()
    {
        _eventMap = new Dictionary<string, (Type abstractType, Type implementingType)>();
    }

    /// <inheritdoc />
    public bool TryGet(string name, [NotNullWhen(true)] out (Type abstractType, Type implementingType)? typeMap)
    {
        typeMap = null;

        if (!_eventMap.ContainsKey(name))
            return false;

        typeMap = _eventMap[name];
        return true;
    }

    /// <inheritdoc />
    public void Register<TInterface, TImplementation>(string name) where TInterface : IPayload
        => _eventMap[name] = (typeof(TInterface), typeof(TImplementation));
}
