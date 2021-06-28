using DbgCensus.EventStream;
using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            await _eventStreamClient.StartAsync(_eventStreamOptions, stoppingToken).ConfigureAwait(false);
        }
    }
}
