using NUnit.Framework;
using UnityEngine;
using ShieldWall.Visual;
using ShieldWall.Data;
using ShieldWall.Formation;
using ShieldWall.Core;
using System.Collections.Generic;

namespace ShieldWall.Tests.Editor
{
    [TestFixture]
    public class BloodBurstVFXTests
    {
        private GameObject _testObject;
        private BloodBurstVFX _bloodBurst;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestBloodBurst");
            _bloodBurst = _testObject.AddComponent<BloodBurstVFX>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
            {
                Object.DestroyImmediate(_testObject);
            }
        }

        [Test]
        public void BloodBurstVFX_CreatesParticleSystem()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            Assert.IsNotNull(particleSystem, "ParticleSystem should be auto-created");
        }

        [Test]
        public void BloodBurstVFX_ParticleSystemIsNotLooping()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            Assert.IsFalse(particleSystem.main.loop, "ParticleSystem should not loop");
        }

        [Test]
        public void BloodBurstVFX_ParticleSystemUsesWorldSpace()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            Assert.AreEqual(ParticleSystemSimulationSpace.World, particleSystem.main.simulationSpace,
                "ParticleSystem should use world space");
        }

        [Test]
        public void BloodBurstVFX_ParticleSystemAutoDestroys()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            Assert.AreEqual(ParticleSystemStopAction.Destroy, particleSystem.main.stopAction,
                "ParticleSystem should auto-destroy on stop");
        }

        [Test]
        public void BloodBurstVFX_HasGravityModifier()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            Assert.Greater(particleSystem.main.gravityModifier.constant, 0f,
                "ParticleSystem should have gravity enabled");
        }

        [Test]
        public void BloodBurstVFX_PlayDoesNotThrow()
        {
            Assert.DoesNotThrow(() => _bloodBurst.Play(),
                "Play() should not throw exceptions");
        }

        [Test]
        public void BloodBurstVFX_HasBurstEmission()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            var emission = particleSystem.emission;
            Assert.Greater(emission.burstCount, 0, "ParticleSystem should have burst emission");
        }

        [Test]
        public void BloodBurstVFX_UsesConeShape()
        {
            var particleSystem = _testObject.GetComponent<ParticleSystem>();
            var shape = particleSystem.shape;
            Assert.AreEqual(ParticleSystemShapeType.Cone, shape.shapeType,
                "ParticleSystem should use cone shape");
        }
    }

    [TestFixture]
    public class BrotherVisualControllerTests
    {
        private GameObject _testObject;
        private BrotherVisualController _controller;
        private ShieldBrotherSO _testBrother;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestBrotherVisualController");
            _controller = _testObject.AddComponent<BrotherVisualController>();
            
            _testBrother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            _testBrother.brotherName = "TestBrother";
            _testBrother.maxHealth = 3;
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
            {
                Object.DestroyImmediate(_testObject);
            }
            if (_testBrother != null)
            {
                Object.DestroyImmediate(_testBrother);
            }
        }

        [Test]
        public void BrotherVisualController_CanBeCreated()
        {
            Assert.IsNotNull(_controller, "BrotherVisualController should be created");
        }

        [Test]
        public void BrotherVisualController_CreateBrotherAtPosition_CreatesChildObject()
        {
            _controller.CreateBrotherAtPosition(WallPosition.Left, _testBrother);
            
            Assert.AreEqual(1, _controller.transform.childCount,
                "Should create one child object for brother visual");
        }

        [Test]
        public void BrotherVisualController_CreateBrotherAtPosition_IgnoresCenter()
        {
            _controller.CreateBrotherAtPosition(WallPosition.Center, _testBrother);
            
            Assert.AreEqual(0, _controller.transform.childCount,
                "Should not create visual for Center position (player position)");
        }

        [Test]
        public void BrotherVisualController_CreateBrotherAtPosition_IgnoresNull()
        {
            _controller.CreateBrotherAtPosition(WallPosition.Left, null);
            
            Assert.AreEqual(0, _controller.transform.childCount,
                "Should not create visual for null brother");
        }

        [Test]
        public void BrotherVisualController_CreateBrotherAtPosition_NameIncludesBrotherName()
        {
            _controller.CreateBrotherAtPosition(WallPosition.Left, _testBrother);
            
            var child = _controller.transform.GetChild(0);
            Assert.IsTrue(child.name.Contains(_testBrother.brotherName),
                "Child object name should contain brother name");
        }

        [Test]
        public void BrotherVisualController_InitializeBrothers_CreatesMultiple()
        {
            var brother2 = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            brother2.brotherName = "TestBrother2";

            var brotherPositions = new Dictionary<WallPosition, ShieldBrotherSO>
            {
                { WallPosition.Left, _testBrother },
                { WallPosition.Right, brother2 }
            };

            _controller.InitializeBrothers(brotherPositions);
            
            Assert.AreEqual(2, _controller.transform.childCount,
                "Should create visuals for all non-center positions");

            Object.DestroyImmediate(brother2);
        }

        [Test]
        public void BrotherVisualController_InitializeBrothers_SkipsCenter()
        {
            var brotherPositions = new Dictionary<WallPosition, ShieldBrotherSO>
            {
                { WallPosition.Center, _testBrother },
                { WallPosition.Left, _testBrother }
            };

            _controller.InitializeBrothers(brotherPositions);
            
            Assert.AreEqual(1, _controller.transform.childCount,
                "Should skip Center position");
        }

        [Test]
        public void BrotherVisualController_LeftPosition_HasCorrectX()
        {
            _controller.CreateBrotherAtPosition(WallPosition.Left, _testBrother);
            
            var child = _controller.transform.GetChild(0);
            Assert.AreEqual(-1.5f, child.localPosition.x, 0.01f,
                "Left position should have X = -1.5");
        }

        [Test]
        public void BrotherVisualController_RightPosition_HasCorrectX()
        {
            _controller.CreateBrotherAtPosition(WallPosition.Right, _testBrother);
            
            var child = _controller.transform.GetChild(0);
            Assert.AreEqual(1.5f, child.localPosition.x, 0.01f,
                "Right position should have X = 1.5");
        }

        [Test]
        public void BrotherVisualController_FarLeftPosition_HasCorrectX()
        {
            _controller.CreateBrotherAtPosition(WallPosition.FarLeft, _testBrother);
            
            var child = _controller.transform.GetChild(0);
            Assert.AreEqual(-3f, child.localPosition.x, 0.01f,
                "FarLeft position should have X = -3");
        }

        [Test]
        public void BrotherVisualController_FarRightPosition_HasCorrectX()
        {
            _controller.CreateBrotherAtPosition(WallPosition.FarRight, _testBrother);
            
            var child = _controller.transform.GetChild(0);
            Assert.AreEqual(3f, child.localPosition.x, 0.01f,
                "FarRight position should have X = 3");
        }
    }

    [TestFixture]
    public class EnemyVisualInstanceTests
    {
        private GameObject _testObject;
        private EnemyVisualInstance _instance;
        private EnemySO _testEnemy;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestEnemyVisual");
            _instance = _testObject.AddComponent<EnemyVisualInstance>();
            
            _testEnemy = ScriptableObject.CreateInstance<EnemySO>();
            _testEnemy.enemyName = "TestEnemy";
            _testEnemy.health = 1;
            _testEnemy.damage = 1;
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
            {
                Object.DestroyImmediate(_testObject);
            }
            if (_testEnemy != null)
            {
                Object.DestroyImmediate(_testEnemy);
            }
        }

        [Test]
        public void EnemyVisualInstance_Initialize_SetsEnemyData()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            Assert.AreEqual(_testEnemy, _instance.EnemyData,
                "EnemyData should be set after initialization");
        }

        [Test]
        public void EnemyVisualInstance_Initialize_CreatesBodyChild()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            var body = _testObject.transform.Find("Body");
            Assert.IsNotNull(body, "Should create Body child object");
        }

        [Test]
        public void EnemyVisualInstance_Initialize_CreatesHeadChild()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            var head = _testObject.transform.Find("Head");
            Assert.IsNotNull(head, "Should create Head child object");
        }

        [Test]
        public void EnemyVisualInstance_Body_IsCapsule()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            var body = _testObject.transform.Find("Body");
            var meshFilter = body.GetComponent<MeshFilter>();
            Assert.IsTrue(meshFilter.sharedMesh.name.Contains("Capsule"),
                "Body should be a capsule primitive");
        }

        [Test]
        public void EnemyVisualInstance_Head_IsCube()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            var head = _testObject.transform.Find("Head");
            var meshFilter = head.GetComponent<MeshFilter>();
            Assert.IsTrue(meshFilter.sharedMesh.name.Contains("Cube"),
                "Head should be a cube primitive");
        }

        [Test]
        public void EnemyVisualInstance_Body_HasNoCollider()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            var body = _testObject.transform.Find("Body");
            var collider = body.GetComponent<Collider>();
            Assert.IsNull(collider, "Body should have collider removed");
        }

        [Test]
        public void EnemyVisualInstance_Head_HasNoCollider()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            var head = _testObject.transform.Find("Head");
            var collider = head.GetComponent<Collider>();
            Assert.IsNull(collider, "Head should have collider removed");
        }

        [Test]
        public void EnemyVisualInstance_PlayHitReaction_DoesNotThrow()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            Assert.DoesNotThrow(() => _instance.PlayHitReaction(),
                "PlayHitReaction should not throw");
        }

        [Test]
        public void EnemyVisualInstance_PlayDeathAnimation_DoesNotThrow()
        {
            _instance.Initialize(_testEnemy, Color.red, null);
            
            Assert.DoesNotThrow(() => _instance.PlayDeathAnimation(),
                "PlayDeathAnimation should not throw");
        }
    }

    [TestFixture]
    public class GameEventsVisualIntegrationTests
    {
        [TearDown]
        public void TearDown()
        {
            GameEvents.ClearAllListeners();
        }

        [Test]
        public void OnEnemyWaveSpawned_CanBeRaised()
        {
            bool eventReceived = false;
            GameEvents.OnEnemyWaveSpawned += (enemies) => eventReceived = true;
            
            GameEvents.RaiseEnemyWaveSpawned(new List<EnemySO>());
            
            Assert.IsTrue(eventReceived, "OnEnemyWaveSpawned event should be raised");
        }

        [Test]
        public void OnEnemyKilled_CanBeRaised()
        {
            bool eventReceived = false;
            GameEvents.OnEnemyKilled += (enemy) => eventReceived = true;
            
            var testEnemy = ScriptableObject.CreateInstance<EnemySO>();
            GameEvents.RaiseEnemyKilled(testEnemy);
            
            Assert.IsTrue(eventReceived, "OnEnemyKilled event should be raised");
            
            Object.DestroyImmediate(testEnemy);
        }

        [Test]
        public void OnBrotherWounded_CanBeRaised()
        {
            bool eventReceived = false;
            int receivedDamage = 0;
            GameEvents.OnBrotherWounded += (brother, damage) => 
            {
                eventReceived = true;
                receivedDamage = damage;
            };
            
            var testBrother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            GameEvents.RaiseBrotherWounded(testBrother, 2);
            
            Assert.IsTrue(eventReceived, "OnBrotherWounded event should be raised");
            Assert.AreEqual(2, receivedDamage, "Damage should be passed correctly");
            
            Object.DestroyImmediate(testBrother);
        }

        [Test]
        public void OnBrotherDied_CanBeRaised()
        {
            bool eventReceived = false;
            GameEvents.OnBrotherDied += (brother) => eventReceived = true;
            
            var testBrother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            GameEvents.RaiseBrotherDied(testBrother);
            
            Assert.IsTrue(eventReceived, "OnBrotherDied event should be raised");
            
            Object.DestroyImmediate(testBrother);
        }

        [Test]
        public void OnPlayerWounded_CanBeRaised()
        {
            bool eventReceived = false;
            int receivedDamage = 0;
            GameEvents.OnPlayerWounded += (damage) => 
            {
                eventReceived = true;
                receivedDamage = damage;
            };
            
            GameEvents.RaisePlayerWounded(3);
            
            Assert.IsTrue(eventReceived, "OnPlayerWounded event should be raised");
            Assert.AreEqual(3, receivedDamage, "Damage should be passed correctly");
        }

        [Test]
        public void OnAttackBlocked_CanBeRaised()
        {
            bool eventReceived = false;
            GameEvents.OnAttackBlocked += (attack) => eventReceived = true;
            
            var attack = new Attack { Damage = 1 };
            GameEvents.RaiseAttackBlocked(attack);
            
            Assert.IsTrue(eventReceived, "OnAttackBlocked event should be raised");
        }

        [Test]
        public void ClearAllListeners_RemovesAllSubscribers()
        {
            bool eventReceived = false;
            GameEvents.OnEnemyKilled += (enemy) => eventReceived = true;
            
            GameEvents.ClearAllListeners();
            
            var testEnemy = ScriptableObject.CreateInstance<EnemySO>();
            GameEvents.RaiseEnemyKilled(testEnemy);
            
            Assert.IsFalse(eventReceived, "Events should not fire after ClearAllListeners");
            
            Object.DestroyImmediate(testEnemy);
        }
    }
}

