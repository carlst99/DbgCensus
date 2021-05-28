using DbgCensus.Rest.Queries;
using System;
using Xunit;

namespace DbgCensus.Tests.Rest.Queries
{
    public class QueryCommandFormatterTests
    {
        [Fact]
        public void TestConstructors()
        {
            QueryCommandFormatter formatter = new("command", '=', "argument");
            Assert.Equal("command", formatter.Command);
            Assert.Equal('=', formatter.ComponentSeparator);
            Assert.Contains("argument", formatter.Arguments);
            Assert.False(formatter.AllowsMultipleArguments);

            formatter = new("command", '=', ',', "argument");
            Assert.Equal("command", formatter.Command);
            Assert.Equal('=', formatter.ComponentSeparator);
            Assert.Equal(',', formatter.ArgumentSeparator);
            Assert.Contains("argument", formatter.Arguments);
            Assert.True(formatter.AllowsMultipleArguments);

            formatter = new("command", '=');
            Assert.Empty(formatter.Arguments);
        }

        [Fact]
        public void TestAddArgumentWithoutMultipleAllowed()
        {
            QueryCommandFormatter formatter = new("command", '=');
            formatter.AddArgument("argument1");
            formatter.AddArgument("argument2");

            Assert.Single(formatter.Arguments);
            Assert.Equal("argument2", formatter.Arguments[0]);
        }

        [Fact]
        public void TestAddArgument()
        {
            QueryCommandFormatter formatter = new("command", '=', ',');
            formatter.AddArgument("argument1");
            formatter.AddArgument("argument2");

            Assert.Contains("argument1", formatter.Arguments);
            Assert.Contains("argument2", formatter.Arguments);
        }

        [Fact]
        public void TestAddArgumentRange()
        {
            QueryCommandFormatter formatter = new("command", '=');
            Assert.Throws<InvalidOperationException>(() => formatter.AddArgumentRange(Array.Empty<string>()));

            formatter = new("command", '=', ',');
            formatter.AddArgumentRange(new string[] { "argument1", "argument2" });

            Assert.Contains("argument1", formatter.Arguments);
            Assert.Contains("argument2", formatter.Arguments);
        }

        [Fact]
        public void TestPropAnyValue()
        {
            QueryCommandFormatter formatter = new("command", '=');
            formatter.AddArgument("argument1");

            Assert.True(formatter.AnyValue);
        }

        [Fact]
        public void TestToString()
        {
            QueryCommandFormatter formatter = new("command", '=', ',');
            Assert.Equal(string.Empty, formatter.ToString());

            formatter.AddArgument("argument1");
            formatter.AddArgument("argument2");
            Assert.Equal("command=argument1,argument2", formatter.ToString());
            Assert.Equal(formatter.ToString(), formatter); // Test implicit casting
        }
    }
}
