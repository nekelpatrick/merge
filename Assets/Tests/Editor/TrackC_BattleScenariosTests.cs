using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using ShieldWall.Data;
using ShieldWall.Core;
using System.Collections.Generic;
using System.IO;

namespace ShieldWall.Tests.Editor
{
    [TestFixture]
    public class BattleScenarioSOTests
    {
        private BattleScenarioSO _scenario;

        [SetUp]
        public void SetUp()
        {
            _scenario = ScriptableObject.CreateInstance<BattleScenarioSO>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_scenario != null)
            {
                Object.DestroyImmediate(_scenario);
            }
        }

        [Test]
        public void BattleScenarioSO_HasDefaultValues()
        {
            Assert.AreEqual("New Scenario", _scenario.scenarioName);
            Assert.AreEqual(Difficulty.Normal, _scenario.difficulty);
            Assert.AreEqual(12, _scenario.startingStamina);
            Assert.AreEqual(5, _scenario.startingPlayerHealth);
            Assert.AreEqual(4, _scenario.startingDiceCount);
            Assert.IsTrue(_scenario.isUnlocked);
            Assert.IsNull(_scenario.prerequisite);
        }

        [Test]
        public void BattleScenarioSO_CanSetStartingConditions()
        {
            _scenario.startingStamina = 15;
            _scenario.startingPlayerHealth = 6;
            _scenario.startingDiceCount = 5;

            Assert.AreEqual(15, _scenario.startingStamina);
            Assert.AreEqual(6, _scenario.startingPlayerHealth);
            Assert.AreEqual(5, _scenario.startingDiceCount);
        }

        [Test]
        public void BattleScenarioSO_CanSetPrerequisite()
        {
            var prerequisite = ScriptableObject.CreateInstance<BattleScenarioSO>();
            prerequisite.scenarioName = "Prerequisite Scenario";

            _scenario.prerequisite = prerequisite;
            _scenario.isUnlocked = false;

            Assert.IsFalse(_scenario.isUnlocked);
            Assert.IsNotNull(_scenario.prerequisite);
            Assert.AreEqual("Prerequisite Scenario", _scenario.prerequisite.scenarioName);

            Object.DestroyImmediate(prerequisite);
        }

        [Test]
        public void BattleScenarioSO_WavesListInitializesEmpty()
        {
            Assert.IsNotNull(_scenario.waves);
            Assert.AreEqual(0, _scenario.waves.Count);
        }

        [Test]
        public void BattleScenarioSO_CanAddWaves()
        {
            var wave1 = ScriptableObject.CreateInstance<WaveConfigSO>();
            var wave2 = ScriptableObject.CreateInstance<WaveConfigSO>();

            _scenario.waves.Add(wave1);
            _scenario.waves.Add(wave2);

            Assert.AreEqual(2, _scenario.waves.Count);

            Object.DestroyImmediate(wave1);
            Object.DestroyImmediate(wave2);
        }

        [Test]
        public void Difficulty_HasCorrectValues()
        {
            Assert.AreEqual(0, (int)Difficulty.Easy);
            Assert.AreEqual(1, (int)Difficulty.Normal);
            Assert.AreEqual(2, (int)Difficulty.Hard);
        }
    }

    [TestFixture]
    public class ScenarioManagerTests
    {
        private GameObject _managerObject;
        private ScenarioManager _manager;

        [SetUp]
        public void SetUp()
        {
            var existing = Object.FindFirstObjectByType<ScenarioManager>();
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            _managerObject = new GameObject("TestScenarioManager");
            _manager = _managerObject.AddComponent<ScenarioManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_managerObject != null)
            {
                Object.DestroyImmediate(_managerObject);
            }
            
            if (ScenarioManager.Instance != null && ScenarioManager.Instance.gameObject != _managerObject)
            {
                Object.DestroyImmediate(ScenarioManager.Instance.gameObject);
            }
        }

        [Test]
        public void ScenarioManager_CanSelectScenario()
        {
            var scenario = ScriptableObject.CreateInstance<BattleScenarioSO>();
            scenario.scenarioName = "Test Scenario";

            _manager.SelectScenario(scenario);

            Assert.AreEqual("Test Scenario", _manager.SelectedScenario.scenarioName);

            Object.DestroyImmediate(scenario);
        }

        [Test]
        public void ScenarioManager_ClearSelectionWorks()
        {
            var scenario = ScriptableObject.CreateInstance<BattleScenarioSO>();
            _manager.SelectScenario(scenario);
            
            _manager.ClearSelection();

            Assert.IsNull(_manager.SelectedScenario);

            Object.DestroyImmediate(scenario);
        }
    }

    [TestFixture]
    public class WaveConfigSOTests
    {
        private WaveConfigSO _wave;

        [SetUp]
        public void SetUp()
        {
            _wave = ScriptableObject.CreateInstance<WaveConfigSO>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_wave != null)
            {
                Object.DestroyImmediate(_wave);
            }
        }

        [Test]
        public void WaveConfigSO_HasDefaultWaveNumber()
        {
            Assert.AreEqual(1, _wave.waveNumber);
        }

        [Test]
        public void WaveConfigSO_EnemiesListInitializesEmpty()
        {
            Assert.IsNotNull(_wave.enemies);
            Assert.AreEqual(0, _wave.enemies.Count);
        }

        [Test]
        public void WaveConfigSO_CanSetScriptedEvent()
        {
            _wave.hasScriptedEvent = true;
            _wave.scriptedEventId = "tutorial_dice";

            Assert.IsTrue(_wave.hasScriptedEvent);
            Assert.AreEqual("tutorial_dice", _wave.scriptedEventId);
        }

        [Test]
        public void EnemySpawn_CanBeCreated()
        {
            var spawn = new EnemySpawn();
            spawn.count = 3;

            Assert.AreEqual(3, spawn.count);
            Assert.IsNull(spawn.enemy);
        }
    }

    [TestFixture]
    public class ScenarioAssetValidationTests
    {
        private const string EnemiesPath = "Assets/ScriptableObjects/Enemies";
        private const string WavesPath = "Assets/ScriptableObjects/Waves";

        [Test]
        public void RequiredEnemyAssets_Exist()
        {
            var requiredEnemies = new[] { "Enemy_Thrall", "Enemy_Warrior", "Enemy_Berserker", "Enemy_Archer" };

            foreach (var enemyName in requiredEnemies)
            {
                var path = $"{EnemiesPath}/{enemyName}.asset";
                var enemy = AssetDatabase.LoadAssetAtPath<EnemySO>(path);
                Assert.IsNotNull(enemy, $"Missing required enemy asset: {enemyName}");
            }
        }

        [Test]
        public void ExistingWaveAssets_AreValid()
        {
            var existingWaves = new[] { "Wave_01", "Wave_02", "Wave_03", "Wave_04", "Wave_05" };

            foreach (var waveName in existingWaves)
            {
                var path = $"{WavesPath}/{waveName}.asset";
                var wave = AssetDatabase.LoadAssetAtPath<WaveConfigSO>(path);
                Assert.IsNotNull(wave, $"Missing existing wave asset: {waveName}");
                Assert.Greater(wave.enemies.Count, 0, $"Wave {waveName} has no enemies");
            }
        }

        [Test]
        public void EnemySO_ThrallHasCorrectStats()
        {
            var thrall = AssetDatabase.LoadAssetAtPath<EnemySO>($"{EnemiesPath}/Enemy_Thrall.asset");
            Assert.IsNotNull(thrall);
            Assert.AreEqual("Thrall", thrall.enemyName);
        }

        [Test]
        public void EnemySO_WarriorHasCorrectStats()
        {
            var warrior = AssetDatabase.LoadAssetAtPath<EnemySO>($"{EnemiesPath}/Enemy_Warrior.asset");
            Assert.IsNotNull(warrior);
            Assert.AreEqual("Warrior", warrior.enemyName);
        }

        [Test]
        public void EnemySO_BerserkerHasCorrectStats()
        {
            var berserker = AssetDatabase.LoadAssetAtPath<EnemySO>($"{EnemiesPath}/Enemy_Berserker.asset");
            Assert.IsNotNull(berserker);
            Assert.AreEqual("Berserker", berserker.enemyName);
        }

        [Test]
        public void EnemySO_ArcherHasCorrectStats()
        {
            var archer = AssetDatabase.LoadAssetAtPath<EnemySO>($"{EnemiesPath}/Enemy_Archer.asset");
            Assert.IsNotNull(archer);
            Assert.AreEqual("Archer", archer.enemyName);
        }
    }

    [TestFixture]
    public class ScenarioGeneratorIntegrationTests
    {
        private const string ScenariosPath = "Assets/ScriptableObjects/Scenarios";
        private const string WavesPath = "Assets/ScriptableObjects/Waves";

        private List<string> _createdAssets = new List<string>();

        [TearDown]
        public void TearDown()
        {
            foreach (var path in _createdAssets)
            {
                if (File.Exists(path))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
            _createdAssets.Clear();
            AssetDatabase.Refresh();
        }

        [Test]
        public void GenerateEasyWaves_CreatesThreeWaves()
        {
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateEasyWaves();
            AssetDatabase.Refresh();

            var wave1 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Easy_01.asset");
            var wave2 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Easy_02.asset");
            var wave3 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Easy_03.asset");

            Assert.IsNotNull(wave1, "Wave_Easy_01 should be created");
            Assert.IsNotNull(wave2, "Wave_Easy_02 should be created");
            Assert.IsNotNull(wave3, "Wave_Easy_03 should be created");

            Assert.AreEqual(1, wave1.waveNumber);
            Assert.AreEqual(2, wave2.waveNumber);
            Assert.AreEqual(3, wave3.waveNumber);

            Assert.IsTrue(wave1.hasScriptedEvent);
            Assert.AreEqual("tutorial_dice", wave1.scriptedEventId);

            _createdAssets.Add($"{WavesPath}/Wave_Easy_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_03.asset");
        }

        [Test]
        public void GenerateHardWaves_CreatesFourWaves()
        {
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateHardWaves();
            AssetDatabase.Refresh();

            var wave1 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Hard_01.asset");
            var wave2 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Hard_02.asset");
            var wave3 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Hard_03.asset");
            var wave4 = AssetDatabase.LoadAssetAtPath<WaveConfigSO>($"{WavesPath}/Wave_Hard_04.asset");

            Assert.IsNotNull(wave1, "Wave_Hard_01 should be created");
            Assert.IsNotNull(wave2, "Wave_Hard_02 should be created");
            Assert.IsNotNull(wave3, "Wave_Hard_03 should be created");
            Assert.IsNotNull(wave4, "Wave_Hard_04 should be created");

            _createdAssets.Add($"{WavesPath}/Wave_Hard_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_03.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_04.asset");
        }

        [Test]
        public void GenerateScenarios_CreatesThreeScenarios()
        {
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateEasyWaves();
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateHardWaves();
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateScenarios();
            AssetDatabase.Refresh();

            var theBreach = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"{ScenariosPath}/Scenario_TheBreach.asset");
            var holdTheLine = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"{ScenariosPath}/Scenario_HoldTheLine.asset");
            var theLastStand = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"{ScenariosPath}/Scenario_TheLastStand.asset");

            Assert.IsNotNull(theBreach, "Scenario_TheBreach should be created");
            Assert.IsNotNull(holdTheLine, "Scenario_HoldTheLine should be created");
            Assert.IsNotNull(theLastStand, "Scenario_TheLastStand should be created");

            Assert.AreEqual("The Breach", theBreach.scenarioName);
            Assert.AreEqual(Difficulty.Easy, theBreach.difficulty);
            Assert.AreEqual(15, theBreach.startingStamina);
            Assert.AreEqual(6, theBreach.startingPlayerHealth);
            Assert.AreEqual(5, theBreach.startingDiceCount);
            Assert.IsTrue(theBreach.isUnlocked);

            Assert.AreEqual("Hold the Line", holdTheLine.scenarioName);
            Assert.AreEqual(Difficulty.Normal, holdTheLine.difficulty);
            Assert.AreEqual(12, holdTheLine.startingStamina);
            Assert.IsTrue(holdTheLine.isUnlocked);

            Assert.AreEqual("The Last Stand", theLastStand.scenarioName);
            Assert.AreEqual(Difficulty.Hard, theLastStand.difficulty);
            Assert.AreEqual(10, theLastStand.startingStamina);
            Assert.AreEqual(4, theLastStand.startingPlayerHealth);
            Assert.IsFalse(theLastStand.isUnlocked);
            Assert.IsNotNull(theLastStand.prerequisite);
            Assert.AreEqual("Hold the Line", theLastStand.prerequisite.scenarioName);

            _createdAssets.Add($"{WavesPath}/Wave_Easy_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_03.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_03.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_04.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheBreach.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_HoldTheLine.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheLastStand.asset");
        }

        [Test]
        public void TheBreach_HasCorrectWaveCount()
        {
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateEasyWaves();
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateScenarios();
            AssetDatabase.Refresh();

            var theBreach = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"{ScenariosPath}/Scenario_TheBreach.asset");
            
            Assert.IsNotNull(theBreach);
            Assert.AreEqual(3, theBreach.waves.Count, "The Breach should have 3 waves");

            _createdAssets.Add($"{WavesPath}/Wave_Easy_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_03.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheBreach.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_HoldTheLine.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheLastStand.asset");
        }

        [Test]
        public void HoldTheLine_HasCorrectWaveCount()
        {
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateScenarios();
            AssetDatabase.Refresh();

            var holdTheLine = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"{ScenariosPath}/Scenario_HoldTheLine.asset");
            
            Assert.IsNotNull(holdTheLine);
            Assert.AreEqual(5, holdTheLine.waves.Count, "Hold the Line should have 5 waves");

            _createdAssets.Add($"{ScenariosPath}/Scenario_TheBreach.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_HoldTheLine.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheLastStand.asset");
        }

        [Test]
        public void TheLastStand_HasCorrectWaveCount()
        {
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateEasyWaves();
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateHardWaves();
            ShieldWall.Editor.ScenarioWaveGenerator.GenerateScenarios();
            AssetDatabase.Refresh();

            var theLastStand = AssetDatabase.LoadAssetAtPath<BattleScenarioSO>($"{ScenariosPath}/Scenario_TheLastStand.asset");
            
            Assert.IsNotNull(theLastStand);
            Assert.AreEqual(7, theLastStand.waves.Count, "The Last Stand should have 7 waves");

            _createdAssets.Add($"{WavesPath}/Wave_Easy_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Easy_03.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_01.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_02.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_03.asset");
            _createdAssets.Add($"{WavesPath}/Wave_Hard_04.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheBreach.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_HoldTheLine.asset");
            _createdAssets.Add($"{ScenariosPath}/Scenario_TheLastStand.asset");
        }
    }
}

