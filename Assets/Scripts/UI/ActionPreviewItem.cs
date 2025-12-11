using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Data;
using ShieldWall.Dice;

namespace ShieldWall.UI
{
    public class ActionPreviewItem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _actionNameText;
        [SerializeField] private TextMeshProUGUI _effectText;
        [SerializeField] private Transform _runeContainer;
        [SerializeField] private Image _runeBadgePrefab;
        [SerializeField] private Image _statusIcon;

        [Header("Colors")]
        [SerializeField] private Color _availableColor = new Color(201f/255f, 162f/255f, 39f/255f);
        [SerializeField] private Color _textColor = new Color(212f/255f, 200f/255f, 184f/255f);

        private ActionSO _action;
        private readonly System.Collections.Generic.List<Image> _runeBadges = new System.Collections.Generic.List<Image>();

        public void SetAction(ActionSO action, bool isAvailable)
        {
            _action = action;

            if (_action == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (_actionNameText != null)
            {
                _actionNameText.text = _action.actionName;
                _actionNameText.color = isAvailable ? _availableColor : _textColor;
            }

            if (_effectText != null)
            {
                _effectText.text = _action.description;
                _effectText.color = _textColor;
            }

            UpdateRuneBadges();

            if (_statusIcon != null)
            {
                _statusIcon.gameObject.SetActive(true);
                _statusIcon.color = isAvailable ? _availableColor : Color.gray;
            }
        }

        private void UpdateRuneBadges()
        {
            if (_action == null || _action.requiredRunes == null || _runeContainer == null) return;

            ClearRuneBadges();

            foreach (var runeType in _action.requiredRunes)
            {
                CreateRuneBadge(runeType);
            }
        }

        private void CreateRuneBadge(RuneType runeType)
        {
            if (_runeBadgePrefab == null || _runeContainer == null) return;

            var badge = Instantiate(_runeBadgePrefab, _runeContainer);
            badge.color = RuneDisplay.GetDefaultColor(runeType);
            
            var badgeText = badge.GetComponentInChildren<TextMeshProUGUI>();
            if (badgeText != null)
            {
                badgeText.text = RuneDisplay.GetSymbol(runeType);
                badgeText.color = Color.white;
            }

            badge.gameObject.SetActive(true);
            _runeBadges.Add(badge);
        }

        private void ClearRuneBadges()
        {
            foreach (var badge in _runeBadges)
            {
                if (badge != null)
                {
                    Destroy(badge.gameObject);
                }
            }
            _runeBadges.Clear();
        }

        private void OnDestroy()
        {
            ClearRuneBadges();
        }
    }
}
