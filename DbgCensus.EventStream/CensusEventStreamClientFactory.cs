using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DbgCensus.EventStream
{
    /// <inheritdoc cref="ICensusEventStreamClientFactory"/>
    public class CensusEventStreamClientFactory<TClient> : ICensusEventStreamClientFactory where TClient : ICensusEventStreamClient
    {
        private readonly Dictionary<string, TClient> _repository;
        private readonly CensusEventStreamOptions _options;
        private readonly IServiceProvider _services;
        private readonly Func<IServiceProvider, string, TClient> _clientFactory;
        private readonly Func<IServiceProvider, JsonSerializerOptions> _deserializationOptions;
        private readonly Func<IServiceProvider, JsonSerializerOptions> _serializationOptions;

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
            : this(options, services, clientFactory, _ => new JsonSerializerOptions(), _ => new JsonSerializerOptions())
        {
        }

        /// <inheritdoc cref="CensusEventStreamClientFactory{TClient}.CensusEventStreamClientFactory(IOptions{CensusEventStreamOptions}, IServiceProvider, Func{IServiceProvider, string, TClient})"/>
        /// <param name="deserializationOptions">The JSON options for each client to use when deserializing event stream objects.</param>
        /// <param name="serializationOptions">The JSON options for each client to use when serializing commands.</param>
        public CensusEventStreamClientFactory(
            IOptions<CensusEventStreamOptions> options,
            IServiceProvider services,
            Func<IServiceProvider, string, TClient> clientFactory,
            Func<IServiceProvider, JsonSerializerOptions> deserializationOptions,
            Func<IServiceProvider, JsonSerializerOptions> serializationOptions)
        {
            _options = options.Value;
            _services = services;
            _clientFactory = clientFactory;
            _deserializationOptions = deserializationOptions;
            _serializationOptions = serializationOptions;

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
