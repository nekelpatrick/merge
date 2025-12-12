using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.UI;

namespace ShieldWall.Visual
{
    public class EnemyVisualController : MonoBehaviour
    {
        [Header("Enemy Prefabs")]
        [SerializeField] private GameObject _enemyThrallPrefab;
        [SerializeField] private GameObject _enemyWarriorPrefab;
        [SerializeField] private GameObject _enemyBerserkerPrefab;
        [SerializeField] private GameObject _enemyArcherPrefab;
        [SerializeField] private GameObject _enemySpearmanPrefab;
        [SerializeField] private GameObject _enemyShieldBreakerPrefab;
        [SerializeField] private GameObject _defaultEnemyPrefab;
        
        [Header("Spawn Settings")]
        [SerializeField] private float _spawnDistanceZ = 7f;
        [SerializeField] private float _spreadX = 2f;
        [SerializeField] private float _spawnHeight = 0f;

        [Header("Enemy Colors")]
        [SerializeField] private Color _thrallColor = new Color(0.42f, 0.27f, 0.14f);
        [SerializeField] private Color _warriorColor = new Color(0.35f, 0.35f, 0.35f);
        [SerializeField] private Color _berserkerColor = new Color(0.55f, 0.13f, 0.13f);
        [SerializeField] private Color _archerColor = new Color(0.18f, 0.35f, 0.15f);
        [SerializeField] private Color _defaultColor = Color.gray;

        [Header("VFX")]
        [SerializeField] private BloodBurstVFX _bloodBurstPrefab;

        private readonly Dictionary<EnemySO, EnemyVisualInstance> _activeEnemies = new Dictionary<EnemySO, EnemyVisualInstance>();

        private void OnEnable()
        {
            GameEvents.OnEnemyWaveSpawned += SpawnEnemyVisuals;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyWaveSpawned -= SpawnEnemyVisuals;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        }

        private void SpawnEnemyVisuals(List<EnemySO> enemies)
        {
            ClearExistingEnemies();

            int count = enemies.Count;
            for (int i = 0; i < count; i++)
            {
                float xOffset = CalculateXOffset(i, count);
                Vector3 spawnPos = new Vector3(xOffset, _spawnHeight, _spawnDistanceZ);
                
                var instance = CreateEnemyVisual(enemies[i], spawnPos);
                _activeEnemies[enemies[i]] = instance;
            }
        }

        private float CalculateXOffset(int index, int total)
        {
            if (total == 1) return 0f;
            
            float normalizedPos = (float)index / (total - 1);
            return Mathf.Lerp(-_spreadX, _spreadX, normalizedPos);
        }

        private EnemyVisualInstance CreateEnemyVisual(EnemySO enemy, Vector3 position)
        {
            GameObject prefabToUse = GetEnemyPrefab(enemy.enemyName);
            
            GameObject visualGO;
            
            if (prefabToUse != null)
            {
                visualGO = Instantiate(prefabToUse, transform);
                visualGO.name = $"Enemy_{enemy.enemyName}";
                visualGO.transform.localPosition = position;
            }
            else
            {
                visualGO = new GameObject($"Enemy_{enemy.enemyName}");
                visualGO.transform.SetParent(transform);
                visualGO.transform.localPosition = position;
                visualGO.AddComponent<EnemyVisualInstance>();
            }

            var instance = visualGO.GetComponent<EnemyVisualInstance>();
            if (instance == null)
            {
                instance = visualGO.AddComponent<EnemyVisualInstance>();
            }
            
            Color color = GetEnemyColor(enemy.enemyName);
            instance.Initialize(enemy, color, _bloodBurstPrefab);

            var existingHealthDisplay = visualGO.GetComponentInChildren<EnemyHealthDisplay>();
            if (existingHealthDisplay == null)
            {
                CreateHealthDisplayFor(visualGO);
            }
            else
            {
                existingHealthDisplay.SetEnemyVisual(instance);
            }

            return instance;
        }
        
        private GameObject GetEnemyPrefab(string enemyName)
        {
            string lowerName = enemyName.ToLower();
            
            if (lowerName.Contains("thrall")) return _enemyThrallPrefab;
            if (lowerName.Contains("warrior")) return _enemyWarriorPrefab;
            if (lowerName.Contains("berserker")) return _enemyBerserkerPrefab;
            if (lowerName.Contains("archer")) return _enemyArcherPrefab;
            if (lowerName.Contains("spearman")) return _enemySpearmanPrefab;
            if (lowerName.Contains("shieldbreaker") || lowerName.Contains("shield breaker"))
                return _enemyShieldBreakerPrefab;
            
            return _defaultEnemyPrefab;
        }
        
        private void CreateHealthDisplayFor(GameObject enemyGO)
        {
            GameObject healthDisplayGO = new GameObject("HealthDisplay");
            healthDisplayGO.transform.SetParent(enemyGO.transform, false);
            healthDisplayGO.transform.localPosition = new Vector3(0, 1.5f, 0);
            
            var canvas = healthDisplayGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            
            var canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(200, 50);
            canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            
            GameObject textGO = new GameObject("HealthText");
            textGO.transform.SetParent(healthDisplayGO.transform, false);
            
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            var text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = "5/5 HP";
            text.fontSize = 36;
            text.fontStyle = TMPro.FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            
            var healthDisplay = healthDisplayGO.AddComponent<EnemyHealthDisplay>();
        }

        private Color GetEnemyColor(string enemyName)
        {
            string lowerName = enemyName.ToLower();
            
            if (lowerName.Contains("thrall")) return _thrallColor;
            if (lowerName.Contains("warrior")) return _warriorColor;
            if (lowerName.Contains("berserker")) return _berserkerColor;
            if (lowerName.Contains("archer")) return _archerColor;
            
            return _defaultColor;
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            if (_activeEnemies.TryGetValue(enemy, out var instance))
            {
                DismembermentType dismembermentType = DetermineDismembermentType();
                instance.PlayDeathAnimationWithDismemberment(dismembermentType);
                _activeEnemies.Remove(enemy);
            }
        }
        
        private DismembermentType DetermineDismembermentType()
        {
            int random = Random.Range(0, 100);
            
            if (random < 40)
            {
                return DismembermentType.Decapitation;
            }
            else if (random < 70)
            {
                return DismembermentType.ArmSword;
            }
            else if (random < 85)
            {
                return DismembermentType.ArmShield;
            }
            else
            {
                return DismembermentType.Random;
            }
        }

        private void ClearExistingEnemies()
        {
            foreach (var kvp in _activeEnemies)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value.gameObject);
                }
            }
            _activeEnemies.Clear();
        }
    }
}

