using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Abstractions.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandling
{
    /// <inheritdoc cref="IEventStreamObjectTypeRepository"/>
    public class EventStreamObjectTypeRepository : IEventStreamObjectTypeRepository
    {
        private readonly Dictionary<Tuple<string, string>, Type> _repository;

        public EventStreamObjectTypeRepository()
        {
            _repository = new Dictionary<Tuple<string, string>, Type>();
        }

        /// <inheritdoc />
        public bool TryGet(string censusService, string censusType, [NotNullWhen(true)] out Type? type)
        {
            type = null;
            Tuple<string, string> censusTypeData = new(censusService, censusType);

            if (!_repository.ContainsKey(censusTypeData))
                return false;

            type = _repository[censusTypeData];
            return true;
        }

        /// <inheritdoc />
        public bool TryRegister<T>(string censusService, string censusType) where T : IEventStreamObject
        {
            Tuple<string, string> censusTypeData = new(censusService, censusType);

            if (_repository.ContainsKey(censusTypeData))
                return false;

            _repository.Add(censusTypeData, typeof(T));
            return true;
        }
    }
}
