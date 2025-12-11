using UnityEngine;

namespace ShieldWall.Dice
{
    public class RuneDie
    {
        public RuneType CurrentFace { get; private set; }
        public bool IsLocked { get; set; }

        private readonly float[] _weights = { 1f, 1f, 1f, 1f, 0.5f, 0.5f };

        public RuneType Roll()
        {
            if (IsLocked) return CurrentFace;
            CurrentFace = GetWeightedRandom();
            return CurrentFace;
        }

        public void Reset()
        {
            IsLocked = false;
            CurrentFace = RuneType.Thurs;
        }

        private RuneType GetWeightedRandom()
        {
            float totalWeight = 0f;
            foreach (var w in _weights) totalWeight += w;

            float random = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < _weights.Length; i++)
            {
                cumulative += _weights[i];
                if (random <= cumulative)
                    return (RuneType)i;
            }

            return RuneType.Thurs;
        }
    }
}

