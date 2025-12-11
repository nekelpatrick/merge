using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShieldWall.Core
{
    public class BattleManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private StaminaManager _staminaManager;

        [Header("Settings")]
        [SerializeField] private bool _autoStartBattle = true;

        private bool _battleEnded;
        private bool _wasVictory;

        public bool BattleEnded => _battleEnded;
        public bool WasVictory => _wasVictory;

        private void OnEnable()
        {
            GameEvents.OnBattleEnded += HandleBattleEnded;
            StaminaManager.OnExhausted += HandleExhaustion;
        }

        private void OnDisable()
        {
            GameEvents.OnBattleEnded -= HandleBattleEnded;
            StaminaManager.OnExhausted -= HandleExhaustion;
        }

        private void Start()
        {
            if (_autoStartBattle)
            {
                StartBattle();
            }
        }

        [ContextMenu("Start Battle")]
        public void StartBattle()
        {
            _battleEnded = false;
            _wasVictory = false;
            
            Debug.Log("BattleManager: Starting battle...");
            
            _turnManager?.StartBattle();
        }

        private void HandleBattleEnded(bool victory)
        {
            if (_battleEnded) return;
            
            _battleEnded = true;
            _wasVictory = victory;
            
            if (victory)
            {
                Debug.Log("BattleManager: VICTORY! All waves cleared!");
            }
            else
            {
                Debug.Log("BattleManager: DEFEAT! The shield wall has fallen.");
            }
        }

        private void HandleExhaustion()
        {
            if (_battleEnded) return;
            
            Debug.Log("BattleManager: Player exhausted - stamina depleted!");
        }

        public void RestartBattle()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

