using DbgCensus.Rest;
using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using System;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries;

public class QueryBuilderTests
{
    [Fact]
    public void TestAllArgumentTypesFormattedCorrectly()
    {
        QueryBuilder builder = new("myColl", GetDefaultOptions());

        // Test that filters, commands and custom params are placed correctly into the query
        builder.Where("field1", SearchModifier.Equals, "x")
            .AddResolve("resolve")
            .WithCustomParameter("custom=1")
            .WhereAll("field2", SearchModifier.Equals, new[] {1, 2})
            .WithCustomParameter("custom2=2")
            .WithTimings();

        Uri endpoint = builder.ConstructEndpoint();
        string[] parts = endpoint.Query.TrimStart('?').Split('&');

        Assert.Contains("field1=x", parts);
        Assert.Contains("c:resolve=resolve", parts);
        Assert.Contains("custom=1", parts);
        Assert.Contains("field2=1,2", parts);
        Assert.Contains("custom2=2", parts);
        Assert.Contains("c:timing=True", parts);
    }

    private static CensusQueryOptions GetDefaultOptions()
        => new()
        {
            Limit = 10,
            LanguageCode = "en",
            Namespace = "ps1",
            RootEndpoint = "https://census.test",
            ServiceId = "aaa"
        };
}
