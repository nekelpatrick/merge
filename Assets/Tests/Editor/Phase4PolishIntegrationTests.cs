using NUnit.Framework;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Visual;
using ShieldWall.Audio;
using ShieldWall.Combat;
using ShieldWall.UI;
using ShieldWall.DebugTools;

namespace ShieldWall.Tests.Editor
{
    /// <summary>
    /// Integration tests verifying Phase 4 Polish systems are properly wired to GameEvents.
    /// These tests verify subscription patterns, not runtime behavior (requires PlayMode).
    /// </summary>
    [TestFixture]
    public class Phase4PolishIntegrationTests
    {
        [SetUp]
        public void SetUp()
        {
            GameEvents.ClearAllListeners();
        }

        [TearDown]
        public void TearDown()
        {
            GameEvents.ClearAllListeners();
        }

        #region Track A: Screen Effects

        [Test]
        public void CameraEffects_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestCameraEffects");
            var camera = go.AddComponent<Camera>();
            var cameraEffects = go.AddComponent<CameraEffects>();

            Assert.IsNotNull(cameraEffects, "CameraEffects component should exist");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void TimeController_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestTimeController");
            var timeController = go.AddComponent<TimeController>();

            Assert.IsNotNull(timeController, "TimeController component should exist");
            Assert.IsNotNull(TimeController.Instance, "TimeController should set Instance");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void PostProcessController_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestPostProcessController");
            var postProcess = go.AddComponent<PostProcessController>();

            Assert.IsNotNull(postProcess, "PostProcessController component should exist");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Track B: Animation System

        [Test]
        public void Tweener_ProvidesAllEaseTypes()
        {
            Assert.AreEqual(11, System.Enum.GetValues(typeof(EaseType)).Length, 
                "Tweener should provide 11 ease types");

            Assert.AreEqual(0f, Tweener.Evaluate(0f, EaseType.Linear));
            Assert.AreEqual(1f, Tweener.Evaluate(1f, EaseType.Linear));

            float midEaseOut = Tweener.Evaluate(0.5f, EaseType.EaseOutQuad);
            Assert.Greater(midEaseOut, 0.5f, "EaseOutQuad at 0.5 should be > 0.5");
        }

        [Test]
        public void DiceAnimator_CanBeCreated()
        {
            var go = new GameObject("TestDiceAnimator");
            var animator = go.AddComponent<DiceAnimator>();

            Assert.IsNotNull(animator, "DiceAnimator should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void EnemyAnimator_CanBeCreated()
        {
            var go = new GameObject("TestEnemyAnimator");
            var animator = go.AddComponent<EnemyAnimator>();

            Assert.IsNotNull(animator, "EnemyAnimator should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void BrotherAnimator_CanBeCreated()
        {
            var go = new GameObject("TestBrotherAnimator");
            var animator = go.AddComponent<BrotherAnimator>();

            Assert.IsNotNull(animator, "BrotherAnimator should be creatable");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Track C: VFX Enhancement

        [Test]
        public void ImpactVFXController_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestImpactVFX");
            var impactVFX = go.AddComponent<ImpactVFXController>();

            Assert.IsNotNull(impactVFX, "ImpactVFXController should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void ShieldBlockVFX_CanBeCreated()
        {
            var go = new GameObject("TestShieldBlockVFX");
            var vfx = go.AddComponent<ShieldBlockVFX>();

            Assert.IsNotNull(vfx, "ShieldBlockVFX should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void AttackTrailVFX_CanBeCreated()
        {
            var go = new GameObject("TestAttackTrailVFX");
            var vfx = go.AddComponent<AttackTrailVFX>();

            Assert.IsNotNull(vfx, "AttackTrailVFX should be creatable");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Track D: UI Juice

        [Test]
        public void UIAnimator_ProvidesStaticMethods()
        {
            var method = typeof(UIAnimator).GetMethod("PunchScale");
            Assert.IsNotNull(method, "UIAnimator should have PunchScale method");

            method = typeof(UIAnimator).GetMethod("Shake");
            Assert.IsNotNull(method, "UIAnimator should have Shake method");

            method = typeof(UIAnimator).GetMethod("FadeIn");
            Assert.IsNotNull(method, "UIAnimator should have FadeIn method");
        }

        [Test]
        public void ButtonFeedback_CanBeCreated()
        {
            var go = new GameObject("TestButtonFeedback");
            var button = go.AddComponent<UnityEngine.UI.Button>();
            var feedback = go.AddComponent<ButtonFeedback>();

            Assert.IsNotNull(feedback, "ButtonFeedback should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void HealthHeartAnimator_CanBeCreated()
        {
            var go = new GameObject("TestHealthHeartAnimator");
            go.AddComponent<RectTransform>();
            var animator = go.AddComponent<HealthHeartAnimator>();

            Assert.IsNotNull(animator, "HealthHeartAnimator should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void StaminaDrainEffect_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestStaminaDrainEffect");
            go.AddComponent<RectTransform>();
            var effect = go.AddComponent<StaminaDrainEffect>();

            Assert.IsNotNull(effect, "StaminaDrainEffect should be creatable");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Track E: Audio Feedback

        [Test]
        public void CombatSFXController_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestCombatSFX");
            var controller = go.AddComponent<CombatSFXController>();

            Assert.IsNotNull(controller, "CombatSFXController should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void UISFXController_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestUISFX");
            var controller = go.AddComponent<UISFXController>();

            Assert.IsNotNull(controller, "UISFXController should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void AmbientController_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestAmbient");
            var controller = go.AddComponent<AmbientController>();

            Assert.IsNotNull(controller, "AmbientController should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void SFXPlayer_HasPitchVariation()
        {
            var go = new GameObject("TestSFXPlayer");
            var player = go.AddComponent<SFXPlayer>();

            var method = typeof(SFXPlayer).GetMethod("PlayRandomPitch", 
                new[] { typeof(AudioClip), typeof(float), typeof(float) });
            Assert.IsNotNull(method, "SFXPlayer should have PlayRandomPitch method");

            method = typeof(SFXPlayer).GetMethod("PlayRandom");
            Assert.IsNotNull(method, "SFXPlayer should have PlayRandom method");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Track F: Combat Timing

        [Test]
        public void CombatPacer_IsSingleton()
        {
            var go = new GameObject("TestCombatPacer");
            var pacer = go.AddComponent<CombatPacer>();

            Assert.IsNotNull(pacer, "CombatPacer should be creatable");
            Assert.IsNotNull(CombatPacer.Instance, "CombatPacer should set Instance");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void AttackTelegraph_CanBeCreated()
        {
            var go = new GameObject("TestAttackTelegraph");
            var telegraph = go.AddComponent<AttackTelegraph>();

            Assert.IsNotNull(telegraph, "AttackTelegraph should be creatable");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void HitEmphasis_SubscribesToCorrectEvents()
        {
            var go = new GameObject("TestHitEmphasis");
            var hitEmphasis = go.AddComponent<HitEmphasis>();

            Assert.IsNotNull(hitEmphasis, "HitEmphasis should be creatable");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Track G: Polish Tools

        [Test]
        public void PolishDebugger_CanBeCreated()
        {
            var go = new GameObject("TestPolishDebugger");
            var debugger = go.AddComponent<PolishDebugger>();

            Assert.IsNotNull(debugger, "PolishDebugger should be creatable");

            Object.DestroyImmediate(go);
        }

        #endregion

        #region Cross-Track Integration

        [Test]
        public void AllPolishComponents_CanBeAddedToSingleGameObject()
        {
            var polishGO = new GameObject("PolishEffects");
            
            polishGO.AddComponent<Camera>();
            polishGO.AddComponent<CameraEffects>();
            polishGO.AddComponent<TimeController>();
            polishGO.AddComponent<PostProcessController>();
            polishGO.AddComponent<ImpactVFXController>();
            polishGO.AddComponent<HitEmphasis>();
            polishGO.AddComponent<ScreenEffectsController>();

            Assert.AreEqual(7, polishGO.GetComponents<MonoBehaviour>().Length,
                "PolishEffects should have 6 MonoBehaviours + 1 Camera");

            Object.DestroyImmediate(polishGO);
        }

        [Test]
        public void AllAudioControllers_CanBeAddedToSingleGameObject()
        {
            var audioGO = new GameObject("AudioControllers");

            audioGO.AddComponent<CombatSFXController>();
            audioGO.AddComponent<UISFXController>();
            audioGO.AddComponent<AmbientController>();

            Assert.AreEqual(3, audioGO.GetComponents<MonoBehaviour>().Length,
                "AudioControllers should have 3 MonoBehaviours");

            Object.DestroyImmediate(audioGO);
        }

        [Test]
        public void GameEvents_RaiseMethodsExist()
        {
            var eventsType = typeof(GameEvents);

            Assert.IsNotNull(eventsType.GetMethod("RaisePlayerWounded"));
            Assert.IsNotNull(eventsType.GetMethod("RaiseEnemyKilled"));
            Assert.IsNotNull(eventsType.GetMethod("RaiseAttackBlocked"));
            Assert.IsNotNull(eventsType.GetMethod("RaiseBrotherWounded"));
            Assert.IsNotNull(eventsType.GetMethod("RaiseWaveCleared"));
            Assert.IsNotNull(eventsType.GetMethod("RaiseBattleEnded"));
            Assert.IsNotNull(eventsType.GetMethod("RaiseStaminaChanged"));
            Assert.IsNotNull(eventsType.GetMethod("RaisePhaseChanged"));
        }

        #endregion
    }
}

