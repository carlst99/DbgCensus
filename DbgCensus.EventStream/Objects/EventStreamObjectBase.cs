using DbgCensus.EventStream.Abstractions.Objects;

namespace DbgCensus.EventStream.Objects
{
    /// <summary>
    /// Provides a default implementation of <see cref="DbgCensus.EventStream.Objects.IEventStreamObject"/>
    /// </summary>
    public record EventStreamObjectBase(string Service, string Type) : IEventStreamObject;
}
