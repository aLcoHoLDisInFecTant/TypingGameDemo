using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TypeRogue
{
    /// <summary>
    /// 武器状态列表项视图：显示单个武器的名称和冷却状态
    /// 功能：
    /// 1. 显示武器名称
    /// 2. 通过 Image 的 fillAmount 显示冷却进度
    /// </summary>
    public sealed class WeaponStatusItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text weaponNameText;
        [SerializeField] private Image cooldownOverlayImage; // 覆盖在上面的半透明遮罩，或者作为背景的进度条

        // 颜色配置（可选，也可以直接在编辑器里调好颜色）
        private Color activeColor = new Color(1f, 1f, 1f, 1f);
        private Color cooldownColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

        public void Initialize(string weaponName)
        {
            if (weaponNameText != null) weaponNameText.text = weaponName;
            // 初始化时重置为就绪状态
            SetCooldown(0, 0);
        }

        public void SetCooldown(float remainingSeconds, float durationSeconds)
        {
            if (durationSeconds <= 0f) durationSeconds = 0.0001f;
            float t = Mathf.Clamp01(remainingSeconds / durationSeconds);

            // 假设 cooldownOverlayImage 是一个半透明黑色遮罩
            // t=1 (刚发射) -> fill=1 (全黑)
            // t=0 (就绪) -> fill=0 (全亮)
            if (cooldownOverlayImage != null)
            {
                cooldownOverlayImage.fillAmount = t;
            }
        }
    }
}
