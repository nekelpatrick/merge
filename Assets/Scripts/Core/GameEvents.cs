using System;
using System.Collections.Generic;
using ShieldWall.Data;
using ShieldWall.Dice;
using ShieldWall.Formation;

namespace ShieldWall.Core
{
    public static class GameEvents
    {
        public static event Action<RuneDie[]> OnDiceRolled;
        public static event Action<int, bool> OnDieLockToggled;
        public static event Action<List<ActionSO>> OnAvailableActionsChanged;

        public static event Action<List<EnemySO>> OnEnemyWaveSpawned;
        public static event Action<EnemySO> OnEnemyKilled;
        public static event Action<Attack> OnAttackBlocked;
        public static event Action<Attack> OnAttackLanded;

        public static event Action<ShieldBrotherSO, int> OnBrotherWounded;
        public static event Action<ShieldBrotherSO> OnBrotherDied;
        public static event Action<int> OnWallIntegrityChanged;

        public static event Action<int> OnStaminaChanged;
        public static event Action<int> OnPlayerWounded;

        public static event Action<TurnPhase> OnPhaseChanged;
        public static event Action<int> OnWaveStarted;
        public static event Action OnWaveCleared;
        public static event Action<bool> OnBattleEnded;

        public static void RaiseDiceRolled(RuneDie[] dice) => OnDiceRolled?.Invoke(dice);
        public static void RaiseDieLockToggled(int index, bool isLocked) => OnDieLockToggled?.Invoke(index, isLocked);
        public static void RaiseAvailableActionsChanged(List<ActionSO> actions) => OnAvailableActionsChanged?.Invoke(actions);

        public static void RaiseEnemyWaveSpawned(List<EnemySO> enemies) => OnEnemyWaveSpawned?.Invoke(enemies);
        public static void RaiseEnemyKilled(EnemySO enemy) => OnEnemyKilled?.Invoke(enemy);
        public static void RaiseAttackBlocked(Attack attack) => OnAttackBlocked?.Invoke(attack);
        public static void RaiseAttackLanded(Attack attack) => OnAttackLanded?.Invoke(attack);

        public static void RaiseBrotherWounded(ShieldBrotherSO brother, int damage) => OnBrotherWounded?.Invoke(brother, damage);
        public static void RaiseBrotherDied(ShieldBrotherSO brother) => OnBrotherDied?.Invoke(brother);
        public static void RaiseWallIntegrityChanged(int aliveBrothers) => OnWallIntegrityChanged?.Invoke(aliveBrothers);

        public static void RaiseStaminaChanged(int stamina) => OnStaminaChanged?.Invoke(stamina);
        public static void RaisePlayerWounded(int damage) => OnPlayerWounded?.Invoke(damage);

        public static void RaisePhaseChanged(TurnPhase phase) => OnPhaseChanged?.Invoke(phase);
        public static void RaiseWaveStarted(int waveNumber) => OnWaveStarted?.Invoke(waveNumber);
        public static void RaiseWaveCleared() => OnWaveCleared?.Invoke();
        public static void RaiseBattleEnded(bool victory) => OnBattleEnded?.Invoke(victory);

        public static void ClearAllListeners()
        {
            OnDiceRolled = null;
            OnDieLockToggled = null;
            OnAvailableActionsChanged = null;
            OnEnemyWaveSpawned = null;
            OnEnemyKilled = null;
            OnAttackBlocked = null;
            OnAttackLanded = null;
            OnBrotherWounded = null;
            OnBrotherDied = null;
            OnWallIntegrityChanged = null;
            OnStaminaChanged = null;
            OnPlayerWounded = null;
            OnPhaseChanged = null;
            OnWaveStarted = null;
            OnWaveCleared = null;
            OnBattleEnded = null;
        }
    }

    public struct Attack
    {
        public EnemySO Source;
        public int Damage;
        public WallPosition Target;
    }
}
