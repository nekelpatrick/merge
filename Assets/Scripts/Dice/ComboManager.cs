using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Dice
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField] private DicePoolManager _dicePoolManager;
        [SerializeField] private ActionSO[] _allActions;

        private List<ActionSO> _currentAvailableActions = new List<ActionSO>();

        public IReadOnlyList<ActionSO> AvailableActions => _currentAvailableActions;

        private void OnEnable()
        {
            GameEvents.OnDieLockToggled += HandleDieLockToggled;
            GameEvents.OnDiceRolled += HandleDiceRolled;
        }

        private void OnDisable()
        {
            GameEvents.OnDieLockToggled -= HandleDieLockToggled;
            GameEvents.OnDiceRolled -= HandleDiceRolled;
        }

        private void HandleDieLockToggled(int index, bool isLocked)
        {
            RecalculateCombos();
        }

        private void HandleDiceRolled(RuneDie[] dice)
        {
            RecalculateCombos();
        }

        [ContextMenu("Recalculate Combos")]
        public void RecalculateCombos()
        {
            if (_dicePoolManager == null)
            {
                Debug.LogWarning("ComboManager: DicePoolManager not assigned");
                return;
            }

            var lockedRunes = _dicePoolManager.GetLockedRunes();
            _currentAvailableActions = ComboResolver.Resolve(lockedRunes, _allActions);
            
            GameEvents.RaiseAvailableActionsChanged(_currentAvailableActions);
        }

        public void ClearCombos()
        {
            _currentAvailableActions.Clear();
            GameEvents.RaiseAvailableActionsChanged(_currentAvailableActions);
        }
    }
}

