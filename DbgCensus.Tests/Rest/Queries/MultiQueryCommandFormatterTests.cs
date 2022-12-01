using DbgCensus.Rest.Queries.Internal;
using System;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries;

public class MultiQueryCommandFormatterTests
{
    [Fact]
    public void TestConstructor()
    {
        MultiQueryCommandFormatter<string> formatter = new("command", ';', ',');

        Assert.Equal("command", formatter.Command);
        Assert.Equal(';', formatter.ComponentSeparator);
        Assert.Equal(',', formatter.ArgumentSeparator);
    }

    [Fact]
    public void TestAddArgument()
    {
        MultiQueryCommandFormatter<string> formatter = new("command", ';', ',');

        Assert.Empty(formatter.Arguments);
        Assert.Throws<ArgumentNullException>(() => formatter.AddArgument(null!));

        formatter.AddArgument("argument");
        formatter.AddArgument("argument1");

        Assert.Equal(2, formatter.Arguments.Count);
        Assert.Contains("argument", formatter.Arguments);
        Assert.Contains("argument1", formatter.Arguments);
    }

    [Fact]
    public void TestAddArgumentRange()
    {
        MultiQueryCommandFormatter<string> formatter = new("command", ';', ',');

        Assert.Throws<ArgumentNullException>(() => formatter.AddArgumentRange(null!));
        Assert.Throws<ArgumentNullException>(() => formatter.AddArgumentRange(new string[] { null! }));

        formatter.AddArgumentRange(new[] { "argument", "argument1" });

        Assert.Equal(2, formatter.Arguments.Count);
        Assert.Contains("argument", formatter.Arguments);
        Assert.Contains("argument1", formatter.Arguments);
    }

    [Fact]
    public void TestToString()
    {
        MultiQueryCommandFormatter<string> formatter = new("command", ';', ',');
        formatter.AddArgument("argument");
        formatter.AddArgument("argument1");

        Assert.Equal("command;argument,argument1", formatter);
    }
}
