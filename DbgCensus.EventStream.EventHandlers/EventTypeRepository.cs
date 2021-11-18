using DbgCensus.EventStream.Abstractions.Objects.Events;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandlers;

/// <inheritdoc cref="IEventTypeRepository"/>
public class EventTypeRepository : IEventTypeRepository
{
    private readonly Dictionary<string, (Type abstractEvent, Type implementingEvent)> _eventMap;
    private readonly Dictionary<string, Type> _repository;

    public IReadOnlyDictionary<string, (Type abstractEvent, Type implementingEvent)> EventMap => _eventMap;

    public EventTypeRepository()
    {
        _eventMap = new Dictionary<string, (Type abstractEvent, Type implementingEvent)>();
        _repository = new Dictionary<string, Type>();
    }

    /// <inheritdoc />
    public bool TryGet(string eventName, [NotNullWhen(true)] out (Type abstractEvent, Type implementingEvent)? typeMap)
    {
        typeMap = null;

        if (!_eventMap.ContainsKey(eventName))
            return false;

        typeMap = _eventMap[eventName];
        return true;
    }

    /// <inheritdoc />
    public void Register<TEventInterface, TEventImplementation>(string eventName) where TEventInterface : IEvent
        => _eventMap[eventName] = (typeof(TEventInterface), typeof(TEventImplementation));
}
