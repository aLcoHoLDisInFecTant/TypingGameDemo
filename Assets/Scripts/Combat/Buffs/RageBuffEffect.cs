using UnityEngine;

namespace TypeRogue.Combat.Buffs
{
    /// <summary>
    /// 狂暴 Buff 效果：提高伤害
    /// </summary>
    [CreateAssetMenu(fileName = "BuffEffect_Rage", menuName = "TypeRogue/BuffEffects/Rage")]
    public class RageBuffEffect : BuffEffect
    {
        [Tooltip("伤害倍率")]
        public float DamageMultiplier = 2.0f;

        public override void Apply(WeaponFireContext context)
        {
            context.DamageMultiplier *= DamageMultiplier;
            Debug.Log($"[Buff] Applied Rage Effect: Damage x {DamageMultiplier}");
        }
    }
}
