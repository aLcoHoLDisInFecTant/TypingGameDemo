using System.Collections.Generic;
using UnityEngine;
using TypeRogue.Data;
using System.Linq;
using TypeRogue.Combat.Buffs;

namespace TypeRogue
{
    public class BuffInstance
    {
        public BuffData Data { get; private set; }
        public float CurrentCooldown { get; set; }
        public bool IsPrimed { get; set; }

        public BuffInstance(BuffData data)
        {
            Data = data;
            CurrentCooldown = 0f;
            IsPrimed = false;
        }
    }

    /// <summary>
    /// 全局 Buff 控制器 (重构版)
    /// 职责：
    /// 1. 管理玩家拥有的 Buff (BuffInstance)
    /// 2. 处理 Buff 的冷却和激活状态 (Primed)
    /// 3. 与武器系统协同结算
    /// </summary>
    public class BuffController : MonoBehaviour
    {
        // 状态变更事件 (用于 UI 更新)
        // 参数：所有 Buff 的实例列表
        public event System.Action<List<BuffInstance>> BuffsChanged;

        private List<BuffInstance> availableBuffs = new List<BuffInstance>();

        /// <summary>
        /// 初始化 Buff 列表
        /// </summary>
        public void Initialize(IEnumerable<BuffData> startingBuffs)
        {
            availableBuffs.Clear();
            if (startingBuffs != null)
            {
                foreach (var data in startingBuffs)
                {
                    if (data != null)
                    {
                        availableBuffs.Add(new BuffInstance(data));
                    }
                }
            }
            NotifyChanged();
        }

        public void AddBuff(BuffData newBuff)
        {
            if (newBuff == null) return;
            
            // Avoid duplicates? Or allow stacking? Assuming unique for now based on TriggerWord
            if (availableBuffs.Any(b => b.Data == newBuff))
            {
                return;
            }

            availableBuffs.Add(new BuffInstance(newBuff));
            NotifyChanged();
        }

        private void Update()
        {
            bool anyChanged = false;
            float dt = Time.deltaTime;

            foreach (var buff in availableBuffs)
            {
                if (buff.CurrentCooldown > 0)
                {
                    buff.CurrentCooldown -= dt;
                    if (buff.CurrentCooldown < 0) buff.CurrentCooldown = 0;
                    anyChanged = true;
                }
            }

            if (anyChanged)
            {
                NotifyChanged();
            }
        }

        /// <summary>
        /// 尝试激活（预备）一个 Buff
        /// </summary>
        public bool TryPrimeBuff(string triggerWord)
        {
            if (string.IsNullOrEmpty(triggerWord)) return false;
            
            var normalizedWord = triggerWord.Trim().ToLowerInvariant();
            
            // 查找匹配的 Buff
            var buff = availableBuffs.FirstOrDefault(b => b.Data.TriggerWord.ToLowerInvariant() == normalizedWord);
            
            if (buff != null)
            {
                // 检查冷却和是否已预备
                if (buff.CurrentCooldown <= 0 && !buff.IsPrimed)
                {
                    buff.IsPrimed = true;
                    NotifyChanged();
                    return true;
                }
            }
            return false;
        }

        public List<BuffInstance> GetPrimedBuffs()
        {
            return availableBuffs.Where(b => b.IsPrimed).ToList();
        }

        public void CommitPrimedBuffs()
        {
            bool changed = false;
            foreach (var buff in availableBuffs)
            {
                if (buff.IsPrimed)
                {
                    buff.IsPrimed = false;
                    buff.CurrentCooldown = buff.Data.Cooldown;
                    changed = true;
                }
            }
            if (changed) NotifyChanged();
        }

        public void ApplyBuffs(WeaponFireContext context)
        {
            foreach (var buff in availableBuffs)
            {
                if (buff.IsPrimed && buff.Data.Effects != null)
                {
                    foreach (var effect in buff.Data.Effects)
                    {
                        if (effect != null)
                        {
                            effect.Apply(context);
                        }
                    }
                }
            }
        }
        
        public void AddBuff(string buffName)
        {
            TryPrimeBuff(buffName);
        }

        public bool HasBuff(string buffName)
        {
             return availableBuffs.Any(b => b.Data.BuffName.Equals(buffName, System.StringComparison.OrdinalIgnoreCase));
        }

        private void NotifyChanged()
        {
            BuffsChanged?.Invoke(availableBuffs);
        }
    }
}
