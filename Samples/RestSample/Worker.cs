using DbgCensus.Core.Exceptions;
using DbgCensus.Core.Objects;
using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSample.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestSample;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IQueryService _queryService;

    public Worker(ILogger<Worker> logger, IQueryService queryService)
    {
        _logger = logger;
        _queryService = queryService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await GetMapStatus(stoppingToken).ConfigureAwait(false);
        await GetCharacter(stoppingToken).ConfigureAwait(false);
        await GetOnlineOutfitMembers(stoppingToken).ConfigureAwait(false);
    }

    private async Task GetCharacter(CancellationToken ct = default)
    {
        _logger.LogInformation("Getting character information for FalconEye36");

        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("character")
            .Where("name.first_lower", SearchModifier.Equals, "falconeye36");

        try
        {
            Character? character = await _queryService.GetAsync<Character>(query, ct).ConfigureAwait(false);
            if (character is null)
            {
                _logger.LogInformation("That character does not exist.");
                return;
            }

            _logger.LogInformation(
                "The character {name} of the {faction} is battle rank {rank}~{asp} with {certs} certs available. They last logged in on {lastLogin}",
                character.Name.First,
                character.FactionId,
                character.BattleRank.Value,
                character.PrestigeLevel,
                character.Certs.AvailablePoints,
                character.Times.LastLoginDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve character.");
        }
    }

    private async Task GetOnlineOutfitMembers(CancellationToken ct = default)
    {
        _logger.LogInformation("Getting online outfit members for TWC2");

        IQueryBuilder query = _queryService.CreateQuery();

        query.OnCollection("outfit")
            .Where("alias_lower", SearchModifier.Equals, "twc2")
            .ShowFields("name", "outfit_id", "alias")
            .AddJoin("outfit_member") // Returns new join object
                .InjectAt("members")
                .ShowFields("character_id")
                .IsList()
                .AddNestedJoin("character")
                    .OnField("character_id")
                    .InjectAt("character")
                    .ShowFields("name.first")
                    .IsInnerJoin()
                    .AddNestedJoin("characters_online_status", (onlineJoin) => // Returns existing join object (or query object, if performed on the root builder) to allow full method chaining
                    {
                        onlineJoin.InjectAt("online_status")
                            .ShowFields("online_status")
                            .IsInnerJoin()
                            .AddNestedJoin("world", (worldJoin) =>
                            {
                                worldJoin.OnField("online_status")
                                    .ToField("world_id")
                                    .InjectAt("ignore_this")
                                    .ShowFields("world_id")
                                    .IsInnerJoin();
                            });
                    });

        try
        {
            OutfitOnlineMembers? outfit = await _queryService.GetAsync<OutfitOnlineMembers>(query, ct).ConfigureAwait(false);
            if (outfit is null)
            {
                _logger.LogInformation("That outfit does not exist.");
                return;
            }

            _logger.LogInformation(
                "The outfit [{alias}] {name} has {onlineCount} members online: {onlineMembers}",
                outfit.OutfitAlias,
                outfit.OutfitName,
                outfit.OnlineMembers.Count,
                string.Join(", ", outfit.OnlineMembers.Select(m => m.Character.Name.First)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve character.");
        }
    }

    private async Task GetMapStatus(CancellationToken ct = default)
    {
        const WorldDefinition world = WorldDefinition.Connery;

        IEnumerable<ushort> zones = new ZoneDefinition[]
        {
                ZoneDefinition.Amerish,
                ZoneDefinition.Esamir,
                ZoneDefinition.Hossin,
                ZoneDefinition.Indar,
                ZoneDefinition.Koltyr
        }.Cast<ushort>();

        IQueryBuilder query = _queryService.CreateQuery()
            .OnCollection("map")
            .Where("world_id", SearchModifier.Equals, world)
            .WhereAll("zone_ids", SearchModifier.Equals, zones);

        try
        {
            List<Map>? maps = await _queryService.GetAsync<List<Map>>(query, ct).ConfigureAwait(false);
            if (maps is null)
                throw new CensusException("Census returned no data");

            string message = $"{world} map status: ";
            foreach (Map m in maps)
            {
                double regionCount = m.Regions.Row.Count(r => r.RowData.FactionId != FactionDefinition.None);
                double ncPercent = (m.Regions.Row.Count(r => r.RowData.FactionId == FactionDefinition.NC) / regionCount) * 100;
                double trPercent = (m.Regions.Row.Count(r => r.RowData.FactionId == FactionDefinition.TR) / regionCount) * 100;
                double vsPercent = (m.Regions.Row.Count(r => r.RowData.FactionId == FactionDefinition.VS) / regionCount) * 100;

                message += $"\n\t- {m.ZoneId} | NC: {ncPercent:F}%, TR: {trPercent:F}%, VS: {vsPercent:F}%";
            }

            _logger.LogInformation(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve maps.");
        }
    }
}
