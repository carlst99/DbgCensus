using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using Microsoft.Extensions.Options;
using Xunit;

namespace DbgCensus.Rest.Tests.Queries
{
    public class QueryFactoryTests
    {
        [Fact]
        public void TestGet()
        {
            QueryFactory factory = new(Options.Create(new CensusQueryOptions
            {
                RootEndpoint = "testEndpoint",
                ServiceId = "testID"
            }));

            IQuery query = factory.Get();
            Assert.NotNull(query);

            Assert.Contains("testendpoint/s:testid", query.ConstructEndpoint().AbsoluteUri.ToLower());
        }
    }
}
