using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Combat;

namespace ShieldWall.UI
{
    public class EnemyIntentManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyWaveController _waveController;
        [SerializeField] private Transform _intentContainer;
        [SerializeField] private EnemyIntentIndicator _intentPrefab;

        private readonly List<EnemyIntentIndicator> _indicators = new List<EnemyIntentIndicator>();
        private List<Attack> _pendingAttacks = new List<Attack>();

        private void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            if (phase == TurnPhase.PlayerTurn)
            {
                ShowIntents();
            }
            else if (phase == TurnPhase.Resolution || phase == TurnPhase.WaveEnd)
            {
                HideIntents();
            }
        }

        public void SetPendingAttacks(List<Attack> attacks)
        {
            _pendingAttacks = attacks ?? new List<Attack>();
        }

        private void ShowIntents()
        {
            if (_pendingAttacks == null || _pendingAttacks.Count == 0) return;

            EnsureIndicatorCount(_pendingAttacks.Count);

            for (int i = 0; i < _pendingAttacks.Count; i++)
            {
                var attack = _pendingAttacks[i];
                _indicators[i].ShowIntent(attack.Target, attack.IgnoresBlocks);
                _indicators[i].gameObject.SetActive(true);
            }

            for (int i = _pendingAttacks.Count; i < _indicators.Count; i++)
            {
                _indicators[i].Hide();
            }
        }

        private void HideIntents()
        {
            foreach (var indicator in _indicators)
            {
                indicator.Hide();
            }
        }

        private void EnsureIndicatorCount(int count)
        {
            while (_indicators.Count < count)
            {
                if (_intentPrefab == null || _intentContainer == null)
                {
                    Debug.LogWarning("EnemyIntentManager: Missing prefab or container reference");
                    return;
                }

                var indicator = Instantiate(_intentPrefab, _intentContainer);
                indicator.Hide();
                _indicators.Add(indicator);
            }
        }

        [ContextMenu("Test Show Intents")]
        private void TestShowIntents()
        {
            var testAttacks = new List<Attack>
            {
                new Attack { Target = Formation.WallPosition.Center, IgnoresBlocks = false, Damage = 1 },
                new Attack { Target = Formation.WallPosition.Left, IgnoresBlocks = false, Damage = 1 },
                new Attack { Target = Formation.WallPosition.Center, IgnoresBlocks = true, Damage = 2 }
            };
            SetPendingAttacks(testAttacks);
            ShowIntents();
        }
    }
}
