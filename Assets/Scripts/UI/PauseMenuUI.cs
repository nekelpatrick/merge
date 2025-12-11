using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;

        private bool _isPaused;

        void Start()
        {
            if (_pausePanel != null)
                _pausePanel.SetActive(false);

            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(Resume);
            if (_restartButton != null)
                _restartButton.onClick.AddListener(Restart);
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.AddListener(QuitToMenu);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        public void Pause()
        {
            _isPaused = true;
            if (_pausePanel != null)
                _pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            _isPaused = false;
            if (_pausePanel != null)
                _pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneLoader.Instance?.ReloadCurrentScene();
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneLoader.Instance?.LoadMainMenu();
        }

        void OnDestroy()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(Resume);
            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(Restart);
            if (_mainMenuButton != null)
                _mainMenuButton.onClick.RemoveListener(QuitToMenu);
        }
    }
}

