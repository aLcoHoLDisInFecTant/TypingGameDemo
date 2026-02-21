using UnityEngine;
using TypeRogue.Data;

namespace TypeRogue.Rogue.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrade_WeaponUnlock", menuName = "TypeRogue/Rogue/WeaponUnlock")]
    public class WeaponUnlockUpgrade : RogueUpgradeDef
    {
        public WeaponData WeaponToUnlock;

        public override void Apply(TypeRogueBootstrap bootstrap)
        {
            if (bootstrap == null || WeaponToUnlock == null) return;
            Debug.Log($"[Rogue] Unlocking Weapon: {WeaponToUnlock.WeaponName}");
            bootstrap.UnlockWeapon(WeaponToUnlock);
        }
    }
}
