using System;
using System.Collections.Generic;
using UnityEngine;
using TypeRogue.Data; // Added namespace
using TypeRogue.Rogue;
using TypeRogue.Rogue.Upgrades;
using TypeRogue.UI.Rogue;

namespace TypeRogue
{
    /// <summary>
    /// 游戏启动入口 (Bootstrap)
    /// 功能：
    /// 1. 初始化核心系统（输入、UI、战斗）
    /// 2. 协调各模块之间的依赖关系
    /// 3. 管理游戏主循环
    /// </summary>
    public sealed class TypeRogueBootstrap : MonoBehaviour
    {
        [Header("Core Systems")]
        [SerializeField] private TypingInputRouter typingInputRouter;
        [SerializeField] private TypingCommandInterpreter typingCommandInterpreter;
        [SerializeField] private WeaponController pistolWeaponController;
        [SerializeField] private WeaponController shotgunWeaponController;
        [SerializeField] private WeaponController rifleWeaponController;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private BuffController buffController; // 新增
        [SerializeField] private RogueManager rogueManager;

        [Header("Data Configuration")]
        [SerializeField] private WeaponData startingWeaponData; // 初始武器数据 (Pistol)
        [SerializeField] private WeaponData shotgunWeaponData;
        [SerializeField] private WeaponData rifleWeaponData;
        [SerializeField] private List<Data.BuffData> startingBuffs; // 初始 Buff 列表

        [Header("UI")]
        [SerializeField] private BattleHudView battleHudView;
        [SerializeField] private TypingInputBarView typingInputBarView;
        [SerializeField] private WeaponStatusPanelView weaponStatusPanelView;
        [SerializeField] private RogueShopView rogueShopView;

        [Header("Run (Temporary)")]
        [SerializeField] private int initialWaveIndex = 1;
        [SerializeField] private int maxWaves = 3;

        public enum GameState
        {
            MainMenu,
            Playing,
            Shop,
            Victory,
            GameOver
        }

        private bool isSetup;
        private int currentWaveIndex = 0;
        private GameState gameState = GameState.MainMenu;
        private WeaponStatusItemView pistolStatusItem; // 缓存手枪的UI Item引用
        private WeaponStatusItemView shotgunStatusItem;
        private WeaponStatusItemView rifleStatusItem;

        public void Initialize(
            TypingInputRouter typingInputRouter,
            TypingCommandInterpreter typingCommandInterpreter,
            WeaponController pistolWeaponController,
            WeaponController shotgunWeaponController,
            WeaponController rifleWeaponController,
            PlayerController playerController,
            EnemySpawner enemySpawner,
            BattleHudView battleHudView,
            TypingInputBarView typingInputBarView,
            WeaponStatusPanelView weaponStatusPanelView,
            RogueManager rogueManager,
            RogueShopView rogueShopView,
            WeaponData startingWeaponData = null)
        {
            this.typingInputRouter = typingInputRouter;
            this.typingCommandInterpreter = typingCommandInterpreter;
            this.pistolWeaponController = pistolWeaponController;
            this.shotgunWeaponController = shotgunWeaponController;
            this.rifleWeaponController = rifleWeaponController;
            this.playerController = playerController;
            this.enemySpawner = enemySpawner;
            this.battleHudView = battleHudView;
            this.typingInputBarView = typingInputBarView;
            this.weaponStatusPanelView = weaponStatusPanelView;
            this.rogueManager = rogueManager;
            this.rogueShopView = rogueShopView;
            if (startingWeaponData != null) this.startingWeaponData = startingWeaponData;

            TrySetup();
        }

        private void Start()
        {
            if (TrySetup())
            {
                // Don't start wave immediately. Wait for "begin".
                gameState = GameState.MainMenu;
                Debug.Log("[Bootstrap] Game Ready. Waiting for 'begin' command.");
                if (battleHudView != null) battleHudView.SetWaveText("Type 'begin' to start");
                return;
            }

            if (typingInputRouter == null) throw new InvalidOperationException($"{nameof(typingInputRouter)} is not assigned.");
            if (typingCommandInterpreter == null) throw new InvalidOperationException($"{nameof(typingCommandInterpreter)} is not assigned.");
            if (pistolWeaponController == null) throw new InvalidOperationException($"{nameof(pistolWeaponController)} is not assigned.");
            if (playerController == null) throw new InvalidOperationException($"{nameof(playerController)} is not assigned.");
            if (battleHudView == null) throw new InvalidOperationException($"{nameof(battleHudView)} is not assigned.");
            if (typingInputBarView == null) throw new InvalidOperationException($"{nameof(typingInputBarView)} is not assigned.");
        }

        private void Awake()
        {
            // 尝试自动获取同GameObject上的组件
            if (typingInputRouter == null) typingInputRouter = GetComponent<TypingInputRouter>();
            if (typingCommandInterpreter == null) typingCommandInterpreter = GetComponent<TypingCommandInterpreter>();
            
            // 注意：如果有多个WeaponController在同一个GameObject上，GetComponent只能获取第一个
            // 建议在Inspector中手动拖拽赋值，或者使用 GetComponents<WeaponController>() 并根据 weaponData 区分
            if (pistolWeaponController == null) pistolWeaponController = GetComponent<WeaponController>();
            
            if (buffController == null) buffController = GetComponent<BuffController>(); // 自动获取
            if (rogueManager == null) rogueManager = GetComponent<RogueManager>();
            
            // 尝试查找场景中的组件（如果没有手动拖拽）
            if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();
            if (enemySpawner == null) enemySpawner = FindFirstObjectByType<EnemySpawner>();
            if (rogueShopView == null) rogueShopView = FindFirstObjectByType<RogueShopView>();

            TrySetup();
        }

        private bool TrySetup()
        {
            if (isSetup) return true;
            if (typingInputRouter == null || typingCommandInterpreter == null || pistolWeaponController == null || 
                battleHudView == null || typingInputBarView == null || playerController == null || enemySpawner == null ||
                weaponStatusPanelView == null || buffController == null) 
            {
                return false;
            }

            // 1. 初始化输入路由
            typingInputRouter.BufferChanged += OnBufferChanged;
            typingInputRouter.WordSubmitted += OnWordSubmitted;

            // 2. 初始化命令解释器
            var buffWords = new List<string>();
            if (startingBuffs != null)
            {
                foreach (var buff in startingBuffs)
                {
                    if (buff != null && !string.IsNullOrEmpty(buff.TriggerWord))
                    {
                        buffWords.Add(buff.TriggerWord);
                    }
                }
            }
            
            // 初始化解释器，传入各武器的别名
            // 默认只解锁手枪 (startingWeaponData)
            var pistolAliases = startingWeaponData != null && startingWeaponData.DefaultAliases != null && startingWeaponData.DefaultAliases.Length > 0 
                ? startingWeaponData.DefaultAliases 
                : new[] { "pistol", "手枪", "p" };

            typingCommandInterpreter.Initialize(
                pistolAliases, 
                new string[0], // Shotgun locked by default
                new string[0], // Rifle locked by default
                buffWords); 
            
            typingCommandInterpreter.Resolved += OnResolved;
            // typingCommandInterpreter.PistolFireRequested += OnPistolFireRequested; // 废弃

            // 3. 初始化战斗与玩家
            // 注入 BuffController 到 WeaponController
            pistolWeaponController.Initialize(playerController.MuzzlePoint, startingWeaponData, buffController);
            if (shotgunWeaponController != null) shotgunWeaponController.Initialize(playerController.MuzzlePoint, shotgunWeaponData, buffController);
            if (rifleWeaponController != null) rifleWeaponController.Initialize(playerController.MuzzlePoint, rifleWeaponData, buffController);
            
            playerController.HpChanged += OnPlayerHpChanged; // 订阅血量变化

            // 4. 初始化 Buff 系统
            buffController.Initialize(startingBuffs);
            buffController.BuffsChanged += OnBuffsChanged;
            
            // 5. 初始化 Rogue 系统
            if (rogueManager != null)
            {
                rogueManager.Initialize(this, rogueShopView);
            }
            enemySpawner.WaveCleared += OnWaveCleared;

            // 6. 初始化 UI (需要 Buff 数据)
            battleHudView.Initialize(playerController.CurrentHp, startingBuffs); 
            battleHudView.SetWave(initialWaveIndex);

            if (weaponStatusPanelView != null)
            {
                weaponStatusPanelView.Initialize();
                // 注册初始武器
                pistolStatusItem = weaponStatusPanelView.AddWeapon("PISTOL");
                // Don't add Shotgun/Rifle initially. They will be added via UnlockWeapon.
                // if (shotgunWeaponController != null) shotgunStatusItem = weaponStatusPanelView.AddWeapon("SHOTGUN");
                // if (rifleWeaponController != null) rifleStatusItem = weaponStatusPanelView.AddWeapon("RIFLE");
            }

            // 7. 初始化输入栏
            typingInputBarView.Initialize(null); // 同上

            isSetup = true;
            Debug.Log("TypeRogue Bootstrap Setup Complete.");
            return true;
        }

        private void OnWaveCleared()
        {
            Debug.Log($"[Bootstrap] Wave {currentWaveIndex + 1} Cleared! Initiating Rogue Shop sequence...");
            // 暂停输入? 暂时不需要，因为RogueManager会接管
            if (rogueManager != null)
            {
                Debug.Log("[Bootstrap] Calling RogueManager.OpenShop()");
                rogueManager.OpenShop();
            }
            else
            {
                Debug.LogWarning("[Bootstrap] RogueManager not assigned! Skipping shop.");
                // 如果没有 RogueManager，直接下一波
                OnShopClosed();
            }
        }

        public void OnShopClosed()
        {
            Debug.Log("[Bootstrap] Shop Closed. Preparing next wave.");
            currentWaveIndex++;
            
            if (currentWaveIndex >= maxWaves)
            {
                Victory();
            }
            else
            {
                Debug.Log($"[Bootstrap] >>> STARTING WAVE: {currentWaveIndex + 1} <<<");
                gameState = GameState.Playing;
                battleHudView.SetWave(currentWaveIndex + 1);
                enemySpawner.StartWave(currentWaveIndex);
            }
        }

        private void Victory()
        {
            Debug.Log("[Bootstrap] VICTORY!");
            gameState = GameState.Victory;
            if (battleHudView != null) battleHudView.SetWaveText("VICTORY! Type 'restart'");
            // Cleanup enemies? They should be dead.
        }

        private void GameOver()
        {
            Debug.Log("[Bootstrap] GAME OVER!");
            gameState = GameState.GameOver;
            if (battleHudView != null) battleHudView.SetWaveText("GAME OVER! Type 'restart'");
            if (enemySpawner != null) enemySpawner.StopSpawning();
        }

        public void UnlockWeapon(WeaponData weapon)
        {
            if (weapon == null) return;
            Debug.Log($"[Bootstrap] Unlocking Weapon: {weapon.WeaponName}");

            // 确定武器类型并注册别名
            TypingResolveResultType type = TypingResolveResultType.UnknownWord;
            string[] aliases = weapon.DefaultAliases;

            if (weapon == shotgunWeaponData)
            {
                type = TypingResolveResultType.WeaponShotgun;
                if (aliases == null || aliases.Length == 0) aliases = new[] { "shotgun", "霰弹枪", "s" };
            }
            else if (weapon == rifleWeaponData)
            {
                type = TypingResolveResultType.WeaponRifle;
                if (aliases == null || aliases.Length == 0) aliases = new[] { "rifle", "步枪", "r" };
            }

            if (type != TypingResolveResultType.UnknownWord && aliases != null)
            {
                foreach (var alias in aliases)
                {
                    typingCommandInterpreter.RegisterAlias(type, alias);
                }
                
                // Add to UI
                if (type == TypingResolveResultType.WeaponShotgun)
                {
                    shotgunStatusItem = weaponStatusPanelView.AddWeapon("SHOTGUN");
                }
                else if (type == TypingResolveResultType.WeaponRifle)
                {
                    rifleStatusItem = weaponStatusPanelView.AddWeapon("RIFLE");
                }
            }
        }

        public void UnlockBuff(Data.BuffData buff)
        {
            if (buff == null) return;
            Debug.Log($"[Bootstrap] Unlocking Buff: {buff.BuffName}");
            
            if (!string.IsNullOrEmpty(buff.TriggerWord))
            {
                typingCommandInterpreter.RegisterAlias(TypingResolveResultType.ModifierAccepted, buff.TriggerWord);
                
                // Add to BuffController. This will trigger BuffsChanged -> UI Update.
                if (buffController != null)
                {
                    buffController.AddBuff(buff);
                }
            }
        }

        public void RegisterAlias(TypingResolveResultType type, string alias)
        {
            typingCommandInterpreter.RegisterAlias(type, alias);
        }

        public void ModifyWeaponStat(string targetWeaponName, StatBoostUpgrade.StatType stat, float value)
        {
            var controllers = new List<WeaponController>();
            if (pistolWeaponController != null) controllers.Add(pistolWeaponController);
            if (shotgunWeaponController != null) controllers.Add(shotgunWeaponController);
            if (rifleWeaponController != null) controllers.Add(rifleWeaponController);

            foreach (var controller in controllers)
            {
                bool isTarget = string.IsNullOrEmpty(targetWeaponName) || 
                                targetWeaponName.Equals("All", StringComparison.OrdinalIgnoreCase) ||
                                controller.WeaponName.Contains(targetWeaponName, StringComparison.OrdinalIgnoreCase);

                if (isTarget)
                {
                    switch (stat)
                    {
                        case StatBoostUpgrade.StatType.DamageMultiplier:
                            controller.BoostDamage(value);
                            break;
                        case StatBoostUpgrade.StatType.FireRate:
                            controller.BoostFireRate(value);
                            break;
                        case StatBoostUpgrade.StatType.SpreadReduction:
                            controller.BoostSpread(value);
                            break;
                        case StatBoostUpgrade.StatType.Piercing:
                            controller.BoostPiercing(Mathf.RoundToInt(value));
                            break;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (!isSetup) return;

            if (typingInputRouter != null)
            {
                typingInputRouter.BufferChanged -= OnBufferChanged;
                typingInputRouter.WordSubmitted -= OnWordSubmitted;
            }

            if (typingCommandInterpreter != null)
            {
                typingCommandInterpreter.Resolved -= OnResolved;
                // typingCommandInterpreter.PistolFireRequested -= OnPistolFireRequested;
            }

            if (playerController != null)
            {
                playerController.HpChanged -= OnPlayerHpChanged;
            }
            
            if (buffController != null)
            {
                buffController.BuffsChanged -= OnBuffsChanged;
            }
            
            if (enemySpawner != null)
            {
                enemySpawner.WaveCleared -= OnWaveCleared;
            }
        }

        private void Update()
        {
            if (!isSetup) return;
            if (pistolStatusItem != null && pistolWeaponController != null)
            {
                pistolStatusItem.SetCooldown(pistolWeaponController.GetCooldownRemainingSeconds(), pistolWeaponController.FireIntervalSeconds);
            }
            if (shotgunStatusItem != null && shotgunWeaponController != null)
            {
                shotgunStatusItem.SetCooldown(shotgunWeaponController.GetCooldownRemainingSeconds(), shotgunWeaponController.FireIntervalSeconds);
            }
            if (rifleStatusItem != null && rifleWeaponController != null)
            {
                rifleStatusItem.SetCooldown(rifleWeaponController.GetCooldownRemainingSeconds(), rifleWeaponController.FireIntervalSeconds);
            }
        }

        private void OnBufferChanged(string buffer)
        {
            typingInputBarView.SetBuffer(buffer);
        }

        private void StartGame()
        {
            Debug.Log("[Bootstrap] Starting Game!");
            gameState = GameState.Playing;
            currentWaveIndex = Mathf.Max(0, initialWaveIndex - 1);
            if (battleHudView != null) battleHudView.SetWave(currentWaveIndex + 1);
            if (enemySpawner != null) enemySpawner.StartWave(currentWaveIndex);
        }

        private void OnWordSubmitted(string word)
        {
            if (gameState == GameState.MainMenu && word.Equals("begin", StringComparison.OrdinalIgnoreCase))
            {
                StartGame();
                typingInputRouter.ClearBuffer();
                return;
            }

            if (gameState != GameState.Playing)
            {
                 if ((gameState == GameState.GameOver || gameState == GameState.Victory) && 
                     word.Equals("restart", StringComparison.OrdinalIgnoreCase))
                 {
                     UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                 }
                 return;
            }

            var result = typingCommandInterpreter.SubmitWord(word);
            switch (result.Type)
            {
                case TypingResolveResultType.WeaponPistol:
                    typingInputRouter.ClearBuffer(); 
                    if (pistolWeaponController != null) pistolWeaponController.TryFire();
                    break;
                
                case TypingResolveResultType.WeaponShotgun:
                    typingInputRouter.ClearBuffer();
                    if (shotgunWeaponController != null) shotgunWeaponController.TryFire();
                    break;
                
                case TypingResolveResultType.WeaponRifle:
                    typingInputRouter.ClearBuffer();
                    if (rifleWeaponController != null) rifleWeaponController.TryFire();
                    break;

                case TypingResolveResultType.ModifierAccepted:
                    // 修饰词被接受（如 "precise"）
                    // 用户希望保留输入框内容，直到触发武器词后再一起清空
                    
                    // 将 Buff 添加到控制器
                    buffController.AddBuff(result.SubmittedWord);
                    Debug.Log($"Modifier accepted: {result.SubmittedWord}");
                    break;
                case TypingResolveResultType.UnknownWord:
                    // 输入错误，保留已输入的内容供玩家修改，或者只清空当前错误的单词
                    // 这里选择清空当前错误单词，避免卡死
                    typingInputRouter.RemoveCurrentWord();
                    break;
            }
        }

        private void OnResolved(TypingResolveResult result)
        {
            battleHudView.SetLastTypingResult(result);
        }

        private void OnBuffsChanged(List<BuffInstance> activeBuffs)
        {
            battleHudView.UpdateBuffState(activeBuffs);
        }

        // private void OnModifiersChanged(IEnumerable<string> activeModifiers) { ... } // 废弃

        private void OnPlayerHpChanged(int newHp)
        {
            battleHudView.SetPlayerHp(newHp);
            Debug.Log($"[UI] Updated Player HP: {newHp}");
            
            if (newHp <= 0 && gameState == GameState.Playing)
            {
                GameOver();
            }
        }
    }
}
