using UnityEngine;
using TypeRogue.Data;

namespace TypeRogue.Rogue.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrade_BuffUnlock", menuName = "TypeRogue/Rogue/BuffUnlock")]
    public class BuffUnlockUpgrade : RogueUpgradeDef
    {
        public BuffData BuffToUnlock;

        public override void Apply(TypeRogueBootstrap bootstrap)
        {
            if (bootstrap == null || BuffToUnlock == null) return;
            Debug.Log($"[Rogue] Unlocking Buff: {BuffToUnlock.BuffName}");
            bootstrap.UnlockBuff(BuffToUnlock);
        }
    }
}
