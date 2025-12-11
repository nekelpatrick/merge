using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;
using ShieldWall.ShieldWall;

namespace ShieldWall.UI
{
    public class WallStatusUI : MonoBehaviour
    {
        [System.Serializable]
        public class BrotherPanel
        {
            public WallPosition Position;
            public GameObject Panel;
            public TextMeshProUGUI NameText;
            public Transform HeartsContainer;
            public Image DeathOverlay;
        }

        [Header("References")]
        [SerializeField] private ShieldWallManager _shieldWallManager;
        [SerializeField] private BrotherPanel[] _brotherPanels;
        [SerializeField] private Image _heartPrefab;

        [Header("Colors")]
        [SerializeField] private Color _healthyColor = new Color(0.55f, 0.13f, 0.13f);
        [SerializeField] private Color _emptyColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color _deathOverlayColor = new Color(0f, 0f, 0f, 0.7f);

        private readonly Dictionary<WallPosition, List<Image>> _heartImages = new Dictionary<WallPosition, List<Image>>();

        private void Start()
        {
            InitializePanels();
        }

        private void OnEnable()
        {
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnBrotherDied += HandleBrotherDied;
        }

        private void OnDisable()
        {
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnBrotherDied -= HandleBrotherDied;
        }

        private void InitializePanels()
        {
            if (_shieldWallManager == null) return;

            foreach (var panel in _brotherPanels)
            {
                var brother = _shieldWallManager.GetBrotherAt(panel.Position);
                if (brother == null)
                {
                    panel.Panel?.SetActive(false);
                    continue;
                }

                panel.Panel?.SetActive(true);
                
                if (panel.NameText != null)
                {
                    panel.NameText.text = brother.Data.brotherName;
                }

                if (panel.DeathOverlay != null)
                {
                    panel.DeathOverlay.gameObject.SetActive(false);
                }

                InitializeHearts(panel, brother.Data.maxHealth);
                UpdateHearts(panel.Position, brother.CurrentHealth, brother.Data.maxHealth);
            }
        }

        private void InitializeHearts(BrotherPanel panel, int maxHealth)
        {
            if (panel.HeartsContainer == null || _heartPrefab == null) return;

            foreach (Transform child in panel.HeartsContainer)
            {
                Destroy(child.gameObject);
            }

            var hearts = new List<Image>();
            for (int i = 0; i < maxHealth; i++)
            {
                var heart = Instantiate(_heartPrefab, panel.HeartsContainer);
                hearts.Add(heart);
            }

            _heartImages[panel.Position] = hearts;
        }

        private void HandleBrotherWounded(ShieldBrotherSO brotherData, int damage)
        {
            if (_shieldWallManager == null) return;

            foreach (var panel in _brotherPanels)
            {
                var brother = _shieldWallManager.GetBrotherAt(panel.Position);
                if (brother?.Data == brotherData)
                {
                    UpdateHearts(panel.Position, brother.CurrentHealth, brother.Data.maxHealth);
                    break;
                }
            }
        }

        private void HandleBrotherDied(ShieldBrotherSO brotherData)
        {
            foreach (var panel in _brotherPanels)
            {
                var brother = _shieldWallManager?.GetBrotherAt(panel.Position);
                if (brother?.Data == brotherData)
                {
                    if (panel.DeathOverlay != null)
                    {
                        panel.DeathOverlay.gameObject.SetActive(true);
                        panel.DeathOverlay.color = _deathOverlayColor;
                    }
                    
                    UpdateHearts(panel.Position, 0, brother.Data.maxHealth);
                    break;
                }
            }
        }

        private void UpdateHearts(WallPosition position, int currentHealth, int maxHealth)
        {
            if (!_heartImages.TryGetValue(position, out var hearts)) return;

            for (int i = 0; i < hearts.Count; i++)
            {
                hearts[i].color = i < currentHealth ? _healthyColor : _emptyColor;
            }
        }
    }
}

