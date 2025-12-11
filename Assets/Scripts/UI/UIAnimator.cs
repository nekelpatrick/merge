using System.Collections;
using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public static class UIAnimator
    {
        public static IEnumerator PunchScale(RectTransform target, float punchAmount, float duration)
        {
            Vector3 original = target.localScale;
            Vector3 punch = Vector3.one * punchAmount;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float punchValue = Tweener.Evaluate(t, EaseType.Punch);
                target.localScale = original + punch * punchValue;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            target.localScale = original;
        }

        public static IEnumerator Shake(RectTransform target, float intensity, float duration)
        {
            Vector2 original = target.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = 1f - (elapsed / duration);
                float currentIntensity = intensity * t;
                target.anchoredPosition = original + new Vector2(
                    Random.Range(-1f, 1f) * currentIntensity,
                    Random.Range(-1f, 1f) * currentIntensity
                );
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            target.anchoredPosition = original;
        }

        public static IEnumerator FadeIn(CanvasGroup group, float duration, EaseType ease = EaseType.EaseOutQuad)
        {
            float elapsed = 0f;
            group.alpha = 0f;

            while (elapsed < duration)
            {
                float t = Tweener.Evaluate(elapsed / duration, ease);
                group.alpha = t;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            group.alpha = 1f;
        }

        public static IEnumerator FadeOut(CanvasGroup group, float duration, EaseType ease = EaseType.EaseOutQuad)
        {
            float elapsed = 0f;
            float startAlpha = group.alpha;

            while (elapsed < duration)
            {
                float t = Tweener.Evaluate(elapsed / duration, ease);
                group.alpha = Mathf.Lerp(startAlpha, 0f, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            group.alpha = 0f;
        }

        public static IEnumerator SlideIn(RectTransform target, Vector2 fromOffset, float duration, EaseType ease = EaseType.EaseOutBack)
        {
            Vector2 targetPos = target.anchoredPosition;
            Vector2 startPos = targetPos + fromOffset;
            target.anchoredPosition = startPos;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Tweener.Evaluate(elapsed / duration, ease);
                target.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            target.anchoredPosition = targetPos;
        }

        public static IEnumerator ScaleBounce(RectTransform target, float targetScale, float duration)
        {
            Vector3 original = target.localScale;
            Vector3 target3 = Vector3.one * targetScale;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Tweener.Evaluate(elapsed / duration, EaseType.EaseOutElastic);
                target.localScale = Vector3.LerpUnclamped(original, target3, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            target.localScale = target3;
        }

        public static IEnumerator FlashColor(UnityEngine.UI.Graphic graphic, Color flashColor, float duration)
        {
            Color original = graphic.color;
            graphic.color = flashColor;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                graphic.color = Color.Lerp(flashColor, original, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            graphic.color = original;
        }

        public static IEnumerator Pulse(RectTransform target, float minScale, float maxScale, float speed)
        {
            Vector3 min = Vector3.one * minScale;
            Vector3 max = Vector3.one * maxScale;

            while (true)
            {
                float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
                target.localScale = Vector3.Lerp(min, max, t);
                yield return null;
            }
        }
    }
}

