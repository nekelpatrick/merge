using System.Reflection;
using UnityEngine;
using ShieldWall.Combat;
using ShieldWall.Data;
using ShieldWall.Dice;
using ShieldWall.Formation;
using ShieldWall.ShieldWall;
using ShieldWall.UI;

namespace ShieldWall.Core
{
    [DefaultExecutionOrder(-100)]
    public class BattleBootstrapper : MonoBehaviour
    {
        [Header("Auto-Wire Settings")]
        [SerializeField] private bool _autoWireOnAwake = true;
        [SerializeField] private bool _logWiring = true;

        private void Awake()
        {
            if (_autoWireOnAwake)
            {
                WireAllReferences();
            }
        }

        [ContextMenu("Wire All References")]
        public void WireAllReferences()
        {
            Log("Starting auto-wire...");

            var dicePoolManager = FindFirstObjectByType<DicePoolManager>();
            var comboManager = FindFirstObjectByType<ComboManager>();
            var waveController = FindFirstObjectByType<EnemyWaveController>();
            var combatResolver = FindFirstObjectByType<CombatResolver>();
            var shieldWallManager = FindFirstObjectByType<ShieldWallManager>();
            var staminaManager = FindFirstObjectByType<StaminaManager>();
            var playerWarrior = FindFirstObjectByType<PlayerWarrior>();
            var turnManager = FindFirstObjectByType<TurnManager>();
            var battleManager = FindFirstObjectByType<BattleManager>();
            var actionSelectionManager = FindFirstObjectByType<ActionSelectionManager>();

            var diceUI = FindFirstObjectByType<DiceUI>();
            var actionUI = FindFirstObjectByType<ActionUI>();
            var healthUI = FindFirstObjectByType<HealthUI>();
            var staminaUI = FindFirstObjectByType<StaminaUI>();
            var wallStatusUI = FindFirstObjectByType<WallStatusUI>();
            var waveUI = FindFirstObjectByType<WaveUI>();
            var gameOverUI = FindFirstObjectByType<GameOverUI>();

            WireTurnManager(turnManager, dicePoolManager, comboManager, waveController, 
                combatResolver, shieldWallManager, staminaManager, playerWarrior);
            
            WireBattleManager(battleManager, turnManager, staminaManager);
            
            WireComboManager(comboManager, dicePoolManager);
            
            WireDiceUI(diceUI, dicePoolManager);
            
            WireActionUI(actionUI);
            
            WireWaveController(waveController);
            
            WireCombatResolver(combatResolver, waveController, shieldWallManager, playerWarrior);
            
            WireShieldWallManager(shieldWallManager);
            
            WireWallStatusUI(wallStatusUI, shieldWallManager);
            
            WireWaveUI(waveUI, waveController);
            
            WireGameOverUI(gameOverUI, battleManager);

            Log("Auto-wire complete!");
        }

        private void WireTurnManager(TurnManager tm, DicePoolManager dpm, ComboManager cm, 
            EnemyWaveController ewc, CombatResolver cr, ShieldWallManager swm, 
            StaminaManager sm, PlayerWarrior pw)
        {
            if (tm == null) { Log("TurnManager not found!"); return; }

            SetPrivateField(tm, "_dicePoolManager", dpm);
            SetPrivateField(tm, "_comboManager", cm);
            SetPrivateField(tm, "_waveController", ewc);
            SetPrivateField(tm, "_combatResolver", cr);
            SetPrivateField(tm, "_shieldWallManager", swm);
            SetPrivateField(tm, "_staminaManager", sm);
            SetPrivateField(tm, "_player", pw);
            
            Log("TurnManager wired");
        }

        private void WireBattleManager(BattleManager bm, TurnManager tm, StaminaManager sm)
        {
            if (bm == null) { Log("BattleManager not found!"); return; }

            SetPrivateField(bm, "_turnManager", tm);
            SetPrivateField(bm, "_staminaManager", sm);
            
            Log("BattleManager wired");
        }

        private void WireComboManager(ComboManager cm, DicePoolManager dpm)
        {
            if (cm == null) { Log("ComboManager not found!"); return; }

            SetPrivateField(cm, "_dicePoolManager", dpm);

            var actions = Resources.LoadAll<ActionSO>("Actions");
            if (actions == null || actions.Length == 0)
            {
                actions = LoadAssetsFromPath<ActionSO>("Assets/ScriptableObjects/Actions");
            }
            
            if (actions != null && actions.Length > 0)
            {
                SetPrivateField(cm, "_allActions", actions);
                Log($"ComboManager wired with {actions.Length} actions");
            }
            else
            {
                Log("ComboManager: No actions found!");
            }
        }

        private void WireDiceUI(DiceUI dui, DicePoolManager dpm)
        {
            if (dui == null) { Log("DiceUI not found!"); return; }

            SetPrivateField(dui, "_dicePoolManager", dpm);

            var runes = Resources.LoadAll<RuneSO>("Runes");
            if (runes == null || runes.Length == 0)
            {
                runes = LoadAssetsFromPath<RuneSO>("Assets/ScriptableObjects/Runes");
            }
            
            if (runes != null && runes.Length > 0)
            {
                SetPrivateField(dui, "_runeData", runes);
                Log($"DiceUI wired with {runes.Length} runes");
            }

            var diePrefab = Resources.Load<DieVisual>("DieVisual");
            if (diePrefab == null)
            {
                diePrefab = LoadAssetFromPath<DieVisual>("Assets/Prefabs/UI/DieVisual.prefab");
            }
            
            if (diePrefab != null)
            {
                SetPrivateField(dui, "_dieVisualPrefab", diePrefab);
                Log("DiceUI: DieVisual prefab assigned");
            }
        }

        private void WireActionUI(ActionUI aui)
        {
            if (aui == null) { Log("ActionUI not found!"); return; }

            var actionPrefab = Resources.Load<ActionButton>("ActionButton");
            if (actionPrefab == null)
            {
                actionPrefab = LoadAssetFromPath<ActionButton>("Assets/Prefabs/UI/ActionButton.prefab");
            }
            
            if (actionPrefab != null)
            {
                SetPrivateField(aui, "_actionButtonPrefab", actionPrefab);
                Log("ActionUI: ActionButton prefab assigned");
            }
        }

        private void WireWaveController(EnemyWaveController ewc)
        {
            if (ewc == null) { Log("EnemyWaveController not found!"); return; }

            var waves = Resources.LoadAll<WaveConfigSO>("Waves");
            if (waves == null || waves.Length == 0)
            {
                waves = LoadAssetsFromPath<WaveConfigSO>("Assets/ScriptableObjects/Waves");
            }
            
            if (waves != null && waves.Length > 0)
            {
                System.Array.Sort(waves, (a, b) => a.waveNumber.CompareTo(b.waveNumber));
                SetPrivateField(ewc, "_waveConfigs", waves);
                Log($"EnemyWaveController wired with {waves.Length} waves");
            }
        }

        private void WireCombatResolver(CombatResolver cr, EnemyWaveController ewc, 
            ShieldWallManager swm, PlayerWarrior pw)
        {
            if (cr == null) { Log("CombatResolver not found!"); return; }

            SetPrivateField(cr, "_waveController", ewc);
            SetPrivateField(cr, "_shieldWallManager", swm);
            SetPrivateField(cr, "_player", pw);
            Log("CombatResolver wired");
        }

        private void WireShieldWallManager(ShieldWallManager swm)
        {
            if (swm == null) { Log("ShieldWallManager not found!"); return; }

            var brothers = Resources.LoadAll<ShieldBrotherSO>("Brothers");
            if (brothers == null || brothers.Length == 0)
            {
                brothers = LoadAssetsFromPath<ShieldBrotherSO>("Assets/ScriptableObjects/Brothers");
            }
            
            if (brothers != null && brothers.Length > 0)
            {
                SetPrivateField(swm, "_brotherData", brothers);
                Log($"ShieldWallManager wired with {brothers.Length} brothers");
            }
        }

        private void WireWallStatusUI(WallStatusUI wui, ShieldWallManager swm)
        {
            if (wui == null) { Log("WallStatusUI not found (optional)"); return; }

            SetPrivateField(wui, "_shieldWallManager", swm);
            Log("WallStatusUI wired");
        }

        private void WireWaveUI(WaveUI wui, EnemyWaveController ewc)
        {
            if (wui == null) { Log("WaveUI not found (optional)"); return; }

            SetPrivateField(wui, "_waveController", ewc);
            Log("WaveUI wired");
        }

        private void WireGameOverUI(GameOverUI goui, BattleManager bm)
        {
            if (goui == null) { Log("GameOverUI not found (optional)"); return; }

            SetPrivateField(goui, "_battleManager", bm);
            Log("GameOverUI wired");
        }

        private void SetPrivateField<T>(object target, string fieldName, T value)
        {
            if (target == null || value == null) return;

            var field = target.GetType().GetField(fieldName, 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogWarning($"Bootstrapper: Field '{fieldName}' not found on {target.GetType().Name}");
            }
        }

        private T[] LoadAssetsFromPath<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });
            var assets = new T[guids.Length];
            
            for (int i = 0; i < guids.Length; i++)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            
            return assets;
#else
            return new T[0];
#endif
        }

        private T LoadAssetFromPath<T>(string path) where T : Object
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return null;
#endif
        }

        private void Log(string message)
        {
            if (_logWiring)
            {
                Debug.Log($"Bootstrapper: {message}");
            }
        }
    }
}

