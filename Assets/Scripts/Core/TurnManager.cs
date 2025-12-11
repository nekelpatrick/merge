using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Combat;
using ShieldWall.Data;
using ShieldWall.Dice;
using ShieldWall.ShieldWall;
using ShieldWall.Formation;

namespace ShieldWall.Core
{
    public class TurnManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DicePoolManager _dicePoolManager;
        [SerializeField] private ComboManager _comboManager;
        [SerializeField] private EnemyWaveController _waveController;
        [SerializeField] private CombatResolver _combatResolver;
        [SerializeField] private ShieldWallManager _shieldWallManager;
        [SerializeField] private StaminaManager _staminaManager;
        [SerializeField] private PlayerWarrior _player;

        [Header("Settings")]
        [SerializeField] private float _phaseTransitionDelay = 0.5f;

        private TurnPhase _currentPhase = TurnPhase.WaveStart;
        private List<Attack> _pendingAttacks = new List<Attack>();
        private bool _battleEnded;

        public TurnPhase CurrentPhase => _currentPhase;
        public bool IsBattleActive => !_battleEnded;

        private void OnEnable()
        {
            ActionSelectionManager.OnActionsConfirmed += HandleActionsConfirmed;
        }

        private void OnDisable()
        {
            ActionSelectionManager.OnActionsConfirmed -= HandleActionsConfirmed;
        }

        [ContextMenu("Start Battle")]
        public void StartBattle()
        {
            _battleEnded = false;
            _staminaManager?.Initialize();
            _player?.Initialize();
            _shieldWallManager?.InitializeBrothers();
            _waveController?.StartBattle();
            
            SetPhase(TurnPhase.WaveStart);
        }

        public void SetPhase(TurnPhase phase)
        {
            _currentPhase = phase;
            GameEvents.RaisePhaseChanged(phase);
            
            Debug.Log($"TurnManager: Phase changed to {phase}");
            
            switch (phase)
            {
                case TurnPhase.WaveStart:
                    StartCoroutine(ExecuteWaveStart());
                    break;
                case TurnPhase.PlayerTurn:
                    ExecutePlayerTurn();
                    break;
                case TurnPhase.Resolution:
                    break;
                case TurnPhase.WaveEnd:
                    StartCoroutine(ExecuteWaveEnd());
                    break;
            }
        }

        private IEnumerator ExecuteWaveStart()
        {
            yield return new WaitForSeconds(_phaseTransitionDelay);

            if (_waveController.AllEnemiesDead || _waveController.CurrentWaveNumber == 0)
            {
                bool hasMore = _waveController.SpawnNextWave();
                if (!hasMore)
                {
                    EndBattle(true);
                    yield break;
                }
            }

            _pendingAttacks = GenerateEnemyAttacks();
            
            Debug.Log($"TurnManager: Wave {_waveController.CurrentWaveNumber} - {_pendingAttacks.Count} attacks incoming");

            yield return new WaitForSeconds(_phaseTransitionDelay);
            
            SetPhase(TurnPhase.PlayerTurn);
        }

        private void ExecutePlayerTurn()
        {
            _dicePoolManager?.ResetForNewTurn();
            _dicePoolManager?.Roll();
            _comboManager?.RecalculateCombos();
        }

        private void HandleActionsConfirmed(List<ActionSO> actions)
        {
            if (_currentPhase != TurnPhase.PlayerTurn) return;
            
            StartCoroutine(ExecuteResolution(actions));
        }

        private IEnumerator ExecuteResolution(List<ActionSO> playerActions)
        {
            SetPhase(TurnPhase.Resolution);
            
            yield return new WaitForSeconds(_phaseTransitionDelay);

            var result = _combatResolver.Resolve(playerActions, _pendingAttacks, _shieldWallManager);

            yield return new WaitForSeconds(_phaseTransitionDelay * 0.5f);

            ProcessAutoDefense();

            yield return new WaitForSeconds(_phaseTransitionDelay * 0.5f);

            ApplyDamage(result);

            yield return new WaitForSeconds(_phaseTransitionDelay);

            if (CheckBattleEnd())
            {
                yield break;
            }

            SetPhase(TurnPhase.WaveEnd);
        }

        private void ProcessAutoDefense()
        {
            for (int i = _pendingAttacks.Count - 1; i >= 0; i--)
            {
                var attack = _pendingAttacks[i];
                
                if (attack.Target != WallPosition.Center)
                {
                    if (_shieldWallManager.TryAutoDefense(attack.Target))
                    {
                        GameEvents.RaiseAttackBlocked(attack);
                        _pendingAttacks.RemoveAt(i);
                        Debug.Log($"TurnManager: Brother auto-defended attack at {attack.Target}");
                    }
                }
            }
        }

        private void ApplyDamage(CombatResolver.ResolutionResult result)
        {
            if (result.DamageToPlayer > 0)
            {
                _player?.TakeDamage(result.DamageToPlayer);
            }

            foreach (var kvp in result.DamageToBrothers)
            {
                _shieldWallManager.ApplyDamageToBrother(kvp.Key, kvp.Value);
            }
        }

        private IEnumerator ExecuteWaveEnd()
        {
            yield return new WaitForSeconds(_phaseTransitionDelay);

            _staminaManager?.TickTurnEnd();

            if (CheckBattleEnd())
            {
                yield break;
            }

            if (_waveController.AllEnemiesDead)
            {
                _staminaManager?.OnWaveCleared();
                Debug.Log("TurnManager: Wave cleared!");
                
                if (!_waveController.HasMoreWaves)
                {
                    EndBattle(true);
                    yield break;
                }
            }

            yield return new WaitForSeconds(_phaseTransitionDelay);
            
            SetPhase(TurnPhase.WaveStart);
        }

        private List<Attack> GenerateEnemyAttacks()
        {
            var enemies = _waveController.GetLiveEnemies();
            return _combatResolver.GenerateEnemyAttacks(enemies, _shieldWallManager);
        }

        private bool CheckBattleEnd()
        {
            if (_player != null && _player.IsDead)
            {
                EndBattle(false);
                return true;
            }

            if (_staminaManager != null && _staminaManager.IsExhausted)
            {
                EndBattle(false);
                return true;
            }

            return false;
        }

        private void EndBattle(bool victory)
        {
            _battleEnded = true;
            Debug.Log($"TurnManager: Battle ended - {(victory ? "VICTORY" : "DEFEAT")}");
            GameEvents.RaiseBattleEnded(victory);
        }
    }
}

