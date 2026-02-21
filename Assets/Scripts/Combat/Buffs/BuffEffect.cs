using UnityEngine;

namespace TypeRogue.Combat.Buffs
{
    /// <summary>
    /// Buff 效果基类 (ScriptableObject)
    /// </summary>
    public abstract class BuffEffect : ScriptableObject
    {
        /// <summary>
        /// 应用 Buff 效果到武器射击上下文
        /// </summary>
        /// <param name="context">射击上下文</param>
        public abstract void Apply(WeaponFireContext context);
    }
}
