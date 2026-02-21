using UnityEngine;
using System.Collections.Generic;
using TypeRogue.Combat.Buffs;

namespace TypeRogue.Data
{
    [CreateAssetMenu(fileName = "NewBuffData", menuName = "TypeRogue/BuffData")]
    public class BuffData : ScriptableObject
    {
        [Header("Basic Info")]
        public string BuffName = "Buff";
        public string TriggerWord = "buff";
        public Sprite Icon;
        [TextArea] public string Description;

        [Header("Stats")]
        public float Cooldown = 2.0f;

        [Header("Effects")]
        public List<BuffEffect> Effects = new List<BuffEffect>();
    }
}