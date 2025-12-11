using System;
using System.Collections;
using UnityEngine;

namespace ShieldWall.Combat
{
    public class CombatPacer : MonoBehaviour
    {
        public static CombatPacer Instance { get; private set; }

        [Header("Resolution Timing")]
        [SerializeField] private float _actionAnticipationDelay = 0.3f;
        [SerializeField] private float _actionExecutionDelay = 0.2f;
        [SerializeField] private float _betweenActionsDelay = 0.15f;

        [Header("Enemy Attack Timing")]
        [SerializeField] private float _enemyTelegraphDuration = 0.4f;
        [SerializeField] private float _betweenEnemyAttacksDelay = 0.25f;

        [Header("Phase Timing")]
        [SerializeField] private float _phaseTransitionDelay = 0.5f;
        [SerializeField] private float _waveStartDelay = 1f;
        [SerializeField] private float _waveEndDelay = 1.5f;

        public float ActionAnticipationDelay => _actionAnticipationDelay;
        public float ActionExecutionDelay => _actionExecutionDelay;
        public float BetweenActionsDelay => _betweenActionsDelay;
        public float EnemyTelegraphDuration => _enemyTelegraphDuration;
        public float BetweenEnemyAttacksDelay => _betweenEnemyAttacksDelay;
        public float PhaseTransitionDelay => _phaseTransitionDelay;
        public float WaveStartDelay => _waveStartDelay;
        public float WaveEndDelay => _waveEndDelay;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ExecuteWithTiming(Action action, float delay, Action onComplete = null)
        {
            StartCoroutine(DelayedExecute(action, delay, onComplete));
        }

        public Coroutine ExecuteSequence(params (Action action, float delay)[] sequence)
        {
            return StartCoroutine(ExecuteSequenceRoutine(sequence));
        }

        private IEnumerator DelayedExecute(Action action, float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
            onComplete?.Invoke();
        }

        private IEnumerator ExecuteSequenceRoutine((Action action, float delay)[] sequence)
        {
            foreach (var (action, delay) in sequence)
            {
                yield return new WaitForSeconds(delay);
                action?.Invoke();
            }
        }

        public IEnumerator WaitForActionAnticipation()
        {
            yield return new WaitForSeconds(_actionAnticipationDelay);
        }

        public IEnumerator WaitForActionExecution()
        {
            yield return new WaitForSeconds(_actionExecutionDelay);
        }

        public IEnumerator WaitBetweenActions()
        {
            yield return new WaitForSeconds(_betweenActionsDelay);
        }

        public IEnumerator WaitForEnemyTelegraph()
        {
            yield return new WaitForSeconds(_enemyTelegraphDuration);
        }

        public IEnumerator WaitBetweenEnemyAttacks()
        {
            yield return new WaitForSeconds(_betweenEnemyAttacksDelay);
        }

        public IEnumerator WaitForPhaseTransition()
        {
            yield return new WaitForSeconds(_phaseTransitionDelay);
        }

        public IEnumerator WaitForWaveStart()
        {
            yield return new WaitForSeconds(_waveStartDelay);
        }

        public IEnumerator WaitForWaveEnd()
        {
            yield return new WaitForSeconds(_waveEndDelay);
        }
    }
}

