using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Dice;

namespace ShieldWall.UI
{
    public class ActionPreviewUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _previewContainer;
        [SerializeField] private ActionPreviewItem _previewItemPrefab;
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private GameObject _emptyStatePanel;
        [SerializeField] private TextMeshProUGUI _emptyStateText;

        [Header("Settings")]
        [SerializeField] private bool _showOnlyAvailable = false;

        private readonly List<ActionPreviewItem> _previewItems = new List<ActionPreviewItem>();
        private List<ActionSO> _currentAvailableActions = new List<ActionSO>();

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

        private void Start()
        {
            if (_headerText != null)
            {
                _headerText.text = "AVAILABLE ACTIONS";
            }

            if (_emptyStateText != null)
            {
                _emptyStateText.text = "Lock dice to ready actions";
            }

            UpdateDisplay();
        }

        private void HandleAvailableActionsChanged(List<ActionSO> actions)
        {
            _currentAvailableActions = actions ?? new List<ActionSO>();
            UpdateDisplay();
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            bool visible = phase == TurnPhase.PlayerTurn;
            gameObject.SetActive(visible);
        }

        private void UpdateDisplay()
        {
            bool hasActions = _currentAvailableActions != null && _currentAvailableActions.Count > 0;

            if (_emptyStatePanel != null)
            {
                _emptyStatePanel.SetActive(!hasActions);
            }

            if (_previewContainer != null)
            {
                _previewContainer.gameObject.SetActive(hasActions);
            }

            if (!hasActions) return;

            EnsureItemCount(_currentAvailableActions.Count);

            for (int i = 0; i < _currentAvailableActions.Count; i++)
            {
                _previewItems[i].SetAction(_currentAvailableActions[i], true);
                _previewItems[i].gameObject.SetActive(true);
            }

            for (int i = _currentAvailableActions.Count; i < _previewItems.Count; i++)
            {
                _previewItems[i].gameObject.SetActive(false);
            }
        }

        private void EnsureItemCount(int count)
        {
            while (_previewItems.Count < count)
            {
                if (_previewItemPrefab == null || _previewContainer == null)
                {
                    Debug.LogWarning("ActionPreviewUI: Missing prefab or container reference");
                    return;
                }

                var item = Instantiate(_previewItemPrefab, _previewContainer);
                _previewItems.Add(item);
            }
        }
    }
}
