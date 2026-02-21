using System;
using System.Collections.Generic;
using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// 拼写命令解释器：将输入的字符串解析为游戏指令
    /// 功能：
    /// 1. 维护指令词典（如"pistol"对应的别名集合）
    /// 2. 解析输入词并返回TypingResolveResult（武器触发或未知指令）
    /// 3. 触发PistolFireRequested等具体游戏行为事件
    /// </summary>
    public sealed class TypingCommandInterpreter : MonoBehaviour
    {
        public event Action<TypingResolveResult> Resolved;
        public event Action<bool> PistolFireRequested; // bool = isPrecise (Legacy, always false now)
        // public event Action<IEnumerable<string>> ModifiersChanged; // Legacy, removed to fix warning

        private readonly HashSet<string> pistolWords = new HashSet<string>();
        private readonly HashSet<string> shotgunWords = new HashSet<string>();
        private readonly HashSet<string> rifleWords = new HashSet<string>();
        private readonly HashSet<string> buffWords = new HashSet<string>();
        
        public void Initialize(
            IEnumerable<string> pistolAliases, 
            IEnumerable<string> shotgunAliases,
            IEnumerable<string> rifleAliases,
            IEnumerable<string> buffTriggerWords)
        {
            InitializeSet(pistolWords, pistolAliases);
            InitializeSet(shotgunWords, shotgunAliases);
            InitializeSet(rifleWords, rifleAliases);
            InitializeSet(buffWords, buffTriggerWords);
        }

        private void InitializeSet(HashSet<string> set, IEnumerable<string> words)
        {
            set.Clear();
            if (words != null)
            {
                foreach (var word in words)
                {
                    if (string.IsNullOrWhiteSpace(word)) continue;
                    set.Add(Normalize(word));
                }
            }
        }

        public void RegisterAlias(TypingResolveResultType type, string alias)
        {
            if (string.IsNullOrWhiteSpace(alias)) return;
            string normalized = Normalize(alias);

            switch (type)
            {
                case TypingResolveResultType.WeaponPistol:
                    pistolWords.Add(normalized);
                    break;
                case TypingResolveResultType.WeaponShotgun:
                    shotgunWords.Add(normalized);
                    break;
                case TypingResolveResultType.WeaponRifle:
                    rifleWords.Add(normalized);
                    break;
                case TypingResolveResultType.ModifierAccepted:
                    buffWords.Add(normalized);
                    break;
                default:
                    Debug.LogWarning($"[TypingCommandInterpreter] Cannot register alias '{alias}' for type {type}");
                    break;
            }
        }

        public TypingResolveResult SubmitWord(string input)
        {
            string normalized = Normalize(input);

            // 1. 检查是否为修饰词 (Buff)
            if (buffWords.Contains(normalized))
            {
                var result = new TypingResolveResult(TypingResolveResultType.ModifierAccepted, input);
                Resolved?.Invoke(result);
                return result;
            }
            
            // 2. 检查是否为动作词 (Weapon)
            if (pistolWords.Contains(normalized))
            {
                bool isPrecise = false; 
                var result = new TypingResolveResult(TypingResolveResultType.WeaponPistol, input, isPrecise);
                Resolved?.Invoke(result);
                PistolFireRequested?.Invoke(isPrecise);
                return result;
            }

            if (shotgunWords.Contains(normalized))
            {
                var result = new TypingResolveResult(TypingResolveResultType.WeaponShotgun, input);
                Resolved?.Invoke(result);
                return result;
            }

            if (rifleWords.Contains(normalized))
            {
                var result = new TypingResolveResult(TypingResolveResultType.WeaponRifle, input);
                Resolved?.Invoke(result);
                return result;
            }

            // 3. 未知词
            var unknownResult = new TypingResolveResult(TypingResolveResultType.UnknownWord, input, false);
            Resolved?.Invoke(unknownResult);
            return unknownResult;
        }

        private static string Normalize(string value)
        {
            return value.Trim().ToLowerInvariant();
        }
    }
}
