using DbgCensus.Rest.Json;
using Xunit;

namespace DbgCensus.Rest.Tests.Json
{
    public class CamelToSnakeCaseJsonNamingPolicyTests
    {
        [Fact]
        public void TestConvertName()
        {
            const string testName = "PascalCase";
            const string outputName = "pascal_case";

            CamelToSnakeCaseJsonNamingPolicy policy = new();

            Assert.Equal(outputName, policy.ConvertName(testName));
        }
    }
}
