using DbgCensus.Rest.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        Assert.False(formatter.AnyArguments);
        Assert.Throws<ArgumentNullException>(() => formatter.AddArgument(null!));

        formatter.AddArgument("argument");
        formatter.AddArgument("argument1");

        Assert.True(formatter.AnyArguments);
        Assert.Contains("argument", formatter.Arguments);
        Assert.Contains("argument1", formatter.Arguments);
    }

    [Fact]
    public void TestAddArgumentRange()
    {
        MultiQueryCommandFormatter<string> formatter = new("command", ';', ',');

        Assert.Throws<ArgumentNullException>(() => formatter.AddArgumentRange(null!));
        Assert.Throws<ArgumentNullException>(() => formatter.AddArgumentRange(new string[] { null! }));

        formatter.AddArgumentRange(new string[] { "argument", "argument1" });

        Assert.True(formatter.AnyArguments);
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
