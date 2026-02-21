using UnityEngine;

namespace TypeRogue.Rogue.Upgrades
{
    [CreateAssetMenu(fileName = "Upgrade_Health", menuName = "TypeRogue/Rogue/Health")]
    public class HealthUpgrade : RogueUpgradeDef
    {
        public int HealAmount;

        public override void Apply(TypeRogueBootstrap bootstrap)
        {
            if (bootstrap == null) return;
            Debug.Log($"[Rogue] Healing player by {HealAmount}");
            bootstrap.HealPlayer(HealAmount);
        }
    }
}
