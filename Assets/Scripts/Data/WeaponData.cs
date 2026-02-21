using UnityEngine;

namespace TypeRogue.Data
{
    /// <summary>
    /// 武器数据配置：定义武器的静态属性
    /// 功能：
    /// 1. 存储武器名称、冷却时间、图标等配置
    /// 2. 可通过 Create -> TypeRogue -> WeaponData 创建
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "TypeRogue/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("武器显示的名称")]
        public string WeaponName = "Weapon";
        
        [Tooltip("武器图标（可选，用于UI显示）")]
        public Sprite Icon;

        [Header("Combat Stats")]
        [Tooltip("射击间隔（秒）")]
        public float FireInterval = 0.5f;
        
        [Tooltip("每次射击发射的子弹数量")]
        public int ProjectilesPerShot = 1;

        [Tooltip("子弹散射总角度（度）。如果是多发子弹，则均匀分布在此角度内；如果是单发，则表示随机偏移范围。")]
        public float SpreadAngle = 0f;

        [Tooltip("是否为轮流发射（连射模式）。如果为 false，则为齐射。")]
        public bool IsSequential = false;

        [Tooltip("连射模式下，每颗子弹的发射间隔（秒）。仅当 IsSequential 为 true 时有效。")]
        public float SequentialInterval = 0.1f;

        [Tooltip("子弹预制体")]
        public Projectile ProjectilePrefab;
    }
}
