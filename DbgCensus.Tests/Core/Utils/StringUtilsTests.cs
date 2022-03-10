using DbgCensus.Core.Utils;
using System;
using Xunit;

namespace DbgCensus.Tests.Core.Utils;

public class StringUtilsTests
{
    [Fact]
    public void TestJoinWithoutNullOrEmptyValues()
    {
        string?[] values = { "1", "", "2", null, "3" };

        Assert.Equal("1,2,3", StringUtils.JoinWithoutNullOrEmptyValues(',', values));
    }

    [Fact]
    public void TestSafeToString()
    {
        const string value = "hello world";
        const object? nullo = null;

        Assert.Throws<ArgumentNullException>(() => StringUtils.SafeToString(nullo));
        Assert.Throws<ArgumentException>(() => StringUtils.SafeToString(new object()));
        Assert.Equal(value, StringUtils.SafeToString(value));
    }
}
