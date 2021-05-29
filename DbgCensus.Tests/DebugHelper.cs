using DbgCensus.Rest;
using DbgCensus.Rest.Abstractions;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Xunit;

namespace DbgCensus.Tests
{
    public class DebugHelper
    {
        [Fact]
        public async void Test()
        {
            IQueryBuilderFactory queryFactory = new QueryBuilderFactory(Options.Create(new CensusQueryOptions()));
            IQueryBuilder builder = queryFactory.Get()
                .OnCollection("world")
                .Where("world_id", SearchModifier.Equals, 1, 10);

            ICensusRestClient client = new CensusRestClient(new Logger<CensusRestClient>(new LoggerFactory()), new System.Net.Http.HttpClient(), new System.Text.Json.JsonSerializerOptions());

            List<World>? worldResult =  await client.GetAsync<List<World>>(builder).ConfigureAwait(false);

            if (worldResult?.Count != 2)
                throw new Exception("No result was returned.");
        }

        public record World
        {
            public int WorldId { get; init; }

            public string State { get; init; }

            TranslationProperty Name { get; init; }

            public World()
            {
                State = string.Empty;
                Name = new TranslationProperty();
            }
        }

        public record TranslationProperty
        {
            [JsonPropertyName("en")]
            public string English { get; init; }

            public TranslationProperty()
            {
                English = string.Empty;
            }
        }
    }
}
