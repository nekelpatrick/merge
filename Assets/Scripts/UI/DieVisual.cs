using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using ShieldWall.Dice;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class DieVisual : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _runeIcon;
        [SerializeField] private Image _lockOverlay;
        [SerializeField] private TextMeshProUGUI _runeText;

        [Header("Lock Visual")]
        [SerializeField] private Color _unlockedColor = Color.white;
        [SerializeField] private Color _lockedColor = new Color(1f, 0.9f, 0.5f);

        [Header("Events")]
        public UnityEvent<int> OnDieClicked;

        private int _dieIndex;
        private bool _isLocked;

        public int DieIndex => _dieIndex;
        public bool IsLocked => _isLocked;

        private void Awake()
        {
            if (_button == null) _button = GetComponent<Button>();
            if (_button != null) _button.onClick.AddListener(HandleClick);
            
            if (_lockOverlay != null) _lockOverlay.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(HandleClick);
        }

        public void Initialize(int index)
        {
            _dieIndex = index;
            _isLocked = false;
            UpdateLockVisual();
        }

        public void SetRune(RuneType runeType, RuneSO runeData = null)
        {
            if (runeData != null)
            {
                if (_runeIcon != null && runeData.icon != null)
                {
                    _runeIcon.sprite = runeData.icon;
                    _runeIcon.color = Color.white;
                }
                else if (_backgroundImage != null)
                {
                    _backgroundImage.color = runeData.color;
                }

                if (_runeText != null)
                {
                    _runeText.text = GetRuneSymbol(runeType);
                }
            }
            else
            {
                if (_runeText != null)
                {
                    _runeText.text = GetRuneSymbol(runeType);
                }
                
                if (_backgroundImage != null)
                {
                    _backgroundImage.color = GetDefaultColor(runeType);
                }
            }
        }

        public void SetLocked(bool locked)
        {
            _isLocked = locked;
            UpdateLockVisual();
        }

        private void UpdateLockVisual()
        {
            if (_lockOverlay != null)
            {
                _lockOverlay.gameObject.SetActive(_isLocked);
            }

            if (_backgroundImage != null)
            {
                var currentColor = _backgroundImage.color;
                if (_isLocked)
                {
                    _backgroundImage.color = new Color(currentColor.r * 1.2f, currentColor.g * 1.1f, currentColor.b * 0.8f, 1f);
                }
            }
        }

        private void HandleClick()
        {
            Debug.Log($"DieVisual: Die {_dieIndex} clicked!");
            OnDieClicked?.Invoke(_dieIndex);
        }

        private string GetRuneSymbol(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "SH",
                RuneType.Tyr => "AX",
                RuneType.Gebo => "SP",
                RuneType.Berkana => "BR",
                RuneType.Othala => "OD",
                RuneType.Laguz => "LO",
                _ => "?"
            };
        }

        private Color GetDefaultColor(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => new Color(0.36f, 0.36f, 0.36f),
                RuneType.Tyr => new Color(0.55f, 0.13f, 0.13f),
                RuneType.Gebo => new Color(0.55f, 0.41f, 0.08f),
                RuneType.Berkana => new Color(0.24f, 0.36f, 0.24f),
                RuneType.Othala => new Color(0.79f, 0.64f, 0.15f),
                RuneType.Laguz => new Color(0.36f, 0.24f, 0.43f),
                _ => Color.white
            };
        }
    }
}

