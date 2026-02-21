using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TypeRogue
{
    /// <summary>
    /// 输入路由控制器：负责捕获原始键盘输入并维护输入缓冲区
    /// 功能：
    /// 1. 监听Unity Input System的键盘输入事件
    /// 2. 处理字符输入、退格键和回车/空格键提交
    /// 3. 提供BufferChanged和WordSubmitted事件供外部订阅
    /// </summary>
    public sealed class TypingInputRouter : MonoBehaviour
    {
        public event Action<string> BufferChanged;
        public event Action<string> WordSubmitted;

        [SerializeField] private int maxBufferLength = 64;

        private readonly StringBuilder buffer = new StringBuilder(64);

        // ... existing code ...

        private void OnEnable()
        {
            // Subscribe to Keyboard input events
            if (Keyboard.current != null)
            {
                Keyboard.current.onTextInput += OnTextInput;
            }
        }

        private void OnDisable()
        {
            if (Keyboard.current != null)
            {
                Keyboard.current.onTextInput -= OnTextInput;
            }
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // 监听回车和空格
            if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
            {
                // 1. 先提交当前单词
                SubmitCurrentWord();

                // 2. 如果是空格，且缓冲区不为空，则追加空格以分隔单词
                // 注意：如果 SubmitCurrentWord 导致了 Buffer 被清空（例如触发了武器），那么这里的 buffer.Length 已经是 0，不会追加空格，这是正确的。
                if (keyboard.spaceKey.wasPressedThisFrame && buffer.Length > 0 && buffer[buffer.Length - 1] != ' ')
                {
                    buffer.Append(' ');
                    BufferChanged?.Invoke(buffer.ToString());
                }
            }

            // 监听退格 (Backspace handled via OnTextInput or specific key check? Usually specific key check)
            // InputSystem onTextInput does not capture Backspace usually.
            if (keyboard.backspaceKey.wasPressedThisFrame)
            {
                if (buffer.Length > 0)
                {
                    buffer.Length--; // Efficient removal
                    string currentBuffer = buffer.ToString();
                    // Debug.Log($"[Input] Backspace. Buffer: {currentBuffer}");
                    BufferChanged?.Invoke(currentBuffer);
                }
            }
        }

        public void ClearBuffer()
        {
            buffer.Clear();
            BufferChanged?.Invoke(buffer.ToString());
        }

        public void AppendCommittedSpace()
        {
            if (buffer.Length >= maxBufferLength) return;
            buffer.Append(' ');
            BufferChanged?.Invoke(buffer.ToString());
        }

        public void RemoveCurrentWord()
        {
            int lastSpaceIndex = buffer.ToString().LastIndexOf(' ');
            int wordStart = lastSpaceIndex < 0 ? 0 : lastSpaceIndex + 1;

            if (wordStart < buffer.Length)
            {
                buffer.Remove(wordStart, buffer.Length - wordStart);
                BufferChanged?.Invoke(buffer.ToString());
            }
        }

        private void OnTextInput(char character)
        {
            // Debug.Log($"[Input] Raw char input: {(int)character}"); // Optional: debug raw ascii
            if (character == ' ' || character == '\n' || character == '\r' || character == '\t') return;
            if (char.IsControl(character)) return;
            if (buffer.Length >= maxBufferLength) return;

            buffer.Append(character);
            string currentBuffer = buffer.ToString();
            Debug.Log($"[Input] Buffer update: {currentBuffer}");
            BufferChanged?.Invoke(currentBuffer);
        }

        private void SubmitCurrentWord()
        {
            // 提交当前单词（以空格或回车触发）
            // 简单逻辑：取最后一个空格后的内容
            string content = buffer.ToString();
            int lastSpace = content.LastIndexOf(' ');
            string word = lastSpace < 0 ? content : content.Substring(lastSpace + 1);

            if (!string.IsNullOrEmpty(word))
            {
                Debug.Log($"[Input] Word submitted: {word}");
                WordSubmitted?.Invoke(word);
            }
        }
    }
}
