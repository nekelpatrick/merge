using System.Collections;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class HitEmphasis : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CameraEffects _cameraEffects;
        [SerializeField] private TimeController _timeController;
        [SerializeField] private PostProcessController _postProcessController;
        [SerializeField] private ImpactVFXController _impactVFX;

        [Header("Player Hit Settings")]
        [SerializeField] private float _playerHitShakeIntensity = 0.2f;
        [SerializeField] private bool _playerHitStop = true;
        [SerializeField] private float _playerHitStopDuration = 0.08f;

        [Header("Enemy Kill Settings")]
        [SerializeField] private float _killShakeIntensity = 0.15f;
        [SerializeField] private bool _killHitStop = true;
        [SerializeField] private float _killHitStopDuration = 0.05f;

        [Header("Block Settings")]
        [SerializeField] private float _blockShakeIntensity = 0.1f;
        [SerializeField] private bool _blockHitStop = false;

        [Header("Brother Hit Settings")]
        [SerializeField] private float _brotherHitShakeIntensity = 0.12f;

        private void Awake()
        {
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
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
        }

        private void HandlePlayerWounded(int damage)
        {
            StartCoroutine(PlayerHitSequence(damage));
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            StartCoroutine(KillSequence(enemy));
        }

        private void HandleAttackBlocked(Attack attack)
        {
            StartCoroutine(BlockSequence(attack));
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            StartCoroutine(BrotherHitSequence(brother, damage));
        }

        private IEnumerator PlayerHitSequence(int damage)
        {
            if (_playerHitStop && _timeController != null)
            {
                _timeController.HitStop(_playerHitStopDuration * damage);
            }

            if (_cameraEffects != null)
            {
                _cameraEffects.DirectionalShake(Vector3.back, _playerHitShakeIntensity * damage);
                _cameraEffects.Punch(0.05f * damage);
            }

            if (_postProcessController != null)
            {
                _postProcessController.TriggerDamageEffect(damage);
            }

            if (_impactVFX != null)
            {
                _impactVFX.SpawnBlood(new Vector3(0, 1.2f, 0.5f), Vector3.back, damage);
            }

            yield return null;
        }

        private IEnumerator KillSequence(EnemySO enemy)
        {
            if (_killHitStop && _timeController != null)
            {
                _timeController.HitStop(_killHitStopDuration);
            }

            if (_cameraEffects != null)
            {
                _cameraEffects.DirectionalShake(Vector3.forward, _killShakeIntensity);
            }

            if (_postProcessController != null)
            {
                _postProcessController.TriggerKillEffect();
            }

            yield return null;
        }

        private IEnumerator BlockSequence(Attack attack)
        {
            if (_blockHitStop && _timeController != null)
            {
                _timeController.HitStop(0.03f);
            }

            if (_cameraEffects != null)
            {
                _cameraEffects.DirectionalShake(Vector3.back, _blockShakeIntensity);
            }

            if (_impactVFX != null)
            {
                _impactVFX.SpawnBlockEffect(new Vector3(0, 1f, 0.6f));
            }

            yield return null;
        }

        private IEnumerator BrotherHitSequence(ShieldBrotherSO brother, int damage)
        {
            if (_cameraEffects != null)
            {
                _cameraEffects.DirectionalShake(Vector3.back, _brotherHitShakeIntensity * damage);
            }

            yield return null;
        }

        public void TriggerCustomHitEmphasis(Vector3 shakeDirection, float intensity, bool hitStop = false, float hitStopDuration = 0.05f)
        {
            if (hitStop && _timeController != null)
                _timeController.HitStop(hitStopDuration);

            if (_cameraEffects != null)
                _cameraEffects.DirectionalShake(shakeDirection, intensity);
        }

        [ContextMenu("Test Player Hit")]
        public void TestPlayerHit() => HandlePlayerWounded(2);

        [ContextMenu("Test Kill")]
        public void TestKill() => HandleEnemyKilled(null);
    }
}

