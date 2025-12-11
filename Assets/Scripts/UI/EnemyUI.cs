using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class EnemyUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _enemyContainer;
        [SerializeField] private GameObject _enemyIconPrefab;

        [Header("Colors")]
        [SerializeField] private Color _thrallColor = new Color(0.4f, 0.4f, 0.4f);
        [SerializeField] private Color _warriorColor = new Color(0.6f, 0.2f, 0.2f);
        [SerializeField] private Color _spearmanColor = new Color(0.5f, 0.4f, 0.2f);

        private readonly List<GameObject> _enemyIcons = new List<GameObject>();
        private readonly Dictionary<string, int> _enemyCounts = new Dictionary<string, int>();

        private void OnEnable()
        {
            GameEvents.OnEnemyWaveSpawned += HandleWaveSpawned;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyWaveSpawned -= HandleWaveSpawned;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        }

        private void HandleWaveSpawned(List<EnemySO> enemies)
        {
            ClearEnemyIcons();
            
            _enemyCounts.Clear();
            foreach (var enemy in enemies)
            {
                if (!_enemyCounts.ContainsKey(enemy.enemyName))
                {
                    _enemyCounts[enemy.enemyName] = 0;
                }
                _enemyCounts[enemy.enemyName]++;
            }

            foreach (var kvp in _enemyCounts)
            {
                CreateEnemyIcon(kvp.Key, kvp.Value);
            }
            
            Debug.Log($"EnemyUI: Displaying {enemies.Count} enemies");
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            if (_enemyCounts.ContainsKey(enemy.enemyName))
            {
                _enemyCounts[enemy.enemyName]--;
                
                if (_enemyCounts[enemy.enemyName] <= 0)
                {
                    _enemyCounts.Remove(enemy.enemyName);
                }
                
                RefreshDisplay();
            }
        }

        private void RefreshDisplay()
        {
            ClearEnemyIcons();
            
            foreach (var kvp in _enemyCounts)
            {
                if (kvp.Value > 0)
                {
                    CreateEnemyIcon(kvp.Key, kvp.Value);
                }
            }
        }

        private void CreateEnemyIcon(string enemyName, int count)
        {
            if (_enemyContainer == null) return;

            GameObject icon;
            
            if (_enemyIconPrefab != null)
            {
                icon = Instantiate(_enemyIconPrefab, _enemyContainer);
            }
            else
            {
                icon = CreateDefaultIcon();
                icon.transform.SetParent(_enemyContainer, false);
            }

            var texts = icon.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text.name.Contains("Name") || text.name.Contains("Label"))
                {
                    text.text = enemyName;
                }
                else if (text.name.Contains("Count"))
                {
                    text.text = $"x{count}";
                }
                else
                {
                    text.text = $"{enemyName} x{count}";
                }
            }

            var images = icon.GetComponentsInChildren<Image>();
            foreach (var img in images)
            {
                if (img.gameObject != icon)
                {
                    img.color = GetEnemyColor(enemyName);
                }
            }

            _enemyIcons.Add(icon);
        }

        private GameObject CreateDefaultIcon()
        {
            var icon = new GameObject("EnemyIcon");
            
            var rectTransform = icon.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(120f, 40f);

            var layout = icon.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 5f;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var bg = icon.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

            var iconChild = new GameObject("Icon");
            iconChild.transform.SetParent(icon.transform, false);
            var iconRect = iconChild.AddComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(30f, 30f);
            var iconImage = iconChild.AddComponent<Image>();
            iconImage.color = Color.red;

            var textChild = new GameObject("Text");
            textChild.transform.SetParent(icon.transform, false);
            var textRect = textChild.AddComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(80f, 30f);
            var text = textChild.AddComponent<TextMeshProUGUI>();
            text.fontSize = 14f;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.color = Color.white;

            return icon;
        }

        private Color GetEnemyColor(string enemyName)
        {
            return enemyName.ToLower() switch
            {
                "thrall" => _thrallColor,
                "warrior" => _warriorColor,
                "spearman" => _spearmanColor,
                _ => Color.gray
            };
        }

        private void ClearEnemyIcons()
        {
            foreach (var icon in _enemyIcons)
            {
                if (icon != null)
                {
                    Destroy(icon);
                }
            }
            _enemyIcons.Clear();
        }
    }
}

