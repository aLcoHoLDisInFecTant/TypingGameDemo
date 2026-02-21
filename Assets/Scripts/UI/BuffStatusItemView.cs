using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TypeRogue.UI
{
    public class BuffStatusItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text buffNameText;
        [SerializeField] private Image cooldownOverlayImage;
        [SerializeField] private Image borderImage; // 用于显示 "Primed" 状态的高亮边框

        private Color normalColor = Color.white;
        private Color primedColor = Color.yellow; // 预备状态颜色

        public void Initialize(string buffName)
        {
            if (buffNameText != null)
            {
                buffNameText.text = buffName.ToUpper();
            }
            SetState(0, 0, false);
        }

        public void SetState(float currentCooldown, float maxCooldown, bool isPrimed)
        {
            // 冷却显示
            if (cooldownOverlayImage != null)
            {
                float t = 0f;
                if (maxCooldown > 0)
                {
                    t = Mathf.Clamp01(currentCooldown / maxCooldown);
                }
                cooldownOverlayImage.fillAmount = t;
            }

            // 预备状态显示
            if (borderImage != null)
            {
                borderImage.color = isPrimed ? primedColor : normalColor;
                borderImage.enabled = isPrimed; // 或者一直显示但在 Primed 时变色
            }
            
            // 也可以改变文字颜色
            if (buffNameText != null)
            {
                buffNameText.color = isPrimed ? primedColor : normalColor;
            }
        }
    }
}