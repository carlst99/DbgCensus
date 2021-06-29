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

            Assert.Equal(string.Empty, policy.ConvertName(string.Empty));
            Assert.Null(policy.ConvertName(null!));
            Assert.Equal("pascal_case_for_properties", policy.ConvertName("PascalCaseForProperties"));
            Assert.Equal("camel_case_for_fields", policy.ConvertName("camelCaseForFields"));
            Assert.Equal("acronym_case_for_whynot", policy.ConvertName("ACRONYMCaseForWHYNOT"));
            Assert.Equal("word", policy.ConvertName("Word"));
        }
    }
}
