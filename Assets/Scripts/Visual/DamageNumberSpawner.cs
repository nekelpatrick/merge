using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject _damageNumberPrefab;

        [Header("Pool Settings")]
        [SerializeField] private int _poolSize = 10;

        [Header("Animation")]
        [SerializeField] private float _floatSpeed = 1f;
        [SerializeField] private float _floatDuration = 1f;
        [SerializeField] private float _fadeStartTime = 0.5f;

        [Header("Spawn Position")]
        [SerializeField] private Transform _spawnParent;
        [SerializeField] private Vector2 _randomOffset = new Vector2(50f, 30f);

        [Header("Colors")]
        [SerializeField] private Color _damageColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color _healColor = new Color(0.2f, 0.9f, 0.2f);
        [SerializeField] private Color _blockColor = new Color(0.5f, 0.5f, 0.5f);

        private readonly Queue<GameObject> _pool = new Queue<GameObject>();

        private void Start()
        {
            CreatePool();
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerDamage;
            GameEvents.OnBrotherWounded += HandleBrotherDamage;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerDamage;
            GameEvents.OnBrotherWounded -= HandleBrotherDamage;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
        }

        private void CreatePool()
        {
            if (_damageNumberPrefab == null)
            {
                CreateDefaultPrefab();
            }

            for (int i = 0; i < _poolSize; i++)
            {
                var obj = Instantiate(_damageNumberPrefab, _spawnParent != null ? _spawnParent : transform);
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        private void CreateDefaultPrefab()
        {
            _damageNumberPrefab = new GameObject("DamageNumber");
            var rect = _damageNumberPrefab.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 50);
            
            var text = _damageNumberPrefab.AddComponent<TextMeshProUGUI>();
            text.fontSize = 36;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = _damageColor;
            
            _damageNumberPrefab.SetActive(false);
        }

        private void HandlePlayerDamage(int damage)
        {
            SpawnNumber($"-{damage}", _damageColor, Vector2.zero);
        }

        private void HandleBrotherDamage(ShieldBrotherSO brother, int damage)
        {
            float xOffset = Random.Range(-200f, 200f);
            SpawnNumber($"-{damage}", _damageColor, new Vector2(xOffset, 100f));
        }

        private void HandleAttackBlocked(Attack attack)
        {
            SpawnNumber("BLOCKED", _blockColor, new Vector2(0, 50f));
        }

        public void SpawnHealNumber(int amount, Vector2 offset)
        {
            SpawnNumber($"+{amount}", _healColor, offset);
        }

        private void SpawnNumber(string text, Color color, Vector2 baseOffset)
        {
            if (_pool.Count == 0) return;

            var obj = _pool.Dequeue();
            obj.SetActive(true);

            var rect = obj.GetComponent<RectTransform>();
            if (rect != null && _spawnParent != null)
            {
                Vector2 offset = baseOffset + new Vector2(
                    Random.Range(-_randomOffset.x, _randomOffset.x),
                    Random.Range(-_randomOffset.y, _randomOffset.y)
                );
                rect.anchoredPosition = offset;
            }

            var tmp = obj.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = text;
                tmp.color = color;
            }

            StartCoroutine(AnimateNumber(obj, color));
        }

        private IEnumerator AnimateNumber(GameObject obj, Color startColor)
        {
            var rect = obj.GetComponent<RectTransform>();
            var tmp = obj.GetComponent<TextMeshProUGUI>();
            
            Vector2 startPos = rect != null ? rect.anchoredPosition : Vector2.zero;
            float elapsed = 0f;

            while (elapsed < _floatDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _floatDuration;

                if (rect != null)
                {
                    rect.anchoredPosition = startPos + Vector2.up * (_floatSpeed * 100f * t);
                }

                if (tmp != null && elapsed > _fadeStartTime)
                {
                    float fadeT = (elapsed - _fadeStartTime) / (_floatDuration - _fadeStartTime);
                    tmp.color = new Color(startColor.r, startColor.g, startColor.b, 1f - fadeT);
                }

                yield return null;
            }

            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}

