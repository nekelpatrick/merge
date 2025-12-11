using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Combat
{
    public class ActionSelectionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _endTurnButton;

        public static event Action<List<ActionSO>> OnActionsConfirmed;

        private readonly List<ActionSO> _selectedActions = new List<ActionSO>();

        public IReadOnlyList<ActionSO> SelectedActions => _selectedActions;

        private void Awake()
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.AddListener(ConfirmActions);
            }
        }

        private void OnDestroy()
        {
            if (_endTurnButton != null)
            {
                _endTurnButton.onClick.RemoveListener(ConfirmActions);
            }
        }

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
                ClearSelection();
            }
            
            if (_endTurnButton != null)
            {
                _endTurnButton.interactable = phase == TurnPhase.PlayerTurn;
            }
        }

        public void SelectAction(ActionSO action)
        {
            if (action != null && !_selectedActions.Contains(action))
            {
                _selectedActions.Add(action);
            }
        }

        public void DeselectAction(ActionSO action)
        {
            _selectedActions.Remove(action);
        }

        public void ToggleAction(ActionSO action)
        {
            if (_selectedActions.Contains(action))
            {
                DeselectAction(action);
            }
            else
            {
                SelectAction(action);
            }
        }

        public void ClearSelection()
        {
            _selectedActions.Clear();
        }

        [ContextMenu("Confirm Actions")]
        public void ConfirmActions()
        {
            Debug.Log($"ActionSelectionManager: Confirming {_selectedActions.Count} actions");
            
            foreach (var action in _selectedActions)
            {
                Debug.Log($"  - {action.actionName} ({action.effectType})");
            }
            
            OnActionsConfirmed?.Invoke(new List<ActionSO>(_selectedActions));
        }

        public List<ActionSO> GetAndClearSelection()
        {
            var actions = new List<ActionSO>(_selectedActions);
            ClearSelection();
            return actions;
        }
    }
}

