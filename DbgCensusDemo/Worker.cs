using DbgCensus.EventStream;
using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbgCensusDemo
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CensusEventStreamOptions _eventStreamOptions;
        private readonly ICensusEventStreamClient _eventStreamClient;

        public Worker(ILogger<Worker> logger, IOptions<CensusEventStreamOptions> eventStreamOptions, ICensusEventStreamClient eventStreamClient)
        {
            _logger = logger;
            _eventStreamOptions = eventStreamOptions.Value;
            _eventStreamClient = eventStreamClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting event stream client");
            try
            {
                await _eventStreamClient.StartAsync(_eventStreamOptions, stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, "An error occured in the event stream client");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventStreamClient.StopAsync().ConfigureAwait(false);
            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
