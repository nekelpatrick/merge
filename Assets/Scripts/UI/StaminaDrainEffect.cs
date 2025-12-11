using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class StaminaDrainEffect : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _staminaFillImage;
        [SerializeField] private Image _staminaBackgroundImage;

        [Header("Drain Animation")]
        [SerializeField] private float _drainPunchAmount = 0.1f;
        [SerializeField] private float _drainPunchDuration = 0.2f;
        [SerializeField] private Color _drainFlashColor = new Color(0.5f, 0.7f, 1f, 1f);

        [Header("Critical Warning")]
        [SerializeField] private float _criticalThreshold = 0.25f;
        [SerializeField] private float _criticalPulseSpeed = 4f;
        [SerializeField] private Color _criticalColor = new Color(1f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color _normalColor = new Color(0.3f, 0.5f, 0.7f, 1f);

        private RectTransform _rectTransform;
        private Color _originalFillColor;
        private Coroutine _pulseCoroutine;
        private Coroutine _drainCoroutine;
        private bool _isCritical;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            
            if (_staminaFillImage != null)
                _originalFillColor = _staminaFillImage.color;
        }

        private void OnEnable()
        {
            GameEvents.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnStaminaChanged -= HandleStaminaChanged;
            StopAllCoroutines();
        }

        private void HandleStaminaChanged(int currentStamina)
        {
            PlayDrainAnimation();
        }

        public void UpdateStaminaRatio(float ratio)
        {
            bool wasCritical = _isCritical;
            _isCritical = ratio <= _criticalThreshold;

            if (_isCritical && !wasCritical)
            {
                StartCriticalPulse();
            }
            else if (!_isCritical && wasCritical)
            {
                StopCriticalPulse();
            }
        }

        public void PlayDrainAnimation()
        {
            if (_drainCoroutine != null)
                StopCoroutine(_drainCoroutine);
            _drainCoroutine = StartCoroutine(DrainRoutine());
        }

        private IEnumerator DrainRoutine()
        {
            if (_staminaFillImage != null)
                _staminaFillImage.color = _drainFlashColor;

            if (_rectTransform != null)
            {
                yield return StartCoroutine(UIAnimator.PunchScale(_rectTransform, _drainPunchAmount, _drainPunchDuration));
            }

            if (_staminaFillImage != null)
            {
                float elapsed = 0f;
                Color targetColor = _isCritical ? _criticalColor : _normalColor;

                while (elapsed < 0.2f)
                {
                    float t = elapsed / 0.2f;
                    _staminaFillImage.color = Color.Lerp(_drainFlashColor, targetColor, t);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                _staminaFillImage.color = targetColor;
            }

            _drainCoroutine = null;
        }

        private void StartCriticalPulse()
        {
            if (_pulseCoroutine != null)
                StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(CriticalPulseRoutine());
        }

        private void StopCriticalPulse()
        {
            if (_pulseCoroutine != null)
            {
                StopCoroutine(_pulseCoroutine);
                _pulseCoroutine = null;
            }

            if (_staminaFillImage != null)
                _staminaFillImage.color = _normalColor;

            if (_staminaBackgroundImage != null)
            {
                var c = _staminaBackgroundImage.color;
                c.a = 1f;
                _staminaBackgroundImage.color = c;
            }
        }

        private IEnumerator CriticalPulseRoutine()
        {
            while (_isCritical)
            {
                float t = (Mathf.Sin(Time.time * _criticalPulseSpeed) + 1f) * 0.5f;

                if (_staminaFillImage != null)
                    _staminaFillImage.color = Color.Lerp(_criticalColor, Color.red, t * 0.3f);

                if (_staminaBackgroundImage != null)
                {
                    var c = _staminaBackgroundImage.color;
                    c.a = Mathf.Lerp(0.5f, 1f, t);
                    _staminaBackgroundImage.color = c;
                }

                yield return null;
            }
        }
    }
}

