using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TypeRogue
{
    /// <summary>
    /// 输入栏视图：显示玩家当前的输入状态和反馈
    /// 功能：
    /// 1. 显示当前输入缓冲区内容，支持光标闪烁效果
    /// 2. 显示输入提示（Hint）和解析结果反馈（Resolve）
    /// 3. 处理解析结果文本的自动隐藏逻辑
    /// </summary>
    public sealed class TypingInputBarView : MonoBehaviour
    {
        [SerializeField] private TMP_Text bufferText;

        [SerializeField] private float caretBlinkIntervalSeconds = 0.5f;

        private string buffer = string.Empty;
        private float caretTimer;
        private bool caretVisible = true;

        public void Initialize(TMP_Text bufferText)
        {
            if (bufferText != null) this.bufferText = bufferText;
        }

        private void Update()
        {
            caretTimer += Time.unscaledDeltaTime;
            if (caretTimer >= caretBlinkIntervalSeconds)
            {
                caretTimer = 0f;
                caretVisible = !caretVisible;
                RenderBuffer();
            }
        }

        public void SetBuffer(string newBuffer)
        {
            buffer = newBuffer ?? string.Empty;
            RenderBuffer();
        }

        private void RenderBuffer()
        {
            if (bufferText == null) return;
            bufferText.text = caretVisible ? $"{buffer}|" : buffer;
        }
    }
}
