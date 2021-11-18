using DbgCensus.EventStream.Abstractions.Objects.Commands;

namespace DbgCensus.EventStream.Objects.Commands;

/// <summary>
/// Represents a base command object.
/// </summary>
/// <param name="Action">The action that the command intends to make.</param>
/// <param name="Service">The service that the command is intended for.</param>
public record CommandBase
(
    string Action,
    string Service
) : ICommand;
