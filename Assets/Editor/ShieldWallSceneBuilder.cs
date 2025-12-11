using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Combat;
using ShieldWall.Dice;
using ShieldWall.ShieldWall;
using ShieldWall.UI;
using ShieldWall.Data;
using ShieldWall.Visual;
using System.Collections.Generic;
using System.IO;

namespace ShieldWall.Editor
{
    public static class ShieldWallSceneBuilder
    {
        #region Complete Setup

        [MenuItem("Shield Wall Builder/Complete Setup/Build Everything (One-Click)", false, 0)]
        public static void BuildEverything()
        {
            Debug.Log("=== SHIELD WALL: BUILDING EVERYTHING ===");
            
            CreateAllGameData();
            BuildBattleSceneComplete();
            BuildMainMenuSceneComplete();
            
            Debug.Log("=== SHIELD WALL: BUILD COMPLETE! ===");
            Debug.Log("Open MainMenu scene and press Play to start the game.");
        }

        [MenuItem("Shield Wall Builder/Complete Setup/Build Battle Scene Only", false, 1)]
        public static void BuildBattleSceneComplete()
        {
            Debug.Log("--- Building Battle Scene ---");
            
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            if (!scene.IsValid())
            {
                Debug.LogError("Could not open Battle.unity!");
                return;
            }

            SetupBattleManagers();
            SetupBattleUI();
            SetupBattleAtmosphere();
            SetupBattleVisuals();
            WireAllBattleReferences();
            AssignAllScriptableObjects();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            Debug.Log("Battle scene complete!");
        }

        [MenuItem("Shield Wall Builder/Complete Setup/Build MainMenu Scene Only", false, 2)]
        public static void BuildMainMenuSceneComplete()
        {
            Debug.Log("--- Building MainMenu Scene ---");
            
            BuildMainMenuScene();
            CreateMenuPrefabs();
            
            Debug.Log("MainMenu scene complete!");
        }

        #endregion

        #region Asset Creation

        [MenuItem("Shield Wall Builder/Asset Creation/Create All Game Data", false, 100)]
        public static void CreateAllGameData()
        {
            Debug.Log("--- Creating All Game Data ---");
            
            CreateRuneAssets();
            CreateBrotherAssets();
            CreateEnemyAssets();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            CreateActionAssets();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            CreateWaveAssets();
            CreateEasyWaveAssets();
            CreateHardWaveAssets();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            CreateScenarioAssets();
            CreateTutorialHintAssets();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("All game data created!");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Runes", false, 101)]
        public static void CreateRuneAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Runes");
            
            CreateRune(RuneType.Thurs, "Shield", new Color(0.36f, 0.36f, 0.36f), "Defense rune - protects against attacks");
            CreateRune(RuneType.Tyr, "Axe", new Color(0.55f, 0.13f, 0.13f), "Attack rune - strikes enemies");
            CreateRune(RuneType.Gebo, "Spear", new Color(0.55f, 0.41f, 0.08f), "Precision rune - targeted attacks");
            CreateRune(RuneType.Berkana, "Brace", new Color(0.24f, 0.36f, 0.24f), "Support rune - reinforcement");
            CreateRune(RuneType.Othala, "Odin", new Color(0.79f, 0.64f, 0.15f), "Wild card rune - can substitute any rune");
            CreateRune(RuneType.Laguz, "Loki", new Color(0.36f, 0.24f, 0.43f), "Chaos rune - unpredictable effects");
            
            Debug.Log("Rune assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Brothers", false, 102)]
        public static void CreateBrotherAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Brothers");
            
            CreateBrother("Bjorn", "+block power");
            CreateBrother("Erik", "Better auto-defend");
            CreateBrother("Gunnar", "+strike power");
            CreateBrother("Olaf", "Extra wound absorption");
            
            Debug.Log("Brother assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Enemies", false, 103)]
        public static void CreateEnemyAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Enemies");
            
            CreateEnemy("Thrall", 1, 1, EnemyTargetingType.Random, false, false, new Color(0.5f, 0.5f, 0.5f));
            CreateEnemy("Warrior", 1, 1, EnemyTargetingType.LowestHealth, false, false, new Color(0.6f, 0.3f, 0.2f));
            CreateEnemy("Spearman", 1, 2, EnemyTargetingType.Player, false, false, new Color(0.4f, 0.35f, 0.2f));
            CreateEnemy("Berserker", 2, 2, EnemyTargetingType.Player, false, false, new Color(0.7f, 0.15f, 0.15f));
            CreateEnemy("Archer", 1, 1, EnemyTargetingType.Random, true, false, new Color(0.3f, 0.5f, 0.3f));
            CreateEnemy("ShieldBreaker", 1, 1, EnemyTargetingType.Player, false, true, new Color(0.3f, 0.3f, 0.5f));
            
            Debug.Log("Enemy assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Actions", false, 104)]
        public static void CreateActionAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Actions");
            
            CreateAction("Block", new[] { RuneType.Thurs, RuneType.Thurs }, ActionEffectType.Block, 1, "Negate 1 attack on you");
            CreateAction("Strike", new[] { RuneType.Tyr, RuneType.Gebo }, ActionEffectType.Strike, 1, "Kill 1 enemy");
            CreateAction("Cover", new[] { RuneType.Thurs, RuneType.Berkana }, ActionEffectType.Cover, 1, "Block attack on adjacent brother");
            CreateAction("Brace", new[] { RuneType.Berkana, RuneType.Berkana }, ActionEffectType.Brace, 1, "Reduce next wound to scratch");
            CreateAction("Counter", new[] { RuneType.Tyr, RuneType.Thurs }, ActionEffectType.Counter, 1, "Block + kill attacker");
            CreateAction("Testudo", new[] { RuneType.Thurs, RuneType.Thurs, RuneType.Thurs }, ActionEffectType.BlockAll, 1, "Block ALL attacks this turn");
            CreateAction("Berserker", new[] { RuneType.Tyr, RuneType.Tyr, RuneType.Tyr }, ActionEffectType.BerserkerRage, 3, "Kill 3 enemies, take 1 wound");
            CreateAction("Rally", new[] { RuneType.Berkana, RuneType.Berkana, RuneType.Berkana }, ActionEffectType.Heal, 1, "Heal 1 wound from each brother");
            CreateAction("WallPush", new[] { RuneType.Thurs, RuneType.Berkana, RuneType.Gebo }, ActionEffectType.Stun, 1, "Stun all enemies for 1 turn");
            CreateAction("SpearWall", new[] { RuneType.Gebo, RuneType.Gebo, RuneType.Gebo }, ActionEffectType.MultiStrike, 2, "Kill 2 enemies");
            
            Debug.Log("Action assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Waves (Normal)", false, 105)]
        public static void CreateWaveAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Waves");
            
            var thrall = LoadEnemy("Thrall");
            var warrior = LoadEnemy("Warrior");
            var spearman = LoadEnemy("Spearman");
            var berserker = LoadEnemy("Berserker");
            var archer = LoadEnemy("Archer");
            var shieldBreaker = LoadEnemy("ShieldBreaker");

            if (thrall == null || warrior == null)
            {
                Debug.LogWarning("Create enemy assets first before creating waves!");
                return;
            }

            CreateWave("Wave_01", 1, new List<EnemySpawn> { new EnemySpawn { enemy = thrall, count = 3 } });
            CreateWave("Wave_02", 2, new List<EnemySpawn> { new EnemySpawn { enemy = thrall, count = 2 }, new EnemySpawn { enemy = warrior, count = 2 } });
            CreateWave("Wave_03", 3, new List<EnemySpawn> { new EnemySpawn { enemy = warrior, count = 2 }, new EnemySpawn { enemy = spearman, count = 2 }, new EnemySpawn { enemy = thrall, count = 1 } });
            
            if (berserker != null && archer != null && shieldBreaker != null)
            {
                CreateWave("Wave_04", 4, new List<EnemySpawn> { new EnemySpawn { enemy = warrior, count = 3 }, new EnemySpawn { enemy = archer, count = 2 }, new EnemySpawn { enemy = shieldBreaker, count = 1 } });
                CreateWave("Wave_05", 5, new List<EnemySpawn> { new EnemySpawn { enemy = berserker, count = 2 }, new EnemySpawn { enemy = spearman, count = 2 }, new EnemySpawn { enemy = shieldBreaker, count = 1 } });
            }
            
            Debug.Log("Normal wave assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Waves (Easy)", false, 106)]
        public static void CreateEasyWaveAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Waves");
            
            var thrall = LoadEnemy("Thrall");
            var warrior = LoadEnemy("Warrior");

            if (thrall == null || warrior == null)
            {
                Debug.LogWarning("Create enemy assets first!");
                return;
            }

            CreateWave("Wave_Easy_01", 1, new List<EnemySpawn> { new EnemySpawn { enemy = thrall, count = 2 } }, true, "tutorial_dice");
            CreateWave("Wave_Easy_02", 2, new List<EnemySpawn> { new EnemySpawn { enemy = thrall, count = 3 } });
            CreateWave("Wave_Easy_03", 3, new List<EnemySpawn> { new EnemySpawn { enemy = thrall, count = 2 }, new EnemySpawn { enemy = warrior, count = 1 } });
            
            Debug.Log("Easy wave assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Waves (Hard)", false, 107)]
        public static void CreateHardWaveAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Waves");
            
            var warrior = LoadEnemy("Warrior");
            var berserker = LoadEnemy("Berserker");
            var archer = LoadEnemy("Archer");

            if (warrior == null || berserker == null || archer == null)
            {
                Debug.LogWarning("Create enemy assets first!");
                return;
            }

            CreateWave("Wave_Hard_01", 1, new List<EnemySpawn> { new EnemySpawn { enemy = warrior, count = 2 }, new EnemySpawn { enemy = berserker, count = 1 } });
            CreateWave("Wave_Hard_02", 2, new List<EnemySpawn> { new EnemySpawn { enemy = berserker, count = 3 } });
            CreateWave("Wave_Hard_03", 3, new List<EnemySpawn> { new EnemySpawn { enemy = archer, count = 2 }, new EnemySpawn { enemy = warrior, count = 2 } });
            CreateWave("Wave_Hard_04", 4, new List<EnemySpawn> { new EnemySpawn { enemy = berserker, count = 4 } });
            
            Debug.Log("Hard wave assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Scenarios", false, 108)]
        public static void CreateScenarioAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Scenarios");

            var easyWaves = new List<WaveConfigSO>();
            var normalWaves = new List<WaveConfigSO>();
            var hardWaves = new List<WaveConfigSO>();

            for (int i = 1; i <= 3; i++)
            {
                var w = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"Assets/ScriptableObjects/Waves/Wave_Easy_0{i}.asset");
                if (w != null) easyWaves.Add(w);
            }
            for (int i = 1; i <= 5; i++)
            {
                var w = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"Assets/ScriptableObjects/Waves/Wave_{i:D2}.asset");
                if (w != null) normalWaves.Add(w);
            }
            for (int i = 1; i <= 4; i++)
            {
                var w = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"Assets/ScriptableObjects/Waves/Wave_Hard_0{i}.asset");
                if (w != null) hardWaves.Add(w);
            }

            var breach = CreateScenario("Scenario_TheBreach", "The Breach",
                "Raiders have broken through the outer wall. Hold them off while the village evacuates.",
                Difficulty.Easy, easyWaves, 15, 6, 5, true, null);

            var hold = CreateScenario("Scenario_HoldTheLine", "Hold the Line",
                "The enemy comes in force. Your shield wall is all that stands between them and your people.",
                Difficulty.Normal, normalWaves, 12, 5, 4, true, null);

            CreateScenario("Scenario_TheLastStand", "The Last Stand",
                "Berserkers and archers. Few supplies. No retreat. Only glory or death awaits.",
                Difficulty.Hard, hardWaves, 10, 4, 4, false, hold);

            Debug.Log("Scenario assets created");
        }

        [MenuItem("Shield Wall Builder/Asset Creation/Create Tutorial Hints", false, 109)]
        public static void CreateTutorialHintAssets()
        {
            EnsureDirectoryExists("Assets/ScriptableObjects/Tutorial");

            CreateHint("Hint_LockDice", "lock_dice", "Click on a die to LOCK it. Locked dice won't re-roll.",
                TurnPhase.PlayerTurn, 1, false, true, 6f);
            CreateHint("Hint_MatchRunes", "match_runes", "Match rune symbols to unlock powerful ACTIONS. Try locking matching dice!",
                TurnPhase.PlayerTurn, 1, true, false, 6f);
            CreateHint("Hint_Brothers", "brothers_block", "Your shield brothers will try to BLOCK attacks for you. Keep them alive!",
                TurnPhase.WaveStart, 2, false, false, 5f);
            CreateHint("Hint_Stamina", "stamina_drain", "STAMINA drains each turn. When it runs out, you lose. Strike fast!",
                TurnPhase.Resolution, 3, false, false, 5f);
            CreateHint("Hint_Berserkers", "berserkers", "BERSERKERS ignore blocks! Kill them quickly or suffer.",
                TurnPhase.WaveStart, 4, false, false, 5f);

            Debug.Log("Tutorial hint assets created");
        }

        #endregion

        #region Battle Scene Setup

        [MenuItem("Shield Wall Builder/Battle Scene/Setup Managers", false, 200)]
        public static void SetupBattleManagers()
        {
            var gameManagerGO = CreateOrFind("GameManager");
            AddComponent<BattleBootstrapper>(gameManagerGO);
            AddComponent<BattleManager>(gameManagerGO);
            AddComponent<TurnManager>(gameManagerGO);
            AddComponent<StaminaManager>(gameManagerGO);

            var diceManagerGO = CreateOrFind("DiceManager");
            AddComponent<DicePoolManager>(diceManagerGO);
            AddComponent<ComboManager>(diceManagerGO);

            var combatManagerGO = CreateOrFind("CombatManager");
            AddComponent<EnemyWaveController>(combatManagerGO);
            AddComponent<CombatResolver>(combatManagerGO);
            AddComponent<ActionSelectionManager>(combatManagerGO);

            var shieldWallGO = CreateOrFind("ShieldWall");
            AddComponent<ShieldWallManager>(shieldWallGO);
            AddComponent<PlayerWarrior>(shieldWallGO);

            Debug.Log("Battle managers created");
        }

        [MenuItem("Shield Wall Builder/Battle Scene/Setup UI Canvas", false, 201)]
        public static void SetupBattleUI()
        {
            CreateUIPrefabs();
            CreateBattleCanvas();
            Debug.Log("Battle UI created");
        }

        [MenuItem("Shield Wall Builder/Battle Scene/Setup Atmosphere", false, 202)]
        public static void SetupBattleAtmosphere()
        {
            SetupLayers();
            CreateVolumeProfile();
            AddVolumeToScene();
            SetupBattleLighting();
            CreateGroundPlane();
            SetupFog();
            Debug.Log("Battle atmosphere created");
        }

        [MenuItem("Shield Wall Builder/Battle Scene/Setup Visual Elements", false, 203)]
        public static void SetupBattleVisuals()
        {
            CreateBattleCamera();
            CreateFirstPersonArms();
            CreateVisualControllers();
            CreateEnemySpawnPoint();
            CreateBrotherSpawnMarkers();
            Debug.Log("Battle visuals created");
        }

        [MenuItem("Shield Wall Builder/Battle Scene/Wire All References", false, 204)]
        public static void WireAllBattleReferences()
        {
            var battleManager = Object.FindFirstObjectByType<BattleManager>();
            var turnManager = Object.FindFirstObjectByType<TurnManager>();
            var staminaManager = Object.FindFirstObjectByType<StaminaManager>();
            var dicePoolManager = Object.FindFirstObjectByType<DicePoolManager>();
            var comboManager = Object.FindFirstObjectByType<ComboManager>();
            var waveController = Object.FindFirstObjectByType<EnemyWaveController>();
            var combatResolver = Object.FindFirstObjectByType<CombatResolver>();
            var actionSelectionManager = Object.FindFirstObjectByType<ActionSelectionManager>();
            var shieldWallManager = Object.FindFirstObjectByType<ShieldWallManager>();
            var playerWarrior = Object.FindFirstObjectByType<PlayerWarrior>();
            var diceUI = Object.FindFirstObjectByType<DiceUI>();
            var waveUI = Object.FindFirstObjectByType<WaveUI>();
            var gameOverUI = Object.FindFirstObjectByType<GameOverUI>();

            if (battleManager != null)
            {
                SetPrivateField(battleManager, "_turnManager", turnManager);
                SetPrivateField(battleManager, "_staminaManager", staminaManager);
            }

            if (turnManager != null)
            {
                SetPrivateField(turnManager, "_dicePoolManager", dicePoolManager);
                SetPrivateField(turnManager, "_comboManager", comboManager);
                SetPrivateField(turnManager, "_waveController", waveController);
                SetPrivateField(turnManager, "_combatResolver", combatResolver);
                SetPrivateField(turnManager, "_shieldWallManager", shieldWallManager);
                SetPrivateField(turnManager, "_staminaManager", staminaManager);
                SetPrivateField(turnManager, "_player", playerWarrior);
            }

            if (combatResolver != null)
                SetPrivateField(combatResolver, "_waveController", waveController);

            if (shieldWallManager != null)
            {
                SetPrivateField(shieldWallManager, "_player", playerWarrior);
                var brotherVC = Object.FindFirstObjectByType<BrotherVisualController>();
                if (brotherVC != null)
                    SetPrivateField(shieldWallManager, "_brotherVisualController", brotherVC);
            }

            if (comboManager != null)
                SetPrivateField(comboManager, "_dicePoolManager", dicePoolManager);

            if (diceUI != null)
                SetPrivateField(diceUI, "_dicePoolManager", dicePoolManager);

            if (waveUI != null)
                SetPrivateField(waveUI, "_waveController", waveController);

            if (gameOverUI != null)
                SetPrivateField(gameOverUI, "_battleManager", battleManager);

            Debug.Log("Battle references wired");
        }

        [MenuItem("Shield Wall Builder/Battle Scene/Assign ScriptableObjects", false, 205)]
        public static void AssignAllScriptableObjects()
        {
            var comboManager = Object.FindFirstObjectByType<ComboManager>();
            var waveController = Object.FindFirstObjectByType<EnemyWaveController>();
            var shieldWallManager = Object.FindFirstObjectByType<ShieldWallManager>();
            var diceUI = Object.FindFirstObjectByType<DiceUI>();

            var actions = LoadAllAssets<ActionSO>("Assets/ScriptableObjects/Actions");
            var waves = LoadAllAssets<WaveConfigSO>("Assets/ScriptableObjects/Waves");
            var brothers = LoadAllAssets<ShieldBrotherSO>("Assets/ScriptableObjects/Brothers");
            var runes = LoadAllAssets<RuneSO>("Assets/ScriptableObjects/Runes");

            if (comboManager != null && actions.Length > 0)
                SetPrivateField(comboManager, "_allActions", actions);

            if (waveController != null && waves.Length > 0)
                SetPrivateField(waveController, "_waveConfigs", waves);

            if (shieldWallManager != null && brothers.Length > 0)
                SetPrivateField(shieldWallManager, "_brotherData", brothers);

            if (diceUI != null && runes.Length > 0)
                SetPrivateField(diceUI, "_runeData", runes);

            Debug.Log("ScriptableObjects assigned");
        }

        #endregion

        #region MainMenu Scene Setup

        [MenuItem("Shield Wall Builder/MainMenu Scene/Build Complete", false, 300)]
        public static void BuildMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateMainMenuCamera();
            CreateEventSystem();
            CreateSceneLoaderObject();
            CreateScenarioManagerObject();
            CreateMainMenuCanvas();

            EditorSceneManager.MarkSceneDirty(scene);
            
            string scenePath = "Assets/Scenes/MainMenu.unity";
            EnsureDirectoryExists("Assets/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);

            AddScenesToBuildSettings();

            Debug.Log("MainMenu scene created");
        }

        [MenuItem("Shield Wall Builder/MainMenu Scene/Create Prefabs", false, 301)]
        public static void CreateMenuPrefabs()
        {
            EnsureDirectoryExists("Assets/Prefabs/UI");
            CreateScenarioCardPrefab();
            CreatePauseMenuPrefab();
            AssetDatabase.SaveAssets();
            Debug.Log("Menu prefabs created");
        }

        [MenuItem("Shield Wall Builder/MainMenu Scene/Add Pause Menu to Battle", false, 302)]
        public static void AddPauseMenuToBattle()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            if (!scene.IsValid()) return;

            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            var pausePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/PauseMenu.prefab");
            if (pausePrefab != null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(pausePrefab, canvas.transform);
                instance.name = "PauseMenu";
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Pause menu added to Battle scene");
        }

        #endregion

        #region Validation

        [MenuItem("Shield Wall Builder/Validation/Validate All Assets", false, 400)]
        public static void ValidateAllAssets()
        {
            int errors = 0;

            var requiredEnemies = new[] { "Thrall", "Warrior", "Berserker", "Archer" };
            foreach (var name in requiredEnemies)
            {
                if (LoadEnemy(name) == null)
                {
                    Debug.LogError($"Missing enemy: {name}");
                    errors++;
                }
            }

            for (int i = 1; i <= 5; i++)
            {
                var wave = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"Assets/ScriptableObjects/Waves/Wave_{i:D2}.asset");
                if (wave == null)
                {
                    Debug.LogWarning($"Missing wave: Wave_{i:D2}");
                }
            }

            var scenarios = new[] { "TheBreach", "HoldTheLine", "TheLastStand" };
            foreach (var name in scenarios)
            {
                var s = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"Assets/ScriptableObjects/Scenarios/Scenario_{name}.asset");
                if (s == null)
                {
                    Debug.LogWarning($"Missing scenario: {name}");
                }
            }

            if (errors == 0)
                Debug.Log("All required assets validated successfully!");
            else
                Debug.LogError($"Validation found {errors} critical errors!");
        }

        #endregion

        #region Asset Creation Helpers

        private static void CreateRune(RuneType type, string displayName, Color color, string description)
        {
            string path = $"Assets/ScriptableObjects/Runes/Rune_{type}.asset";
            if (AssetDatabase.LoadAssetAtPath<RuneSO>(path) != null) return;

            var rune = ScriptableObject.CreateInstance<RuneSO>();
            rune.runeType = type;
            rune.runeName = displayName;
            rune.color = color;
            rune.description = description;
            AssetDatabase.CreateAsset(rune, path);
        }

        private static void CreateBrother(string name, string specialty)
        {
            string path = $"Assets/ScriptableObjects/Brothers/Brother_{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<ShieldBrotherSO>(path) != null) return;

            var brother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            brother.brotherName = name;
            brother.maxHealth = 3;
            brother.autoDefendChance = 0.5f;
            brother.specialty = specialty;
            AssetDatabase.CreateAsset(brother, path);
        }

        private static void CreateEnemy(string name, int health, int damage, EnemyTargetingType targeting,
            bool ignoresBlocks, bool destroysBlock, Color tintColor)
        {
            string path = $"Assets/ScriptableObjects/Enemies/Enemy_{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<EnemySO>(path) != null) return;

            var enemy = ScriptableObject.CreateInstance<EnemySO>();
            enemy.enemyName = name;
            enemy.health = health;
            enemy.damage = damage;
            enemy.targeting = targeting;
            enemy.ignoresBlocks = ignoresBlocks;
            enemy.destroysBlock = destroysBlock;
            enemy.tintColor = tintColor;
            AssetDatabase.CreateAsset(enemy, path);
        }

        private static void CreateAction(string name, RuneType[] runes, ActionEffectType effect, int power, string description)
        {
            string path = $"Assets/ScriptableObjects/Actions/Action_{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<ActionSO>(path) != null) return;

            var action = ScriptableObject.CreateInstance<ActionSO>();
            action.actionName = name;
            action.requiredRunes = runes;
            action.effectType = effect;
            action.effectPower = power;
            action.description = description;
            AssetDatabase.CreateAsset(action, path);
        }

        private static WaveConfigSO CreateWave(string assetName, int waveNumber, List<EnemySpawn> enemies,
            bool hasScriptedEvent = false, string scriptedEventId = "")
        {
            string path = $"Assets/ScriptableObjects/Waves/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<WaveConfigSO>(path);
            if (existing != null) return existing;

            var wave = ScriptableObject.CreateInstance<WaveConfigSO>();
            wave.waveNumber = waveNumber;
            wave.enemies = enemies;
            wave.hasScriptedEvent = hasScriptedEvent;
            wave.scriptedEventId = scriptedEventId;
            AssetDatabase.CreateAsset(wave, path);
            return wave;
        }

        private static BattleScenarioSO CreateScenario(string assetName, string scenarioName, string description,
            Difficulty difficulty, List<WaveConfigSO> waves, int stamina, int health, int dice,
            bool unlocked, BattleScenarioSO prerequisite)
        {
            string path = $"Assets/ScriptableObjects/Scenarios/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>(path);
            if (existing != null)
            {
                existing.scenarioName = scenarioName;
                existing.description = description;
                existing.difficulty = difficulty;
                existing.waves = waves;
                existing.startingStamina = stamina;
                existing.startingPlayerHealth = health;
                existing.startingDiceCount = dice;
                existing.isUnlocked = unlocked;
                existing.prerequisite = prerequisite;
                EditorUtility.SetDirty(existing);
                return existing;
            }

            var scenario = ScriptableObject.CreateInstance<BattleScenarioSO>();
            scenario.scenarioName = scenarioName;
            scenario.description = description;
            scenario.difficulty = difficulty;
            scenario.waves = waves;
            scenario.startingStamina = stamina;
            scenario.startingPlayerHealth = health;
            scenario.startingDiceCount = dice;
            scenario.isUnlocked = unlocked;
            scenario.prerequisite = prerequisite;
            AssetDatabase.CreateAsset(scenario, path);
            return scenario;
        }

        private static void CreateHint(string fileName, string hintId, string text,
            TurnPhase phase, int wave, bool requiresLocked, bool requiresNoLocked, float duration)
        {
            string path = $"Assets/ScriptableObjects/Tutorial/{fileName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<TutorialHintSO>(path);
            if (existing != null) return;

            var hint = ScriptableObject.CreateInstance<TutorialHintSO>();
            hint.hintId = hintId;
            hint.hintText = text;
            hint.triggerPhase = phase;
            hint.triggerWave = wave;
            hint.requiresDiceLocked = requiresLocked;
            hint.requiresNoDiceLocked = requiresNoLocked;
            hint.displayDuration = duration;
            hint.autoDismiss = true;
            hint.pauseGame = false;
            AssetDatabase.CreateAsset(hint, path);
        }

        private static EnemySO LoadEnemy(string name)
        {
            return AssetDatabase.LoadAssetAtPath<EnemySO>($"Assets/ScriptableObjects/Enemies/Enemy_{name}.asset");
        }

        #endregion

        #region Scene Setup Helpers

        private static void SetupLayers()
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            SetLayerName(layers, 6, "PlayerView");
            SetLayerName(layers, 7, "Brothers");
            SetLayerName(layers, 8, "Enemies");
            SetLayerName(layers, 9, "Environment");

            tagManager.ApplyModifiedProperties();
        }

        private static void SetLayerName(SerializedProperty layers, int index, string name)
        {
            var layer = layers.GetArrayElementAtIndex(index);
            if (string.IsNullOrEmpty(layer.stringValue))
                layer.stringValue = name;
        }

        private static void CreateVolumeProfile()
        {
            string path = "Assets/Settings/BattleVolumeProfile.asset";
            EnsureDirectoryExists("Assets/Settings");

            if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null) return;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();

            var colorGrading = profile.Add<ColorAdjustments>(true);
            colorGrading.saturation.Override(-15f);
            colorGrading.contrast.Override(10f);

            var vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(0.25f);
            vignette.smoothness.Override(0.4f);
            vignette.color.Override(new Color(0.16f, 0.09f, 0.06f));

            var bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(0.5f);
            bloom.threshold.Override(1f);
            bloom.scatter.Override(0.6f);

            var grain = profile.Add<FilmGrain>(true);
            grain.type.Override(FilmGrainLookup.Medium1);
            grain.intensity.Override(0.1f);

            AssetDatabase.CreateAsset(profile, path);
        }

        private static void AddVolumeToScene()
        {
            var existing = GameObject.Find("Global Volume");
            if (existing != null) Object.DestroyImmediate(existing);

            var volumeGO = new GameObject("Global Volume");
            var volume = volumeGO.AddComponent<Volume>();
            volume.isGlobal = true;

            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/BattleVolumeProfile.asset");
            if (profile != null)
                volume.profile = profile;
        }

        private static void SetupBattleLighting()
        {
            var existing = GameObject.Find("Directional Light");
            if (existing == null)
            {
                existing = new GameObject("Directional Light");
                existing.AddComponent<Light>();
            }

            var light = existing.GetComponent<Light>();
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            light.intensity = 0.8f;
            light.color = new Color(1f, 0.91f, 0.82f);
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.6f;

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);
        }

        private static void CreateGroundPlane()
        {
            var existing = GameObject.Find("Ground");
            if (existing != null) return;

            string matPath = "Assets/Art/Materials/Ground.mat";
            EnsureDirectoryExists("Assets/Art/Materials");

            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = new Color(0.29f, 0.22f, 0.16f);
                mat.SetFloat("_Smoothness", 0.1f);
                AssetDatabase.CreateAsset(mat, matPath);
            }

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
            ground.GetComponent<Renderer>().material = mat;
            ground.layer = 9;
            ground.isStatic = true;
        }

        private static void SetupFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 10f;
            RenderSettings.fogEndDistance = 50f;
            RenderSettings.fogColor = new Color(0.1f, 0.1f, 0.12f);
        }

        private static void CreateBattleCamera()
        {
            var existingCam = Camera.main;
            if (existingCam != null) Object.DestroyImmediate(existingCam.gameObject);

            var cameraGO = new GameObject("Main Camera");
            cameraGO.tag = "MainCamera";
            var camera = cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<AudioListener>();
            
            cameraGO.transform.position = new Vector3(0, 1.6f, 0);
            cameraGO.transform.rotation = Quaternion.identity;
            
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 60f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
        }

        private static void CreateFirstPersonArms()
        {
            var existing = GameObject.Find("PlayerArms");
            if (existing != null) Object.DestroyImmediate(existing);

            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning("No main camera found. Run CreateBattleCamera first.");
                return;
            }

            var playerArms = new GameObject("PlayerArms");
            playerArms.transform.SetParent(mainCamera.transform);
            playerArms.transform.localPosition = Vector3.zero;
            playerArms.transform.localRotation = Quaternion.identity;
            playerArms.layer = 6;

            var shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shield.name = "Shield";
            shield.transform.SetParent(playerArms.transform);
            shield.transform.localPosition = new Vector3(-0.4f, -0.3f, 0.6f);
            shield.transform.localScale = new Vector3(0.5f, 0.6f, 0.05f);
            shield.transform.localRotation = Quaternion.Euler(0, 15f, 0);
            shield.layer = 6;
            var shieldMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            shieldMat.color = new Color(0.4f, 0.25f, 0.1f);
            shield.GetComponent<Renderer>().material = shieldMat;
            Object.DestroyImmediate(shield.GetComponent<Collider>());

            var leftArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            leftArm.name = "LeftArm";
            leftArm.transform.SetParent(playerArms.transform);
            leftArm.transform.localPosition = new Vector3(-0.35f, -0.5f, 0.4f);
            leftArm.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            leftArm.transform.localRotation = Quaternion.Euler(60f, 0, -10f);
            leftArm.layer = 6;
            var armMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            armMat.color = new Color(0.7f, 0.55f, 0.4f);
            leftArm.GetComponent<Renderer>().material = armMat;
            Object.DestroyImmediate(leftArm.GetComponent<Collider>());

            var rightArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rightArm.name = "RightArm";
            rightArm.transform.SetParent(playerArms.transform);
            rightArm.transform.localPosition = new Vector3(0.35f, -0.5f, 0.4f);
            rightArm.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);
            rightArm.transform.localRotation = Quaternion.Euler(60f, 0, 10f);
            rightArm.layer = 6;
            rightArm.GetComponent<Renderer>().material = armMat;
            Object.DestroyImmediate(rightArm.GetComponent<Collider>());

            var weapon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            weapon.name = "Axe";
            weapon.transform.SetParent(rightArm.transform);
            weapon.transform.localPosition = new Vector3(0, 0.8f, 0);
            weapon.transform.localScale = new Vector3(0.15f, 0.6f, 0.02f);
            weapon.layer = 6;
            var axeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            axeMat.color = new Color(0.4f, 0.4f, 0.45f);
            weapon.GetComponent<Renderer>().material = axeMat;
            Object.DestroyImmediate(weapon.GetComponent<Collider>());

            var fpArms = playerArms.AddComponent<FirstPersonArms>();
            SetPrivateField(fpArms, "_leftArm", leftArm.transform);
            SetPrivateField(fpArms, "_rightArm", rightArm.transform);
            SetPrivateField(fpArms, "_shield", shield.transform);
        }

        private static void CreateVisualControllers()
        {
            var visualsGO = CreateOrFind("VisualControllers");

            var enemyVC = AddComponent<EnemyVisualController>(visualsGO);
            var brotherVC = AddComponent<BrotherVisualController>(visualsGO);

            Debug.Log("Visual controllers added: EnemyVisualController, BrotherVisualController");
        }

        private static void CreateEnemySpawnPoint()
        {
            var existing = GameObject.Find("EnemySpawnArea");
            if (existing != null) return;

            var spawnPoint = new GameObject("EnemySpawnArea");
            spawnPoint.transform.position = new Vector3(0, 1, 7);
        }

        private static void CreateBrotherSpawnMarkers()
        {
            CreateMarkerIfMissing("BrotherPos_FarLeft", new Vector3(-3, 0, 1));
            CreateMarkerIfMissing("BrotherPos_Left", new Vector3(-1.5f, 0, 1));
            CreateMarkerIfMissing("BrotherPos_Right", new Vector3(1.5f, 0, 1));
            CreateMarkerIfMissing("BrotherPos_FarRight", new Vector3(3, 0, 1));
        }

        private static void CreateMarkerIfMissing(string name, Vector3 position)
        {
            if (GameObject.Find(name) != null) return;
            var marker = new GameObject(name);
            marker.transform.position = position;
        }

        #endregion

        #region UI Setup Helpers

        private static void CreateUIPrefabs()
        {
            EnsureDirectoryExists("Assets/Prefabs/UI");
            CreateDieVisualPrefab();
            CreateActionButtonPrefab();
            CreateHealthHeartPrefab();
            AssetDatabase.SaveAssets();
        }

        private static void CreateDieVisualPrefab()
        {
            string path = "Assets/Prefabs/UI/DieVisual.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("DieVisual");
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80, 80);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var runeTextGO = new GameObject("RuneText");
            runeTextGO.transform.SetParent(go.transform, false);
            var runeRect = runeTextGO.AddComponent<RectTransform>();
            runeRect.anchorMin = Vector2.zero;
            runeRect.anchorMax = Vector2.one;
            runeRect.sizeDelta = Vector2.zero;
            var runeText = runeTextGO.AddComponent<TextMeshProUGUI>();
            runeText.text = "ᚦ";
            runeText.fontSize = 48;
            runeText.alignment = TextAlignmentOptions.Center;
            runeText.color = Color.white;

            var lockOverlayGO = new GameObject("LockOverlay");
            lockOverlayGO.transform.SetParent(go.transform, false);
            var lockRect = lockOverlayGO.AddComponent<RectTransform>();
            lockRect.anchorMin = Vector2.zero;
            lockRect.anchorMax = Vector2.one;
            lockRect.sizeDelta = Vector2.zero;
            var lockImage = lockOverlayGO.AddComponent<Image>();
            lockImage.color = new Color(1f, 0.8f, 0f, 0.3f);
            lockOverlayGO.SetActive(false);

            var dieVisual = go.AddComponent<DieVisual>();
            SetPrivateField(dieVisual, "_button", button);
            SetPrivateField(dieVisual, "_backgroundImage", image);
            SetPrivateField(dieVisual, "_runeText", runeText);
            SetPrivateField(dieVisual, "_lockOverlay", lockImage);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateActionButtonPrefab()
        {
            string path = "Assets/Prefabs/UI/ActionButton.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("ActionButton");
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(120, 60);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.4f, 0.3f, 0.2f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var nameTextGO = new GameObject("NameText");
            nameTextGO.transform.SetParent(go.transform, false);
            var nameRect = nameTextGO.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.sizeDelta = Vector2.zero;
            var nameText = nameTextGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Action";
            nameText.fontSize = 16;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;

            var runeTextGO = new GameObject("RuneText");
            runeTextGO.transform.SetParent(go.transform, false);
            var runeRect = runeTextGO.AddComponent<RectTransform>();
            runeRect.anchorMin = new Vector2(0, 0);
            runeRect.anchorMax = new Vector2(1, 0.5f);
            runeRect.sizeDelta = Vector2.zero;
            var runeText = runeTextGO.AddComponent<TextMeshProUGUI>();
            runeText.text = "ᚦᚦ";
            runeText.fontSize = 20;
            runeText.alignment = TextAlignmentOptions.Center;
            runeText.color = new Color(0.8f, 0.7f, 0.5f);

            var actionButton = go.AddComponent<ActionButton>();
            SetPrivateField(actionButton, "_button", button);
            SetPrivateField(actionButton, "_nameText", nameText);
            SetPrivateField(actionButton, "_runeRequirementText", runeText);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateHealthHeartPrefab()
        {
            string path = "Assets/Prefabs/UI/HealthHeart.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("HealthHeart");
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(24, 24);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.55f, 0.13f, 0.13f);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateBattleCanvas()
        {
            var existingCanvas = Object.FindFirstObjectByType<Canvas>();
            if (existingCanvas != null) Object.DestroyImmediate(existingCanvas.gameObject);

            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            CreateTopBar(canvasGO.transform);
            CreateDicePanel(canvasGO.transform);
            CreateActionsPanel(canvasGO.transform);
            CreateBottomBar(canvasGO.transform);
            CreateEndTurnButton(canvasGO.transform);
            CreateGameOverPanel(canvasGO.transform);

            CreateEventSystem();
        }

        private static void CreateTopBar(Transform parent)
        {
            var topBar = CreateUIPanel("TopBar", parent);
            var topRect = topBar.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0, 1);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.pivot = new Vector2(0.5f, 1);
            topRect.sizeDelta = new Vector2(0, 60);
            topRect.anchoredPosition = Vector2.zero;
            topBar.GetComponent<Image>().color = new Color(0.15f, 0.1f, 0.08f, 0.9f);

            var topLayout = topBar.AddComponent<HorizontalLayoutGroup>();
            topLayout.padding = new RectOffset(20, 20, 10, 10);
            topLayout.spacing = 20;
            topLayout.childAlignment = TextAnchor.MiddleCenter;
            topLayout.childControlWidth = false;
            topLayout.childControlHeight = true;

            var waveTextGO = new GameObject("WaveText");
            waveTextGO.transform.SetParent(topBar.transform, false);
            waveTextGO.AddComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
            var waveText = waveTextGO.AddComponent<TextMeshProUGUI>();
            waveText.text = "Wave 1/3";
            waveText.fontSize = 28;
            waveText.alignment = TextAlignmentOptions.Left;
            waveText.color = new Color(0.83f, 0.78f, 0.72f);
            var waveUI = waveTextGO.AddComponent<WaveUI>();
            SetPrivateField(waveUI, "_waveText", waveText);

            var spacer = new GameObject("Spacer");
            spacer.transform.SetParent(topBar.transform, false);
            spacer.AddComponent<RectTransform>().sizeDelta = new Vector2(400, 40);
            spacer.AddComponent<LayoutElement>().flexibleWidth = 1;

            var staminaPanel = CreateUIPanel("StaminaPanel", topBar.transform);
            staminaPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 40);
            staminaPanel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var staminaTextGO = new GameObject("StaminaText");
            staminaTextGO.transform.SetParent(staminaPanel.transform, false);
            var staminaTextRect = staminaTextGO.AddComponent<RectTransform>();
            staminaTextRect.anchorMin = Vector2.zero;
            staminaTextRect.anchorMax = Vector2.one;
            staminaTextRect.sizeDelta = Vector2.zero;
            var staminaText = staminaTextGO.AddComponent<TextMeshProUGUI>();
            staminaText.text = "Stamina: 12/12";
            staminaText.fontSize = 22;
            staminaText.alignment = TextAlignmentOptions.Center;
            staminaText.color = new Color(0.24f, 0.35f, 0.43f);

            var staminaUI = staminaPanel.AddComponent<StaminaUI>();
            SetPrivateField(staminaUI, "_staminaText", staminaText);
        }

        private static void CreateDicePanel(Transform parent)
        {
            var dicePanel = CreateUIPanel("DicePanel", parent);
            var diceRect = dicePanel.GetComponent<RectTransform>();
            diceRect.anchorMin = new Vector2(0.5f, 0);
            diceRect.anchorMax = new Vector2(0.5f, 0);
            diceRect.pivot = new Vector2(0.5f, 0);
            diceRect.sizeDelta = new Vector2(500, 120);
            diceRect.anchoredPosition = new Vector2(0, 180);
            dicePanel.GetComponent<Image>().color = new Color(0.2f, 0.15f, 0.1f, 0.9f);

            var diceContainer = new GameObject("DiceContainer");
            diceContainer.transform.SetParent(dicePanel.transform, false);
            var containerRect = diceContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0, 0.3f);
            containerRect.anchorMax = new Vector2(1, 1);
            containerRect.sizeDelta = Vector2.zero;
            containerRect.offsetMin = new Vector2(10, 0);
            containerRect.offsetMax = new Vector2(-10, -10);

            var layout = diceContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            var rerollButton = CreateButton("RerollButton", dicePanel.transform, "Reroll (2)");
            var rerollRect = rerollButton.GetComponent<RectTransform>();
            rerollRect.anchorMin = new Vector2(0.5f, 0);
            rerollRect.anchorMax = new Vector2(0.5f, 0);
            rerollRect.pivot = new Vector2(0.5f, 0);
            rerollRect.sizeDelta = new Vector2(120, 30);
            rerollRect.anchoredPosition = new Vector2(0, 5);

            var diceUI = dicePanel.AddComponent<DiceUI>();
            var diePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/DieVisual.prefab");
            if (diePrefab != null)
                SetPrivateField(diceUI, "_dieVisualPrefab", diePrefab.GetComponent<DieVisual>());
            SetPrivateField(diceUI, "_diceContainer", containerRect);
            SetPrivateField(diceUI, "_rerollButton", rerollButton.GetComponent<Button>());
            SetPrivateField(diceUI, "_rerollCountText", rerollButton.GetComponentInChildren<TextMeshProUGUI>());
        }

        private static void CreateActionsPanel(Transform parent)
        {
            var actionsPanel = CreateUIPanel("ActionsPanel", parent);
            var actionsRect = actionsPanel.GetComponent<RectTransform>();
            actionsRect.anchorMin = new Vector2(0.5f, 0);
            actionsRect.anchorMax = new Vector2(0.5f, 0);
            actionsRect.pivot = new Vector2(0.5f, 0);
            actionsRect.sizeDelta = new Vector2(700, 80);
            actionsRect.anchoredPosition = new Vector2(0, 310);
            actionsPanel.GetComponent<Image>().color = new Color(0.25f, 0.2f, 0.15f, 0.9f);

            var actionsContainer = new GameObject("ActionsContainer");
            actionsContainer.transform.SetParent(actionsPanel.transform, false);
            var containerRect = actionsContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.sizeDelta = Vector2.zero;
            containerRect.offsetMin = new Vector2(10, 10);
            containerRect.offsetMax = new Vector2(-10, -10);

            var layout = actionsContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            var actionUI = actionsPanel.AddComponent<ActionUI>();
            var actionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/ActionButton.prefab");
            if (actionPrefab != null)
                SetPrivateField(actionUI, "_actionButtonPrefab", actionPrefab.GetComponent<ActionButton>());
            SetPrivateField(actionUI, "_actionContainer", containerRect);
        }

        private static void CreateBottomBar(Transform parent)
        {
            var bottomBar = CreateUIPanel("BottomBar", parent);
            var bottomRect = bottomBar.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0, 0);
            bottomRect.anchorMax = new Vector2(1, 0);
            bottomRect.pivot = new Vector2(0.5f, 0);
            bottomRect.sizeDelta = new Vector2(0, 100);
            bottomRect.anchoredPosition = Vector2.zero;
            bottomBar.GetComponent<Image>().color = new Color(0.12f, 0.08f, 0.06f, 0.95f);

            var bottomLayout = bottomBar.AddComponent<HorizontalLayoutGroup>();
            bottomLayout.padding = new RectOffset(20, 20, 10, 10);
            bottomLayout.spacing = 20;
            bottomLayout.childAlignment = TextAnchor.MiddleCenter;

            var healthPanel = CreateUIPanel("PlayerHealthPanel", bottomBar.transform);
            healthPanel.GetComponent<Image>().color = new Color(0.2f, 0.15f, 0.1f, 0.8f);
            healthPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 60);
            healthPanel.AddComponent<LayoutElement>().preferredWidth = 200;

            var heartsContainer = new GameObject("HeartsContainer");
            heartsContainer.transform.SetParent(healthPanel.transform, false);
            var heartsRect = heartsContainer.AddComponent<RectTransform>();
            heartsRect.anchorMin = Vector2.zero;
            heartsRect.anchorMax = Vector2.one;
            heartsRect.sizeDelta = Vector2.zero;

            var heartsLayout = heartsContainer.AddComponent<HorizontalLayoutGroup>();
            heartsLayout.spacing = 5;
            heartsLayout.childAlignment = TextAnchor.MiddleCenter;
            heartsLayout.childControlWidth = false;
            heartsLayout.childControlHeight = false;

            var healthUI = healthPanel.AddComponent<HealthUI>();
            SetPrivateField(healthUI, "_heartsContainer", heartsRect);
            var heartPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/HealthHeart.prefab");
            if (heartPrefab != null)
                SetPrivateField(healthUI, "_heartPrefab", heartPrefab.GetComponent<Image>());
        }

        private static void CreateEndTurnButton(Transform parent)
        {
            var endTurnButton = CreateButton("EndTurnButton", parent, "END TURN");
            var endTurnRect = endTurnButton.GetComponent<RectTransform>();
            endTurnRect.anchorMin = new Vector2(1, 0);
            endTurnRect.anchorMax = new Vector2(1, 0);
            endTurnRect.pivot = new Vector2(1, 0);
            endTurnRect.sizeDelta = new Vector2(150, 50);
            endTurnRect.anchoredPosition = new Vector2(-20, 120);
            endTurnButton.GetComponent<Image>().color = new Color(0.55f, 0.13f, 0.13f);

            var actionSelectionManager = Object.FindFirstObjectByType<ActionSelectionManager>();
            if (actionSelectionManager != null)
                SetPrivateField(actionSelectionManager, "_endTurnButton", endTurnButton.GetComponent<Button>());
        }

        private static void CreateGameOverPanel(Transform parent)
        {
            var gameOverPanel = CreateUIPanel("GameOverPanel", parent);
            var goRect = gameOverPanel.GetComponent<RectTransform>();
            goRect.anchorMin = Vector2.zero;
            goRect.anchorMax = Vector2.one;
            goRect.sizeDelta = Vector2.zero;
            gameOverPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
            gameOverPanel.SetActive(false);

            var victoryPanel = CreateUIPanel("VictoryPanel", gameOverPanel.transform);
            var victoryRect = victoryPanel.GetComponent<RectTransform>();
            victoryRect.anchorMin = new Vector2(0.5f, 0.5f);
            victoryRect.anchorMax = new Vector2(0.5f, 0.5f);
            victoryRect.sizeDelta = new Vector2(400, 200);
            victoryPanel.GetComponent<Image>().color = new Color(0.2f, 0.4f, 0.2f, 0.95f);

            var victoryTitleGO = new GameObject("VictoryTitle");
            victoryTitleGO.transform.SetParent(victoryPanel.transform, false);
            var vtRect = victoryTitleGO.AddComponent<RectTransform>();
            vtRect.anchorMin = new Vector2(0, 0.6f);
            vtRect.anchorMax = new Vector2(1, 1);
            vtRect.sizeDelta = Vector2.zero;
            var victoryTitle = victoryTitleGO.AddComponent<TextMeshProUGUI>();
            victoryTitle.text = "VICTORY!";
            victoryTitle.fontSize = 48;
            victoryTitle.alignment = TextAlignmentOptions.Center;
            victoryTitle.color = new Color(0.79f, 0.64f, 0.15f);

            var victoryRestartBtn = CreateButton("RestartButton", victoryPanel.transform, "Fight Again");
            var vrRect = victoryRestartBtn.GetComponent<RectTransform>();
            vrRect.anchorMin = new Vector2(0.5f, 0);
            vrRect.anchorMax = new Vector2(0.5f, 0);
            vrRect.pivot = new Vector2(0.5f, 0);
            vrRect.sizeDelta = new Vector2(150, 40);
            vrRect.anchoredPosition = new Vector2(0, 20);

            var defeatPanel = CreateUIPanel("DefeatPanel", gameOverPanel.transform);
            var defeatRect = defeatPanel.GetComponent<RectTransform>();
            defeatRect.anchorMin = new Vector2(0.5f, 0.5f);
            defeatRect.anchorMax = new Vector2(0.5f, 0.5f);
            defeatRect.sizeDelta = new Vector2(400, 200);
            defeatPanel.GetComponent<Image>().color = new Color(0.4f, 0.15f, 0.15f, 0.95f);
            defeatPanel.SetActive(false);

            var defeatTitleGO = new GameObject("DefeatTitle");
            defeatTitleGO.transform.SetParent(defeatPanel.transform, false);
            var dtRect = defeatTitleGO.AddComponent<RectTransform>();
            dtRect.anchorMin = new Vector2(0, 0.6f);
            dtRect.anchorMax = new Vector2(1, 1);
            dtRect.sizeDelta = Vector2.zero;
            var defeatTitle = defeatTitleGO.AddComponent<TextMeshProUGUI>();
            defeatTitle.text = "DEFEAT";
            defeatTitle.fontSize = 48;
            defeatTitle.alignment = TextAlignmentOptions.Center;
            defeatTitle.color = new Color(0.8f, 0.2f, 0.2f);

            var defeatCauseGO = new GameObject("DefeatCause");
            defeatCauseGO.transform.SetParent(defeatPanel.transform, false);
            var dcRect = defeatCauseGO.AddComponent<RectTransform>();
            dcRect.anchorMin = new Vector2(0, 0.3f);
            dcRect.anchorMax = new Vector2(1, 0.6f);
            dcRect.sizeDelta = Vector2.zero;
            var defeatCause = defeatCauseGO.AddComponent<TextMeshProUGUI>();
            defeatCause.text = "The shield wall has fallen.";
            defeatCause.fontSize = 20;
            defeatCause.alignment = TextAlignmentOptions.Center;
            defeatCause.color = Color.white;

            var defeatRestartBtn = CreateButton("RestartButton", defeatPanel.transform, "Try Again");
            var drRect = defeatRestartBtn.GetComponent<RectTransform>();
            drRect.anchorMin = new Vector2(0.5f, 0);
            drRect.anchorMax = new Vector2(0.5f, 0);
            drRect.pivot = new Vector2(0.5f, 0);
            drRect.sizeDelta = new Vector2(150, 40);
            drRect.anchoredPosition = new Vector2(0, 20);

            var gameOverUI = gameOverPanel.AddComponent<GameOverUI>();
            SetPrivateField(gameOverUI, "_victoryPanel", victoryPanel);
            SetPrivateField(gameOverUI, "_defeatPanel", defeatPanel);
            SetPrivateField(gameOverUI, "_victoryTitleText", victoryTitle);
            SetPrivateField(gameOverUI, "_defeatTitleText", defeatTitle);
            SetPrivateField(gameOverUI, "_defeatCauseText", defeatCause);
            SetPrivateField(gameOverUI, "_victoryRestartButton", victoryRestartBtn.GetComponent<Button>());
            SetPrivateField(gameOverUI, "_defeatRestartButton", defeatRestartBtn.GetComponent<Button>());
        }

        #endregion

        #region MainMenu Helpers

        private static void CreateMainMenuCamera()
        {
            var cameraGO = new GameObject("Main Camera");
            var camera = cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<AudioListener>();
            cameraGO.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.08f, 0.06f);
        }

        private static void CreateEventSystem()
        {
            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        private static void CreateSceneLoaderObject()
        {
            var sceneLoaderGO = new GameObject("SceneLoader");
            sceneLoaderGO.AddComponent<SceneLoader>();
        }

        private static void CreateScenarioManagerObject()
        {
            var scenarioManagerGO = new GameObject("ScenarioManager");
            scenarioManagerGO.AddComponent<ScenarioManager>();
        }

        private static void CreateMainMenuCanvas()
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            var mainPanel = CreateUIPanel("MainPanel", canvasGO.transform);
            var mainRect = mainPanel.GetComponent<RectTransform>();
            mainRect.anchorMin = Vector2.zero;
            mainRect.anchorMax = Vector2.one;
            mainRect.sizeDelta = Vector2.zero;
            mainPanel.GetComponent<Image>().color = new Color(0.1f, 0.08f, 0.06f, 0.95f);

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(mainPanel.transform, false);
            var titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.sizeDelta = new Vector2(600, 100);
            var title = titleGO.AddComponent<TextMeshProUGUI>();
            title.text = "SHIELD WALL";
            title.fontSize = 72;
            title.alignment = TextAlignmentOptions.Center;
            title.color = new Color(0.79f, 0.64f, 0.15f);

            var buttonContainer = new GameObject("ButtonContainer");
            buttonContainer.transform.SetParent(mainPanel.transform, false);
            var buttonContainerRect = buttonContainer.AddComponent<RectTransform>();
            buttonContainerRect.anchorMin = new Vector2(0.5f, 0.2f);
            buttonContainerRect.anchorMax = new Vector2(0.5f, 0.6f);
            buttonContainerRect.sizeDelta = new Vector2(300, 300);

            var buttonLayout = buttonContainer.AddComponent<VerticalLayoutGroup>();
            buttonLayout.spacing = 20;
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = false;

            var playButton = CreateButton("PlayButton", buttonContainer.transform, "PLAY");
            playButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
            playButton.GetComponent<Image>().color = new Color(0.24f, 0.36f, 0.24f);

            var scenariosButton = CreateButton("ScenariosButton", buttonContainer.transform, "SCENARIOS");
            scenariosButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);

            var quitButton = CreateButton("QuitButton", buttonContainer.transform, "QUIT");
            quitButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
            quitButton.GetComponent<Image>().color = new Color(0.55f, 0.13f, 0.13f);

            var mainMenuUI = mainPanel.AddComponent<MainMenuUI>();
            SetPrivateField(mainMenuUI, "_playButton", playButton.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_scenariosButton", scenariosButton.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_quitButton", quitButton.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_mainPanel", mainPanel);
        }

        private static void CreateScenarioCardPrefab()
        {
            string path = "Assets/Prefabs/UI/ScenarioCard.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("ScenarioCard");
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300, 200);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.15f, 0.1f, 0.9f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var nameTextGO = new GameObject("NameText");
            nameTextGO.transform.SetParent(go.transform, false);
            var nameRect = nameTextGO.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.7f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.sizeDelta = Vector2.zero;
            var nameText = nameTextGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Scenario Name";
            nameText.fontSize = 24;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = new Color(0.79f, 0.64f, 0.15f);

            var descTextGO = new GameObject("DescriptionText");
            descTextGO.transform.SetParent(go.transform, false);
            var descRect = descTextGO.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.05f, 0.3f);
            descRect.anchorMax = new Vector2(0.95f, 0.7f);
            descRect.sizeDelta = Vector2.zero;
            var descText = descTextGO.AddComponent<TextMeshProUGUI>();
            descText.text = "Description";
            descText.fontSize = 14;
            descText.alignment = TextAlignmentOptions.Center;
            descText.color = Color.white;

            var diffTextGO = new GameObject("DifficultyText");
            diffTextGO.transform.SetParent(go.transform, false);
            var diffRect = diffTextGO.AddComponent<RectTransform>();
            diffRect.anchorMin = new Vector2(0, 0);
            diffRect.anchorMax = new Vector2(1, 0.2f);
            diffRect.sizeDelta = Vector2.zero;
            var diffText = diffTextGO.AddComponent<TextMeshProUGUI>();
            diffText.text = "Normal";
            diffText.fontSize = 18;
            diffText.alignment = TextAlignmentOptions.Center;
            diffText.color = new Color(0.79f, 0.64f, 0.15f);

            var scenarioCardUI = go.AddComponent<ScenarioCardUI>();
            SetPrivateField(scenarioCardUI, "_nameText", nameText);
            SetPrivateField(scenarioCardUI, "_descriptionText", descText);
            SetPrivateField(scenarioCardUI, "_difficultyText", diffText);
            SetPrivateField(scenarioCardUI, "_selectButton", button);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreatePauseMenuPrefab()
        {
            string path = "Assets/Prefabs/UI/PauseMenu.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null) return;

            var go = new GameObject("PauseMenu");
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;

            var canvasGroup = go.AddComponent<CanvasGroup>();

            var background = go.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.7f);

            var pausePanel = CreateUIPanel("PausePanel", go.transform);
            var panelRect = pausePanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(300, 250);
            pausePanel.GetComponent<Image>().color = new Color(0.15f, 0.1f, 0.08f, 0.95f);

            var pauseTitle = new GameObject("Title");
            pauseTitle.transform.SetParent(pausePanel.transform, false);
            var titleRect = pauseTitle.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = Vector2.zero;
            var titleText = pauseTitle.AddComponent<TextMeshProUGUI>();
            titleText.text = "PAUSED";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.79f, 0.64f, 0.15f);

            var buttonContainer = new GameObject("Buttons");
            buttonContainer.transform.SetParent(pausePanel.transform, false);
            var buttonContainerRect = buttonContainer.AddComponent<RectTransform>();
            buttonContainerRect.anchorMin = new Vector2(0.1f, 0.1f);
            buttonContainerRect.anchorMax = new Vector2(0.9f, 0.75f);
            buttonContainerRect.sizeDelta = Vector2.zero;

            var buttonLayout = buttonContainer.AddComponent<VerticalLayoutGroup>();
            buttonLayout.spacing = 15;
            buttonLayout.childAlignment = TextAnchor.MiddleCenter;
            buttonLayout.childControlWidth = true;
            buttonLayout.childControlHeight = false;

            var resumeButton = CreateButton("ResumeButton", buttonContainer.transform, "Resume");
            resumeButton.GetComponent<Image>().color = new Color(0.24f, 0.36f, 0.24f);

            var restartButton = CreateButton("RestartButton", buttonContainer.transform, "Restart");

            var mainMenuButton = CreateButton("MainMenuButton", buttonContainer.transform, "Main Menu");
            mainMenuButton.GetComponent<Image>().color = new Color(0.55f, 0.13f, 0.13f);

            var pauseMenuUI = go.AddComponent<PauseMenuUI>();
            SetPrivateField(pauseMenuUI, "_pausePanel", go);
            SetPrivateField(pauseMenuUI, "_resumeButton", resumeButton.GetComponent<Button>());
            SetPrivateField(pauseMenuUI, "_restartButton", restartButton.GetComponent<Button>());
            SetPrivateField(pauseMenuUI, "_mainMenuButton", mainMenuButton.GetComponent<Button>());

            go.SetActive(false);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void AddScenesToBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            string mainMenuPath = "Assets/Scenes/MainMenu.unity";
            string battlePath = "Assets/Scenes/Battle.unity";

            bool hasMainMenu = false;
            bool hasBattle = false;

            foreach (var scene in scenes)
            {
                if (scene.path == mainMenuPath) hasMainMenu = true;
                if (scene.path == battlePath) hasBattle = true;
            }

            if (!hasMainMenu)
                scenes.Insert(0, new EditorBuildSettingsScene(mainMenuPath, true));
            if (!hasBattle)
                scenes.Add(new EditorBuildSettingsScene(battlePath, true));

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        #endregion

        #region General Helpers

        private static GameObject CreateOrFind(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;
            return new GameObject(name);
        }

        private static T AddComponent<T>(GameObject go) where T : Component
        {
            var existing = go.GetComponent<T>();
            if (existing != null) return existing;
            return go.AddComponent<T>();
        }

        private static GameObject CreateUIPanel(string name, Transform parent)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string text)
        {
            var buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            var rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 40);

            var image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.3f, 0.25f, 0.2f);

            var button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return buttonGO;
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            if (obj == null) return;
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
                EditorUtility.SetDirty(obj as Object);
            }
        }

        private static T[] LoadAllAssets<T>(string folderPath) where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });
            var assets = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return assets;
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folder = Path.GetFileName(path);
                if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(folder))
                {
                    if (!AssetDatabase.IsValidFolder(parent))
                        EnsureDirectoryExists(parent);
                    AssetDatabase.CreateFolder(parent, folder);
                }
            }
        }

        #endregion
    }
}

