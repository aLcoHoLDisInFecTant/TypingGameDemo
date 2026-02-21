using NUnit.Framework;
using UnityEngine;

namespace TypeRogue.Tests
{
    public sealed class TypingCommandInterpreterTests
    {
        [Test]
        public void SubmitWord_PistolAliases_AreRecognized()
        {
            var go = new GameObject("Test");
            var interpreter = go.AddComponent<TypeRogue.TypingCommandInterpreter>();
            interpreter.Initialize(
                new[] { "pistol", "手枪" }, 
                new string[0], 
                new string[0], 
                new string[0]);

            var pistolResult = interpreter.SubmitWord("pistol");
            Assert.AreEqual(TypeRogue.TypingResolveResultType.WeaponPistol, pistolResult.Type);

            var aliasResult = interpreter.SubmitWord("手枪");
            Assert.AreEqual(TypeRogue.TypingResolveResultType.WeaponPistol, aliasResult.Type);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void SubmitWord_ShotgunAndRifle_AreRecognized()
        {
            var go = new GameObject("Test");
            var interpreter = go.AddComponent<TypeRogue.TypingCommandInterpreter>();
            interpreter.Initialize(
                new[] { "pistol" }, 
                new[] { "shotgun", "霰弹枪" }, 
                new[] { "rifle", "步枪" }, 
                new string[0]);

            var shotgunResult = interpreter.SubmitWord("shotgun");
            Assert.AreEqual(TypeRogue.TypingResolveResultType.WeaponShotgun, shotgunResult.Type);

            var rifleResult = interpreter.SubmitWord("rifle");
            Assert.AreEqual(TypeRogue.TypingResolveResultType.WeaponRifle, rifleResult.Type);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void SubmitWord_UnknownWord_ReturnsUnknown()
        {
            var go = new GameObject("Test");
            var interpreter = go.AddComponent<TypeRogue.TypingCommandInterpreter>();
            interpreter.Initialize(
                new[] { "pistol", "手枪" }, 
                new string[0], 
                new string[0], 
                new string[0]);

            var result = interpreter.SubmitWord("unknown");
            Assert.AreEqual(TypeRogue.TypingResolveResultType.UnknownWord, result.Type);

            Object.DestroyImmediate(go);
        }
    }
}
