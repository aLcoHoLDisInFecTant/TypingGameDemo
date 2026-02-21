using UnityEngine;
using TypeRogue;

namespace TypeRogue.Rogue
{
    public abstract class RogueUpgradeDef : ScriptableObject
    {
        public string Title;
        [TextArea] public string Description;
        public Sprite Icon;
        
        public enum RarityType { Common, Rare, Legendary }
        public RarityType Rarity = RarityType.Common;

        /// <summary>
        /// Apply this upgrade to the game state.
        /// </summary>
        public abstract void Apply(TypeRogueBootstrap bootstrap);
    }
}
