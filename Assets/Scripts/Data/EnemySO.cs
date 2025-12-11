using UnityEngine;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Enemy_", menuName = "ShieldWall/Enemy")]
    public class EnemySO : ScriptableObject
    {
        [Header("Basic Stats")]
        public string enemyName;
        public int health = 1;
        public int damage = 1;
        public EnemyTargetingType targeting = EnemyTargetingType.Random;
        
        [Header("Special Abilities")]
        public bool ignoresBlocks;
        public bool destroysBlock;
        
        [Header("Visuals")]
        public Sprite icon;
        public Color tintColor = Color.white;
    }
}

