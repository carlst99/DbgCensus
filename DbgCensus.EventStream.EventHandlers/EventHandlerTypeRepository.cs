using DbgCensus.EventStream.EventHandlers.Abstractions;
using DbgCensus.EventStream.EventHandlers.Abstractions.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.EventStream.EventHandlers
{
    /// <inheritdoc cref="IEventHandlerTypeRepository"/>
    public class EventHandlerTypeRepository : IEventHandlerTypeRepository
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
        public IReadOnlyList<Type> GetHandlerTypes<TEvent>() where TEvent : IEventStreamObject
            => GetHandlerTypes(typeof(TEvent));

        /// <inheritdoc />
        public IReadOnlyList<Type> GetHandlerTypes(Type eventObjectType)
        {
            if (!eventObjectType.GetInterfaces().Contains(typeof(IEventStreamObject)))
                throw new ArgumentException("The type must derive from " + nameof(IEventStreamObject), nameof(eventObjectType));

            Type keyType = typeof(ICensusEventHandler<>).MakeGenericType(new Type[] { eventObjectType });
            if (_repository.ContainsKey(keyType))
                return _repository[keyType];
            else
                return Array.Empty<Type>();
        }

        /// <inheritdoc/>
        public void RegisterHandler<THandler>() where THandler : ICensusEventHandler
        {
            Type handlerType = typeof(THandler);

            Type[] handlerTypeInterfaces = handlerType.GetInterfaces();
            IEnumerable<Type> handlerInterfaces = handlerTypeInterfaces.Where(
                r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(ICensusEventHandler<>));

            foreach (Type handlerInterface in handlerInterfaces)
            {
                if (!_repository.ContainsKey(handlerInterface))
                    _repository.Add(handlerInterface, new List<Type>());

                if (!_repository[handlerInterface].Contains(handlerType))
                    _repository[handlerInterface].Add(handlerType);
            }
        }
    }
}
