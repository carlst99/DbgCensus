namespace DbgCensus.EventStream.Abstractions.Objects.Characters;

/// <summary>
/// Represents a PlayerLogin event.
/// </summary>
public interface IPlayerLogin : IEvent
{
    /// <summary>
    /// Gets the ID of the character that logged in.
    /// </summary>
    ulong CharacterID { get; }
}
