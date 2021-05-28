using DbgCensus.Rest.Abstractions.Queries;
using DbgCensus.Rest.Queries;
using System;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries
{
    public class QueryFilterTests
    {
        private const string FIELD_NAME = "field";
        private const string FILTER_VALUE = "filterValue";
        private static readonly SearchModifier MODIFIER = SearchModifier.CONTAINS;

        [Fact]
        public void TestConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryFilter(string.Empty, FILTER_VALUE, MODIFIER));
            Assert.Throws<ArgumentNullException>(() => new QueryFilter(FIELD_NAME, string.Empty, MODIFIER));

            QueryFilter filter = new(FIELD_NAME, FILTER_VALUE, MODIFIER);
            Assert.Equal(FIELD_NAME, filter.Field);
            Assert.Equal(FILTER_VALUE, filter.Value);
            Assert.Equal(MODIFIER, filter.Modifier);
        }

        [Fact]
        public void TestToString()
        {
            QueryFilter filter = new(FIELD_NAME, FILTER_VALUE, MODIFIER);
            Assert.Equal($"{FIELD_NAME}={MODIFIER}{FILTER_VALUE}", filter.ToString());
            Assert.Equal(filter.ToString(), filter); // Test implicit casting
        }
    }
}
