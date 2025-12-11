using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ShieldWall.Core;
using ShieldWall.Dice;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class DiceUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DicePoolManager _dicePoolManager;
        [SerializeField] private Transform _diceContainer;
        [SerializeField] private DieVisual _dieVisualPrefab;
        [SerializeField] private Button _rerollButton;
        [SerializeField] private TMPro.TextMeshProUGUI _rerollCountText;

        [Header("Rune Data")]
        [SerializeField] private RuneSO[] _runeData;

        [Header("Settings")]
        [SerializeField] private int _maxRerollsPerTurn = 2;

        private readonly List<DieVisual> _dieVisuals = new List<DieVisual>();
        private int _rerollsRemaining;
        private bool _canInteract = true;

        private void OnEnable()
        {
            GameEvents.OnDiceRolled += HandleDiceRolled;
            GameEvents.OnDieLockToggled += HandleDieLockToggled;
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnDiceRolled -= HandleDiceRolled;
            GameEvents.OnDieLockToggled -= HandleDieLockToggled;
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        private void Start()
        {
            if (_rerollButton != null)
            {
                _rerollButton.onClick.AddListener(HandleRerollClicked);
            }
            
            ResetForNewTurn();
        }

        private void OnDestroy()
        {
            if (_rerollButton != null)
            {
                _rerollButton.onClick.RemoveListener(HandleRerollClicked);
            }
        }

        public void ResetForNewTurn()
        {
            _rerollsRemaining = _maxRerollsPerTurn;
            UpdateRerollUI();
        }

        public void SetInteractable(bool canInteract)
        {
            _canInteract = canInteract;
            UpdateRerollUI();
        }

        private void HandleDiceRolled(RuneDie[] dice)
        {
            EnsureDieVisualsCount(dice.Length);
            
            for (int i = 0; i < dice.Length; i++)
            {
                var runeData = GetRuneData(dice[i].CurrentFace);
                _dieVisuals[i].SetRune(dice[i].CurrentFace, runeData);
                _dieVisuals[i].SetLocked(dice[i].IsLocked);
                _dieVisuals[i].gameObject.SetActive(true);
            }
            
            for (int i = dice.Length; i < _dieVisuals.Count; i++)
            {
                _dieVisuals[i].gameObject.SetActive(false);
            }
        }

        private void HandleDieLockToggled(int index, bool isLocked)
        {
            if (index >= 0 && index < _dieVisuals.Count)
            {
                _dieVisuals[index].SetLocked(isLocked);
            }
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            _canInteract = phase == TurnPhase.PlayerTurn;
            UpdateRerollUI();
            
            if (phase == TurnPhase.PlayerTurn)
            {
                ResetForNewTurn();
            }
        }

        private void EnsureDieVisualsCount(int count)
        {
            while (_dieVisuals.Count < count)
            {
                if (_dieVisualPrefab == null || _diceContainer == null)
                {
                    Debug.LogWarning("DiceUI: Missing prefab or container reference");
                    return;
                }
                
                var visual = Instantiate(_dieVisualPrefab, _diceContainer);
                visual.Initialize(_dieVisuals.Count);
                visual.OnDieClicked.AddListener(HandleDieClicked);
                _dieVisuals.Add(visual);
            }
        }

        private void HandleDieClicked(int index)
        {
            if (!_canInteract) return;
            
            _dicePoolManager?.ToggleDieLock(index);
        }

        private void HandleRerollClicked()
        {
            if (!_canInteract) return;
            if (_rerollsRemaining <= 0) return;
            
            _rerollsRemaining--;
            UpdateRerollUI();
            
            _dicePoolManager?.Roll();
        }

        private void UpdateRerollUI()
        {
            if (_rerollCountText != null)
            {
                _rerollCountText.text = $"Rerolls: {_rerollsRemaining}";
            }

            if (_rerollButton != null)
            {
                _rerollButton.interactable = _canInteract && _rerollsRemaining > 0;
            }
        }

        private RuneSO GetRuneData(RuneType type)
        {
            if (_runeData == null) return null;
            
            foreach (var rune in _runeData)
            {
                if (rune != null && rune.runeType == type)
                {
                    return rune;
                }
            }
            return null;
        }
    }
}

