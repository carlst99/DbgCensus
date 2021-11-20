namespace DbgCensus.EventStream.Abstractions.Objects.Events.Characters;

/// <summary>
/// Represents a PlayerLogout event.
/// </summary>
public interface IPlayerLogout : IEvent
{
    /// <summary>
    /// Gets the ID of the character that logged out.
    /// </summary>
    ulong CharacterID { get; }
}
