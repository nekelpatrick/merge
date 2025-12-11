using UnityEngine;
using TMPro;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class PhaseBannerUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _phaseText;
        [SerializeField] private TextMeshProUGUI _ctaText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Settings")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _displayDuration = 2f;

        [Header("Phase Messages")]
        [SerializeField] private string _waveStartPhaseText = "ENEMIES APPROACH";
        [SerializeField] private string _waveStartCTAText = "Prepare to defend!";
        
        [SerializeField] private string _playerTurnPhaseText = "YOUR TURN";
        [SerializeField] private string _playerTurnCTAText = "Lock dice to ready actions, then confirm";
        
        [SerializeField] private string _resolutionPhaseText = "RESOLVING";
        [SerializeField] private string _resolutionCTAText = "Actions executing...";
        
        [SerializeField] private string _waveEndPhaseText = "TURN COMPLETE";
        [SerializeField] private string _waveEndCTAText = "Stamina decreased";

        private Coroutine _fadeCoroutine;

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        private void Start()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0;
            }
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            string phaseText = "";
            string ctaText = "";

            switch (phase)
            {
                case TurnPhase.WaveStart:
                    phaseText = _waveStartPhaseText;
                    ctaText = _waveStartCTAText;
                    break;
                case TurnPhase.PlayerTurn:
                    phaseText = _playerTurnPhaseText;
                    ctaText = _playerTurnCTAText;
                    break;
                case TurnPhase.Resolution:
                    phaseText = _resolutionPhaseText;
                    ctaText = _resolutionCTAText;
                    break;
                case TurnPhase.WaveEnd:
                    phaseText = _waveEndPhaseText;
                    ctaText = _waveEndCTAText;
                    break;
            }

            ShowBanner(phaseText, ctaText);
        }

        private void ShowBanner(string phaseText, string ctaText)
        {
            if (_phaseText != null)
            {
                _phaseText.text = phaseText;
            }

            if (_ctaText != null)
            {
                _ctaText.text = ctaText;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeInAndOut());
        }

        private System.Collections.IEnumerator FadeInAndOut()
        {
            if (_canvasGroup == null) yield break;

            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / _fadeInDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1;

            yield return new WaitForSeconds(_displayDuration);

            elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / _fadeInDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0;
        }
    }
}
