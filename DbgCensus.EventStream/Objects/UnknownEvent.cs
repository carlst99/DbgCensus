using DbgCensus.EventStream.Abstractions.Objects;

namespace DbgCensus.EventStream.Objects
{
    public record UnknownEvent(string RawData) : IEventStreamObject;
}
