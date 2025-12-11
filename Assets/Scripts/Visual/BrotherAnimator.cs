using System.Collections;
using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.Visual
{
    public class BrotherAnimator : MonoBehaviour
    {
        [Header("Idle Animation")]
        [SerializeField] private float _idleSwayAmount = 0.05f;
        [SerializeField] private float _idleSwaySpeed = 0.5f;
        [SerializeField] private float _idleBreathAmount = 0.02f;
        [SerializeField] private float _idleBreathSpeed = 0.3f;

        [Header("Wound Animation")]
        [SerializeField] private float _woundStaggerDistance = 0.2f;
        [SerializeField] private float _woundLeanAngle = 10f;
        [SerializeField] private float _woundDuration = 0.3f;
        [SerializeField] private Color _woundFlashColor = new Color(1f, 0.3f, 0.3f, 1f);

        [Header("Death Animation")]
        [SerializeField] private float _deathFallAngle = 60f;
        [SerializeField] private float _deathDropDistance = 0.3f;
        [SerializeField] private float _deathDuration = 0.8f;
        [SerializeField] private float _deathFadeDuration = 0.5f;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Vector3 _currentLean;
        private Renderer[] _renderers;
        private Coroutine _idleCoroutine;
        private Coroutine _animationCoroutine;
        private bool _isAlive = true;
        private int _woundCount;

        public void Initialize(Vector3 position)
        {
            _originalPosition = position;
            _originalRotation = transform.rotation;
            _renderers = GetComponentsInChildren<Renderer>();
            _currentLean = Vector3.zero;
        }

        public void PlayWoundAnimation(int totalWounds, System.Action onComplete = null)
        {
            _woundCount = totalWounds;
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(WoundRoutine(onComplete));
        }

        public void PlayDeathAnimation(System.Action onComplete = null)
        {
            _isAlive = false;
            StopIdle();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(DeathRoutine(onComplete));
        }

        public void StartIdle()
        {
            if (_idleCoroutine != null)
                StopCoroutine(_idleCoroutine);
            _idleCoroutine = StartCoroutine(IdleRoutine());
        }

        public void StopIdle()
        {
            if (_idleCoroutine != null)
            {
                StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
            }
        }

        private IEnumerator WoundRoutine(System.Action onComplete)
        {
            Vector3 staggerPos = _originalPosition + Vector3.back * _woundStaggerDistance;
            float leanAngle = _woundLeanAngle * _woundCount;
            _currentLean = new Vector3(leanAngle, 0, Random.Range(-5f, 5f));

            FlashColor(_woundFlashColor);

            yield return StartCoroutine(Tweener.TweenVector3(
                transform.position,
                staggerPos,
                _woundDuration * 0.3f,
                EaseType.EaseOutQuad,
                v => transform.position = v
            ));

            Quaternion targetRot = _originalRotation * Quaternion.Euler(_currentLean);
            yield return StartCoroutine(Tweener.TweenVector3(
                transform.rotation.eulerAngles,
                targetRot.eulerAngles,
                _woundDuration * 0.4f,
                EaseType.EaseOutQuad,
                v => transform.rotation = Quaternion.Euler(v)
            ));

            yield return StartCoroutine(Tweener.TweenVector3(
                transform.position,
                _originalPosition,
                _woundDuration * 0.3f,
                EaseType.EaseOutQuad,
                v => transform.position = v
            ));

            ResetColor();
            onComplete?.Invoke();
            _animationCoroutine = null;
        }

        private IEnumerator DeathRoutine(System.Action onComplete)
        {
            float fallDirection = Random.value > 0.5f ? 1f : -1f;
            Vector3 fallRotation = new Vector3(_deathFallAngle, 0, _deathFallAngle * 0.5f * fallDirection);
            Vector3 fallPosition = _originalPosition + Vector3.down * _deathDropDistance + Vector3.back * 0.2f;

            Quaternion startRot = transform.rotation;
            Quaternion endRot = Quaternion.Euler(fallRotation);
            Vector3 startPos = transform.position;

            float elapsed = 0f;
            while (elapsed < _deathDuration)
            {
                float t = Tweener.Evaluate(elapsed / _deathDuration, EaseType.EaseInQuad);
                transform.position = Vector3.Lerp(startPos, fallPosition, t);
                transform.rotation = Quaternion.Slerp(startRot, endRot, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < _deathFadeDuration)
            {
                float t = elapsed / _deathFadeDuration;
                SetAlpha(1f - t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetAlpha(0f);
            onComplete?.Invoke();
            _animationCoroutine = null;
        }

        private IEnumerator IdleRoutine()
        {
            float swayOffset = Random.value * Mathf.PI * 2f;
            float breathOffset = Random.value * Mathf.PI * 2f;

            while (_isAlive)
            {
                float sway = Mathf.Sin(Time.time * _idleSwaySpeed + swayOffset) * _idleSwayAmount;
                float breath = Mathf.Sin(Time.time * _idleBreathSpeed + breathOffset) * _idleBreathAmount;

                Vector3 offset = new Vector3(sway, breath, 0);
                Quaternion leanRot = Quaternion.Euler(_currentLean);

                transform.position = _originalPosition + offset;
                transform.rotation = _originalRotation * leanRot * Quaternion.Euler(0, 0, sway * 5f);

                yield return null;
            }
        }

        private void FlashColor(Color color)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                    renderer.material.color = color;
            }
        }

        private void ResetColor()
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                    renderer.material.color = Color.white;
            }
        }

        private void SetAlpha(float alpha)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    var color = renderer.material.color;
                    color.a = alpha;
                    renderer.material.color = color;
                }
            }
        }

        private void OnDisable()
        {
            StopIdle();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
        }
    }
}

