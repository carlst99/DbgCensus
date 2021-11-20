using DbgCensus.EventStream.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventStreamSample;

public class EventStreamWorker : BackgroundService
{
    private readonly ILogger<EventStreamWorker> _logger;
    private readonly IEventStreamClientFactory _clientFactory;

    private IEventStreamClient? _client;

    public EventStreamWorker(ILogger<EventStreamWorker> logger, IEventStreamClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting event stream client");

        while (!stoppingToken.IsCancellationRequested)
        {
            _client = _clientFactory.GetClient<EventStreamWorker>();

            try
            {
                await _client.StartAsync(ct: stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not TaskCanceledException)
            {
                _logger.LogError(ex, "An error occured in the event stream client");
            }
            finally
            {
                _client.Dispose();
            }

            await Task.Delay(15000, stoppingToken).ConfigureAwait(false);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client?.IsRunning == true)
            await _client.StopAsync().ConfigureAwait(false);

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}
