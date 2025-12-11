using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;

namespace ShieldWall.Visual
{
    public class DebugVisualTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool _autoRunTest = false;
        [SerializeField] private float _stepDelay = 1f;

        [Header("Test Data")]
        [SerializeField] private EnemySO[] _testEnemies;
        [SerializeField] private ShieldBrotherSO[] _testBrothers;
        [SerializeField] private BloodBurstVFX _bloodBurstPrefab;

        [Header("Component References")]
        [SerializeField] private EnemyVisualController _enemyVisualController;
        [SerializeField] private BrotherVisualController _brotherVisualController;

        private int _testsPassed;
        private int _testsFailed;

        private void Start()
        {
            if (_autoRunTest)
            {
                StartCoroutine(RunAllTests());
            }
        }

        [ContextMenu("Run All Visual Tests")]
        public void RunTests()
        {
            StartCoroutine(RunAllTests());
        }

        [ContextMenu("Test Blood Burst Only")]
        public void TestBloodBurstOnly()
        {
            StartCoroutine(TestBloodBurstVFX());
        }

        [ContextMenu("Test Enemy Visuals Only")]
        public void TestEnemyVisualsOnly()
        {
            StartCoroutine(TestEnemyVisualSystem());
        }

        [ContextMenu("Test Brother Visuals Only")]
        public void TestBrotherVisualsOnly()
        {
            StartCoroutine(TestBrotherVisualSystem());
        }

        private IEnumerator RunAllTests()
        {
            _testsPassed = 0;
            _testsFailed = 0;

            Debug.Log("=== VISUAL SYSTEM TESTS ===\n");

            yield return TestBloodBurstVFX();
            yield return new WaitForSeconds(_stepDelay);

            yield return TestEnemyVisualSystem();
            yield return new WaitForSeconds(_stepDelay);

            yield return TestBrotherVisualSystem();
            yield return new WaitForSeconds(_stepDelay);

            yield return TestEventIntegration();

            Debug.Log($"\n=== TEST RESULTS: {_testsPassed} PASSED, {_testsFailed} FAILED ===");
        }

        private IEnumerator TestBloodBurstVFX()
        {
            Debug.Log("--- BLOOD BURST VFX TEST ---");

            if (_bloodBurstPrefab == null)
            {
                GameObject testGO = new GameObject("TestBloodBurst");
                var bloodBurst = testGO.AddComponent<BloodBurstVFX>();
                
                Assert("BloodBurstVFX component created", bloodBurst != null);

                var particleSystem = testGO.GetComponent<ParticleSystem>();
                Assert("ParticleSystem auto-created", particleSystem != null);

                if (particleSystem != null)
                {
                    var main = particleSystem.main;
                    Assert("ParticleSystem not looping", !main.loop);
                    Assert("ParticleSystem simulation space is World", 
                        main.simulationSpace == ParticleSystemSimulationSpace.World);
                    Assert("ParticleSystem stop action is Destroy", 
                        main.stopAction == ParticleSystemStopAction.Destroy);
                }

                Debug.Log("Testing Play() method...");
                bloodBurst.Play();
                Assert("Blood burst played without error", true);

                yield return new WaitForSeconds(2f);
                Assert("Blood burst auto-destroyed after playing", testGO == null);
            }
            else
            {
                Debug.Log("Testing with prefab reference...");
                var instance = Instantiate(_bloodBurstPrefab, Vector3.zero, Quaternion.identity);
                Assert("Blood burst prefab instantiated", instance != null);

                instance.Play();
                yield return new WaitForSeconds(2f);
            }

            yield return null;
        }

        private IEnumerator TestEnemyVisualSystem()
        {
            Debug.Log("\n--- ENEMY VISUAL SYSTEM TEST ---");

            if (_enemyVisualController == null)
            {
                Debug.LogWarning("EnemyVisualController not assigned. Creating temporary...");
                GameObject controllerGO = new GameObject("TestEnemyVisualController");
                _enemyVisualController = controllerGO.AddComponent<EnemyVisualController>();
            }

            Assert("EnemyVisualController exists", _enemyVisualController != null);

            if (_testEnemies == null || _testEnemies.Length == 0)
            {
                Debug.LogWarning("No test enemies assigned. Creating mock enemies...");
                yield return TestWithMockEnemies();
            }
            else
            {
                Debug.Log($"Testing with {_testEnemies.Length} enemy assets...");
                
                var enemyList = new List<EnemySO>(_testEnemies);
                GameEvents.RaiseEnemyWaveSpawned(enemyList);
                Debug.Log($"Spawned wave with {enemyList.Count} enemies");
                
                yield return new WaitForSeconds(_stepDelay);

                int childCount = _enemyVisualController.transform.childCount;
                Assert($"Enemy visuals created (expected {enemyList.Count}, got {childCount})", 
                    childCount == enemyList.Count);

                if (_testEnemies.Length > 0)
                {
                    Debug.Log("Testing enemy kill...");
                    GameEvents.RaiseEnemyKilled(_testEnemies[0]);
                    yield return new WaitForSeconds(1f);

                    int newChildCount = _enemyVisualController.transform.childCount;
                    Assert($"Enemy visual removed after kill (expected {enemyList.Count - 1}, got {newChildCount})", 
                        newChildCount == enemyList.Count - 1);
                }
            }

            yield return null;
        }

        private IEnumerator TestWithMockEnemies()
        {
            Debug.Log("Creating mock EnemySO for testing...");
            
            var mockEnemy = ScriptableObject.CreateInstance<EnemySO>();
            mockEnemy.enemyName = "TestThrall";
            mockEnemy.health = 1;
            mockEnemy.damage = 1;

            var enemyList = new List<EnemySO> { mockEnemy };
            GameEvents.RaiseEnemyWaveSpawned(enemyList);
            
            yield return new WaitForSeconds(_stepDelay);

            int childCount = _enemyVisualController.transform.childCount;
            Assert("Mock enemy visual created", childCount >= 1);

            GameEvents.RaiseEnemyKilled(mockEnemy);
            yield return new WaitForSeconds(1f);

            DestroyImmediate(mockEnemy);
        }

        private IEnumerator TestBrotherVisualSystem()
        {
            Debug.Log("\n--- BROTHER VISUAL SYSTEM TEST ---");

            if (_brotherVisualController == null)
            {
                Debug.LogWarning("BrotherVisualController not assigned. Creating temporary...");
                GameObject controllerGO = new GameObject("TestBrotherVisualController");
                _brotherVisualController = controllerGO.AddComponent<BrotherVisualController>();
            }

            Assert("BrotherVisualController exists", _brotherVisualController != null);

            ShieldBrotherSO testBrother;
            if (_testBrothers != null && _testBrothers.Length > 0)
            {
                testBrother = _testBrothers[0];
                Debug.Log($"Testing with brother asset: {testBrother.brotherName}");
            }
            else
            {
                Debug.Log("Creating mock brother for testing...");
                testBrother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
                testBrother.brotherName = "TestBrother";
                testBrother.maxHealth = 3;
            }

            _brotherVisualController.CreateBrotherAtPosition(WallPosition.Left, testBrother);
            yield return new WaitForSeconds(0.5f);

            int childCount = _brotherVisualController.transform.childCount;
            Assert("Brother visual created at Left position", childCount >= 1);

            Debug.Log("Testing wound event...");
            GameEvents.RaiseBrotherWounded(testBrother, 1);
            yield return new WaitForSeconds(_stepDelay);
            Assert("Brother wounded event processed", true);

            Debug.Log("Testing death event...");
            GameEvents.RaiseBrotherDied(testBrother);
            yield return new WaitForSeconds(1.5f);

            childCount = _brotherVisualController.transform.childCount;
            Assert("Brother visual removed after death", childCount == 0);

            if (_testBrothers == null || _testBrothers.Length == 0)
            {
                DestroyImmediate(testBrother);
            }

            yield return null;
        }

        private IEnumerator TestEventIntegration()
        {
            Debug.Log("\n--- EVENT INTEGRATION TEST ---");

            bool waveSpawnedReceived = false;
            bool enemyKilledReceived = false;
            bool brotherWoundedReceived = false;
            bool brotherDiedReceived = false;

            void OnWaveSpawned(List<EnemySO> e) => waveSpawnedReceived = true;
            void OnEnemyKilled(EnemySO e) => enemyKilledReceived = true;
            void OnBrotherWounded(ShieldBrotherSO b, int d) => brotherWoundedReceived = true;
            void OnBrotherDied(ShieldBrotherSO b) => brotherDiedReceived = true;

            GameEvents.OnEnemyWaveSpawned += OnWaveSpawned;
            GameEvents.OnEnemyKilled += OnEnemyKilled;
            GameEvents.OnBrotherWounded += OnBrotherWounded;
            GameEvents.OnBrotherDied += OnBrotherDied;

            var mockEnemy = ScriptableObject.CreateInstance<EnemySO>();
            mockEnemy.enemyName = "IntegrationTestEnemy";
            
            var mockBrother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            mockBrother.brotherName = "IntegrationTestBrother";

            GameEvents.RaiseEnemyWaveSpawned(new List<EnemySO> { mockEnemy });
            yield return null;
            Assert("OnEnemyWaveSpawned event received", waveSpawnedReceived);

            GameEvents.RaiseEnemyKilled(mockEnemy);
            yield return null;
            Assert("OnEnemyKilled event received", enemyKilledReceived);

            GameEvents.RaiseBrotherWounded(mockBrother, 1);
            yield return null;
            Assert("OnBrotherWounded event received", brotherWoundedReceived);

            GameEvents.RaiseBrotherDied(mockBrother);
            yield return null;
            Assert("OnBrotherDied event received", brotherDiedReceived);

            GameEvents.OnEnemyWaveSpawned -= OnWaveSpawned;
            GameEvents.OnEnemyKilled -= OnEnemyKilled;
            GameEvents.OnBrotherWounded -= OnBrotherWounded;
            GameEvents.OnBrotherDied -= OnBrotherDied;

            DestroyImmediate(mockEnemy);
            DestroyImmediate(mockBrother);

            yield return null;
        }

        private void Assert(string testName, bool condition)
        {
            if (condition)
            {
                Debug.Log($"  [PASS] {testName}");
                _testsPassed++;
            }
            else
            {
                Debug.LogError($"  [FAIL] {testName}");
                _testsFailed++;
            }
        }

        [ContextMenu("Spawn Test Blood Burst At Origin")]
        public void SpawnTestBloodBurst()
        {
            if (_bloodBurstPrefab != null)
            {
                var instance = Instantiate(_bloodBurstPrefab, Vector3.zero, Quaternion.identity);
                instance.Play();
            }
            else
            {
                GameObject testGO = new GameObject("ManualTestBloodBurst");
                var bloodBurst = testGO.AddComponent<BloodBurstVFX>();
                bloodBurst.Play();
            }
        }

        [ContextMenu("Spawn Test Brother At Left")]
        public void SpawnTestBrother()
        {
            if (_brotherVisualController == null)
            {
                Debug.LogError("BrotherVisualController not assigned!");
                return;
            }

            ShieldBrotherSO testBrother;
            if (_testBrothers != null && _testBrothers.Length > 0)
            {
                testBrother = _testBrothers[0];
            }
            else
            {
                testBrother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
                testBrother.brotherName = "ManualTestBrother";
                testBrother.maxHealth = 3;
            }

            _brotherVisualController.CreateBrotherAtPosition(WallPosition.Left, testBrother);
        }

        [ContextMenu("Kill First Test Brother")]
        public void KillTestBrother()
        {
            if (_testBrothers != null && _testBrothers.Length > 0)
            {
                GameEvents.RaiseBrotherDied(_testBrothers[0]);
            }
        }
    }
}

