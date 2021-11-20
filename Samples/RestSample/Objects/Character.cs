using DbgCensus.Core.Objects;
using System;

namespace RestSample.Objects;

public record Character
(
    ulong CharacterId,
    Name Name,
    FactionDefinition FactionId,
    int TitleId,
    Character.CharacterTimes Times,
    Character.CharacterCerts Certs,
    Character.CharacterBattleRank BattleRank,
    int ProfileId,
    int PrestigeLevel
)
{
    public record CharacterTimes(DateTimeOffset CreationDate, DateTimeOffset LastSaveDate, DateTimeOffset LastLoginDate, uint LoginCount, uint MinutesPlayed);

    public record CharacterCerts(uint EarnedPoints, uint GiftedPoints, uint SpentPoints, uint AvailablePoints, double PercentToNext);

    public record CharacterBattleRank(int PercentToNext, int Value);
}
