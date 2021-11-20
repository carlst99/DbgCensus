using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandlers.Services;

/// <inheritdoc cref="IPayloadTypeRepository"/>
public class PayloadTypeRepository : IPayloadTypeRepository
{
    private readonly Dictionary<string, (Type AbstractType, Type ImplementingType)> _eventMap;

    public IReadOnlyDictionary<string, (Type AbstractType, Type ImplementingType)> EventMap => _eventMap;

    public PayloadTypeRepository()
    {
        _eventMap = new Dictionary<string, (Type AbstractType, Type ImplementingType)>();
    }

    /// <inheritdoc />
    public bool TryGet(string name, [NotNullWhen(true)] out (Type AbstractType, Type ImplementingType)? typeMap)
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
