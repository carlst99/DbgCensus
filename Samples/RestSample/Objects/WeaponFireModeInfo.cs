using DbgCensus.Core.Objects;
using System.Collections.Generic;

namespace RestSample.Objects;

public record WeaponFireModeInfo
(
    uint ItemID,
    uint ItemCategoryID,
    uint FactionID,
    GlobalizedString Name,
    GlobalizedString Description,
    uint ImageSetID,
    WeaponFireModeInfo.ItemToWeapon ItemIdJoinItemToWeapon
)
{
    public record ItemToWeapon
    (
        uint ItemID,
        uint WeaponID,
        Weapon WeaponIdJoinWeapon
    );

    public record Weapon
    (
        uint WeaponID,
        uint WeaponGroupID,
        ushort EquipMS,
        ushort UnequipMS,
        IReadOnlyList<WeaponToFireGroup> WeaponIdJoinWeaponToFireGroup
    );

    public record WeaponToFireGroup
    (
        uint WeaponID,
        uint FireGroupID,
        byte FireGroupIndex,
        FireGroup FireGroupIdJoinFireGroup
    );

    public record FireGroup
    (
        uint FireGroupID,
        ushort TransitionDurationMS,
        IReadOnlyList<FireGroupToFireMode> FireGroupIdJoinFireGroupToFireMode
    );

    public record FireGroupToFireMode
    (
        uint FireGroupID,
        uint FireModeID,
        byte FireModeIndex,
        IReadOnlyList<FireMode2> FireModeIdJoinFireMode2
    );

    public record FireMode2
    (
        uint FireModeId,
        bool UseInWater
    );
}
