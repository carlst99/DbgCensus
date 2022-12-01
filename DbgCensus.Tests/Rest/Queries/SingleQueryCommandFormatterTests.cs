using DbgCensus.Rest.Queries.Internal;
using System;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries;

public class SingleQueryCommandFormatterTests
{
    [Fact]
    public void TestConstructor()
    {
        SingleQueryCommandFormatter<string> formatter = new("command", ',');

        Assert.Equal("command", formatter.Command);
        Assert.Equal(',', formatter.ComponentSeparator);
    }

    [Fact]
    public void TestSetArgument()
    {
        SingleQueryCommandFormatter<string> formatter = new("command", ',');

        Assert.False(formatter.HasArgument);
        Assert.Throws<ArgumentNullException>(() => formatter.SetArgument(null!));

        formatter.SetArgument("argument");

        Assert.True(formatter.HasArgument);
        Assert.Equal("argument", formatter.Argument);

        formatter.SetArgument("argument1");
        Assert.Equal("argument1", formatter.Argument);
    }

    [Fact]
    public void TestToString()
    {
        SingleQueryCommandFormatter<string> formatter = new("command", ',');
        formatter.SetArgument("argument");

        Assert.Equal("command,argument", formatter);
    }
}
