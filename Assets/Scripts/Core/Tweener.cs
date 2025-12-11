using System;
using System.Collections;
using UnityEngine;

namespace ShieldWall.Core
{
    public enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseOutBack,
        EaseOutElastic,
        EaseOutBounce,
        Punch
    }

    public static class Tweener
    {
        public static float Evaluate(float t, EaseType ease)
        {
            t = Mathf.Clamp01(t);
            return ease switch
            {
                EaseType.Linear => t,
                EaseType.EaseInQuad => t * t,
                EaseType.EaseOutQuad => t * (2f - t),
                EaseType.EaseInOutQuad => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t,
                EaseType.EaseInCubic => t * t * t,
                EaseType.EaseOutCubic => 1f - Mathf.Pow(1f - t, 3f),
                EaseType.EaseInOutCubic => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f,
                EaseType.EaseOutBack => 1f + 2.70158f * Mathf.Pow(t - 1f, 3f) + 1.70158f * Mathf.Pow(t - 1f, 2f),
                EaseType.EaseOutElastic => t == 0f ? 0f : t == 1f ? 1f : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI) / 3f) + 1f,
                EaseType.EaseOutBounce => EaseOutBounceFunc(t),
                EaseType.Punch => Mathf.Sin(t * Mathf.PI) * (1f - t),
                _ => t
            };
        }

        private static float EaseOutBounceFunc(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
                return n1 * t * t;
            if (t < 2f / d1)
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1)
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        public static IEnumerator TweenFloat(float from, float to, float duration, EaseType ease, Action<float> onUpdate, Action onComplete = null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Evaluate(elapsed / duration, ease);
                onUpdate?.Invoke(Mathf.LerpUnclamped(from, to, t));
                elapsed += Time.deltaTime;
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }

        public static IEnumerator TweenVector3(Vector3 from, Vector3 to, float duration, EaseType ease, Action<Vector3> onUpdate, Action onComplete = null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Evaluate(elapsed / duration, ease);
                onUpdate?.Invoke(Vector3.LerpUnclamped(from, to, t));
                elapsed += Time.deltaTime;
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }

        public static IEnumerator TweenColor(Color from, Color to, float duration, EaseType ease, Action<Color> onUpdate, Action onComplete = null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Evaluate(elapsed / duration, ease);
                onUpdate?.Invoke(Color.LerpUnclamped(from, to, t));
                elapsed += Time.deltaTime;
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }

        public static IEnumerator PunchScale(Transform target, Vector3 punch, float duration)
        {
            Vector3 original = target.localScale;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float punchValue = Evaluate(t, EaseType.Punch);
                target.localScale = original + punch * punchValue;
                elapsed += Time.deltaTime;
                yield return null;
            }
            target.localScale = original;
        }

        public static IEnumerator PunchPosition(Transform target, Vector3 punch, float duration)
        {
            Vector3 original = target.localPosition;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float punchValue = Evaluate(t, EaseType.Punch);
                target.localPosition = original + punch * punchValue;
                elapsed += Time.deltaTime;
                yield return null;
            }
            target.localPosition = original;
        }

        public static IEnumerator PunchRotation(Transform target, Vector3 punch, float duration)
        {
            Quaternion original = target.localRotation;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float punchValue = Evaluate(t, EaseType.Punch);
                target.localRotation = original * Quaternion.Euler(punch * punchValue);
                elapsed += Time.deltaTime;
                yield return null;
            }
            target.localRotation = original;
        }

        public static IEnumerator Shake(Transform target, float intensity, float duration, bool useUnscaledTime = false)
        {
            Vector3 original = target.localPosition;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = 1f - (elapsed / duration);
                float currentIntensity = intensity * t;
                target.localPosition = original + new Vector3(
                    UnityEngine.Random.Range(-1f, 1f) * currentIntensity,
                    UnityEngine.Random.Range(-1f, 1f) * currentIntensity,
                    0f
                );
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
            target.localPosition = original;
        }

        public static IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration, EaseType ease = EaseType.EaseOutQuad)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Evaluate(elapsed / duration, ease);
                group.alpha = Mathf.Lerp(from, to, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            group.alpha = to;
        }
    }
}

