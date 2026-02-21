using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeRogue.Data;

namespace TypeRogue
{
    /// <summary>
    /// 敌人生成器：基于 WaveData 生成敌人
    /// 功能：
    /// 1. 按波次配置生成敌人
    /// 2. 支持多种敌人类型、数量、间隔
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public event System.Action WaveCleared;

        [Header("Wave Configuration")]
        [SerializeField] private List<WaveData> waves;
        [SerializeField] private bool autoStart = false; // 由 Bootstrap 控制启动

        [Header("Spawn Area")]
        [SerializeField] private SpriteRenderer spawnArea; // 引用场景中的方形 Sprite

        private Coroutine currentWaveCoroutine;
        private int activeEnemyCount = 0;
        private bool isWaveSpawning = false;

        private void Start()
        {
            if (autoStart && waves != null && waves.Count > 0)
            {
                StartWave(0);
            }
        }

        public void StartWave(int waveIndex)
        {
            if (waves == null || waveIndex < 0 || waveIndex >= waves.Count)
            {
                Debug.LogWarning($"[EnemySpawner] Invalid wave index: {waveIndex}");
                return;
            }

            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
            }
            
            Debug.Log($"[EnemySpawner] Starting Wave {waveIndex}");
            activeEnemyCount = 0;
            isWaveSpawning = true;
            currentWaveCoroutine = StartCoroutine(ProcessWaveRoutine(waves[waveIndex]));
        }

        public void StopSpawning()
        {
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
                currentWaveCoroutine = null;
            }
            isWaveSpawning = false;
        }

        private IEnumerator ProcessWaveRoutine(WaveData waveData)
        {
            if (waveData == null || waveData.Groups == null) yield break;

            foreach (var group in waveData.Groups)
            {
                if (group == null) continue;

                // 组前延迟
                if (group.PreDelay > 0)
                {
                    yield return new WaitForSeconds(group.PreDelay);
                }

                // 生成该组敌人
                for (int i = 0; i < group.Count; i++)
                {
                    SpawnEnemy(group.EnemyType);

                    // 组内生成间隔（除了最后一个）
                    if (group.SpawnInterval > 0 && i < group.Count - 1)
                    {
                        yield return new WaitForSeconds(group.SpawnInterval);
                    }
                }

                // 组后延迟
                if (group.PostDelay > 0)
                {
                    yield return new WaitForSeconds(group.PostDelay);
                }
            }
            
            Debug.Log("[EnemySpawner] Wave Spawning Completed");
            currentWaveCoroutine = null;
            isWaveSpawning = false;
            
            CheckWaveClear();
        }

        private void SpawnEnemy(EnemyData enemyData)
        {
            if (spawnArea == null) return;
            if (enemyData == null || enemyData.Prefab == null)
            {
                Debug.LogWarning("[EnemySpawner] Missing EnemyData or Prefab");
                return;
            }

            // 获取 Sprite 的世界坐标包围盒
            Bounds bounds = spawnArea.bounds;

            // 在包围盒内随机取点
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            
            // Z轴通常为0，或者保持与SpawnArea一致
            Vector3 spawnPos = new Vector3(randomX, randomY, spawnArea.transform.position.z);
            
            var enemy = Instantiate(enemyData.Prefab, spawnPos, Quaternion.identity);
            enemy.Initialize(enemyData);
            
            activeEnemyCount++;
            enemy.Died += OnEnemyDied;
        }

        private void OnEnemyDied(Enemy enemy)
        {
            enemy.Died -= OnEnemyDied;
            activeEnemyCount--;
            Debug.Log($"[EnemySpawner] Enemy Died. Remaining active enemies: {activeEnemyCount}");
            CheckWaveClear();
        }

        private void CheckWaveClear()
        {
            if (!isWaveSpawning && activeEnemyCount <= 0)
            {
                Debug.Log("==========================================");
                Debug.Log($"[EnemySpawner] >>> WAVE CLEARED! <<<");
                Debug.Log("==========================================");
                WaveCleared?.Invoke();
            }
        }
    }
}
