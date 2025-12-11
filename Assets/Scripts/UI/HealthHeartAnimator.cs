using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class HealthHeartAnimator : MonoBehaviour
    {
        [Header("Pulse Settings")]
        [SerializeField] private float _pulseMinScale = 0.95f;
        [SerializeField] private float _pulseMaxScale = 1.05f;
        [SerializeField] private float _pulseSpeed = 2f;

        [Header("Damage Settings")]
        [SerializeField] private float _damagePunchScale = 0.3f;
        [SerializeField] private float _damagePunchDuration = 0.2f;
        [SerializeField] private Color _damageFlashColor = Color.white;
        [SerializeField] private float _damageFlashDuration = 0.1f;

        [Header("Critical Settings")]
        [SerializeField] private float _criticalPulseSpeed = 4f;
        [SerializeField] private Color _criticalColor = new Color(0.8f, 0.2f, 0.2f);

        [Header("Shatter Settings")]
        [SerializeField] private float _shatterDuration = 0.5f;
        [SerializeField] private float _shatterRotation = 30f;

        private RectTransform _rectTransform;
        private Image _image;
        private Color _originalColor;
        private Vector3 _originalScale;
        private Coroutine _pulseCoroutine;
        private Coroutine _effectCoroutine;
        private bool _isCritical;
        private bool _isShattered;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            
            if (_image != null)
                _originalColor = _image.color;
            
            _originalScale = _rectTransform.localScale;
        }

        private void OnEnable()
        {
            StartPulse();
        }

        private void OnDisable()
        {
            StopPulse();
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
        }

        public void StartPulse()
        {
            if (_isShattered) return;

            if (_pulseCoroutine != null)
                StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        public void StopPulse()
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }
            _rectTransform.localScale = _originalScale;
        }

        public void SetCritical(bool critical)
        {
            _isCritical = critical;
            if (_image != null)
                _image.color = critical ? _criticalColor : _originalColor;
        }

        public void PlayDamageAnimation()
        {
            if (_isShattered) return;

            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(DamageRoutine());
        }

        public void PlayShatterAnimation(System.Action onComplete = null)
        {
            if (_isShattered) return;
            _isShattered = true;

            StopPulse();
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(ShatterRoutine(onComplete));
        }

        public void Reset()
        {
            _isShattered = false;
            _isCritical = false;
            
            if (_image != null)
            {
                _image.color = _originalColor;
                var c = _image.color;
                c.a = 1f;
                _image.color = c;
            }
            
            _rectTransform.localScale = _originalScale;
            _rectTransform.localRotation = Quaternion.identity;
            
            StartPulse();
        }

        private IEnumerator PulseRoutine()
        {
            float offset = Random.value * Mathf.PI * 2f;

            while (!_isShattered)
            {
                float speed = _isCritical ? _criticalPulseSpeed : _pulseSpeed;
                float t = (Mathf.Sin(Time.time * speed + offset) + 1f) * 0.5f;
                float scale = Mathf.Lerp(_pulseMinScale, _pulseMaxScale, t);
                _rectTransform.localScale = _originalScale * scale;
                yield return null;
            }
        }

        private IEnumerator DamageRoutine()
        {
            if (_image != null)
            {
                _image.color = _damageFlashColor;
            }

            yield return StartCoroutine(UIAnimator.PunchScale(_rectTransform, _damagePunchScale, _damagePunchDuration));

            float elapsed = 0f;
            Color startColor = _damageFlashColor;
            Color endColor = _isCritical ? _criticalColor : _originalColor;

            while (elapsed < _damageFlashDuration)
            {
                float t = elapsed / _damageFlashDuration;
                if (_image != null)
                    _image.color = Color.Lerp(startColor, endColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (_image != null)
                _image.color = endColor;

            _effectCoroutine = null;
        }

        private IEnumerator ShatterRoutine(System.Action onComplete)
        {
            float elapsed = 0f;
            float rotationDir = Random.value > 0.5f ? 1f : -1f;
            Vector3 startScale = _rectTransform.localScale;

            while (elapsed < _shatterDuration)
            {
                float t = elapsed / _shatterDuration;
                float eased = Tweener.Evaluate(t, EaseType.EaseInQuad);

                _rectTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);
                _rectTransform.localRotation = Quaternion.Euler(0, 0, _shatterRotation * rotationDir * eased);

                if (_image != null)
                {
                    var c = _image.color;
                    c.a = 1f - eased;
                    _image.color = c;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            _rectTransform.localScale = Vector3.zero;
            onComplete?.Invoke();
            _effectCoroutine = null;
        }
    }
}

