using UnityEngine;

namespace TypeRogue.Rogue.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrade_StatBoost", menuName = "TypeRogue/Rogue/StatBoost")]
    public class StatBoostUpgrade : RogueUpgradeDef
    {
        public enum StatType
        {
            DamageMultiplier,
            FireRate,
            SpreadReduction,
            Piercing
        }

        public string TargetWeaponName = "All"; // "All", "Pistol", "Shotgun", "Rifle"
        public StatType Stat;
        public float Value;

        public override void Apply(TypeRogueBootstrap bootstrap)
        {
            if (bootstrap == null) return;
            Debug.Log($"[Rogue] Boosting {Stat} by {Value} for {TargetWeaponName}");
            bootstrap.ModifyWeaponStat(TargetWeaponName, Stat, Value);
        }
    }
}
