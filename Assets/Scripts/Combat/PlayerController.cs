using System;
using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// 玩家控制器：管理玩家状态和受击逻辑
    /// 功能：
    /// 1. 存储玩家HP
    /// 2. 提供受击接口
    /// 3. 管理枪口位置（作为子对象）
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public event Action<int> HpChanged; // HP 变更事件

        [SerializeField] private int maxHp = 100;
        [SerializeField] private Transform muzzlePoint;

        private int currentHp;

        public Transform MuzzlePoint => muzzlePoint;
        public int CurrentHp => currentHp;

        private void Awake()
        {
            currentHp = maxHp;
            if (muzzlePoint == null)
            {
                muzzlePoint = transform; // 默认使用自身位置
            }
        }

        public void Heal(int amount)
        {
            currentHp = Mathf.Min(maxHp, currentHp + amount);
            Debug.Log($"[Player] Healed {amount} HP. Current HP: {currentHp}");
            HpChanged?.Invoke(currentHp);
        }

        public void TakeDamage(int amount)
        {
            currentHp = Mathf.Max(0, currentHp - amount);
            Debug.Log($"[Player] Took {amount} damage. Current HP: {currentHp}");
            
            HpChanged?.Invoke(currentHp);

            if (currentHp <= 0)
            {
                Debug.Log("[Player] Game Over (HP <= 0)");
            }
        }
    }
}
