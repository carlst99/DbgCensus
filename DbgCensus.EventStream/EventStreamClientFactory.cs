using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace DbgCensus.EventStream
{
    /// <inheritdoc cref="IEventStreamClientFactory"/>
    public class EventStreamClientFactory : IEventStreamClientFactory
    {
        private readonly Dictionary<string, IEventStreamClient> _repository;
        private readonly IServiceProvider _services;
        private readonly IOptions<EventStreamOptions> _options;
        private readonly Func<IServiceProvider, IOptions<EventStreamOptions>, string, IEventStreamClient> _clientFactory;

        /// <summary>
        /// Initialises a new instance of the <see cref="EventStreamClientFactory"/> class.
        /// </summary>
        /// <param name="services">The service provider.</param>
        /// <param name="options">This parameter is currently unused.</param>
        /// <param name="clientFactory">The factory to use when creating new instances of an <see cref="IEventStreamClient"/>.</param>
        public EventStreamClientFactory(
            IServiceProvider services,
            IOptions<EventStreamOptions> options,
            Func<IServiceProvider, IOptions<EventStreamOptions>, string, IEventStreamClient> clientFactory)
        {
            _options = options;
            _services = services;
            _clientFactory = clientFactory;

            _repository = new Dictionary<string, IEventStreamClient>();
        }

        /// <inheritdoc />
        public IEventStreamClient GetClient(string name, EventStreamOptions? options = null)
        {
            if (options is null)
                options = _options.Value;

            if (!_repository.ContainsKey(name) || _repository[name].IsDisposed)
                _repository[name] = _clientFactory.Invoke(_services, Options.Create(options), name);

            return _repository[name];
        }

        /// <inheritdoc />
        public IEventStreamClient GetClient(EventStreamOptions? options = null)
            => GetClient(Guid.NewGuid().ToString(), options);

        /// <inheritdoc />
        public IEventStreamClient GetClient<TConsumer>(EventStreamOptions? options = null)
            => GetClient(nameof(TConsumer), options);
    }
}
