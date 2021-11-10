using DbgCensus.Core.Utils;
using Xunit;

namespace DbgCensus.Tests.Core.Utils;

public class StringUtilsTests
{
    [Fact]
    public void TestJoinWithoutNullOrEmptyValuesI()
    {
        string?[] values = new string?[] { "1", "", "2", null, "3" };

        Assert.Equal("1,2,3", StringUtils.JoinWithoutNullOrEmptyValues(',', values));
    }
}
