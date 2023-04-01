using Microsoft.Extensions.Options;
using System;

namespace DbgCensus.Tests.Helpers;
public class TestableOptionsMonitor<T> : IOptionsMonitor<T>
{
    public T CurrentValue { get; }

    public TestableOptionsMonitor(T currentValue)
    {
        CurrentValue = currentValue;
    }

    public T Get(string? name)
        => CurrentValue;

    public IDisposable OnChange(Action<T, string> listener)
        => throw new NotImplementedException();
}
