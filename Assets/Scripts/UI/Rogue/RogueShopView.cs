using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TypeRogue.Rogue;

namespace TypeRogue.UI.Rogue
{
    public class RogueShopView : MonoBehaviour
    {
        public event Action<int> OptionSelected;

        [SerializeField] private List<UpgradeCardView> cardViews;
        [SerializeField] private GameObject contentPanel;

        private void Awake()
        {
            if (contentPanel != null) contentPanel.SetActive(false);
        }

        public void Show(List<RogueUpgradeDef> upgrades)
        {
            Debug.Log("[RogueShopView] Showing Shop UI");

            // Ensure the GameObject hosting this script is active so Update() runs and UI is visible
            gameObject.SetActive(true);

            if (contentPanel != null)
            {
                contentPanel.SetActive(true);
                // Ensure it's drawn on top of other UI elements
                contentPanel.transform.SetAsLastSibling();
            }
            else
            {
                Debug.LogError("[RogueShopView] ContentPanel reference is MISSING! Please assign it in the Inspector.");
                return;
            }

            if (cardViews == null || cardViews.Count == 0)
            {
                Debug.LogError("[RogueShopView] CardViews list is EMPTY! Please assign UpgradeCardView objects in the Inspector.");
                return;
            }

            Debug.Log($"[RogueShopView] Configuring {upgrades.Count} cards.");

            for (int i = 0; i < cardViews.Count; i++)
            {
                if (i < upgrades.Count)
                {
                    cardViews[i].gameObject.SetActive(true);
                    cardViews[i].Setup(upgrades[i], i);
                    Debug.Log($"[RogueShopView] Card {i} set to: {upgrades[i].Title}");
                }
                else
                {
                    cardViews[i].gameObject.SetActive(false);
                }
            }
        }

        public void Hide()
        {
            Debug.Log("[RogueShopView] Hiding Shop UI");
            if (contentPanel != null) contentPanel.SetActive(false);
        }

        private void Update()
        {
            if (contentPanel != null && contentPanel.activeSelf)
            {
                var keyboard = Keyboard.current;
                if (keyboard == null) return;

                if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
                {
                    Debug.Log("[RogueShopView] Input Detected: 1");
                    OptionSelected?.Invoke(0);
                }
                if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
                {
                    Debug.Log("[RogueShopView] Input Detected: 2");
                    OptionSelected?.Invoke(1);
                }
                if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame)
                {
                    Debug.Log("[RogueShopView] Input Detected: 3");
                    OptionSelected?.Invoke(2);
                }
            }
        }
    }
}
