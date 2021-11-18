using DbgCensus.Core.Objects;
using DbgCensus.EventStream.Abstractions.Objects.Events.Characters;
using System;

namespace DbgCensus.EventStream.Objects.Events.Characters;

/// <summary>
/// Initializes a new instance of the <see cref="PlayerLogin"/> record.
/// </summary>
/// <param name="CharacterID">The ID of the character that logged in.</param>
/// <param name="EventName">The name of the event.</param>
/// <param name="Timestamp">The time at which the event occured.</param>
/// <param name="WorldID">The world on which the event occured.</param>
public record PlayerLogin
(
    ulong CharacterID,
    string EventName,
    DateTimeOffset Timestamp,
    WorldDefinition WorldID
) : IPlayerLogin;
