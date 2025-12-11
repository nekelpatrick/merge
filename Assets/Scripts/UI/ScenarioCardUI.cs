using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class ScenarioCardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _difficultyText;
        [SerializeField] private TextMeshProUGUI _waveCountText;
        [SerializeField] private Button _selectButton;
        [SerializeField] private GameObject _lockedOverlay;
        [SerializeField] private Image _difficultyIcon;

        [Header("Difficulty Colors")]
        [SerializeField] private Color _easyColor = new Color(0.24f, 0.36f, 0.24f);
        [SerializeField] private Color _normalColor = new Color(0.79f, 0.64f, 0.15f);
        [SerializeField] private Color _hardColor = new Color(0.55f, 0.13f, 0.13f);

        private BattleScenarioSO _scenario;

        public event Action<BattleScenarioSO> OnSelected;

        public void Initialize(BattleScenarioSO scenario)
        {
            _scenario = scenario;

            if (_nameText != null)
                _nameText.text = scenario.scenarioName;
            if (_descriptionText != null)
                _descriptionText.text = scenario.description;
            if (_waveCountText != null)
                _waveCountText.text = $"{scenario.waves.Count} Waves";

            SetupDifficulty(scenario.difficulty);
            SetupLockState(scenario.isUnlocked);

            if (_selectButton != null)
                _selectButton.onClick.AddListener(OnSelectClicked);
        }

        private void SetupDifficulty(Difficulty difficulty)
        {
            if (_difficultyText != null)
            {
                _difficultyText.text = difficulty.ToString();

                Color color = difficulty switch
                {
                    Difficulty.Easy => _easyColor,
                    Difficulty.Normal => _normalColor,
                    Difficulty.Hard => _hardColor,
                    _ => _normalColor
                };

                _difficultyText.color = color;
                if (_difficultyIcon != null)
                    _difficultyIcon.color = color;
            }
        }

        private void SetupLockState(bool unlocked)
        {
            if (_selectButton != null)
                _selectButton.interactable = unlocked;
            if (_lockedOverlay != null)
                _lockedOverlay.SetActive(!unlocked);
        }

        private void OnSelectClicked()
        {
            OnSelected?.Invoke(_scenario);
        }

        void OnDestroy()
        {
            if (_selectButton != null)
                _selectButton.onClick.RemoveListener(OnSelectClicked);
        }
    }
}

