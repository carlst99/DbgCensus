using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Abstractions.Objects;
using DbgCensus.EventStream.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbgCensus.EventStream.EventHandling
{
    /// <inheritdoc cref="IEventHandlerRepository"/>
    public class EventHandlerRepository : IEventHandlerRepository
    {
        private readonly IServiceProvider _services;
        private readonly Dictionary<Type, List<Type>> _repository;

        /// <summary>
        /// Initialises a new instance of the <see cref="EventHandlerRepository"/> class.
        /// </summary>
        public EventHandlerRepository(IServiceProvider services)
        {
            _services = services;
            _repository = new Dictionary<Type, List<Type>>();
        }

        /// <inheritdoc />
        public IReadOnlyList<ICensusEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent : IEventStreamObject
        {
            Type key = typeof(ICensusEventHandler<TEvent>);
            if (!_repository.ContainsKey(key))
                return Array.Empty<ICensusEventHandler<TEvent>>();

            List<ICensusEventHandler<TEvent>> handlers = new();
            using IServiceScope scope = _services.CreateScope();
            foreach (Type handler in _repository[key])
                handlers.Add((ICensusEventHandler<TEvent>)scope.ServiceProvider.GetRequiredService(handler));

            return handlers;
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetHandlerTypes<TEvent>() where TEvent : IEventStreamObject
            => GetHandlerTypes(typeof(TEvent));

        public IReadOnlyList<Type> GetHandlerTypes(Type handlerType)
        {
            if (!handlerType.IsCensusEventHandler())
                throw new ArgumentException("The type must derive from " + nameof(ICensusEventHandler), nameof(handlerType));

            if (_repository.ContainsKey(handlerType))
                return _repository[handlerType];
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
