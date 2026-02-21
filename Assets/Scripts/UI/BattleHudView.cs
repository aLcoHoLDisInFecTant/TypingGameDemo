using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TypeRogue.UI;

namespace TypeRogue
{
    /// <summary>
    /// 战斗HUD视图：管理战斗界面的UI显示
    /// 功能：
    /// 1. 显示波次、玩家HP、当前武器名称
    /// 2. 可视化武器冷却状态（文本倒计时和填充条）
    /// 3. 提供Initialize方法进行UI组件的依赖注入
    /// </summary>
    public sealed class BattleHudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text waveText;
        [SerializeField] private TMP_Text playerHpText;
        [SerializeField] private TMP_Text lastTypingResultText;
        [SerializeField] private WeaponStatusPanelView weaponStatusPanelView;
        [SerializeField] private BuffStatusPanelView buffStatusPanelView;

        public void Initialize(int initialHp, System.Collections.Generic.IEnumerable<Data.BuffData> initialBuffs)
        {
            // 自动查找子组件作为后备
            if (weaponStatusPanelView == null) weaponStatusPanelView = GetComponentInChildren<WeaponStatusPanelView>();
            if (buffStatusPanelView == null) buffStatusPanelView = GetComponentInChildren<BuffStatusPanelView>();

            SetPlayerHp(initialHp);
            SetLastTypingResult(default); // Clear
            if (weaponStatusPanelView != null) weaponStatusPanelView.Initialize();
            if (buffStatusPanelView != null) buffStatusPanelView.Initialize(initialBuffs);
        }

        public void UpdateBuffState(System.Collections.Generic.List<BuffInstance> buffInstances)
        {
            if (buffStatusPanelView != null)
            {
                buffStatusPanelView.UpdateBuffState(buffInstances);
            }
        }

        // 兼容旧接口
        public void UpdateBuffs(System.Collections.Generic.IEnumerable<string> buffs)
        {
            // Ignored
        }

        public void SetWave(int waveIndex)
        {
            if (waveText != null) waveText.text = $"Wave {waveIndex}";
        }

        public void SetWaveText(string text)
        {
            if (waveText != null) waveText.text = text;
        }

        public void SetPlayerHp(int hp)
        {
            if (playerHpText != null) playerHpText.text = $"HP {hp}";
        }

        public void SetLastTypingResult(TypingResolveResult result)
        {
            if (lastTypingResultText == null) return;

            switch (result.Type)
            {
                case TypingResolveResultType.WeaponPistol:
                    lastTypingResultText.text = $"OK: {result.SubmittedWord}";
                    break;
                case TypingResolveResultType.UnknownWord:
                    lastTypingResultText.text = $"ERR: {result.SubmittedWord}";
                    break;
                default:
                    lastTypingResultText.text = string.Empty;
                    break;
            }
        }

        public void AddWeapon(string weaponName)
        {
            if (weaponStatusPanelView != null)
            {
                weaponStatusPanelView.AddWeapon(weaponName);
            }
        }
    }
}
