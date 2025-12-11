using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [SerializeField] private List<TutorialHintSO> _hints;
        [SerializeField] private UI.TutorialHintUI _hintUI;

        private HashSet<string> _shownHints = new HashSet<string>();
        private int _currentWave = 1;
        private int _lockedDiceCount = 0;

        private const string PREFS_KEY = "ShieldWall_ShownHints";

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            LoadShownHints();
        }

        void OnEnable()
        {
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
            GameEvents.OnWaveStarted += HandleWaveStarted;
            GameEvents.OnDieLockToggled += HandleDieLockToggled;
        }

        void OnDisable()
        {
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
            GameEvents.OnWaveStarted -= HandleWaveStarted;
            GameEvents.OnDieLockToggled -= HandleDieLockToggled;
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            CheckHints(phase);
        }

        private void HandleWaveStarted(int wave)
        {
            _currentWave = wave;
            CheckHints(TurnPhase.WaveStart);
        }

        private void HandleDieLockToggled(int index, bool locked)
        {
            _lockedDiceCount += locked ? 1 : -1;
            _lockedDiceCount = Mathf.Max(0, _lockedDiceCount);

            if (locked && _lockedDiceCount == 1)
                CheckHints(TurnPhase.PlayerTurn);
        }

        private void CheckHints(TurnPhase currentPhase)
        {
            foreach (var hint in _hints)
            {
                if (ShouldShowHint(hint, currentPhase))
                {
                    ShowHint(hint);
                    break;
                }
            }
        }

        private bool ShouldShowHint(TutorialHintSO hint, TurnPhase currentPhase)
        {
            if (_shownHints.Contains(hint.hintId))
                return false;

            if (hint.triggerPhase != currentPhase)
                return false;

            if (hint.triggerWave > 0 && _currentWave != hint.triggerWave)
                return false;

            if (hint.requiresDiceLocked && _lockedDiceCount == 0)
                return false;

            if (hint.requiresNoDiceLocked && _lockedDiceCount > 0)
                return false;

            return true;
        }

        private void ShowHint(TutorialHintSO hint)
        {
            _shownHints.Add(hint.hintId);
            SaveShownHints();

            _hintUI?.ShowHint(hint);

            if (hint.pauseGame)
                Time.timeScale = 0f;
        }

        public void DismissCurrentHint()
        {
            _hintUI?.HideHint();
            Time.timeScale = 1f;
        }

        public void ResetTutorial()
        {
            _shownHints.Clear();
            PlayerPrefs.DeleteKey(PREFS_KEY);
        }

        private void LoadShownHints()
        {
            string data = PlayerPrefs.GetString(PREFS_KEY, "");
            if (!string.IsNullOrEmpty(data))
            {
                string[] hints = data.Split(',');
                foreach (string h in hints)
                    _shownHints.Add(h);
            }
        }

        private void SaveShownHints()
        {
            string data = string.Join(",", _shownHints);
            PlayerPrefs.SetString(PREFS_KEY, data);
        }
    }
}

