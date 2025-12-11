using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Hover Settings")]
        [SerializeField] private float _hoverScale = 1.05f;
        [SerializeField] private float _hoverDuration = 0.1f;

        [Header("Click Settings")]
        [SerializeField] private float _clickPunchScale = 0.1f;
        [SerializeField] private float _clickPunchDuration = 0.15f;

        [Header("Disabled Settings")]
        [SerializeField] private float _disabledAlpha = 0.5f;

        [Header("Glow Settings")]
        [SerializeField] private bool _enableGlow;
        [SerializeField] private Image _glowImage;
        [SerializeField] private Color _glowColor = new Color(1f, 0.9f, 0.5f, 0.3f);

        private RectTransform _rectTransform;
        private Button _button;
        private Vector3 _originalScale;
        private Coroutine _scaleCoroutine;
        private Coroutine _glowCoroutine;
        private bool _isHovered;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
            _originalScale = _rectTransform.localScale;

            if (_glowImage != null)
            {
                var c = _glowColor;
                c.a = 0f;
                _glowImage.color = c;
            }
        }

        private void OnEnable()
        {
            UpdateInteractableState();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            _isHovered = true;
            AnimateToScale(_originalScale * _hoverScale);

            if (_enableGlow && _glowImage != null)
            {
                StartGlow();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            AnimateToScale(_originalScale);
            StopGlow();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            if (_scaleCoroutine != null)
                StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = StartCoroutine(UIAnimator.PunchScale(_rectTransform, _clickPunchScale, _clickPunchDuration));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isHovered)
                AnimateToScale(_originalScale * _hoverScale);
            else
                AnimateToScale(_originalScale);
        }

        private void AnimateToScale(Vector3 targetScale)
        {
            if (_scaleCoroutine != null)
                StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = StartCoroutine(ScaleRoutine(targetScale));
        }

        private IEnumerator ScaleRoutine(Vector3 targetScale)
        {
            Vector3 startScale = _rectTransform.localScale;
            float elapsed = 0f;

            while (elapsed < _hoverDuration)
            {
                float t = Tweener.Evaluate(elapsed / _hoverDuration, EaseType.EaseOutQuad);
                _rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _rectTransform.localScale = targetScale;
            _scaleCoroutine = null;
        }

        private void StartGlow()
        {
            if (_glowCoroutine != null)
                StopCoroutine(_glowCoroutine);
            _glowCoroutine = StartCoroutine(GlowRoutine());
        }

        private void StopGlow()
        {
            if (_glowCoroutine != null)
            {
                StopCoroutine(_glowCoroutine);
                _glowCoroutine = null;
            }

            if (_glowImage != null)
            {
                var c = _glowColor;
                c.a = 0f;
                _glowImage.color = c;
            }
        }

        private IEnumerator GlowRoutine()
        {
            while (_isHovered && _glowImage != null)
            {
                float t = (Mathf.Sin(Time.unscaledTime * 3f) + 1f) * 0.5f;
                var c = _glowColor;
                c.a = Mathf.Lerp(_glowColor.a * 0.5f, _glowColor.a, t);
                _glowImage.color = c;
                yield return null;
            }
        }

        private void UpdateInteractableState()
        {
            if (!_button.interactable)
            {
                var colors = _button.colors;
                colors.disabledColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, _disabledAlpha);
                _button.colors = colors;
            }
        }

        private void OnDisable()
        {
            StopGlow();
            if (_scaleCoroutine != null)
                StopCoroutine(_scaleCoroutine);
            _rectTransform.localScale = _originalScale;
        }
    }
}

