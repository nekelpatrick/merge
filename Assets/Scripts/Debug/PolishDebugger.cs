using UnityEngine;
using ShieldWall.Visual;
using ShieldWall.Core;

namespace ShieldWall.DebugTools
{
    public class PolishDebugger : MonoBehaviour
    {
        [Header("Enable/Disable")]
        [SerializeField] private bool _enableDebugKeys = true;

        [Header("References (Auto-found if null)")]
        [SerializeField] private CameraEffects _cameraEffects;
        [SerializeField] private TimeController _timeController;
        [SerializeField] private PostProcessController _postProcessController;
        [SerializeField] private ImpactVFXController _impactVFX;
        [SerializeField] private HitEmphasis _hitEmphasis;

        private void Awake()
        {
            #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            _enableDebugKeys = false;
            #endif

            FindReferences();
        }

        private void FindReferences()
        {
            if (_cameraEffects == null)
                _cameraEffects = FindFirstObjectByType<CameraEffects>();
            if (_timeController == null)
                _timeController = FindFirstObjectByType<TimeController>();
            if (_postProcessController == null)
                _postProcessController = FindFirstObjectByType<PostProcessController>();
            if (_impactVFX == null)
                _impactVFX = FindFirstObjectByType<ImpactVFXController>();
            if (_hitEmphasis == null)
                _hitEmphasis = FindFirstObjectByType<HitEmphasis>();
        }

        private void Update()
        {
            if (!_enableDebugKeys) return;

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestCameraShake();
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                TestCameraPunch();
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                TestHitStop();
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                TestSlowMotion();
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                TestBloodVFX();
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                TestBlockVFX();
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                TestPostProcessDamage();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                TestPlayerWoundedEvent();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                TestEnemyKilledEvent();
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                TestWaveClearedEvent();
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                ToggleDebugKeys();
            }
        }

        private void TestCameraShake()
        {
            UnityEngine.Debug.Log("[PolishDebug] F1: Camera Shake");
            _cameraEffects?.DirectionalShake(Vector3.back, 0.2f);
        }

        private void TestCameraPunch()
        {
            UnityEngine.Debug.Log("[PolishDebug] F2: Camera Punch");
            _cameraEffects?.Punch(0.1f);
        }

        private void TestHitStop()
        {
            UnityEngine.Debug.Log("[PolishDebug] F3: Hit Stop");
            _timeController?.HitStop(0.1f);
        }

        private void TestSlowMotion()
        {
            UnityEngine.Debug.Log("[PolishDebug] F4: Slow Motion");
            _timeController?.SlowMotion(1f, 0.3f);
        }

        private void TestBloodVFX()
        {
            UnityEngine.Debug.Log("[PolishDebug] F5: Blood VFX");
            _impactVFX?.SpawnBlood(new Vector3(0, 1.2f, 0.5f), Vector3.back, 2);
        }

        private void TestBlockVFX()
        {
            UnityEngine.Debug.Log("[PolishDebug] F6: Block VFX");
            _impactVFX?.SpawnBlockEffect(new Vector3(0, 1f, 0.6f));
        }

        private void TestPostProcessDamage()
        {
            UnityEngine.Debug.Log("[PolishDebug] F7: Post Process Damage");
            _postProcessController?.TriggerDamageEffect(2);
        }

        private void TestPlayerWoundedEvent()
        {
            UnityEngine.Debug.Log("[PolishDebug] F8: Player Wounded Event");
            GameEvents.RaisePlayerWounded(2);
        }

        private void TestEnemyKilledEvent()
        {
            UnityEngine.Debug.Log("[PolishDebug] F9: Enemy Killed Event");
            GameEvents.RaiseEnemyKilled(null);
        }

        private void TestWaveClearedEvent()
        {
            UnityEngine.Debug.Log("[PolishDebug] F10: Wave Cleared Event");
            GameEvents.RaiseWaveCleared();
        }

        private void ToggleDebugKeys()
        {
            _enableDebugKeys = !_enableDebugKeys;
            UnityEngine.Debug.Log($"[PolishDebug] Debug keys: {(_enableDebugKeys ? "ENABLED" : "DISABLED")}");
        }

        private void OnGUI()
        {
            if (!_enableDebugKeys) return;

            GUILayout.BeginArea(new Rect(10, 10, 200, 200));
            GUILayout.Label("Polish Debug (F12 to toggle)");
            GUILayout.Label("F1: Shake | F2: Punch");
            GUILayout.Label("F3: HitStop | F4: SlowMo");
            GUILayout.Label("F5: Blood | F6: Block");
            GUILayout.Label("F7: PostProcess");
            GUILayout.Label("F8-F10: Game Events");
            GUILayout.EndArea();
        }
    }
}

