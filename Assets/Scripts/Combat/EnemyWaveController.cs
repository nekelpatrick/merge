using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Combat
{
    public class EnemyWaveController : MonoBehaviour
    {
        [SerializeField] private WaveConfigSO[] _waveConfigs;

        private readonly List<Enemy> _activeEnemies = new List<Enemy>();
        private int _currentWaveIndex = -1;
        private int _totalWaves;

        public IReadOnlyList<Enemy> ActiveEnemies => _activeEnemies;
        public int CurrentWaveNumber => _currentWaveIndex + 1;
        public int TotalWaves => _totalWaves;
        public bool AllEnemiesDead => _activeEnemies.TrueForAll(e => e.IsDead);
        public bool HasMoreWaves => _currentWaveIndex < _totalWaves - 1;

        private void Awake()
        {
            _totalWaves = _waveConfigs?.Length ?? 0;
        }

        public void StartBattle()
        {
            _currentWaveIndex = -1;
            _activeEnemies.Clear();
        }

        public bool SpawnNextWave()
        {
            if (!HasMoreWaves) return false;
            
            _currentWaveIndex++;
            SpawnWave(_currentWaveIndex);
            return true;
        }

        public void SpawnWave(int waveIndex)
        {
            _activeEnemies.Clear();

            if (_waveConfigs == null || waveIndex < 0 || waveIndex >= _waveConfigs.Length)
            {
                Debug.LogWarning($"EnemyWaveController: Invalid wave index {waveIndex}");
                return;
            }

            var config = _waveConfigs[waveIndex];
            _currentWaveIndex = waveIndex;

            foreach (var spawn in config.enemies)
            {
                if (spawn.enemy == null) continue;
                
                for (int i = 0; i < spawn.count; i++)
                {
                    _activeEnemies.Add(new Enemy(spawn.enemy));
                }
            }

            var enemyDataList = new List<EnemySO>();
            foreach (var enemy in _activeEnemies)
            {
                enemyDataList.Add(enemy.Data);
            }

            GameEvents.RaiseWaveStarted(CurrentWaveNumber);
            GameEvents.RaiseEnemyWaveSpawned(enemyDataList);

            Debug.Log($"EnemyWaveController: Spawned wave {CurrentWaveNumber} with {_activeEnemies.Count} enemies");
        }

        public void RemoveEnemy(Enemy enemy)
        {
            if (_activeEnemies.Remove(enemy))
            {
                GameEvents.RaiseEnemyKilled(enemy.Data);
            }

            if (AllEnemiesDead)
            {
                GameEvents.RaiseWaveCleared();
            }
        }

        public void KillEnemy(int index)
        {
            if (index < 0 || index >= _activeEnemies.Count) return;
            
            var enemy = _activeEnemies[index];
            enemy.TakeDamage(enemy.CurrentHealth);
            RemoveEnemy(enemy);
        }

        public List<Enemy> GetLiveEnemies()
        {
            return _activeEnemies.FindAll(e => !e.IsDead);
        }

        public int GetLiveEnemyCount()
        {
            int count = 0;
            foreach (var enemy in _activeEnemies)
            {
                if (!enemy.IsDead) count++;
            }
            return count;
        }
    }
}

