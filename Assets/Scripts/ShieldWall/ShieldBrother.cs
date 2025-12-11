using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;

namespace ShieldWall.ShieldWall
{
    public class ShieldBrother
    {
        public ShieldBrotherSO Data { get; }
        public WallPosition Position { get; }
        public int CurrentHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        public ShieldBrother(ShieldBrotherSO data, WallPosition position)
        {
            Data = data;
            Position = position;
            CurrentHealth = data.maxHealth;
        }

        public void TakeDamage(int amount)
        {
            int previousHealth = CurrentHealth;
            CurrentHealth -= amount;
            
            if (CurrentHealth < 0) CurrentHealth = 0;

            if (CurrentHealth != previousHealth)
            {
                GameEvents.RaiseBrotherWounded(Data, amount);
                Debug.Log($"ShieldBrother {Data.brotherName}: Took {amount} damage. Health: {CurrentHealth}/{Data.maxHealth}");
            }

            if (CurrentHealth <= 0 && previousHealth > 0)
            {
                GameEvents.RaiseBrotherDied(Data);
                Debug.Log($"ShieldBrother {Data.brotherName}: Died!");
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth += amount;
            if (CurrentHealth > Data.maxHealth)
            {
                CurrentHealth = Data.maxHealth;
            }
        }

        public bool AttemptAutoDefense()
        {
            float chance = Data.autoDefendChance;
            bool success = Random.value < chance;
            
            Debug.Log($"ShieldBrother {Data.brotherName}: Auto-defense {(success ? "SUCCESS" : "FAILED")} ({chance * 100}% chance)");
            
            return success;
        }

        public void Reset()
        {
            CurrentHealth = Data.maxHealth;
        }
    }
}

