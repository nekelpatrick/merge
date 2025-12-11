using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ShieldWall.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private CanvasGroup _fadePanel;
        [SerializeField] private float _fadeDuration = 0.5f;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneWithFade(sceneName));
        }

        public void LoadMainMenu() => LoadScene("MainMenu");
        public void LoadBattle() => LoadScene("Battle");

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        private IEnumerator LoadSceneWithFade(string sceneName)
        {
            if (_fadePanel != null)
            {
                float elapsed = 0f;
                while (elapsed < _fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _fadePanel.alpha = elapsed / _fadeDuration;
                    yield return null;
                }
                _fadePanel.alpha = 1f;
            }

            yield return SceneManager.LoadSceneAsync(sceneName);

            if (_fadePanel != null)
            {
                float elapsed = 0f;
                while (elapsed < _fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    _fadePanel.alpha = 1f - (elapsed / _fadeDuration);
                    yield return null;
                }
                _fadePanel.alpha = 0f;
            }
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

