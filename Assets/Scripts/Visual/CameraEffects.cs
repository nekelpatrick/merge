using System.Collections;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class CameraEffects : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private float _baseShakeIntensity = 0.15f;
        [SerializeField] private float _shakeDuration = 0.25f;
        [SerializeField] private AnimationCurve _shakeFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Punch Settings")]
        [SerializeField] private float _punchDistance = 0.1f;
        [SerializeField] private float _punchDuration = 0.15f;
        [SerializeField] private float _punchFOVAmount = 5f;

        [Header("References")]
        [SerializeField] private Camera _camera;

        private Vector3 _originalPosition;
        private float _originalFOV;
        private Coroutine _shakeCoroutine;
        private Coroutine _punchCoroutine;

        private void Awake()
        {
            if (_camera == null)
                _camera = Camera.main;

            if (_camera != null)
            {
                _originalPosition = _camera.transform.localPosition;
                _originalFOV = _camera.fieldOfView;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
        }

        private void HandlePlayerWounded(int damage)
        {
            DirectionalShake(Vector3.back, _baseShakeIntensity * damage * 1.5f);
            Punch(_punchDistance * damage);
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            DirectionalShake(Vector3.back, _baseShakeIntensity * 0.5f);
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            DirectionalShake(Vector3.forward, _baseShakeIntensity * 0.3f);
        }

        private void HandleAttackBlocked(Attack attack)
        {
            DirectionalShake(Vector3.back, _baseShakeIntensity * 0.4f);
        }

        public void DirectionalShake(Vector3 direction, float intensity)
        {
            if (_shakeCoroutine != null)
                StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = StartCoroutine(ShakeRoutine(direction.normalized, intensity));
        }

        public void Punch(float distance)
        {
            if (_punchCoroutine != null)
                StopCoroutine(_punchCoroutine);
            _punchCoroutine = StartCoroutine(PunchRoutine(distance));
        }

        private IEnumerator ShakeRoutine(Vector3 direction, float intensity)
        {
            if (_camera == null) yield break;

            float elapsed = 0f;
            Vector3 startPos = _originalPosition;

            while (elapsed < _shakeDuration)
            {
                float t = elapsed / _shakeDuration;
                float falloff = _shakeFalloff.Evaluate(t);
                float currentIntensity = intensity * falloff;

                float offsetX = (Random.value * 2f - 1f) * currentIntensity;
                float offsetY = (Random.value * 2f - 1f) * currentIntensity;
                Vector3 dirOffset = direction * currentIntensity * 0.5f;

                _camera.transform.localPosition = startPos + new Vector3(offsetX, offsetY, 0) + dirOffset;

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _camera.transform.localPosition = _originalPosition;
            _shakeCoroutine = null;
        }

        private IEnumerator PunchRoutine(float distance)
        {
            if (_camera == null) yield break;

            float elapsed = 0f;
            float targetFOV = _originalFOV + _punchFOVAmount;

            while (elapsed < _punchDuration * 0.3f)
            {
                float t = elapsed / (_punchDuration * 0.3f);
                _camera.transform.localPosition = Vector3.Lerp(_originalPosition, _originalPosition + Vector3.back * distance, t);
                _camera.fieldOfView = Mathf.Lerp(_originalFOV, targetFOV, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            elapsed = 0f;
            Vector3 punchedPos = _camera.transform.localPosition;
            float punchedFOV = _camera.fieldOfView;

            while (elapsed < _punchDuration * 0.7f)
            {
                float t = elapsed / (_punchDuration * 0.7f);
                t = 1f - Mathf.Pow(1f - t, 3f);
                _camera.transform.localPosition = Vector3.Lerp(punchedPos, _originalPosition, t);
                _camera.fieldOfView = Mathf.Lerp(punchedFOV, _originalFOV, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _camera.transform.localPosition = _originalPosition;
            _camera.fieldOfView = _originalFOV;
            _punchCoroutine = null;
        }

        [ContextMenu("Test Shake")]
        public void TestShake() => DirectionalShake(Vector3.back, _baseShakeIntensity);

        [ContextMenu("Test Punch")]
        public void TestPunch() => Punch(_punchDistance);
    }
}

