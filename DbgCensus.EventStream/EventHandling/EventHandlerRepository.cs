﻿using DbgCensus.EventStream.Abstractions.EventHandling;
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
        private readonly Dictionary<Type, List<Type>> _repository;

        /// <summary>
        /// Initialises a new instance of the <see cref="EventHandlerRepository"/> class.
        /// </summary>
        public EventHandlerRepository()
        {
            _repository = new Dictionary<Type, List<Type>>();
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetHandlerTypes(Type eventObjectType)
        {
            if (!eventObjectType.IsCensusEventHandler())
                throw new ArgumentException("The type must derive from " + nameof(ICensusEventHandler), nameof(eventObjectType));

            if (_repository.ContainsKey(eventObjectType))
                return _repository[eventObjectType];
            else
                return Array.Empty<Type>();
        }

        /// <inheritdoc />
        public IReadOnlyList<Type> GetHandlerTypes<TEvent>() where TEvent : IEventStreamObject
            => GetHandlerTypes(typeof(TEvent));

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
