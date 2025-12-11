using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class ImpactVFXController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private BloodBurstVFX _bloodBurstPrefab;
        [SerializeField] private ShieldBlockVFX _shieldBlockPrefab;
        
        [Header("Pool Settings")]
        [SerializeField] private int _initialPoolSize = 10;

        [Header("Spawn Settings")]
        [SerializeField] private Vector3 _playerPosition = new Vector3(0, 1.2f, 0.5f);
        [SerializeField] private Vector3 _brotherPositionOffset = new Vector3(0, 1f, 0);

        private Queue<BloodBurstVFX> _bloodPool = new Queue<BloodBurstVFX>();
        private Queue<ShieldBlockVFX> _blockPool = new Queue<ShieldBlockVFX>();

        private void Awake()
        {
            InitializePools();
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
        }

        private void InitializePools()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                if (_bloodBurstPrefab != null)
                {
                    var blood = Instantiate(_bloodBurstPrefab, transform);
                    blood.gameObject.SetActive(false);
                    _bloodPool.Enqueue(blood);
                }

                if (_shieldBlockPrefab != null)
                {
                    var block = Instantiate(_shieldBlockPrefab, transform);
                    block.gameObject.SetActive(false);
                    _blockPool.Enqueue(block);
                }
            }
        }

        private void HandlePlayerWounded(int damage)
        {
            SpawnBlood(_playerPosition, Vector3.back, damage);
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            SpawnBlood(_brotherPositionOffset, Vector3.back, damage);
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            Vector3 enemyPos = new Vector3(Random.Range(-2f, 2f), 1f, 5f);
            SpawnBlood(enemyPos, Vector3.back, 2);
        }

        private void HandleAttackBlocked(Attack attack)
        {
            SpawnBlockEffect(_playerPosition);
        }

        public void SpawnBlood(Vector3 position, Vector3 direction, int intensity = 1)
        {
            var blood = GetBloodFromPool();
            if (blood != null)
            {
                blood.transform.position = position;
                blood.transform.forward = direction;
                blood.gameObject.SetActive(true);
                blood.Play(intensity);
                StartCoroutine(ReturnToPoolAfterDelay(blood, 2f));
            }
        }

        public void SpawnBlockEffect(Vector3 position)
        {
            var block = GetBlockFromPool();
            if (block != null)
            {
                block.transform.position = position;
                block.gameObject.SetActive(true);
                block.Play();
                StartCoroutine(ReturnBlockToPoolAfterDelay(block, 1f));
            }
        }

        private BloodBurstVFX GetBloodFromPool()
        {
            if (_bloodPool.Count > 0)
                return _bloodPool.Dequeue();

            if (_bloodBurstPrefab != null)
                return Instantiate(_bloodBurstPrefab, transform);

            return null;
        }

        private ShieldBlockVFX GetBlockFromPool()
        {
            if (_blockPool.Count > 0)
                return _blockPool.Dequeue();

            if (_shieldBlockPrefab != null)
                return Instantiate(_shieldBlockPrefab, transform);

            return null;
        }

        private System.Collections.IEnumerator ReturnToPoolAfterDelay(BloodBurstVFX blood, float delay)
        {
            yield return new WaitForSeconds(delay);
            blood.gameObject.SetActive(false);
            _bloodPool.Enqueue(blood);
        }

        private System.Collections.IEnumerator ReturnBlockToPoolAfterDelay(ShieldBlockVFX block, float delay)
        {
            yield return new WaitForSeconds(delay);
            block.gameObject.SetActive(false);
            _blockPool.Enqueue(block);
        }
    }
}

