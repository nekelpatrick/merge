using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _victoryPanel;
        [SerializeField] private GameObject _defeatPanel;

        [Header("Victory UI")]
        [SerializeField] private TextMeshProUGUI _victoryTitleText;
        [SerializeField] private TextMeshProUGUI _victoryStatsText;
        [SerializeField] private Button _victoryRestartButton;

        [Header("Defeat UI")]
        [SerializeField] private TextMeshProUGUI _defeatTitleText;
        [SerializeField] private TextMeshProUGUI _defeatCauseText;
        [SerializeField] private Button _defeatRestartButton;

        [Header("References")]
        [SerializeField] private BattleManager _battleManager;

        private void Awake()
        {
            HideAll();
        }

        private void OnEnable()
        {
            GameEvents.OnBattleEnded += HandleBattleEnded;
            
            if (_victoryRestartButton != null)
                _victoryRestartButton.onClick.AddListener(HandleRestart);
            if (_defeatRestartButton != null)
                _defeatRestartButton.onClick.AddListener(HandleRestart);
        }

        private void OnDisable()
        {
            GameEvents.OnBattleEnded -= HandleBattleEnded;
            
            if (_victoryRestartButton != null)
                _victoryRestartButton.onClick.RemoveListener(HandleRestart);
            if (_defeatRestartButton != null)
                _defeatRestartButton.onClick.RemoveListener(HandleRestart);
        }

        private void HideAll()
        {
            if (_victoryPanel != null) _victoryPanel.SetActive(false);
            if (_defeatPanel != null) _defeatPanel.SetActive(false);
        }

        private void HandleBattleEnded(bool victory)
        {
            if (victory)
            {
                ShowVictory();
            }
            else
            {
                ShowDefeat();
            }
        }

        private void ShowVictory()
        {
            HideAll();
            
            if (_victoryPanel != null)
            {
                _victoryPanel.SetActive(true);
            }

            if (_victoryTitleText != null)
            {
                _victoryTitleText.text = "VICTORY!";
            }

            if (_victoryStatsText != null)
            {
                _victoryStatsText.text = "The shield wall held. Glory to the fallen!";
            }
        }

        private void ShowDefeat()
        {
            HideAll();
            
            if (_defeatPanel != null)
            {
                _defeatPanel.SetActive(true);
            }

            if (_defeatTitleText != null)
            {
                _defeatTitleText.text = "DEFEAT";
            }

            if (_defeatCauseText != null)
            {
                var cause = DetermineDefeatCause();
                _defeatCauseText.text = cause;
            }
        }

        private string DetermineDefeatCause()
        {
            var staminaManager = FindFirstObjectByType<StaminaManager>();
            if (staminaManager != null && staminaManager.IsExhausted)
            {
                return "Exhaustion claimed you. Your strength failed.";
            }

            var player = FindFirstObjectByType<ShieldWall.PlayerWarrior>();
            if (player != null && player.IsDead)
            {
                return "You fell in battle. The wall is broken.";
            }

            return "The shield wall has fallen.";
        }

        private void HandleRestart()
        {
            _battleManager?.RestartBattle();
        }
    }
}

