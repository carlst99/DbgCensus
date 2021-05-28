using DbgCensus.Core.Json;
using Xunit;

namespace DbgCensus.Tests.Core.Json
{
    public class SnakeCaseJsonNamingPolicyTests
    {
        [Fact]
        public void TestConvertName()
        {
            SnakeCaseJsonNamingPolicy policy = new();

            Assert.Equal("pascal_case", policy.ConvertName("PascalCase"));
            Assert.Equal("camel_case", policy.ConvertName("camelCase"));
            Assert.Equal("acronym_case", policy.ConvertName("ACRONYMCase"));
            Assert.Equal("word", policy.ConvertName("Word"));
        }
    }
}
