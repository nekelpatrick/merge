using UnityEngine;

namespace ShieldWall.Audio
{
    public static class AudioUtility
    {
        public const float MIN_DECIBELS = -80f;
        public const float MAX_DECIBELS = 0f;

        public static float NormalizedToDecibels(float normalized)
        {
            if (normalized <= 0f) return MIN_DECIBELS;
            return Mathf.Log10(normalized) * 20f;
        }

        public static float DecibelsToNormalized(float decibels)
        {
            if (decibels <= MIN_DECIBELS) return 0f;
            return Mathf.Pow(10f, decibels / 20f);
        }
    }
}

