using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Dice;

namespace ShieldWall.UI
{
    /// <summary>
    /// Simple UI component for displaying a rune badge (colored background + rune symbol).
    /// Used by ActionPreviewItem to show rune requirements.
    /// </summary>
    public class RuneBadgeUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _symbolText;
        
        [Header("Settings")]
        [SerializeField] private bool _useFullName = false;
        [SerializeField] private Color _textColor = Color.white;
        
        public void SetRune(RuneType runeType)
        {
            if (_backgroundImage != null)
            {
                _backgroundImage.color = RuneDisplay.GetDefaultColor(runeType);
            }
            
            if (_symbolText != null)
            {
                _symbolText.text = _useFullName ? RuneDisplay.GetFullName(runeType) : RuneDisplay.GetSymbol(runeType);
                _symbolText.color = _textColor;
            }
        }
        
        public void SetRune(RuneType runeType, Color backgroundColor)
        {
            if (_backgroundImage != null)
            {
                _backgroundImage.color = backgroundColor;
            }
            
            if (_symbolText != null)
            {
                _symbolText.text = _useFullName ? RuneDisplay.GetFullName(runeType) : RuneDisplay.GetSymbol(runeType);
                _symbolText.color = _textColor;
            }
        }
    }
}

