using DbgCensus.Core.Objects;
using System;
using Xunit;

namespace DbgCensus.Tests.Core.Objects;

public class OptionalTests
{
    [Fact]
    public void TestEmptyOptional()
    {
        Optional<string> empty = default;

        Assert.False(empty.HasValue);
        Assert.False(empty.IsDefined());

        Assert.False(empty.IsDefined(out string? definedValue));
        Assert.Null(definedValue);

        Assert.Equal(default, empty.GetValueOrDefault());

        Assert.Throws<InvalidOperationException>(() => empty.Value);
    }

    [Fact]
    public void TestPresentOptional()
    {
        const string value = "hello world";
        Optional<string> present = new(value);

        Assert.True(present.HasValue);
        Assert.True(present.IsDefined());

        Assert.True(present.IsDefined(out string? definedValue));
        Assert.Equal(value, definedValue);

        Assert.Equal(value, present.GetValueOrDefault());

        Assert.Equal(value, present.Value);
    }

    [Fact]
    public void TestNullOptional()
    {
        Optional<string?> nullo = new(null);

        Assert.True(nullo.HasValue);
        Assert.False(nullo.IsDefined());

        Assert.False(nullo.IsDefined(out string? definedValue));
        Assert.Null(definedValue);

        Assert.Null(nullo.GetValueOrDefault());

        Assert.Null(nullo.Value);
    }

    [Fact]
    public void TestEquals()
    {
        Optional<string?> present = new("hello world");
        Optional<string?> missing = default;
        Optional<string?> nullo = new(null);

        Assert.Equal(present, present);
        Assert.Equal(nullo, nullo);
        Assert.Equal(missing, missing);

        Assert.NotEqual(present, nullo);
        Assert.NotEqual(present, missing);
        Assert.NotEqual(missing, nullo);
    }
}
