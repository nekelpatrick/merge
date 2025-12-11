using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class TutorialHintUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private Button _dismissButton;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.2f;

        private TutorialHintSO _currentHint;
        private float _displayTimer;
        private bool _isShowing;

        void Awake()
        {
            _panel.SetActive(false);
            if (_dismissButton != null)
                _dismissButton.onClick.AddListener(OnDismissClicked);
        }

        void Update()
        {
            if (_isShowing && _currentHint != null && _currentHint.autoDismiss)
            {
                _displayTimer -= Time.unscaledDeltaTime;
                if (_displayTimer <= 0)
                    HideHint();
            }
        }

        public void ShowHint(TutorialHintSO hint)
        {
            _currentHint = hint;
            _hintText.text = hint.hintText;
            _displayTimer = hint.displayDuration;

            _panel.SetActive(true);
            _isShowing = true;

            StartCoroutine(FadeIn());
        }

        public void HideHint()
        {
            _isShowing = false;
            StartCoroutine(FadeOut());
        }

        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = elapsed / _fadeInDuration;
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            float elapsed = 0f;
            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = 1f - (elapsed / _fadeOutDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
            _panel.SetActive(false);
            _currentHint = null;
        }

        private void OnDismissClicked()
        {
            Tutorial.TutorialManager.Instance?.DismissCurrentHint();
        }
    }
}

