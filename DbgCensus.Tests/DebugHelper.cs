using DbgCensus.Rest;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbgCensus.Tests
{
    public class DebugHelper
    {
        [Fact]
        public void Test()
        {
            IQueryBuilderFactory queryFactory = new QueryBuilderFactory(Options.Create(new CensusQueryOptions()));
            IQueryBuilder builder = queryFactory.Get();

            builder.OnCollection("testCollection")
                .Where("outfit_alias", SearchModifier.Equals, "uvoc")
                .ShowFields("name", "outfit_id", "alias")
                .AddJoin("outfit_member")
                    .InjectAt("members")
                    .ShowFields("character_id")
                    .IsList();

            string uri = builder.ConstructEndpoint().AbsoluteUri;

            return;
                    //.AddNestedJoin("character")
                    //    .OnField("character_id")
                    //    .InjectAt("character")
                    //    .ShowFields("name.first")
                    //    .IsInnerJoin()
                    //    .AddNestedJoin("characters_online_status")
                    //        .InjectAt("online_status")
                    //        .ShowFields("online_status")
                    //        .IsInnerJoin()
                    //        .AddNestedJoin("world")
                    //            .OnField("online_status")
                    //            .ToField("world_id")
                    //            .InjectAt("ignore_this")
                    //            .ShowFields("world_id")
                    //            .IsInnerJoin();
        }
    }
}
