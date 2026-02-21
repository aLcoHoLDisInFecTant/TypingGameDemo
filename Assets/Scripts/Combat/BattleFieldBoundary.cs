using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// 战场边界控制器：销毁离开战场的物体
    /// 用法：
    /// 1. 挂载在代表战场的 Sprite 上
    /// 2. 确保该物体有 BoxCollider2D (IsTrigger = True)
    /// 3. 确保该物体有 Rigidbody2D (BodyType = Kinematic/Static) 以启用物理检测
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class BattleFieldBoundary : MonoBehaviour
    {
        private void OnTriggerExit2D(Collider2D other)
        {
            // 当物体（如子弹、敌人）完全离开碰撞体范围时触发
            
            // 检查是否是子弹
            if (other.TryGetComponent<Projectile>(out _))
            {
                Destroy(other.gameObject);
                // Debug.Log($"Projectile {other.name} left battlefield and was destroyed.");
            }
            // 检查是否是敌人
            else if (other.TryGetComponent<Enemy>(out _))
            {
                Destroy(other.gameObject);
                // Debug.Log($"Enemy {other.name} left battlefield and was destroyed.");
            }
        }
        
        // 可选：为了确保编辑器配置正确，在Reset时自动设置Collider为Trigger
        private void Reset()
        {
            var col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }
    }
}
