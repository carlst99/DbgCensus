using DbgCensus.EventStream.Abstractions.EventHandling;
using DbgCensus.EventStream.Objects.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DbgCensus.EventStream.EventHandling
{
    /// <inheritdoc cref="IServiceMessageTypeRepository"/>
    public class ServiceMessageTypeRepository : IServiceMessageTypeRepository
    {
        private readonly Dictionary<Tuple<string, string>, Type> _repository;

        public ServiceMessageTypeRepository()
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
        public bool TryRegister<TObject, TPayload>(string censusService, string censusType) where TObject : ServiceMessage<TPayload>
        {
            Tuple<string, string> censusTypeData = new(censusService, censusType);

            if (_repository.ContainsKey(censusTypeData))
                return false;

            _repository.Add(censusTypeData, typeof(TObject));
            return true;
        }
    }
}
