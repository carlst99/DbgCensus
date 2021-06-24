using DbgCensus.EventStream.Abstractions.Commands;

namespace DbgCensus.EventStream.Commands
{
    /// <summary>
    /// Provides a default implementation of <see cref="DbgCensus.EventStream.Abstractions.Commands.IEventStreamCommand"/>
    /// </summary>
    public abstract record EventStreamCommandBase(string Action, string Service) : IEventStreamCommand;
}
