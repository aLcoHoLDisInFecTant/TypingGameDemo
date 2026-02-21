using UnityEngine;

namespace TypeRogue.Combat.Buffs
{
    /// <summary>
    /// 精准 Buff 效果：将散射角度设为 0
    /// </summary>
    [CreateAssetMenu(fileName = "BuffEffect_Precise", menuName = "TypeRogue/BuffEffects/Precise")]
    public class PreciseBuffEffect : BuffEffect
    {
        public override void Apply(WeaponFireContext context)
        {
            context.SpreadAngle = 0f;
            Debug.Log("[Buff] Applied Precise Effect: Spread set to 0");
        }
    }
}
