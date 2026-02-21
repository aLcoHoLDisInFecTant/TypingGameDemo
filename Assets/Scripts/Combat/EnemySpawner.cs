using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// 敌人生成器：基于 SpawnArea Sprite 范围生成敌人
    /// 功能：
    /// 1. 在指定的 SpawnArea 范围内随机生成敌人
    /// 2. 控制生成间隔
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private float spawnInterval = 2f;
        
        [Header("Spawn Area")]
        [SerializeField] private SpriteRenderer spawnArea; // 引用场景中的方形 Sprite

        private float spawnTimer;

        private void Update()
        {
            if (spawnArea == null) return;

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            if (enemyPrefab == null || spawnArea == null) return;

            // 获取 Sprite 的世界坐标包围盒
            Bounds bounds = spawnArea.bounds;

            // 在包围盒内随机取点
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            
            // Z轴通常为0，或者保持与SpawnArea一致
            Vector3 spawnPos = new Vector3(randomX, randomY, spawnArea.transform.position.z);
            
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
