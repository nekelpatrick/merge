using System;
using UnityEngine;

namespace ShieldWall.Core
{
    public class StaminaManager : MonoBehaviour
    {
        [SerializeField] private int _startingStamina = 12;
        [SerializeField] private int _staminaPerTurn = 1;
        [SerializeField] private int _staminaPerReroll = 1;
        [SerializeField] private int _staminaOnWaveClear = 2;

        private int _currentStamina;

        public int CurrentStamina => _currentStamina;
        public int MaxStamina => _startingStamina;
        public bool IsExhausted => _currentStamina <= 0;

        public static event Action OnExhausted;

        private void Awake()
        {
            _currentStamina = _startingStamina;
        }

        public void Initialize()
        {
            _currentStamina = _startingStamina;
            GameEvents.RaiseStaminaChanged(_currentStamina);
        }

        public bool CanSpend(int amount)
        {
            return _currentStamina >= amount;
        }

        public bool SpendStamina(int amount)
        {
            if (_currentStamina < amount) return false;
            
            _currentStamina -= amount;
            GameEvents.RaiseStaminaChanged(_currentStamina);
            
            Debug.Log($"StaminaManager: Spent {amount} stamina. Remaining: {_currentStamina}");
            
            CheckExhaustion();
            return true;
        }

        public void SpendReroll()
        {
            SpendStamina(_staminaPerReroll);
        }

        public void TickTurnEnd()
        {
            SpendStamina(_staminaPerTurn);
        }

        public void RestoreStamina(int amount)
        {
            _currentStamina += amount;
            if (_currentStamina > _startingStamina) _currentStamina = _startingStamina;
            
            GameEvents.RaiseStaminaChanged(_currentStamina);
            Debug.Log($"StaminaManager: Restored {amount} stamina. Current: {_currentStamina}");
        }

        public void OnWaveCleared()
        {
            RestoreStamina(_staminaOnWaveClear);
        }

        private void CheckExhaustion()
        {
            if (_currentStamina <= 0)
            {
                Debug.Log("StaminaManager: Player exhausted!");
                OnExhausted?.Invoke();
                GameEvents.RaiseBattleEnded(false);
            }
        }
    }
}

