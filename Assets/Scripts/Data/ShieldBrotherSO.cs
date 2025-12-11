using UnityEngine;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Brother_", menuName = "ShieldWall/ShieldBrother")]
    public class ShieldBrotherSO : ScriptableObject
    {
        public string brotherName;
        public int maxHealth = 3;
        [Range(0f, 1f)] public float autoDefendChance = 0.5f;
        public string specialty;
        public Sprite portrait;
    }
}

