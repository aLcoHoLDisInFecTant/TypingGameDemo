using TypeRogue.Data;

namespace TypeRogue.Combat.Buffs
{
    /// <summary>
    /// 武器射击上下文：包含本次射击的所有参数，可被 Buff 修改
    /// </summary>
    public class WeaponFireContext
    {
        public float SpreadAngle;
        public int ProjectileCount;
        public float SequentialInterval;
        public bool IsSequential;
        public float DamageMultiplier = 1f;
        public float SpeedMultiplier = 1f;
        public int BaseDamage = 1;
        public int PiercingCount = 0;

        public WeaponFireContext(WeaponData data)
        {
            if (data != null)
            {
                SpreadAngle = data.SpreadAngle;
                ProjectileCount = data.ProjectilesPerShot;
                SequentialInterval = data.SequentialInterval;
                IsSequential = data.IsSequential;
                BaseDamage = data.BaseDamage;
                PiercingCount = data.PiercingCount;
            }
        }
    }
}
