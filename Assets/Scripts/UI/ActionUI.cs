using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class ActionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _actionContainer;
        [SerializeField] private ActionButton _actionButtonPrefab;

        private readonly List<ActionButton> _actionButtons = new List<ActionButton>();
        private readonly List<ActionSO> _currentActions = new List<ActionSO>();

        public IReadOnlyList<ActionSO> CurrentActions => _currentActions;

        private void OnEnable()
        {
            GameEvents.OnAvailableActionsChanged += HandleAvailableActionsChanged;
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnAvailableActionsChanged -= HandleAvailableActionsChanged;
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        private void HandleAvailableActionsChanged(List<ActionSO> actions)
        {
            _currentActions.Clear();
            _currentActions.AddRange(actions);
            
            RebuildActionButtons();
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            bool visible = phase == TurnPhase.PlayerTurn;
            gameObject.SetActive(visible);
            
            if (phase != TurnPhase.PlayerTurn)
            {
                ClearSelection();
            }
        }

        private void RebuildActionButtons()
        {
            EnsureButtonCount(_currentActions.Count);
            
            for (int i = 0; i < _currentActions.Count; i++)
            {
                _actionButtons[i].SetAction(_currentActions[i]);
                _actionButtons[i].gameObject.SetActive(true);
            }
            
            for (int i = _currentActions.Count; i < _actionButtons.Count; i++)
            {
                _actionButtons[i].gameObject.SetActive(false);
            }
        }

        private void EnsureButtonCount(int count)
        {
            while (_actionButtons.Count < count)
            {
                if (_actionButtonPrefab == null || _actionContainer == null)
                {
                    Debug.LogWarning("ActionUI: Missing prefab or container reference");
                    return;
                }
                
                var button = Instantiate(_actionButtonPrefab, _actionContainer);
                button.OnActionClicked.AddListener(HandleActionClicked);
                _actionButtons.Add(button);
            }
        }

        private void HandleActionClicked(ActionSO action)
        {
            var manager = FindFirstObjectByType<Combat.ActionSelectionManager>();
            if (manager != null)
            {
                manager.ToggleAction(action);
                UpdateSelectionVisuals(manager.SelectedActions);
            }
        }

        private void UpdateSelectionVisuals(IReadOnlyList<ActionSO> selectedActions)
        {
            foreach (var button in _actionButtons)
            {
                bool isSelected = selectedActions.Contains(button.Action);
                button.SetSelected(isSelected);
            }
        }

        private void ClearSelection()
        {
            foreach (var button in _actionButtons)
            {
                button.SetSelected(false);
            }
        }

        public void RefreshSelectionVisuals()
        {
            var manager = FindFirstObjectByType<Combat.ActionSelectionManager>();
            if (manager != null)
            {
                UpdateSelectionVisuals(manager.SelectedActions);
            }
        }
    }
}

