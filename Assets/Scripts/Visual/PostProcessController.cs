using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class PostProcessController : MonoBehaviour
    {
        [Header("Volume Reference")]
        [SerializeField] private Volume _volume;

        [Header("Damage Settings")]
        [SerializeField] private float _damageVignetteIntensity = 0.5f;
        [SerializeField] private float _damageChromaticAberration = 0.5f;
        [SerializeField] private float _damageEffectDuration = 0.3f;

        [Header("Kill Settings")]
        [SerializeField] private float _killSaturationBoost = 20f;
        [SerializeField] private float _killEffectDuration = 0.2f;

        private Vignette _vignette;
        private ChromaticAberration _chromaticAberration;
        private ColorAdjustments _colorAdjustments;

        private float _baseVignetteIntensity;
        private float _baseSaturation;
        private Coroutine _effectCoroutine;

        private void Awake()
        {
            if (_volume == null)
                _volume = FindFirstObjectByType<Volume>();

            if (_volume != null && _volume.profile != null)
            {
                _volume.profile.TryGet(out _vignette);
                _volume.profile.TryGet(out _chromaticAberration);
                _volume.profile.TryGet(out _colorAdjustments);

                if (_vignette != null)
                    _baseVignetteIntensity = _vignette.intensity.value;
                if (_colorAdjustments != null)
                    _baseSaturation = _colorAdjustments.saturation.value;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            ResetEffects();
        }

        private void HandlePlayerWounded(int damage)
        {
            TriggerDamageEffect(damage);
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            TriggerKillEffect();
        }

        public void TriggerDamageEffect(int intensity = 1)
        {
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(DamageEffectRoutine(intensity));
        }

        public void TriggerKillEffect()
        {
            if (_effectCoroutine != null)
                StopCoroutine(_effectCoroutine);
            _effectCoroutine = StartCoroutine(KillEffectRoutine());
        }

        private IEnumerator DamageEffectRoutine(int intensity)
        {
            float targetVignette = _baseVignetteIntensity + _damageVignetteIntensity * intensity;
            float targetChromatic = _damageChromaticAberration * intensity;

            if (_vignette != null)
                _vignette.intensity.Override(targetVignette);
            if (_chromaticAberration != null)
                _chromaticAberration.intensity.Override(targetChromatic);

            float elapsed = 0f;
            while (elapsed < _damageEffectDuration)
            {
                float t = elapsed / _damageEffectDuration;
                t = 1f - Mathf.Pow(1f - t, 2f);

                if (_vignette != null)
                    _vignette.intensity.Override(Mathf.Lerp(targetVignette, _baseVignetteIntensity, t));
                if (_chromaticAberration != null)
                    _chromaticAberration.intensity.Override(Mathf.Lerp(targetChromatic, 0f, t));

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            ResetEffects();
            _effectCoroutine = null;
        }

        private IEnumerator KillEffectRoutine()
        {
            float targetSaturation = _baseSaturation + _killSaturationBoost;

            if (_colorAdjustments != null)
                _colorAdjustments.saturation.Override(targetSaturation);

            float elapsed = 0f;
            while (elapsed < _killEffectDuration)
            {
                float t = elapsed / _killEffectDuration;
                if (_colorAdjustments != null)
                    _colorAdjustments.saturation.Override(Mathf.Lerp(targetSaturation, _baseSaturation, t));

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            ResetEffects();
            _effectCoroutine = null;
        }

        private void ResetEffects()
        {
            if (_vignette != null)
                _vignette.intensity.Override(_baseVignetteIntensity);
            if (_chromaticAberration != null)
                _chromaticAberration.intensity.Override(0f);
            if (_colorAdjustments != null)
                _colorAdjustments.saturation.Override(_baseSaturation);
        }

        [ContextMenu("Test Damage Effect")]
        public void TestDamageEffect() => TriggerDamageEffect(2);

        [ContextMenu("Test Kill Effect")]
        public void TestKillEffect() => TriggerKillEffect();
    }
}

