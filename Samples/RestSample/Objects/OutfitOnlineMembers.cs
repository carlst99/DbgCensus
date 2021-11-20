using System.Collections.Generic;
using System.Text.Json.Serialization;
using static RestSample.Objects.OutfitOnlineMembers;

namespace RestSample.Objects;

/// <summary>
/// Initialises a new instance of the <see cref="OutfitOnlineMembers"/> record.
/// The query model for <see href="https://census.daybreakgames.com/get/ps2/outfit?outfit_id=37562651025751157&c:show=name,outfit_id,alias&c:join=outfit_member%5Einject_at:members%5Eshow:character_id%5Eouter:1%5Elist:1(character%5Eshow:name.first%5Einject_at:character%5Eouter:0%5Eon:character_id(characters_online_status%5Einject_at:online_status%5Eshow:online_status%5Eouter:0(world%5Eon:online_status%5Eto:world_id%5Eouter:0%5Eshow:world_id%5Einject_at:ignore_this))"/>.
/// </summary>
/// <param name="OutfitID">The ID of the outfit.</param>
/// <param name="OutfitName">The name of the outfit.</param>
/// <param name="OutfitAlias">The alias (tag) of the outfit.</param>
/// <param name="OnlineMembers">The online members of the outfit.</param>
public record OutfitOnlineMembers
(
    ulong OutfitID,
    [property: JsonPropertyName("name")] string OutfitName,
    [property: JsonPropertyName("alias")] string OutfitAlias,
    [property: JsonPropertyName("members")] IReadOnlyList<MemberModel>? OnlineMembers
)
{
    public record MemberModel
    (
        ulong CharacterID,
        CharacterModel Character
    );

    public record CharacterModel
    (
        Name Name,
        OnlineStatusModel OnlineStatus
    );

    public record OnlineStatusModel
    (
        [property: JsonPropertyName("online_status")] bool IsOnline
    );
}
