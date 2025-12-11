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
        public bool IsStunned { get; private set; }

        public Enemy(EnemySO data)
        {
            Data = data;
            CurrentHealth = data.health;
            IsStunned = false;
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public void ApplyStun()
        {
            IsStunned = true;
        }

        public void ClearStun()
        {
            IsStunned = false;
        }

        public Attack CreateAttack(WallPosition target)
        {
            return new Attack
            {
                Source = Data,
                Damage = Data.damage,
                Target = target,
                IgnoresBlocks = Data.ignoresBlocks,
                DestroysBlock = Data.destroysBlock
            };
        }

        public void Reset()
        {
            CurrentHealth = Data.health;
            IsStunned = false;
        }
    }
}

