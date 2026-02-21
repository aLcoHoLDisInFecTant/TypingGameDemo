using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// 简单的子弹控制器：处理直线飞行和碰撞检测
    /// 功能：
    /// 1. 沿指定方向（通常是上方）飞行
    /// 2. 碰到敌人时造成伤害并销毁自身
    /// 3. 超出屏幕范围自动销毁
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        // [SerializeField] private int damage = 1; // Removed to avoid confusion with WeaponData.BaseDamage
        [SerializeField] private float maxLifetime = 5f;
        [SerializeField] private int piercingCount = 0;

        private Vector2 direction;
        private int damage = 1; // Internal state
        private float lifetime;

        public void Initialize(Vector2 direction, int damage, float speedMultiplier = 1f, int piercingCount = 0)
        {
            this.direction = direction;
            this.damage = damage;
            this.speed *= speedMultiplier;
            this.piercingCount = piercingCount;

            // 根据飞行方向调整子弹朝向 (视觉效果)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private void Update()
        {
            // 使用 Space.World 确保沿世界坐标系的方向飞行
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            
            lifetime += Time.deltaTime;
            if (lifetime >= maxLifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damage);
                
                if (piercingCount > 0)
                {
                    piercingCount--;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
