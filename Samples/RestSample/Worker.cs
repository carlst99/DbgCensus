using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSample.Objects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RestSample
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICensusRestClient _client;
        private readonly IQueryBuilderFactory _queryFactory;

        public Worker(ILogger<Worker> logger, ICensusRestClient client, IQueryBuilderFactory queryFactory)
        {
            _logger = logger;
            _client = client;
            _queryFactory = queryFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IQueryBuilder query = _queryFactory.Get()
                .OnCollection("character")
                .Where("character_id", SearchModifier.Equals, 5428703501287322737);

            try
            {
                Character? character = await _client.GetAsync<Character>(query, stoppingToken).ConfigureAwait(false);
                if (character is null)
                {
                    _logger.LogInformation("That character does not exist.");
                }
                else
                {
                    _logger.LogInformation(
                        "The character {name} of the {faction} is battle rank {rank}~{asp} with {certs} certs available. They last logged in on {lastLogin}",
                        character.Name.First,
                        character.FactionId,
                        character.BattleRank.Value,
                        character.PrestigeLevel,
                        character.Certs.AvailablePoints,
                        character.Times.LastLoginDate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve character.");
            }
        }
    }
}
