using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;

namespace ShieldWall.Combat
{
    public class Enemy
    {
        public EnemySO Data { get; }
        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        public Enemy(EnemySO data)
        {
            Data = data;
            CurrentHealth = data.health;
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public Attack CreateAttack(WallPosition target)
        {
            return new Attack
            {
                Source = Data,
                Damage = Data.damage,
                Target = target
            };
        }

        public void Reset()
        {
            CurrentHealth = Data.health;
        }
    }
}

