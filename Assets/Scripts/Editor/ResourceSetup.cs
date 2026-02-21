using UnityEngine;
using UnityEditor;
using TypeRogue.Data;
using TypeRogue.Combat.Buffs;
using System.Collections.Generic;
using TypeRogue; // For EnemySpawner, WeaponController, etc.
using System.Reflection; // For FieldInfo

using TypeRogue.Rogue.Upgrades; // Added

public class ResourceSetup
{
    [MenuItem("TypeRogue/Setup Resources")]
    public static void Setup()
    {
        // 1. Ensure directories exist
        EnsureFolder("Assets/Data");
        EnsureFolder("Assets/Data/Enemies");
        EnsureFolder("Assets/Data/Waves");
        EnsureFolder("Assets/Data/Weapons");
        EnsureFolder("Assets/Data/Buffs");
        EnsureFolder("Assets/Data/Upgrades"); // New folder for upgrades

        // 2. Load Prefabs
        var enemyPrefab = AssetDatabase.LoadAssetAtPath<Enemy>("Assets/Resource/Prefabs/Enemy.prefab");
        var bulletPrefab = AssetDatabase.LoadAssetAtPath<Projectile>("Assets/Resource/Prefabs/Bullet.prefab");

        if (enemyPrefab == null) { Debug.LogError("Enemy prefab not found at Assets/Resource/Prefabs/Enemy.prefab"); return; }
        if (bulletPrefab == null) { Debug.LogError("Bullet prefab not found at Assets/Resource/Prefabs/Bullet.prefab"); return; }

        // 3. Create EnemyData
        var enemyBasic = CreateAsset<EnemyData>("Assets/Data/Enemies/Enemy_Basic.asset");
        enemyBasic.EnemyName = "Basic";
        enemyBasic.Prefab = enemyPrefab;
        enemyBasic.MaxHp = 3;
        enemyBasic.MoveSpeed = 2f;
        enemyBasic.DamageToPlayer = 10;
        EditorUtility.SetDirty(enemyBasic);

        var enemyFast = CreateAsset<EnemyData>("Assets/Data/Enemies/Enemy_Fast.asset");
        enemyFast.EnemyName = "Fast";
        enemyFast.Prefab = enemyPrefab; // Using same prefab for now
        enemyFast.MaxHp = 2;
        enemyFast.MoveSpeed = 4f;
        enemyFast.DamageToPlayer = 5;
        EditorUtility.SetDirty(enemyFast);

        var enemyTank = CreateAsset<EnemyData>("Assets/Data/Enemies/Enemy_Tank.asset");
        enemyTank.EnemyName = "Tank";
        enemyTank.Prefab = enemyPrefab;
        enemyTank.MaxHp = 10;
        enemyTank.MoveSpeed = 1f;
        enemyTank.DamageToPlayer = 20;
        EditorUtility.SetDirty(enemyTank);

        // 4. Create WaveData (Level 1)
        var waveData = CreateAsset<WaveData>("Assets/Data/Waves/Wave_Level1.asset");
        waveData.Groups = new List<WaveGroup>();
        
        // Group 1: 3 Basic enemies
        var group1 = new WaveGroup();
        group1.EnemyType = enemyBasic;
        group1.Count = 3;
        group1.SpawnInterval = 1.5f;
        group1.PreDelay = 2f;
        waveData.Groups.Add(group1);

        // Group 2: 2 Fast enemies
        var group2 = new WaveGroup();
        group2.EnemyType = enemyFast;
        group2.Count = 2;
        group2.SpawnInterval = 1.0f;
        group2.PreDelay = 3f;
        waveData.Groups.Add(group2);

        // Group 3: 1 Tank
        var group3 = new WaveGroup();
        group3.EnemyType = enemyTank;
        group3.Count = 1;
        group3.PreDelay = 5f;
        waveData.Groups.Add(group3);

        EditorUtility.SetDirty(waveData);

        // 5. Create BuffData (Rage)
        var rageEffect = CreateAsset<RageBuffEffect>("Assets/Data/Buffs/Effect_Rage.asset");
        rageEffect.DamageMultiplier = 2.0f;
        EditorUtility.SetDirty(rageEffect);

        var rageBuff = CreateAsset<BuffData>("Assets/Data/Buffs/Buff_Rage.asset");
        rageBuff.BuffName = "Rage";
        rageBuff.TriggerWord = "rage";
        rageBuff.Effects = new List<BuffEffect> { rageEffect };
        EditorUtility.SetDirty(rageBuff);

        // 6. Create WeaponData
        var shotgun = CreateAsset<WeaponData>("Assets/Data/Weapons/Weapon_Shotgun.asset");
        shotgun.WeaponName = "Shotgun";
        shotgun.FireInterval = 1.0f;
        shotgun.ProjectilesPerShot = 5;
        shotgun.SpreadAngle = 30f;
        shotgun.IsSequential = false;
        shotgun.BaseDamage = 1;
        shotgun.PiercingCount = 0;
        shotgun.ProjectilePrefab = bulletPrefab;
        shotgun.DefaultAliases = new string[] { "shotgun", "s", "霰弹枪" };
        EditorUtility.SetDirty(shotgun);

        var rifle = CreateAsset<WeaponData>("Assets/Data/Weapons/Weapon_Rifle.asset");
        rifle.WeaponName = "Rifle";
        rifle.FireInterval = 0.1f;
        rifle.ProjectilesPerShot = 1;
        rifle.SpreadAngle = 5f;
        rifle.IsSequential = true;
        rifle.SequentialInterval = 0.1f;
        rifle.BaseDamage = 1;
        rifle.PiercingCount = 1; // Pierces 1 enemy
        rifle.ProjectilePrefab = bulletPrefab;
        rifle.DefaultAliases = new string[] { "rifle", "r", "步枪" };
        EditorUtility.SetDirty(rifle);

        // 7. Create Upgrades
        // Unlock Shotgun
        var unlockShotgun = CreateAsset<WeaponUnlockUpgrade>("Assets/Data/Upgrades/Upgrade_UnlockShotgun.asset");
        unlockShotgun.Title = "Unlock Shotgun";
        unlockShotgun.Description = "Unlock the Shotgun weapon for crowd control.";
        unlockShotgun.WeaponToUnlock = shotgun;
        unlockShotgun.Rarity = TypeRogue.Rogue.RogueUpgradeDef.RarityType.Rare;
        EditorUtility.SetDirty(unlockShotgun);

        // Unlock Rifle
        var unlockRifle = CreateAsset<WeaponUnlockUpgrade>("Assets/Data/Upgrades/Upgrade_UnlockRifle.asset");
        unlockRifle.Title = "Unlock Rifle";
        unlockRifle.Description = "Unlock the Assault Rifle for rapid fire.";
        unlockRifle.WeaponToUnlock = rifle;
        unlockRifle.Rarity = TypeRogue.Rogue.RogueUpgradeDef.RarityType.Legendary;
        EditorUtility.SetDirty(unlockRifle);

        // Boost Damage (All)
        var damageBoost = CreateAsset<StatBoostUpgrade>("Assets/Data/Upgrades/Upgrade_DamageBoost.asset");
        damageBoost.Title = "Damage Boost";
        damageBoost.Description = "Increase damage of all weapons by 20%.";
        damageBoost.TargetWeaponName = "All";
        damageBoost.Stat = StatBoostUpgrade.StatType.DamageMultiplier;
        damageBoost.Value = 0.2f;
        damageBoost.Rarity = TypeRogue.Rogue.RogueUpgradeDef.RarityType.Common;
        EditorUtility.SetDirty(damageBoost);

        // Boost Fire Rate (Pistol)
        var fireRateBoost = CreateAsset<StatBoostUpgrade>("Assets/Data/Upgrades/Upgrade_PistolFireRate.asset");
        fireRateBoost.Title = "Rapid Pistol";
        fireRateBoost.Description = "Increase Pistol fire rate by 30%.";
        fireRateBoost.TargetWeaponName = "Pistol"; // Assuming default weapon name is Pistol
        fireRateBoost.Stat = StatBoostUpgrade.StatType.FireRate;
        fireRateBoost.Value = 0.3f;
        fireRateBoost.Rarity = TypeRogue.Rogue.RogueUpgradeDef.RarityType.Common;
        EditorUtility.SetDirty(fireRateBoost);

        // Alias (Example) - Not easy to automate as it requires user input, but we can create a template
        var aliasUpgrade = CreateAsset<AliasUpgrade>("Assets/Data/Upgrades/Upgrade_CustomAlias.asset");
        aliasUpgrade.Title = "Quick Shot";
        aliasUpgrade.Description = "Register 'bang' as alias for Pistol.";
        aliasUpgrade.TargetType = TypeRogue.TypingResolveResultType.WeaponPistol;
        aliasUpgrade.NewAlias = "bang";
        aliasUpgrade.Rarity = TypeRogue.Rogue.RogueUpgradeDef.RarityType.Common;
        EditorUtility.SetDirty(aliasUpgrade);

        // 8. Assign to RogueManager in Scene
        var rogueManager = Object.FindFirstObjectByType<TypeRogue.Rogue.RogueManager>();
        if (rogueManager != null)
        {
            // Use SerializedObject to modify private field
            SerializedObject so = new SerializedObject(rogueManager);
            SerializedProperty upgradePoolProp = so.FindProperty("upgradePool");
            
            if (upgradePoolProp != null)
            {
                upgradePoolProp.ClearArray();
                var upgrades = new List<TypeRogue.Rogue.RogueUpgradeDef> { unlockShotgun, unlockRifle, damageBoost, fireRateBoost, aliasUpgrade };
                
                for (int i = 0; i < upgrades.Count; i++)
                {
                    upgradePoolProp.InsertArrayElementAtIndex(i);
                    upgradePoolProp.GetArrayElementAtIndex(i).objectReferenceValue = upgrades[i];
                }
                so.ApplyModifiedProperties();
                Debug.Log("Assigned upgrades to RogueManager in scene.");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Resources Created Successfully!");

        // 7. Scene Configuration
        SetupScene(waveData, rageBuff, shotgun, rifle);
    }

    private static void SetupScene(WaveData waveData, BuffData rageBuff, WeaponData shotgunData, WeaponData rifleData)
    {
        // Find EnemySpawner
        var spawner = Object.FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            var wavesField = typeof(EnemySpawner).GetField("waves", BindingFlags.NonPublic | BindingFlags.Instance);
            if (wavesField != null)
            {
                var list = new List<WaveData> { waveData };
                wavesField.SetValue(spawner, list);
                EditorUtility.SetDirty(spawner);
                Debug.Log("Assigned WaveData to EnemySpawner.");
            }
        }
        else
        {
            Debug.LogWarning("EnemySpawner not found in scene.");
        }

        // Find Bootstrap
        var bootstrap = Object.FindAnyObjectByType<TypeRogueBootstrap>();
        if (bootstrap != null)
        {
            var type = typeof(TypeRogueBootstrap);

            // Assign Buffs
            var buffsField = type.GetField("startingBuffs", BindingFlags.NonPublic | BindingFlags.Instance);
            if (buffsField != null)
            {
                var list = (List<BuffData>)buffsField.GetValue(bootstrap);
                if (list == null) list = new List<BuffData>();
                if (!list.Contains(rageBuff))
                {
                    list.Add(rageBuff);
                    buffsField.SetValue(bootstrap, list);
                    Debug.Log("Added Rage Buff to Bootstrap.");
                }
            }

            // Assign WeaponData
            var shotgunDataField = type.GetField("shotgunWeaponData", BindingFlags.NonPublic | BindingFlags.Instance);
            if (shotgunDataField != null) shotgunDataField.SetValue(bootstrap, shotgunData);
            
            var rifleDataField = type.GetField("rifleWeaponData", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rifleDataField != null) rifleDataField.SetValue(bootstrap, rifleData);

            Debug.Log("Assigned Shotgun/Rifle Data to Bootstrap.");

            // Find and Assign WeaponControllers (Best Effort)
            var controllers = Object.FindObjectsByType<WeaponController>(FindObjectsSortMode.None);
            foreach (var wc in controllers)
            {
                if (wc.name.Contains("Shotgun"))
                {
                    var field = type.GetField("shotgunWeaponController", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null) field.SetValue(bootstrap, wc);
                    Debug.Log($"Assigned {wc.name} to shotgunWeaponController.");
                }
                else if (wc.name.Contains("Rifle"))
                {
                    var field = type.GetField("rifleWeaponController", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null) field.SetValue(bootstrap, wc);
                    Debug.Log($"Assigned {wc.name} to rifleWeaponController.");
                }
            }
            
            EditorUtility.SetDirty(bootstrap);
        }
        else
        {
            Debug.LogWarning("TypeRogueBootstrap not found in scene.");
        }
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
            string folder = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }

    private static T CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
        return asset;
    }
}
