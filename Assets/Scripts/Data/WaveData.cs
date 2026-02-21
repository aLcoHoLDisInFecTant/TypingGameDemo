using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypeRogue.Data
{
    [Serializable]
    public class WaveGroup
    {
        [Tooltip("Enemy Type to spawn")]
        public EnemyData EnemyType;

        [Tooltip("Number of enemies to spawn in this group")]
        public int Count = 1;

        [Tooltip("Time interval between each spawn in this group. Set to 0 for simultaneous spawning.")]
        public float SpawnInterval = 1f;

        [Tooltip("Delay before this group starts spawning")]
        public float PreDelay = 0f;

        [Tooltip("Delay after this group finishes spawning, before the next group starts")]
        public float PostDelay = 0f;
    }

    [CreateAssetMenu(fileName = "NewWaveData", menuName = "TypeRogue/WaveData")]
    public class WaveData : ScriptableObject
    {
        [Tooltip("List of enemy groups to spawn sequentially")]
        public List<WaveGroup> Groups = new List<WaveGroup>();
    }
}
