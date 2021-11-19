using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.EventStream.EventHandlers;

/// <inheritdoc cref="IPayloadHandlerTypeRepository"/>
public class EventHandlerTypeRepository : IPayloadHandlerTypeRepository
{
    private readonly Dictionary<Type, List<Type>> _repository;

    /// <summary>
    /// Initialises a new instance of the <see cref="EventHandlerTypeRepository"/> class.
    /// </summary>
    public EventHandlerTypeRepository()
    {
        _repository = new Dictionary<Type, List<Type>>();
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> GetHandlerTypes<TEvent>() where TEvent : IPayload
        => GetHandlerTypes(typeof(TEvent));

    /// <inheritdoc />
    public IReadOnlyList<Type> GetHandlerTypes(Type eventObjectType)
    {
        if (!eventObjectType.GetInterfaces().Contains(typeof(IPayload)))
            throw new ArgumentException("The type must derive from " + nameof(IPayload), nameof(eventObjectType));

        Type keyType = typeof(IPayloadHandler<>).MakeGenericType(new Type[] { eventObjectType });
        if (_repository.ContainsKey(keyType))
            return _repository[keyType];
        else
            return Array.Empty<Type>();
    }

    /// <inheritdoc/>
    public void RegisterHandler<THandler>() where THandler : IPayloadHandler
    {
        Type handlerType = typeof(THandler);

        Type[] handlerTypeInterfaces = handlerType.GetInterfaces();
        IEnumerable<Type> handlerInterfaces = handlerTypeInterfaces.Where(
            r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IPayloadHandler<>));

        foreach (Type handlerInterface in handlerInterfaces)
        {
            if (!_repository.ContainsKey(handlerInterface))
                _repository.Add(handlerInterface, new List<Type>());

            if (!_repository[handlerInterface].Contains(handlerType))
                _repository[handlerInterface].Add(handlerType);
        }
    }
}
