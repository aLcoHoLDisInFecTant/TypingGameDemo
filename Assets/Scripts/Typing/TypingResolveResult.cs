// TypeRogue - TypingResolveResult
// 输入判定结果：把一次“提交词”的解析结果以结构体返回，并用枚举标识类型。

namespace TypeRogue
{
    public readonly struct TypingResolveResult
    {
        public TypingResolveResultType Type { get; }
        public string SubmittedWord { get; }
        public bool IsPrecise { get; }

        public TypingResolveResult(TypingResolveResultType type, string submittedWord, bool isPrecise = false)
        {
            Type = type;
            SubmittedWord = submittedWord;
            IsPrecise = isPrecise;
        }
    }

    public enum TypingResolveResultType
    {
        None = 0,
        WeaponPistol = 1,
        WeaponShotgun = 4,
        WeaponRifle = 5,
        ModifierAccepted = 2,
        UnknownWord = 3
    }
}
