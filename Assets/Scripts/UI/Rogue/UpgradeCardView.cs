using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TypeRogue.UI.Rogue
{
    public class UpgradeCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI indexText; // Displays "1", "2", "3"

        public void Setup(TypeRogue.Rogue.RogueUpgradeDef upgrade, int index)
        {
            if (upgrade == null) return;

            if (titleText != null) titleText.text = upgrade.Title;
            if (descriptionText != null) descriptionText.text = upgrade.Description;
            if (iconImage != null && upgrade.Icon != null) iconImage.sprite = upgrade.Icon;
            if (indexText != null) indexText.text = (index + 1).ToString();
        }
    }
}
