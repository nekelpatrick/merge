using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Formation;

namespace ShieldWall.ShieldWall
{
    public class PlayerWarrior : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 5;

        private int _currentHealth;

        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        public bool IsDead => _currentHealth <= 0;
        public WallPosition Position => WallPosition.Center;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void Initialize()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            int previousHealth = _currentHealth;
            _currentHealth -= amount;
            
            if (_currentHealth < 0) _currentHealth = 0;
            
            if (_currentHealth != previousHealth)
            {
                GameEvents.RaisePlayerWounded(amount);
                Debug.Log($"PlayerWarrior: Took {amount} damage. Health: {_currentHealth}/{_maxHealth}");
            }
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        }

        public bool IsAlive()
        {
            return _currentHealth > 0;
        }
    }
}

