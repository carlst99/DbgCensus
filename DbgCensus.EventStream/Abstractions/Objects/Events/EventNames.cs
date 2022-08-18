namespace DbgCensus.EventStream.Abstractions.Objects.Events;

public static class EventNames
{
    // Character-level events
    public const string AchievementEarned = "AchievementEarned";
    public const string BattleRankUp = "BattleRankUp";
    public const string Death = "Death";
    public const string GainExperience = "GainExperience";
    public const string ItemAdded = "ItemAdded";
    public const string PlayerFacilityCapture = "PlayerFacilityCapture";
    public const string PlayerFacilityDefend = "PlayerFacilityDefend";
    public const string PlayerLogin = "PlayerLogin";
    public const string PlayerLogout = "PlayerLogin";
    public const string SkillAdded = "SkillAdded";
    public const string VehicleDestroy = "VehicleDestroy";

    // World-level events
    public const string ContinentLock = "ContinentLock";
    public const string ContinentUnlock = "ContinentUnlock";
    public const string FacilityControl = "FacilityControl";
    public const string MetagameEvent = "MetagameEvent";

    /// <summary>
    /// Gets an event name for a particular experience event.
    /// Experience event IDs can be found in the <c>event</c>
    /// collection of the Census REST API.
    /// </summary>
    /// <param name="expEventID">The ID of the experience event.</param>
    /// <returns>A valid event name.</returns>
    public static string GetExperienceEventName(int expEventID)
        => $"GainExperience_experience_id_{expEventID}";
}
