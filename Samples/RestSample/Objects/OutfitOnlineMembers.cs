using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RestSample.Objects
{
    // The query model for
    // https://census.daybreakgames.com/get/ps2/outfit?outfit_id=37562651025751157&c:show=name,outfit_id,alias&c:join=outfit_member%5Einject_at:members%5Eshow:character_id%5Eouter:1%5Elist:1(character%5Eshow:name.first%5Einject_at:character%5Eouter:0%5Eon:character_id(characters_online_status%5Einject_at:online_status%5Eshow:online_status%5Eouter:0(world%5Eon:online_status%5Eto:world_id%5Eouter:0%5Eshow:world_id%5Einject_at:ignore_this))
    public record OutfitOnlineMembers
    {
        public record MemberModel(ulong CharacterId, MemberModel.CharacterModel Character)
        {
            public record CharacterModel(Name Name, CharacterModel.OnlineStatusModel OnlineStatus)
            {
                public record OnlineStatusModel
                {
                    [JsonPropertyName("online_status")]
                    public bool IsOnline { get; init; }
                }
            }
        }

        public long OutfitId { get; init; }

        [JsonPropertyName("name")]
        public string OutfitName { get; init; }

        [JsonPropertyName("alias")]
        public string OutfitAlias { get; init; }

        [JsonPropertyName("members")]
        public IReadOnlyList<MemberModel> OnlineMembers { get; init; }

        public OutfitOnlineMembers()
        {
            OutfitName = string.Empty;
            OutfitAlias = string.Empty;
            OnlineMembers = new List<MemberModel>().AsReadOnly();
        }
    }
}
