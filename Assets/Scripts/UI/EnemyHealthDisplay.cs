using UnityEngine;
using TMPro;
using ShieldWall.Visual;
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
        [SerializeField] private EnemyVisualInstance _enemyVisual;
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
            if (_enemyVisual == null)
            {
                _enemyVisual = GetComponentInParent<EnemyVisualInstance>();
            }
            
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\PatrickLocal\merge\.cursor\debug.log", $"{{\"location\":\"EnemyHealthDisplay.cs:Start\",\"message\":\"EnemyHealthDisplay started\",\"data\":{{\"hasEnemyVisual\":{(_enemyVisual!=null).ToString().ToLower()},\"enemyName\":\"{_enemyVisual?.EnemyData?.enemyName}\"}},\"timestamp\":{System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},\"sessionId\":\"debug-session\",\"hypothesisId\":\"A\"}}\n"); } catch {}
            // #endregion
            
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
            if (_enemyVisual == null || _enemyVisual.RuntimeEnemy == null || _healthText == null)
            {
                // #region agent log
                try { System.IO.File.AppendAllText(@"c:\Users\PatrickLocal\merge\.cursor\debug.log", $"{{\"location\":\"EnemyHealthDisplay.cs:UpdateHealthDisplay\",\"message\":\"Missing references\",\"data\":{{\"hasEnemyVisual\":{(_enemyVisual!=null).ToString().ToLower()},\"hasRuntimeEnemy\":{(_enemyVisual?.RuntimeEnemy!=null).ToString().ToLower()},\"hasHealthText\":{(_healthText!=null).ToString().ToLower()}}},\"timestamp\":{System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},\"sessionId\":\"debug-session\",\"hypothesisId\":\"B\"}}\n"); } catch {}
                // #endregion
                return;
            }
            
            int currentHealth = _enemyVisual.RuntimeEnemy.CurrentHealth;
            int maxHealth = _enemyVisual.RuntimeEnemy.Data.health;
            
            _healthText.text = $"{currentHealth}/{maxHealth} HP";
            
            // #region agent log
            try { System.IO.File.AppendAllText(@"c:\Users\PatrickLocal\merge\.cursor\debug.log", $"{{\"location\":\"EnemyHealthDisplay.cs:UpdateHealthDisplay\",\"message\":\"Health updated\",\"data\":{{\"currentHealth\":{currentHealth},\"maxHealth\":{maxHealth},\"displayText\":\"{_healthText.text}\"}},\"timestamp\":{System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},\"sessionId\":\"debug-session\",\"hypothesisId\":\"C\"}}\n"); } catch {}
            // #endregion
            
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
        
        public void SetEnemyVisual(EnemyVisualInstance enemyVisual)
        {
            _enemyVisual = enemyVisual;
            UpdateHealthDisplay();
        }
        
        [ContextMenu("Force Update")]
        private void ForceUpdate()
        {
            UpdateHealthDisplay();
        }
    }
}
