using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _scenariosButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _settingsBackButton;

        [Header("Panels")]
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _scenarioPanel;
        [SerializeField] private GameObject _settingsPanel;

        void Start()
        {
            ShowMainPanel();

            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlayClicked);
            if (_scenariosButton != null)
                _scenariosButton.onClick.AddListener(OnScenariosClicked);
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);
            if (_quitButton != null)
                _quitButton.onClick.AddListener(OnQuitClicked);
            if (_settingsBackButton != null)
                _settingsBackButton.onClick.AddListener(ShowMainPanel);
        }

        private void OnPlayClicked()
        {
            ScenarioManager.Instance?.LoadDefaultScenario();
            SceneLoader.Instance?.LoadBattle();
        }

        private void OnScenariosClicked()
        {
            ShowScenarioPanel();
        }

        private void OnSettingsClicked()
        {
            ShowSettingsPanel();
        }

        private void OnQuitClicked()
        {
            SceneLoader.Instance?.QuitGame();
        }

        public void ShowMainPanel()
        {
            if (_mainPanel != null)
                _mainPanel.SetActive(true);
            if (_scenarioPanel != null)
                _scenarioPanel.SetActive(false);
            if (_settingsPanel != null)
                _settingsPanel.SetActive(false);
        }

        public void ShowScenarioPanel()
        {
            if (_mainPanel != null)
                _mainPanel.SetActive(false);
            if (_scenarioPanel != null)
                _scenarioPanel.SetActive(true);
            if (_settingsPanel != null)
                _settingsPanel.SetActive(false);
        }

        public void ShowSettingsPanel()
        {
            if (_mainPanel != null)
                _mainPanel.SetActive(false);
            if (_scenarioPanel != null)
                _scenarioPanel.SetActive(false);
            if (_settingsPanel != null)
                _settingsPanel.SetActive(true);
        }

        void OnDestroy()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(OnPlayClicked);
            if (_scenariosButton != null)
                _scenariosButton.onClick.RemoveListener(OnScenariosClicked);
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(OnQuitClicked);
            if (_settingsBackButton != null)
                _settingsBackButton.onClick.RemoveListener(ShowMainPanel);
        }
    }
}

