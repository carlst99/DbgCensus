using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace DbgCensus.EventStream
{
    /// <inheritdoc cref="ICensusEventStreamClientFactory"/>
    public class CensusEventStreamClientFactory<TClient> : ICensusEventStreamClientFactory where TClient : ICensusEventStreamClient
    {
        private readonly Dictionary<string, TClient> _repository;
        private readonly CensusEventStreamOptions _options;
        private readonly IServiceProvider _services;
        private readonly Func<IServiceProvider, string, TClient> _clientFactory;

        /// <summary>
        /// Initialises a new instance of the <see cref="CensusEventStreamClientFactory{TClient}"/> class.
        /// </summary>
        /// <param name="options">This parameter is currently unused.</param>
        /// <param name="services">The service provider.</param>
        /// <param name="clientFactory">The factory to use when creating new instances of an <see cref="ICensusEventStreamClient"/>.</param>
        public CensusEventStreamClientFactory(
            IOptions<CensusEventStreamOptions> options,
            IServiceProvider services,
            Func<IServiceProvider, string, TClient> clientFactory)
        {
            _options = options.Value;
            _services = services;
            _clientFactory = clientFactory;

            _repository = new Dictionary<string, TClient>();
        }

        /// <inheritdoc />
        public ICensusEventStreamClient GetClient(string name, CensusEventStreamOptions? options = null)
        {
            if (options is null)
                options = _options;

            if (!_repository.ContainsKey(name) || _repository[name].IsDisposed)
                _repository[name] = _clientFactory.Invoke(_services, name);

            return _repository[name];
        }

        /// <inheritdoc />
        public ICensusEventStreamClient GetClient(CensusEventStreamOptions? options = null)
            => GetClient(Guid.NewGuid().ToString(), options);

        /// <inheritdoc />
        public ICensusEventStreamClient GetClient<TConsumer>(CensusEventStreamOptions? options = null)
            => GetClient(nameof(TConsumer), options);
    }
}
