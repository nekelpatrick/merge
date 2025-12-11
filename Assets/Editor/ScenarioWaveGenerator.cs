using UnityEngine;
using UnityEditor;
using ShieldWall.Data;
using System.Collections.Generic;
using System.IO;

namespace ShieldWall.Editor
{
    [System.Obsolete("Use ShieldWallSceneBuilder instead. This class is deprecated and will be removed in a future version.")]
    public static class ScenarioWaveGenerator
    {
        private const string WavesPath = "Assets/ScriptableObjects/Waves";
        private const string ScenariosPath = "Assets/ScriptableObjects/Scenarios";
        private const string EnemiesPath = "Assets/ScriptableObjects/Enemies";

        // [MenuItem("Shield Wall/Content/Generate All Scenarios and Waves")]
        public static void GenerateAll()
        {
            EnsureDirectoriesExist();
            GenerateEasyWaves();
            GenerateHardWaves();
            GenerateScenarios();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Track C: All scenarios and waves generated successfully!");
        }

        // [MenuItem("Shield Wall/Content/Generate Easy Waves Only")]
        public static void GenerateEasyWaves()
        {
            EnsureDirectoriesExist();
            
            var thrall = LoadEnemy("Enemy_Thrall");
            var warrior = LoadEnemy("Enemy_Warrior");

            if (thrall == null || warrior == null)
            {
                Debug.LogError("Could not load required enemy assets!");
                return;
            }

            CreateWave("Wave_Easy_01", 1, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = thrall, count = 2 }
            }, true, "tutorial_dice");

            CreateWave("Wave_Easy_02", 2, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = thrall, count = 3 }
            });

            CreateWave("Wave_Easy_03", 3, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = thrall, count = 2 },
                new EnemySpawn { enemy = warrior, count = 1 }
            });

            Debug.Log("Easy waves generated: Wave_Easy_01, Wave_Easy_02, Wave_Easy_03");
        }

        // [MenuItem("Shield Wall/Content/Generate Hard Waves Only")]
        public static void GenerateHardWaves()
        {
            EnsureDirectoriesExist();
            
            var warrior = LoadEnemy("Enemy_Warrior");
            var berserker = LoadEnemy("Enemy_Berserker");
            var archer = LoadEnemy("Enemy_Archer");

            if (warrior == null || berserker == null || archer == null)
            {
                Debug.LogError("Could not load required enemy assets!");
                return;
            }

            CreateWave("Wave_Hard_01", 1, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = warrior, count = 2 },
                new EnemySpawn { enemy = berserker, count = 1 }
            });

            CreateWave("Wave_Hard_02", 2, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = berserker, count = 3 }
            });

            CreateWave("Wave_Hard_03", 3, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = archer, count = 2 },
                new EnemySpawn { enemy = warrior, count = 2 }
            });

            CreateWave("Wave_Hard_04", 4, new List<EnemySpawn>
            {
                new EnemySpawn { enemy = berserker, count = 4 }
            });

            Debug.Log("Hard waves generated: Wave_Hard_01, Wave_Hard_02, Wave_Hard_03, Wave_Hard_04");
        }

        // [MenuItem("Shield Wall/Content/Generate Scenarios Only")]
        public static void GenerateScenarios()
        {
            EnsureDirectoriesExist();

            var holdTheLine = CreateScenario(
                "Scenario_HoldTheLine",
                "Hold the Line",
                "The enemy comes in force. Your shield wall is all that stands between them and your people.",
                Difficulty.Normal,
                new string[] { "Wave_01", "Wave_02", "Wave_03", "Wave_04", "Wave_05" },
                startingStamina: 12,
                startingPlayerHealth: 5,
                startingDiceCount: 4,
                isUnlocked: true
            );

            CreateScenario(
                "Scenario_TheBreach",
                "The Breach",
                "Raiders have broken through the outer wall. Hold them off while the village evacuates. A good place to learn the basics.",
                Difficulty.Easy,
                new string[] { "Wave_Easy_01", "Wave_Easy_02", "Wave_Easy_03" },
                startingStamina: 15,
                startingPlayerHealth: 6,
                startingDiceCount: 5,
                isUnlocked: true
            );

            CreateScenario(
                "Scenario_TheLastStand",
                "The Last Stand",
                "Berserkers and archers. Few supplies. No retreat. Only glory or death awaits.",
                Difficulty.Hard,
                new string[] { "Wave_Hard_01", "Wave_Hard_02", "Wave_03", "Wave_Hard_03", "Wave_04", "Wave_Hard_04", "Wave_05" },
                startingStamina: 10,
                startingPlayerHealth: 4,
                startingDiceCount: 4,
                isUnlocked: false,
                prerequisite: holdTheLine
            );

            Debug.Log("Scenarios generated: Scenario_TheBreach, Scenario_HoldTheLine, Scenario_TheLastStand");
        }

        private static void EnsureDirectoriesExist()
        {
            if (!Directory.Exists(ScenariosPath))
            {
                Directory.CreateDirectory(ScenariosPath);
                AssetDatabase.Refresh();
                Debug.Log($"Created directory: {ScenariosPath}");
            }
        }

        private static EnemySO LoadEnemy(string name)
        {
            string path = $"{EnemiesPath}/{name}.asset";
            var enemy = AssetDatabase.LoadAssetAtPath<EnemySO>(path);
            if (enemy == null)
            {
                Debug.LogWarning($"Could not load enemy at path: {path}");
            }
            return enemy;
        }

        private static WaveConfigSO LoadWave(string name)
        {
            string path = $"{WavesPath}/{name}.asset";
            var wave = AssetDatabase.LoadAssetAtPath<WaveConfigSO>(path);
            if (wave == null)
            {
                Debug.LogWarning($"Could not load wave at path: {path}");
            }
            return wave;
        }

        private static WaveConfigSO CreateWave(
            string assetName,
            int waveNumber,
            List<EnemySpawn> enemies,
            bool hasScriptedEvent = false,
            string scriptedEventId = "")
        {
            string path = $"{WavesPath}/{assetName}.asset";
            
            var existing = AssetDatabase.LoadAssetAtPath<WaveConfigSO>(path);
            if (existing != null)
            {
                existing.waveNumber = waveNumber;
                existing.enemies = enemies;
                existing.hasScriptedEvent = hasScriptedEvent;
                existing.scriptedEventId = scriptedEventId;
                EditorUtility.SetDirty(existing);
                return existing;
            }

            var wave = ScriptableObject.CreateInstance<WaveConfigSO>();
            wave.waveNumber = waveNumber;
            wave.enemies = enemies;
            wave.hasScriptedEvent = hasScriptedEvent;
            wave.scriptedEventId = scriptedEventId;

            AssetDatabase.CreateAsset(wave, path);
            return wave;
        }

        private static BattleScenarioSO CreateScenario(
            string assetName,
            string scenarioName,
            string description,
            Difficulty difficulty,
            string[] waveNames,
            int startingStamina,
            int startingPlayerHealth,
            int startingDiceCount,
            bool isUnlocked,
            BattleScenarioSO prerequisite = null)
        {
            string path = $"{ScenariosPath}/{assetName}.asset";

            var waves = new List<WaveConfigSO>();
            foreach (var waveName in waveNames)
            {
                var wave = LoadWave(waveName);
                if (wave != null)
                {
                    waves.Add(wave);
                }
                else
                {
                    Debug.LogWarning($"Scenario {scenarioName}: Could not find wave {waveName}");
                }
            }

            var existing = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>(path);
            if (existing != null)
            {
                existing.scenarioName = scenarioName;
                existing.description = description;
                existing.difficulty = difficulty;
                existing.waves = waves;
                existing.startingStamina = startingStamina;
                existing.startingPlayerHealth = startingPlayerHealth;
                existing.startingDiceCount = startingDiceCount;
                existing.isUnlocked = isUnlocked;
                existing.prerequisite = prerequisite;
                EditorUtility.SetDirty(existing);
                return existing;
            }

            var scenario = ScriptableObject.CreateInstance<BattleScenarioSO>();
            scenario.scenarioName = scenarioName;
            scenario.description = description;
            scenario.difficulty = difficulty;
            scenario.waves = waves;
            scenario.startingStamina = startingStamina;
            scenario.startingPlayerHealth = startingPlayerHealth;
            scenario.startingDiceCount = startingDiceCount;
            scenario.isUnlocked = isUnlocked;
            scenario.prerequisite = prerequisite;

            AssetDatabase.CreateAsset(scenario, path);
            return scenario;
        }
    }
}

