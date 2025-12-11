using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class EnemyVisualController : MonoBehaviour
    {
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
            GameObject visualGO = new GameObject($"Enemy_{enemy.enemyName}");
            visualGO.transform.SetParent(transform);
            visualGO.transform.localPosition = position;

            var instance = visualGO.AddComponent<EnemyVisualInstance>();
            Color color = GetEnemyColor(enemy.enemyName);
            instance.Initialize(enemy, color, _bloodBurstPrefab);

            return instance;
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

