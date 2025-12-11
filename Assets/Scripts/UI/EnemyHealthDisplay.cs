using UnityEngine;
using TMPro;
using ShieldWall.Combat;

namespace ShieldWall.UI
{
    /// <summary>
    /// Displays enemy health above their head in world space.
    /// Auto-updates when enemy takes damage.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Enemy _enemy;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Canvas _canvas;
        
        [Header("Display Settings")]
        [SerializeField] private Vector3 _offset = new Vector3(0, 1.5f, 0);
        [SerializeField] private float _updateInterval = 0.1f;
        [SerializeField] private Color _healthyColor = Color.white;
        [SerializeField] private Color _woundedColor = Color.yellow;
        [SerializeField] private Color _criticalColor = Color.red;
        [SerializeField] private float _criticalThreshold = 0.3f;
        [SerializeField] private float _woundedThreshold = 0.6f;
        
        private Camera _mainCamera;
        private float _lastUpdateTime;
        
        private void Awake()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            
            if (_canvas != null)
            {
                _canvas.renderMode = RenderMode.WorldSpace;
                _canvas.worldCamera = Camera.main;
                
                RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
                if (canvasRect != null)
                {
                    canvasRect.sizeDelta = new Vector2(2f, 0.5f);
                    canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }
            }
            
            if (_healthText == null)
            {
                _healthText = GetComponentInChildren<TextMeshProUGUI>();
            }
            
            if (_healthText != null)
            {
                _healthText.alignment = TextAlignmentOptions.Center;
                _healthText.fontSize = 36;
                _healthText.fontStyle = TMPro.FontStyles.Bold;
            }
            
            _mainCamera = Camera.main;
        }
        
        private void Start()
        {
            if (_enemy == null)
            {
                _enemy = GetComponentInParent<Enemy>();
            }
            
            UpdateHealthDisplay();
        }
        
        private void LateUpdate()
        {
            if (_mainCamera != null && _canvas != null)
            {
                _canvas.transform.LookAt(_canvas.transform.position + _mainCamera.transform.rotation * Vector3.forward,
                    _mainCamera.transform.rotation * Vector3.up);
            }
            
            if (Time.time - _lastUpdateTime > _updateInterval)
            {
                UpdateHealthDisplay();
                _lastUpdateTime = Time.time;
            }
        }
        
        private void UpdateHealthDisplay()
        {
            if (_enemy == null || _healthText == null) return;
            
            int currentHealth = _enemy.CurrentHealth;
            int maxHealth = _enemy.MaxHealth;
            
            _healthText.text = $"{currentHealth}/{maxHealth} HP";
            
            float healthPercent = (float)currentHealth / maxHealth;
            
            if (healthPercent <= _criticalThreshold)
            {
                _healthText.color = _criticalColor;
            }
            else if (healthPercent <= _woundedThreshold)
            {
                _healthText.color = _woundedColor;
            }
            else
            {
                _healthText.color = _healthyColor;
            }
            
            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        
        public void SetEnemy(Enemy enemy)
        {
            _enemy = enemy;
            UpdateHealthDisplay();
        }
        
        [ContextMenu("Force Update")]
        private void ForceUpdate()
        {
            UpdateHealthDisplay();
        }
    }
}
