using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;

namespace ShieldWall.UI
{
    public class HealthUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _heartsContainer;
        [SerializeField] private Image _heartPrefab;

        [Header("Heart Sprites")]
        [SerializeField] private Sprite _heartFull;
        [SerializeField] private Sprite _heartEmpty;

        [Header("Colors")]
        [SerializeField] private Color _fullColor = new Color(0.55f, 0.13f, 0.13f);
        [SerializeField] private Color _emptyColor = new Color(0.3f, 0.3f, 0.3f);

        [Header("Settings")]
        [SerializeField] private int _maxHealth = 5;

        private readonly List<Image> _hearts = new List<Image>();
        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            InitializeHearts();
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
        }

        private void InitializeHearts()
        {
            foreach (Transform child in _heartsContainer)
            {
                Destroy(child.gameObject);
            }
            _hearts.Clear();

            for (int i = 0; i < _maxHealth; i++)
            {
                var heart = Instantiate(_heartPrefab, _heartsContainer);
                _hearts.Add(heart);
            }

            UpdateHeartVisuals();
        }

        private void HandlePlayerWounded(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth < 0) _currentHealth = 0;
            
            UpdateHeartVisuals();
        }

        public void SetHealth(int current, int max)
        {
            _maxHealth = max;
            _currentHealth = current;
            
            if (_hearts.Count != _maxHealth)
            {
                InitializeHearts();
            }
            else
            {
                UpdateHeartVisuals();
            }
        }

        private void UpdateHeartVisuals()
        {
            for (int i = 0; i < _hearts.Count; i++)
            {
                bool isFull = i < _currentHealth;
                
                if (_heartFull != null && _heartEmpty != null)
                {
                    _hearts[i].sprite = isFull ? _heartFull : _heartEmpty;
                }
                
                _hearts[i].color = isFull ? _fullColor : _emptyColor;
            }
        }
    }
}

