using System.Collections.Generic;
using UnityEngine;
using TypeRogue.Rogue.Upgrades;
using TypeRogue.UI.Rogue; // Added namespace for RogueShopView

namespace TypeRogue.Rogue
{
    public class RogueManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private List<RogueUpgradeDef> upgradePool;
        [SerializeField] private int optionsCount = 3;

        [Header("References")]
        [SerializeField] private TypeRogueBootstrap bootstrap;
        [SerializeField] private RogueShopView shopView;

        private List<RogueUpgradeDef> currentOptions = new List<RogueUpgradeDef>();
        private bool isShopOpen = false;

        public void Initialize(TypeRogueBootstrap bootstrap, RogueShopView shopView)
        {
            this.bootstrap = bootstrap;
            this.shopView = shopView;
            
            if (this.shopView != null)
            {
                this.shopView.OptionSelected += OnOptionSelected;
                this.shopView.Hide();
            }
        }

        public void OpenShop()
        {
            if (upgradePool == null || upgradePool.Count == 0)
            {
                Debug.LogWarning("[RogueManager] Upgrade pool is empty!");
                CloseShop();
                return;
            }

            Debug.Log("[RogueManager] Opening Shop...");
            GenerateOptions();
            
            Debug.Log($"[RogueManager] Generated {currentOptions.Count} options:");
            for (int i = 0; i < currentOptions.Count; i++)
            {
                Debug.Log($"  Option {i + 1}: {currentOptions[i].Title} - {currentOptions[i].Description}");
            }

            shopView.Show(currentOptions);
            isShopOpen = true;
            
            // Pause Game logic if needed (e.g., stop time or input)
            // Time.timeScale = 0; // Optional
        }

        private void GenerateOptions()
        {
            currentOptions.Clear();
            List<RogueUpgradeDef> tempPool = new List<RogueUpgradeDef>(upgradePool);

            for (int i = 0; i < optionsCount; i++)
            {
                if (tempPool.Count == 0) break;

                int index = Random.Range(0, tempPool.Count);
                currentOptions.Add(tempPool[index]);
                tempPool.RemoveAt(index); // Prevent duplicates in one roll
            }
        }

        private void OnOptionSelected(int index)
        {
            if (!isShopOpen) return;
            Debug.Log($"[RogueManager] Player selected option index: {index}");

            if (index < 0 || index >= currentOptions.Count)
            {
                Debug.LogWarning($"[RogueManager] Invalid selection index: {index}. Valid range: 0-{currentOptions.Count - 1}");
                return;
            }

            var selectedUpgrade = currentOptions[index];
            Debug.Log($"[RogueManager] Selected Upgrade: {selectedUpgrade.Title}");
            ApplyUpgrade(selectedUpgrade);
            
            CloseShop();
        }

        private void ApplyUpgrade(RogueUpgradeDef upgrade)
        {
            if (upgrade != null)
            {
                Debug.Log($"[RogueManager] Applying upgrade: {upgrade.Title}");
                upgrade.Apply(bootstrap);
            }
        }

        private void CloseShop()
        {
            isShopOpen = false;
            shopView.Hide();
            
            // Notify Bootstrap to continue to next wave
            bootstrap.OnShopClosed();
        }

        private void OnDestroy()
        {
            if (shopView != null)
            {
                shopView.OptionSelected -= OnOptionSelected;
            }
        }
    }
}
