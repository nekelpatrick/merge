using UnityEngine;
using UnityEngine.SceneManagement;
using ShieldWall.Core;
using ShieldWall.Combat;
using ShieldWall.Dice;
using ShieldWall.ShieldWall;
using ShieldWall.UI;

namespace ShieldWall.Setup
{
    /// <summary>
    /// Runtime scene setup and initialization for Battle scene.
    /// Ensures all managers are properly initialized and connected.
    /// </summary>
    public class BattleSceneSetup : MonoBehaviour
    {
        [Header("Manager References")]
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private TurnManager _turnManager;
        [SerializeField] private DicePoolManager _dicePoolManager;
        [SerializeField] private ShieldWallManager _shieldWallManager;
        [SerializeField] private EnemyWaveController _waveController;
        
        [Header("UI References")]
        [SerializeField] private DiceUI _diceUI;
        [SerializeField] private ActionUI _actionUI;
        [SerializeField] private WallStatusUI _wallStatusUI;
        [SerializeField] private HealthUI _healthUI;
        [SerializeField] private StaminaUI _staminaUI;
        [SerializeField] private WaveUI _waveUI;
        
        [Header("Optional Phase 5.5 UI")]
        [SerializeField] private ActionPreviewUI _actionPreviewUI;
        [SerializeField] private PhaseBannerUI _phaseBannerUI;
        [SerializeField] private EnemyIntentManager _enemyIntentManager;
        
        private bool _isInitialized;
        
        private void Awake()
        {
            if (_isInitialized) return;
            
            ValidateReferences();
            InitializeManagers();
            WireUIReferences();
            
            _isInitialized = true;
            
            Debug.Log("[BattleSceneSetup] Battle scene initialized successfully.");
        }
        
        private void ValidateReferences()
        {
            if (_gameManager == null) Debug.LogError("[BattleSceneSetup] GameManager reference missing!");
            if (_turnManager == null) Debug.LogError("[BattleSceneSetup] TurnManager reference missing!");
            if (_dicePoolManager == null) Debug.LogError("[BattleSceneSetup] DicePoolManager reference missing!");
            if (_shieldWallManager == null) Debug.LogError("[BattleSceneSetup] ShieldWallManager reference missing!");
            if (_waveController == null) Debug.LogError("[BattleSceneSetup] EnemyWaveController reference missing!");
            
            if (_diceUI == null) Debug.LogWarning("[BattleSceneSetup] DiceUI reference missing!");
            if (_actionUI == null) Debug.LogWarning("[BattleSceneSetup] ActionUI reference missing!");
        }
        
        private void InitializeManagers()
        {
            if (_gameManager != null && _turnManager != null)
            {
                _gameManager.Initialize();
            }
        }
        
        private void WireUIReferences()
        {
            if (_diceUI != null && _dicePoolManager != null)
            {
                _diceUI.gameObject.SetActive(true);
            }
            
            if (_actionUI != null)
            {
                _actionUI.gameObject.SetActive(true);
            }
            
            if (_actionPreviewUI != null)
            {
                Debug.Log("[BattleSceneSetup] Phase 5.5: ActionPreviewUI detected and enabled.");
            }
            
            if (_phaseBannerUI != null)
            {
                Debug.Log("[BattleSceneSetup] Phase 5.5: PhaseBannerUI detected and enabled.");
            }
            
            if (_enemyIntentManager != null)
            {
                Debug.Log("[BattleSceneSetup] Phase 5.5: EnemyIntentManager detected and enabled.");
            }
        }
        
        [ContextMenu("Validate Scene Setup")]
        private void ValidateSetup()
        {
            ValidateReferences();
            
            Debug.Log("=== Battle Scene Setup Validation ===");
            Debug.Log($"Core Managers: {(_gameManager != null && _turnManager != null ? "OK" : "MISSING")}");
            Debug.Log($"Dice System: {(_dicePoolManager != null && _diceUI != null ? "OK" : "MISSING")}");
            Debug.Log($"Shield Wall: {(_shieldWallManager != null && _wallStatusUI != null ? "OK" : "MISSING")}");
            Debug.Log($"Combat System: {(_waveController != null ? "OK" : "MISSING")}");
            Debug.Log($"Phase 5.5 UX: {(_actionPreviewUI != null && _phaseBannerUI != null ? "ENABLED" : "DISABLED")}");
        }
        
        [ContextMenu("Find All Managers")]
        private void FindAllManagers()
        {
            _gameManager = FindFirstObjectByType<GameManager>();
            _turnManager = FindFirstObjectByType<TurnManager>();
            _dicePoolManager = FindFirstObjectByType<DicePoolManager>();
            _shieldWallManager = FindFirstObjectByType<ShieldWallManager>();
            _waveController = FindFirstObjectByType<EnemyWaveController>();
            
            _diceUI = FindFirstObjectByType<DiceUI>();
            _actionUI = FindFirstObjectByType<ActionUI>();
            _wallStatusUI = FindFirstObjectByType<WallStatusUI>();
            _healthUI = FindFirstObjectByType<HealthUI>();
            _staminaUI = FindFirstObjectByType<StaminaUI>();
            _waveUI = FindFirstObjectByType<WaveUI>();
            
            _actionPreviewUI = FindFirstObjectByType<ActionPreviewUI>();
            _phaseBannerUI = FindFirstObjectByType<PhaseBannerUI>();
            _enemyIntentManager = FindFirstObjectByType<EnemyIntentManager>();
            
            Debug.Log("[BattleSceneSetup] Auto-found all managers and UI components.");
        }
    }
}
