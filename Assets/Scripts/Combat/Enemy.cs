using System;
using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// 敌人控制器：处理移动、生命值和死亡逻辑
    /// 功能：
    /// 1. 向下移动（模拟进攻）
    /// 2. 接收伤害并判断死亡
    /// 3. 碰到玩家防线（底部）时造成伤害
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        public event Action<Enemy> Died;

        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int maxHp = 3;
        [SerializeField] private float damageToPlayer = 10f; // 暂未使用，预留给攻击逻辑

        public Vector3 Velocity => Vector3.down * moveSpeed;

        private int currentHp;

        private void Awake()
        {
            currentHp = maxHp;
        }

        private void Update()
        {
            // 向下移动
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Debug.Log($"[Enemy] Trigger Enter: {other.gameObject.name} (Layer: {other.gameObject.layer})");
            
            // 检查是否撞到了玩家
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                Debug.Log($"[Enemy] Hit Player! Dealing {damageToPlayer} damage.");
                player.TakeDamage((int)damageToPlayer);
                Die(); // 撞击后自毁
            }
        }

        public void TakeDamage(int amount)
        {
            currentHp -= amount;
            if (currentHp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Died?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
