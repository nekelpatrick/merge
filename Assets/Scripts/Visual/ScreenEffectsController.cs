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
    }
}

