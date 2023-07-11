using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.EventStream.EventHandlers.Services;

/// <inheritdoc cref="IPayloadHandlerTypeRepository"/>
public class PayloadHandlerTypeRepository : IPayloadHandlerTypeRepository
{
    private readonly Dictionary<Type, List<Type>> _repository;

    /// <summary>
    /// Initialises a new instance of the <see cref="PayloadHandlerTypeRepository"/> class.
    /// </summary>
    public PayloadHandlerTypeRepository()
    {
        _repository = new Dictionary<Type, List<Type>>();
    }

    /// <inheritdoc />
    public IReadOnlyList<Type> GetHandlerTypes<TPayload>() where TPayload : IPayload
        => GetHandlerTypes(typeof(TPayload));

    /// <inheritdoc />
    public IReadOnlyList<Type> GetHandlerTypes(Type payloadType)
    {
        if (!payloadType.GetInterfaces().Contains(typeof(IPayload)))
            throw new ArgumentException("The type must derive from " + nameof(IPayload), nameof(payloadType));

        Type keyType = typeof(IPayloadHandler<>).MakeGenericType(payloadType);

        return _repository.TryGetValue(keyType, out List<Type>? types)
            ? types
            : Array.Empty<Type>();
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
