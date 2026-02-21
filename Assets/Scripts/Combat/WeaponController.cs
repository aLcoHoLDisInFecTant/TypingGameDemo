using System;
using UnityEngine;
using System.Collections;
using TypeRogue.Data;
using TypeRogue.Combat.Buffs;

namespace TypeRogue
{
    /// <summary>
    /// 基础武器控制器：处理基于 WeaponData 的武器发射逻辑
    /// </summary>
    public sealed class WeaponController : MonoBehaviour
    {
        public event Action Fired;

        [Header("Configuration")]
        [SerializeField] private WeaponData weaponData; // 可以在 Inspector 中预设，也可以通过 Initialize 注入
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private float projectileSpeed = 10f; // 用于预判计算，应与 Projectile Prefab 保持一致

        // 依赖引用
        private BuffController buffController;

        // 运行时状态
        private float lastFireTime = -999f;
        private bool isFiringSequential = false; // 是否正在进行连射
        private int preciseShotsRemaining = 0; // 剩余的精准射击次数 (Legacy)
        
        // 属性访问器
        public string WeaponName => weaponData != null ? weaponData.WeaponName : "Unknown";
        public float FireIntervalSeconds => weaponData != null ? weaponData.FireInterval : 0.5f;

        public void Initialize(Transform muzzle, WeaponData data = null, BuffController buffController = null)
        {
            this.muzzleTransform = muzzle;
            if (data != null)
            {
                this.weaponData = data;
            }
            this.buffController = buffController;
        }

        /// <summary>
        /// 激活精准Buff，使下一次射击无散射
        /// </summary>
        public void ApplyPreciseBuff(int shotCount = 1)
        {
            preciseShotsRemaining += shotCount;
            Debug.Log($"[Weapon] Precise Buff applied! Remaining precise shots: {preciseShotsRemaining}");
        }

        public bool TryFire()
        {
            if (!CanFire())
            {
                return false;
            }

            lastFireTime = Time.time;
            Fired?.Invoke();
            
            // 1. 创建射击上下文
            var context = new WeaponFireContext(weaponData);
            
            // 2. 应用 Buff
            if (buffController != null)
            {
                buffController.ApplyBuffs(context);
            }
            else
            {
                // 兼容旧逻辑
                if (preciseShotsRemaining > 0)
                {
                    context.SpreadAngle = 0f;
                    preciseShotsRemaining--;
                }
            }

            // 3. 发射逻辑
            if (weaponData != null && weaponData.ProjectilePrefab != null && muzzleTransform != null)
            {
                if (context.IsSequential && context.ProjectileCount > 1)
                {
                    StartCoroutine(FireSequentialCoroutine(context));
                }
                else
                {
                    FireSimultaneous(context);
                }
            }
            
            // 4. 结算 Buff (进入冷却)
            if (buffController != null)
            {
                buffController.CommitPrimedBuffs();
            }

            return true;
        }

        private void FireSimultaneous(WeaponFireContext context)
        {
            int projectileCount = Mathf.Max(1, context.ProjectileCount);
            float totalSpread = context.SpreadAngle;
            int damage = Mathf.CeilToInt(context.BaseDamage * context.DamageMultiplier);

            // 1. 计算基础目标方向（预判后）
            // 注意：齐射时，所有子弹基于同一个时刻的预判
            Vector2 baseDirection = GetNearestEnemyDirection();

            for (int i = 0; i < projectileCount; i++)
            {
                SpawnProjectile(baseDirection, totalSpread, damage, context.SpeedMultiplier, context.PiercingCount);
            }
        }

        private IEnumerator FireSequentialCoroutine(WeaponFireContext context)
        {
            isFiringSequential = true;
            int projectileCount = Mathf.Max(1, context.ProjectileCount);
            float totalSpread = context.SpreadAngle;
            float interval = Mathf.Max(0.01f, context.SequentialInterval);
            int damage = Mathf.CeilToInt(context.BaseDamage * context.DamageMultiplier);

            // 1. 锁定当前最近的敌人
            Enemy lockedTarget = GetNearestEnemy();
            Vector3 lastKnownTargetPos = Vector3.zero;
            bool hasLockedTarget = lockedTarget != null;
            if (hasLockedTarget)
            {
                lastKnownTargetPos = lockedTarget.transform.position;
            }

            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 fireDirection;

                if (hasLockedTarget)
                {
                    if (lockedTarget != null)
                    {
                        lastKnownTargetPos = lockedTarget.transform.position;
                        fireDirection = CalculatePredictiveAim(lockedTarget.transform.position, Vector3.zero, muzzleTransform.position, projectileSpeed);
                    }
                    else
                    {
                        // 目标已死亡，向其最后已知位置发射
                        fireDirection = (lastKnownTargetPos - muzzleTransform.position).normalized;
                    }
                }
                else
                {
                    fireDirection = Vector2.up;
                }

                SpawnProjectile(fireDirection, totalSpread, damage, context.SpeedMultiplier, context.PiercingCount);

                if (i < projectileCount - 1)
                {
                    yield return new WaitForSeconds(interval);
                }
            }
            isFiringSequential = false;
        }

        private void SpawnProjectile(Vector2 baseDirection, float totalSpread, int damage, float speedMultiplier, int piercingCount)
        {
            // 计算随机角度偏移 (-Spread/2 到 +Spread/2)
            float randomAngle = UnityEngine.Random.Range(-totalSpread / 2f, totalSpread / 2f);
            
            // 应用角度偏移
            Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * baseDirection;
            
            // 计算子弹自身的旋转角度 (用于Sprite朝向)
            float spriteAngle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0, 0, spriteAngle);

            if (weaponData.ProjectilePrefab != null)
            {
                var projectile = Instantiate(weaponData.ProjectilePrefab, muzzleTransform.position, rotation);
                projectile.Initialize(finalDirection, damage, speedMultiplier, piercingCount);
            }
        }

        private Vector2 CalculatePredictiveAim(Vector3 targetPos, Vector3 targetVelocity, Vector3 shooterPos, float projectileSpeed)
        {
            float distance = Vector3.Distance(shooterPos, targetPos);
            float timeToHit = distance / projectileSpeed;
            Vector3 predictedPos = targetPos + (targetVelocity * timeToHit);
            return (predictedPos - shooterPos).normalized;
        }

        private Enemy GetNearestEnemy()
        {
             // 查找场景中所有敌人 (性能优化提示：如果敌人很多，应维护一个静态列表)
            var enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            
            Enemy nearestEnemy = null;
            float minDistanceSqr = float.MaxValue;
            Vector3 currentPos = muzzleTransform.position;

            foreach (var enemy in enemies)
            {
                float distSqr = (enemy.transform.position - currentPos).sqrMagnitude;
                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    nearestEnemy = enemy;
                }
            }
            return nearestEnemy;
        }

        // 旧方法保留但不再使用内部逻辑查找，改为调用新的 GetNearestEnemy
        private Vector2 GetNearestEnemyDirection()
        {
            var enemy = GetNearestEnemy();
            if (enemy != null)
            {
                return CalculatePredictiveAim(enemy.transform.position, Vector3.zero, muzzleTransform.position, projectileSpeed);
            }
            return Vector2.up;
        }

        public bool CanFire()
        {
            // 如果正在连射中，不允许再次开火
            if (isFiringSequential) return false;
            return GetCooldownRemainingSeconds() <= 0f;
        }

        public float GetCooldownRemainingSeconds()
        {
            float elapsed = Time.time - lastFireTime;
            return Mathf.Max(0f, FireIntervalSeconds - elapsed);
        }
    }
}
