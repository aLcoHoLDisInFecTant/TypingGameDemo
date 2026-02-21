using UnityEngine;

namespace TypeRogue.Rogue.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrade_Alias", menuName = "TypeRogue/Rogue/Alias")]
    public class AliasUpgrade : RogueUpgradeDef
    {
        public TypingResolveResultType TargetType; // WeaponPistol, WeaponShotgun, etc.
        public string NewAlias;

        public override void Apply(TypeRogueBootstrap bootstrap)
        {
            if (bootstrap == null) return;
            Debug.Log($"[Rogue] Registering Alias: '{NewAlias}' for {TargetType}");
            bootstrap.RegisterAlias(TargetType, NewAlias);
        }
    }
}
