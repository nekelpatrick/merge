using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class StaminaUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider _staminaSlider;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _staminaText;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = new Color(0.24f, 0.35f, 0.43f);
        [SerializeField] private Color _warningColor = new Color(0.77f, 0.36f, 0.15f);
        [SerializeField] private Color _criticalColor = new Color(0.55f, 0.13f, 0.13f);

        [Header("Settings")]
        [SerializeField] private int _warningThreshold = 4;
        [SerializeField] private int _criticalThreshold = 2;
        [SerializeField] private int _maxStamina = 12;

        private int _currentStamina;

        private void Awake()
        {
            _currentStamina = _maxStamina;
            UpdateVisuals();
        }

        private void OnEnable()
        {
            GameEvents.OnStaminaChanged += HandleStaminaChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnStaminaChanged -= HandleStaminaChanged;
        }

        private void HandleStaminaChanged(int stamina)
        {
            _currentStamina = stamina;
            UpdateVisuals();
        }

        public void SetMaxStamina(int max)
        {
            _maxStamina = max;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (_staminaSlider != null)
            {
                _staminaSlider.maxValue = _maxStamina;
                _staminaSlider.value = _currentStamina;
            }

            if (_staminaText != null)
            {
                _staminaText.text = $"{_currentStamina}/{_maxStamina}";
            }

            UpdateColor();
        }

        private void UpdateColor()
        {
            Color targetColor;
            
            if (_currentStamina <= _criticalThreshold)
            {
                targetColor = _criticalColor;
            }
            else if (_currentStamina <= _warningThreshold)
            {
                targetColor = _warningColor;
            }
            else
            {
                targetColor = _normalColor;
            }

            if (_fillImage != null)
            {
                _fillImage.color = targetColor;
            }

            if (_staminaText != null)
            {
                _staminaText.color = _currentStamina <= _criticalThreshold ? _criticalColor : Color.white;
            }
        }
    }
}

