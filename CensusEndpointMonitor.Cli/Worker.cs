using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CensusEndpointMonitor.Cli
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueryFactory _queryFactory;
        private readonly ICensusRestClient _censusClient;

        public Worker(
            ILogger<Worker> logger,
            IQueryFactory queryFactory,
            ICensusRestClient censusClient)
        {
            _logger = logger;
            _queryFactory = queryFactory;
            _censusClient = censusClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bool firstTimeRun = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                if (firstTimeRun)
                    firstTimeRun = false;
                else
                    await Task.Delay(60000, stoppingToken).ConfigureAwait(false);

                IQuery collectionsQuery = _queryFactory.Get();

                List<CensusCollection>? collections;
                try
                {
                    collections = await _censusClient.GetAsync<List<CensusCollection>>(collectionsQuery, stoppingToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not get census collections");
                    continue;
                }

                if (collections is null)
                {
                    _logger.LogError("The collections result could not be parsed.");
                    continue;
                }

                _logger.LogInformation("Collection count: {count}", collections.Count);
            }
        }
    }
}
