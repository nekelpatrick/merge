using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using ShieldWall.Data;
using ShieldWall.Dice;

namespace ShieldWall.UI
{
    public class ActionButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _runeRequirementText;
        [SerializeField] private Image _selectedOverlay;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = new Color(1f, 0.9f, 0.5f);

        [Header("Events")]
        public UnityEvent<ActionSO> OnActionClicked;

        private ActionSO _action;
        private bool _isSelected;

        public ActionSO Action => _action;
        public bool IsSelected => _isSelected;

        private void Awake()
        {
            if (_button == null) _button = GetComponent<Button>();
            if (_button != null) _button.onClick.AddListener(HandleClick);
            
            if (_selectedOverlay != null) _selectedOverlay.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(HandleClick);
        }

        public void SetAction(ActionSO action)
        {
            _action = action;
            _isSelected = false;
            UpdateDisplay();
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            UpdateSelectionVisual();
        }

        private void UpdateDisplay()
        {
            if (_action == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_nameText != null)
            {
                _nameText.text = _action.actionName;
            }

            if (_iconImage != null && _action.icon != null)
            {
                _iconImage.sprite = _action.icon;
                _iconImage.gameObject.SetActive(true);
            }
            else if (_iconImage != null)
            {
                _iconImage.gameObject.SetActive(false);
            }

            if (_runeRequirementText != null)
            {
                _runeRequirementText.text = GetRuneRequirementString();
            }

            UpdateSelectionVisual();
        }

        private void UpdateSelectionVisual()
        {
            if (_selectedOverlay != null)
            {
                _selectedOverlay.gameObject.SetActive(_isSelected);
            }

            if (_button != null)
            {
                var colors = _button.colors;
                colors.normalColor = _isSelected ? _selectedColor : _normalColor;
                _button.colors = colors;
            }
        }

        private string GetRuneRequirementString()
        {
            if (_action == null || _action.requiredRunes == null) return "";

            var sb = new System.Text.StringBuilder();
            foreach (var rune in _action.requiredRunes)
            {
                sb.Append(GetRuneSymbol(rune));
            }
            return sb.ToString();
        }

        private string GetRuneSymbol(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "[SH]",
                RuneType.Tyr => "[AX]",
                RuneType.Gebo => "[SP]",
                RuneType.Berkana => "[BR]",
                RuneType.Othala => "[OD]",
                RuneType.Laguz => "[LO]",
                _ => "[?]"
            };
        }

        private void HandleClick()
        {
            OnActionClicked?.Invoke(_action);
        }
    }
}

