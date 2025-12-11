using System.Collections;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class TimeController : MonoBehaviour
    {
        public static TimeController Instance { get; private set; }

        [Header("Hit Stop Settings")]
        [SerializeField] private float _hitStopDuration = 0.05f;
        [SerializeField] private float _hitStopTimeScale = 0.0f;

        [Header("Slow Motion Settings")]
        [SerializeField] private float _slowMoDuration = 1f;
        [SerializeField] private float _slowMoTimeScale = 0.3f;
        [SerializeField] private AnimationCurve _slowMoEaseOut = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine _timeEffectCoroutine;
        private float _targetTimeScale = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnWaveCleared += HandleWaveCleared;
            GameEvents.OnBattleEnded += HandleBattleEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnWaveCleared -= HandleWaveCleared;
            GameEvents.OnBattleEnded -= HandleBattleEnded;
            ResetTimeScale();
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            HitStop();
        }

        private void HandleWaveCleared()
        {
            SlowMotion(_slowMoDuration, _slowMoTimeScale);
        }

        private void HandleBattleEnded(bool victory)
        {
            if (victory)
                SlowMotion(_slowMoDuration * 2f, _slowMoTimeScale * 0.5f);
        }

        public void HitStop()
        {
            HitStop(_hitStopDuration);
        }

        public void HitStop(float duration)
        {
            if (_timeEffectCoroutine != null)
                StopCoroutine(_timeEffectCoroutine);
            _timeEffectCoroutine = StartCoroutine(HitStopRoutine(duration));
        }

        public void SlowMotion(float duration, float timeScale)
        {
            if (_timeEffectCoroutine != null)
                StopCoroutine(_timeEffectCoroutine);
            _timeEffectCoroutine = StartCoroutine(SlowMotionRoutine(duration, timeScale));
        }

        private IEnumerator HitStopRoutine(float duration)
        {
            Time.timeScale = _hitStopTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = _targetTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _timeEffectCoroutine = null;
        }

        private IEnumerator SlowMotionRoutine(float duration, float timeScale)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            float elapsed = 0f;
            float halfDuration = duration * 0.5f;

            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                float t = _slowMoEaseOut.Evaluate(elapsed / halfDuration);
                Time.timeScale = Mathf.Lerp(timeScale, _targetTimeScale, t);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = _targetTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _timeEffectCoroutine = null;
        }

        public void ResetTimeScale()
        {
            if (_timeEffectCoroutine != null)
                StopCoroutine(_timeEffectCoroutine);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            _targetTimeScale = 1f;
        }

        [ContextMenu("Test Hit Stop")]
        public void TestHitStop() => HitStop();

        [ContextMenu("Test Slow Motion")]
        public void TestSlowMotion() => SlowMotion(_slowMoDuration, _slowMoTimeScale);
    }
}

