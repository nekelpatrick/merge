using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class ScreenEffectsController : MonoBehaviour
    {
        [Header("Camera Shake")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _shakeIntensity = 0.1f;
        [SerializeField] private float _shakeDuration = 0.2f;

        [Header("Blood Vignette")]
        [SerializeField] private Image _vignetteImage;
        [SerializeField] private Color _damageColor = new Color(0.5f, 0, 0, 0.5f);
        [SerializeField] private float _vignetteFadeDuration = 0.5f;

        [Header("Hit Flash")]
        [SerializeField] private Image _flashImage;
        [SerializeField] private Color _flashColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private float _flashDuration = 0.1f;

        [Header("Stamina Pulse")]
        [SerializeField] private Image _staminaPulseImage;
        [SerializeField] private Color _staminaColor = new Color(61f/255f, 90f/255f, 110f/255f, 0.3f);
        [SerializeField] private float _staminaPulseDuration = 0.3f;

        private Vector3 _cameraOriginalPos;
        private bool _isShaking;

        private void Awake()
        {
            if (_cameraTransform == null)
            {
                _cameraTransform = Camera.main?.transform;
            }

            if (_cameraTransform != null)
            {
                _cameraOriginalPos = _cameraTransform.localPosition;
            }

            if (_vignetteImage != null)
            {
                _vignetteImage.color = Color.clear;
            }

            if (_flashImage != null)
            {
                _flashImage.color = Color.clear;
            }

            if (_staminaPulseImage != null)
            {
                _staminaPulseImage.color = Color.clear;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
            GameEvents.OnAttackLanded += HandleAttackLanded;
            GameEvents.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
            GameEvents.OnAttackLanded -= HandleAttackLanded;
            GameEvents.OnStaminaChanged -= HandleStaminaChanged;
        }

        private void HandlePlayerWounded(int damage)
        {
            StartCoroutine(ShakeCamera(_shakeIntensity * damage));
            StartCoroutine(FlashVignette(_damageColor));
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            StartCoroutine(ShakeCamera(_shakeIntensity * 0.5f));
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            StartCoroutine(FlashScreen(_flashColor));
        }

        private void HandleAttackBlocked(Attack attack)
        {
            StartCoroutine(ShakeCamera(_shakeIntensity * 0.3f));
        }

        private void HandleAttackLanded(Attack attack)
        {
            StartCoroutine(ShakeCamera(_shakeIntensity * 0.5f * attack.Damage));
        }

        private void HandleStaminaChanged(int newStamina)
        {
            StartCoroutine(PulseStamina());
        }

        [ContextMenu("Test Camera Shake")]
        public void TestShake()
        {
            StartCoroutine(ShakeCamera(_shakeIntensity));
        }

        [ContextMenu("Test Vignette")]
        public void TestVignette()
        {
            StartCoroutine(FlashVignette(_damageColor));
        }

        private IEnumerator ShakeCamera(float intensity)
        {
            if (_cameraTransform == null || _isShaking) yield break;

            _isShaking = true;
            float elapsed = 0f;

            while (elapsed < _shakeDuration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                _cameraTransform.localPosition = _cameraOriginalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _cameraTransform.localPosition = _cameraOriginalPos;
            _isShaking = false;
        }

        private IEnumerator FlashVignette(Color color)
        {
            if (_vignetteImage == null) yield break;

            _vignetteImage.color = color;
            
            float elapsed = 0f;
            while (elapsed < _vignetteFadeDuration)
            {
                float t = elapsed / _vignetteFadeDuration;
                _vignetteImage.color = Color.Lerp(color, Color.clear, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _vignetteImage.color = Color.clear;
        }

        private IEnumerator FlashScreen(Color color)
        {
            if (_flashImage == null) yield break;

            _flashImage.color = color;
            yield return new WaitForSeconds(_flashDuration);
            _flashImage.color = Color.clear;
        }

        private IEnumerator PulseStamina()
        {
            if (_staminaPulseImage == null) yield break;

            _staminaPulseImage.color = _staminaColor;
            
            float elapsed = 0f;
            while (elapsed < _staminaPulseDuration)
            {
                float t = elapsed / _staminaPulseDuration;
                _staminaPulseImage.color = Color.Lerp(_staminaColor, Color.clear, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _staminaPulseImage.color = Color.clear;
        }
    }
}

