using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;
using ShieldWall.Formation;

namespace ShieldWall.UI
{
    public class EnemyIntentIndicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _backgroundImage;

        [Header("Icons")]
        [SerializeField] private Sprite _attackPlayerIcon;
        [SerializeField] private Sprite _attackBrotherIcon;
        [SerializeField] private Sprite _unblockableIcon;

        [Header("Colors")]
        [SerializeField] private Color _playerTargetColor = new Color(139f/255f, 32f/255f, 32f/255f);
        [SerializeField] private Color _brotherTargetColor = new Color(201f/255f, 162f/255f, 39f/255f);
        [SerializeField] private Color _unblockableColor = new Color(196f/255f, 92f/255f, 38f/255f);

        private WallPosition _targetPosition;
        private bool _isUnblockable;

        private void Awake()
        {
            Hide();
        }

        public void ShowIntent(WallPosition target, bool unblockable)
        {
            _targetPosition = target;
            _isUnblockable = unblockable;

            bool targetsPlayer = target == WallPosition.Center;

            if (_iconImage != null)
            {
                if (unblockable && _unblockableIcon != null)
                {
                    _iconImage.sprite = _unblockableIcon;
                    _iconImage.color = _unblockableColor;
                }
                else if (targetsPlayer && _attackPlayerIcon != null)
                {
                    _iconImage.sprite = _attackPlayerIcon;
                    _iconImage.color = _playerTargetColor;
                }
                else if (!targetsPlayer && _attackBrotherIcon != null)
                {
                    _iconImage.sprite = _attackBrotherIcon;
                    _iconImage.color = _brotherTargetColor;
                }

                _iconImage.gameObject.SetActive(true);
            }

            if (_backgroundImage != null)
            {
                _backgroundImage.color = unblockable ? _unblockableColor : 
                                         targetsPlayer ? _playerTargetColor : _brotherTargetColor;
                _backgroundImage.gameObject.SetActive(true);
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
