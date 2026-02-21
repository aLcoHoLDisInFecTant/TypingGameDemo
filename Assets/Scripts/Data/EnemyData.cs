using UnityEngine;

namespace TypeRogue.Data
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "TypeRogue/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public string EnemyName = "Enemy";
        public Enemy Prefab;
        public int MaxHp = 3;
        public float MoveSpeed = 2f;
        public int DamageToPlayer = 10;
    }
}
