using DbgCensus.Rest.Queries.Internal;
using System;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries;

public class QueryResolveTests
{
    private const string RESOLVE_TO = "resolve";
    private static readonly string[] SHOW_FIELDS = { "show1", "show2" };

    [Fact]
    public void TestConstructor()
    {
        Assert.Throws<ArgumentNullException>(() => new QueryResolve(string.Empty, SHOW_FIELDS));

        QueryResolve resolve = new(RESOLVE_TO, SHOW_FIELDS);
        Assert.Equal(RESOLVE_TO, resolve.ResolveTo);
        Assert.Equal(SHOW_FIELDS, resolve.ShowFields);
    }

    [Fact]
    public void TestToString()
    {
        QueryResolve resolve = new(RESOLVE_TO, SHOW_FIELDS);
        Assert.Equal($"{RESOLVE_TO}({string.Join(',', SHOW_FIELDS)})", resolve.ToString());
    }
}
