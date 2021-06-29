using DbgCensus.EventStream;
using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CensusEventStreamOptions _options;
        private readonly ICensusEventStreamClient _client;

        public Worker(ILogger<Worker> logger, IOptions<CensusEventStreamOptions> eventStreamOptions, ICensusEventStreamClient eventStreamClient)
        {
            _logger = logger;
            _options = eventStreamOptions.Value;
            _client = eventStreamClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting event stream client");

            try
            {
                await _client.StartAsync(_options, stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, "An error occured in the event stream client");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.StopAsync().ConfigureAwait(false);
            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
