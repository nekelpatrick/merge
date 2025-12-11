using UnityEngine;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Enemy_", menuName = "ShieldWall/Enemy")]
    public class EnemySO : ScriptableObject
    {
        public string enemyName;
        public int health = 1;
        public int damage = 1;
        public EnemyTargetingType targeting = EnemyTargetingType.Random;
        public Sprite icon;
    }
}

