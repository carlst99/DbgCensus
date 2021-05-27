using DbgCensus.Core.Exceptions;
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

                IQueryBuilder collectionsQuery = _queryFactory.Get();

                List<CensusCollection>? collections;
                try
                {
                    collections = await _censusClient.GetAsync<List<CensusCollection>>(collectionsQuery, stoppingToken).ConfigureAwait(false);
                }
                catch (CensusException ex)
                {
                    _logger.LogError(ex, "An error occured within the Census API.");
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to make the Census request.");
                    continue;
                }

                if (collections is null)
                {
                    _logger.LogError("No collections were returned. Is the API dead?");
                    continue;
                }

                _logger.LogInformation("Collection count: {count}", collections.Count);
                await CheckCollections(collections).ConfigureAwait(false);
            }
        }

        private async Task CheckCollections(List<CensusCollection> collections)
        {
            foreach (CensusCollection collection in collections)
            {
                IQueryBuilder query = _queryFactory.Get();
                query.OnCollection(collection.Name);
            }
        }
    }
}
