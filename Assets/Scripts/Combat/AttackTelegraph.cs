using System.Collections;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Visual;

namespace ShieldWall.Combat
{
    public class AttackTelegraph : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color _telegraphColor = new Color(1f, 0.3f, 0.3f, 0.8f);
        [SerializeField] private float _flashSpeed = 4f;
        [SerializeField] private float _pulseScale = 1.1f;

        [Header("Timing")]
        [SerializeField] private float _telegraphDuration = 0.4f;
        [SerializeField] private float _windupDuration = 0.2f;

        [Header("Audio")]
        [SerializeField] private AudioClip _telegraphSound;
        [SerializeField] private AudioSource _audioSource;

        public float TelegraphDuration => _telegraphDuration + _windupDuration;

        public IEnumerator PlayTelegraph(Transform enemyVisual, System.Action onComplete = null)
        {
            if (enemyVisual == null)
            {
                yield return new WaitForSeconds(TelegraphDuration);
                onComplete?.Invoke();
                yield break;
            }

            PlayTelegraphSound();

            var renderers = enemyVisual.GetComponentsInChildren<Renderer>();
            var originalColors = new Color[renderers.Length];
            var originalScales = new Vector3[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].material.color;
                originalScales[i] = renderers[i].transform.localScale;
            }

            float elapsed = 0f;
            while (elapsed < _telegraphDuration)
            {
                float t = elapsed / _telegraphDuration;
                float flash = (Mathf.Sin(t * _flashSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
                float scale = 1f + (flash * (_pulseScale - 1f));

                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = Color.Lerp(originalColors[i], _telegraphColor, flash * 0.5f);
                    renderers[i].transform.localScale = originalScales[i] * scale;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            Vector3 currentPos = enemyVisual.position;
            Vector3 pullbackPos = currentPos + Vector3.back * 0.3f;

            elapsed = 0f;
            while (elapsed < _windupDuration * 0.6f)
            {
                float t = Tweener.Evaluate(elapsed / (_windupDuration * 0.6f), EaseType.EaseOutQuad);
                enemyVisual.position = Vector3.Lerp(currentPos, pullbackPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            Vector3 strikePos = currentPos + Vector3.forward * 0.2f;
            elapsed = 0f;
            while (elapsed < _windupDuration * 0.4f)
            {
                float t = Tweener.Evaluate(elapsed / (_windupDuration * 0.4f), EaseType.EaseInQuad);
                enemyVisual.position = Vector3.Lerp(pullbackPos, strikePos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
                renderers[i].transform.localScale = originalScales[i];
            }
            enemyVisual.position = currentPos;

            onComplete?.Invoke();
        }

        public IEnumerator PlayGroupTelegraph(Transform[] enemyVisuals, System.Action onComplete = null)
        {
            if (enemyVisuals == null || enemyVisuals.Length == 0)
            {
                yield return new WaitForSeconds(TelegraphDuration);
                onComplete?.Invoke();
                yield break;
            }

            PlayTelegraphSound();

            var allRenderers = new System.Collections.Generic.List<(Renderer renderer, Color originalColor, Vector3 originalScale, Transform parent)>();

            foreach (var visual in enemyVisuals)
            {
                if (visual == null) continue;
                var renderers = visual.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    allRenderers.Add((r, r.material.color, r.transform.localScale, visual));
                }
            }

            float elapsed = 0f;
            while (elapsed < _telegraphDuration)
            {
                float t = elapsed / _telegraphDuration;
                float flash = (Mathf.Sin(t * _flashSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
                float scale = 1f + (flash * (_pulseScale - 1f));

                foreach (var (renderer, originalColor, originalScale, _) in allRenderers)
                {
                    renderer.material.color = Color.Lerp(originalColor, _telegraphColor, flash * 0.5f);
                    renderer.transform.localScale = originalScale * scale;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (var (renderer, originalColor, originalScale, _) in allRenderers)
            {
                renderer.material.color = originalColor;
                renderer.transform.localScale = originalScale;
            }

            onComplete?.Invoke();
        }

        private void PlayTelegraphSound()
        {
            if (_telegraphSound != null && _audioSource != null)
            {
                _audioSource.pitch = Random.Range(0.9f, 1.1f);
                _audioSource.PlayOneShot(_telegraphSound, 0.5f);
            }
        }
    }
}

