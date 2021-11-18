using DbgCensus.EventStream.Abstractions.Objects.Commands;

namespace DbgCensus.EventStream.Commands;

/// <summary>
/// Provides a default implementation of <see cref="Abstractions.Objects.Commands.IEventStreamCommand"/>
/// </summary>
public abstract record EventStreamCommandBase(string Action, string Service) : IEventStreamCommand;
