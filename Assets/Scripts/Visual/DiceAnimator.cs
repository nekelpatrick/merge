using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;

namespace ShieldWall.Visual
{
    public class DiceAnimator : MonoBehaviour
    {
        [Header("Roll Animation")]
        [SerializeField] private float _rollDuration = 0.5f;
        [SerializeField] private int _rollCycles = 8;
        [SerializeField] private float _rollScalePunch = 0.2f;

        [Header("Lock Animation")]
        [SerializeField] private float _lockPunchScale = 0.15f;
        [SerializeField] private float _lockPunchDuration = 0.2f;
        [SerializeField] private Color _lockGlowColor = new Color(1f, 0.85f, 0.3f, 0.6f);
        [SerializeField] private float _lockPulseDuration = 1f;

        [Header("Hover Animation")]
        [SerializeField] private float _hoverScaleMultiplier = 1.1f;
        [SerializeField] private float _hoverDuration = 0.1f;

        private RectTransform _rectTransform;
        private Image _backgroundImage;
        private Coroutine _animationCoroutine;
        private Coroutine _pulseCoroutine;
        private Vector3 _originalScale;
        private Color _originalColor;
        private bool _isLocked;

        public void Initialize(RectTransform rectTransform, Image backgroundImage)
        {
            _rectTransform = rectTransform;
            _backgroundImage = backgroundImage;
            _originalScale = rectTransform.localScale;
            if (backgroundImage != null)
                _originalColor = backgroundImage.color;
        }

        public void PlayRollAnimation(System.Action onCycleComplete = null)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(RollRoutine(onCycleComplete));
        }

        public void PlayLockAnimation(bool locked)
        {
            _isLocked = locked;
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(LockRoutine(locked));
        }

        public void PlayHoverAnimation(bool hovering)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(HoverRoutine(hovering));
        }

        private IEnumerator RollRoutine(System.Action onCycleComplete)
        {
            if (_rectTransform == null) yield break;

            float cycleTime = _rollDuration / _rollCycles;
            
            for (int i = 0; i < _rollCycles; i++)
            {
                float t = (float)i / _rollCycles;
                float currentScale = 1f + _rollScalePunch * (1f - t);
                float currentRotation = 360f * t;

                _rectTransform.localScale = _originalScale * currentScale;
                _rectTransform.localRotation = Quaternion.Euler(0, 0, currentRotation);

                onCycleComplete?.Invoke();

                yield return new WaitForSeconds(cycleTime * (0.5f + t * 0.5f));
            }

            yield return StartCoroutine(Tweener.TweenFloat(
                _rectTransform.localScale.x,
                _originalScale.x,
                0.1f,
                EaseType.EaseOutBounce,
                v => _rectTransform.localScale = Vector3.one * v
            ));

            _rectTransform.localRotation = Quaternion.identity;
            _animationCoroutine = null;
        }

        private IEnumerator LockRoutine(bool locked)
        {
            if (_rectTransform == null) yield break;

            yield return StartCoroutine(Tweener.PunchScale(
                _rectTransform,
                Vector3.one * _lockPunchScale,
                _lockPunchDuration
            ));

            if (locked)
            {
                StartPulse();
            }
            else
            {
                StopPulse();
            }

            _animationCoroutine = null;
        }

        private IEnumerator HoverRoutine(bool hovering)
        {
            if (_rectTransform == null) yield break;

            Vector3 targetScale = hovering ? _originalScale * _hoverScaleMultiplier : _originalScale;

            yield return StartCoroutine(Tweener.TweenVector3(
                _rectTransform.localScale,
                targetScale,
                _hoverDuration,
                EaseType.EaseOutQuad,
                v => _rectTransform.localScale = v
            ));

            _animationCoroutine = null;
        }

        private void StartPulse()
        {
            if (_pulseCoroutine != null)
                StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        private void StopPulse()
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }
            if (_backgroundImage != null)
                _backgroundImage.color = _originalColor;
        }

        private IEnumerator PulseRoutine()
        {
            while (_isLocked && _backgroundImage != null)
            {
                float elapsed = 0f;
                while (elapsed < _lockPulseDuration)
                {
                    float t = elapsed / _lockPulseDuration;
                    float pulse = Mathf.Sin(t * Mathf.PI * 2f) * 0.5f + 0.5f;
                    _backgroundImage.color = Color.Lerp(_originalColor, _lockGlowColor, pulse * 0.3f);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
            }
            _pulseCoroutine = null;
        }

        private void OnDisable()
        {
            StopPulse();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
        }
    }
}

