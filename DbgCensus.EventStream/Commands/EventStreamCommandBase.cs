using DbgCensus.EventStream.Abstractions.Commands;

namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Provides a default implementation of <see cref="DbgCensus.EventStream.Commands.IEventStreamCommand"/>
    /// </summary>
    public record EventStreamCommandBase(string Action, string Service) : IEventStreamCommand;
}
