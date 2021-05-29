using DbgCensus.Rest;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries
{
    public class QueryFactoryTests
    {
        [Fact]
        public void TestGet()
        {
            QueryBuilderFactory factory = GetDefaultFactory();

            IQueryBuilder query = factory.Get();
            Assert.NotNull(query);

            CensusQueryOptions defaultOptions = GetDefaultOptions();
            Assert.Contains(defaultOptions.RootEndpoint + "/s:" + defaultOptions.ServiceId, query.ConstructEndpoint().AbsoluteUri.ToLower());
        }

        [Fact]
        public void TestGetWithOptions()
        {
            const string rootEndpoint = "aaabbbccc123";
            QueryBuilderFactory factory = GetDefaultFactory();

            CensusQueryOptions options = GetDefaultOptions();
            options.RootEndpoint = rootEndpoint;

            IQueryBuilder query = factory.Get(options);

            Assert.Contains(rootEndpoint, query.ConstructEndpoint().AbsoluteUri.ToLower());
        }

        private static QueryBuilderFactory GetDefaultFactory() => new(Options.Create(GetDefaultOptions()));

        private static CensusQueryOptions GetDefaultOptions() => new()
        {
            Namespace = "testnamespace",
            RootEndpoint = "testendpoint",
            ServiceId = "testid",
            LanguageCode = CensusLanguage.English,
            Limit = 420
        };
    }
}
