using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class ScenarioSelectUI : MonoBehaviour
    {
        [SerializeField] private List<BattleScenarioSO> _scenarios;
        [SerializeField] private Transform _cardContainer;
        [SerializeField] private ScenarioCardUI _cardPrefab;
        [SerializeField] private Button _backButton;
        [SerializeField] private MainMenuUI _mainMenu;

        private List<ScenarioCardUI> _cards = new List<ScenarioCardUI>();

        void Start()
        {
            CreateCards();
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);
        }

        private void CreateCards()
        {
            if (_cardPrefab == null || _cardContainer == null) return;

            foreach (var scenario in _scenarios)
            {
                if (scenario == null) continue;

                var card = Instantiate(_cardPrefab, _cardContainer);
                card.Initialize(scenario);
                card.OnSelected += HandleScenarioSelected;
                _cards.Add(card);
            }
        }

        private void HandleScenarioSelected(BattleScenarioSO scenario)
        {
            ScenarioManager.Instance?.SelectScenario(scenario);
            SceneLoader.Instance?.LoadBattle();
        }

        private void OnBackClicked()
        {
            if (_mainMenu != null)
                _mainMenu.ShowMainPanel();
        }

        void OnDestroy()
        {
            foreach (var card in _cards)
            {
                if (card != null)
                    card.OnSelected -= HandleScenarioSelected;
            }

            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);
        }
    }
}

