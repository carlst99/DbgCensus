using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="PlayerLogout"/> record.
/// </summary>
/// <param name="CharacterID">The ID of the character that logged out.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
public record PlayerLogout
(
    ulong CharacterID,
    string EventName,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID
) : IPlayerLogout;
