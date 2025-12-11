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
            Debug.Log($"ComboManager: Die {index} lock toggled to {isLocked}");
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
            Debug.Log($"ComboManager: Locked runes = [{string.Join(", ", lockedRunes)}]");
            
            if (_allActions == null || _allActions.Length == 0)
            {
                Debug.LogWarning("ComboManager: No actions assigned!");
                return;
            }
            
            _currentAvailableActions = ComboResolver.Resolve(lockedRunes, _allActions);
            Debug.Log($"ComboManager: Found {_currentAvailableActions.Count} available actions");
            
            foreach (var action in _currentAvailableActions)
            {
                Debug.Log($"  - {action.actionName}");
            }
            
            GameEvents.RaiseAvailableActionsChanged(_currentAvailableActions);
        }

        public void ClearCombos()
        {
            _currentAvailableActions.Clear();
            GameEvents.RaiseAvailableActionsChanged(_currentAvailableActions);
        }
    }
}

