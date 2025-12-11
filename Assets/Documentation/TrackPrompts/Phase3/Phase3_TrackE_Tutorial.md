# Phase 3 - Track E: Tutorial System

## Assignment

You are implementing **Track E** of Phase 3: Content + Polish.

Your focus is creating a tutorial hint system that guides new players through the first waves.

---

## Your Scope

### Files to CREATE

| File | Purpose |
|------|---------|
| `Assets/Scripts/Data/TutorialHintSO.cs` | Hint ScriptableObject definition |
| `Assets/Scripts/Tutorial/TutorialManager.cs` | Controls hint display timing |
| `Assets/Scripts/UI/TutorialHintUI.cs` | Hint panel UI component |

### Assets to CREATE

| Asset | Purpose |
|-------|---------|
| `Assets/ScriptableObjects/Tutorial/Hint_LockDice.asset` | "Click dice to LOCK" |
| `Assets/ScriptableObjects/Tutorial/Hint_MatchRunes.asset` | "Match runes for ACTIONS" |
| `Assets/ScriptableObjects/Tutorial/Hint_Brothers.asset` | "Brothers BLOCK for you" |
| `Assets/ScriptableObjects/Tutorial/Hint_Stamina.asset` | "STAMINA drains each turn" |
| `Assets/ScriptableObjects/Tutorial/Hint_Berserkers.asset` | "Some enemies ignore blocks" |
| `Assets/Prefabs/UI/TutorialHintPanel.prefab` | Hint UI prefab |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/Dice/*` — Dice system
- Existing UI files in `Assets/Scripts/UI/`

---

## Implementation Details

### E1: TutorialHintSO

```csharp
using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Hint_", menuName = "ShieldWall/TutorialHint")]
    public class TutorialHintSO : ScriptableObject
    {
        public string hintId;
        [TextArea(2, 4)] public string hintText;
        
        [Header("Trigger Conditions")]
        public TurnPhase triggerPhase;
        public int triggerWave = 1;
        public bool requiresDiceLocked;
        public bool requiresNoDiceLocked;
        
        [Header("Display")]
        public float displayDuration = 5f;
        public bool autoDismiss = true;
        public bool pauseGame;
    }
}
```

### E2: TutorialManager

```csharp
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
        [SerializeField] private TutorialHintUI _hintUI;
        
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
            CheckHints(TurnPhase.EnemyReveal);
        }
        
        private void HandleDieLockToggled(int index, bool locked)
        {
            _lockedDiceCount += locked ? 1 : -1;
            _lockedDiceCount = Mathf.Max(0, _lockedDiceCount);
            
            // Check for hints that trigger on first lock
            if (locked && _lockedDiceCount == 1)
                CheckHints(TurnPhase.DiceRoll);
        }
        
        private void CheckHints(TurnPhase currentPhase)
        {
            foreach (var hint in _hints)
            {
                if (ShouldShowHint(hint, currentPhase))
                {
                    ShowHint(hint);
                    break; // Only show one hint at a time
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
```

### E3: TutorialHintUI

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Data;

namespace ShieldWall.UI
{
    public class TutorialHintUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private Button _dismissButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [Header("Animation")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.2f;
        
        private TutorialHintSO _currentHint;
        private float _displayTimer;
        private bool _isShowing;
        
        void Awake()
        {
            _panel.SetActive(false);
            if (_dismissButton != null)
                _dismissButton.onClick.AddListener(OnDismissClicked);
        }
        
        void Update()
        {
            if (_isShowing && _currentHint != null && _currentHint.autoDismiss)
            {
                _displayTimer -= Time.unscaledDeltaTime;
                if (_displayTimer <= 0)
                    HideHint();
            }
        }
        
        public void ShowHint(TutorialHintSO hint)
        {
            _currentHint = hint;
            _hintText.text = hint.hintText;
            _displayTimer = hint.displayDuration;
            
            _panel.SetActive(true);
            _isShowing = true;
            
            StartCoroutine(FadeIn());
        }
        
        public void HideHint()
        {
            _isShowing = false;
            StartCoroutine(FadeOut());
        }
        
        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = elapsed / _fadeInDuration;
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }
        
        private System.Collections.IEnumerator FadeOut()
        {
            float elapsed = 0f;
            while (elapsed < _fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = 1f - (elapsed / _fadeOutDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
            _panel.SetActive(false);
            _currentHint = null;
        }
        
        private void OnDismissClicked()
        {
            Tutorial.TutorialManager.Instance?.DismissCurrentHint();
        }
    }
}
```

### E4: Hint Configurations

**Hint_LockDice:**
```yaml
hintId: "lock_dice"
hintText: "Click on a die to LOCK it. Locked dice won't re-roll."
triggerPhase: DiceRoll
triggerWave: 1
requiresDiceLocked: false
requiresNoDiceLocked: true
displayDuration: 6
autoDismiss: true
pauseGame: false
```

**Hint_MatchRunes:**
```yaml
hintId: "match_runes"
hintText: "Match rune symbols to unlock powerful ACTIONS. Try locking matching dice!"
triggerPhase: DiceRoll
triggerWave: 1
requiresDiceLocked: true
requiresNoDiceLocked: false
displayDuration: 6
autoDismiss: true
pauseGame: false
```

**Hint_Brothers:**
```yaml
hintId: "brothers_block"
hintText: "Your shield brothers will try to BLOCK attacks for you. Keep them alive!"
triggerPhase: EnemyReveal
triggerWave: 2
displayDuration: 5
autoDismiss: true
pauseGame: false
```

**Hint_Stamina:**
```yaml
hintId: "stamina_drain"
hintText: "STAMINA drains each turn. When it runs out, you lose. Strike fast!"
triggerPhase: StaminaTick
triggerWave: 3
displayDuration: 5
autoDismiss: true
pauseGame: false
```

**Hint_Berserkers:**
```yaml
hintId: "berserkers"
hintText: "BERSERKERS ignore blocks! Kill them quickly or suffer."
triggerPhase: EnemyReveal
triggerWave: 4
displayDuration: 5
autoDismiss: true
pauseGame: false
```

### E5: UI Prefab Setup

**TutorialHintPanel.prefab:**
- Panel (RectTransform: top of screen, 400x100)
  - Background (Image: semi-transparent dark #1A1A1A, 80% alpha)
  - HintText (TextMeshPro: white, centered, 24pt)
  - DismissButton (Button: small X in corner)
- CanvasGroup component for fading

**Anchors:** Top-center, pivot at top
**Position:** (0, -50) from top

---

## Success Criteria

- [ ] TutorialHintSO compiles without errors
- [ ] TutorialManager singleton works
- [ ] TutorialHintUI shows/hides properly
- [ ] 5 hint assets created
- [ ] Hints save to PlayerPrefs (don't repeat)
- [ ] First hint shows on Wave 1, DiceRoll phase
- [ ] Hints fade in/out smoothly
- [ ] Dismiss button works
- [ ] Time.timeScale reset after pause hints

---

## Test Steps

1. Clear PlayerPrefs: Edit > Clear All PlayerPrefs
2. Add TutorialManager to Battle scene
3. Add TutorialHintPanel to Canvas
4. Wire up references
5. Press Play
6. Wave 1 starts → "Lock dice" hint appears
7. Lock a die → "Match runes" hint appears
8. Wave 2 starts → "Brothers block" hint appears
9. Stop and replay → hints should NOT reappear
10. Call TutorialManager.Instance.ResetTutorial() → hints reappear

---

## Reference Files

- `Assets/Scripts/Core/GameEvents.cs` — Event definitions
- `Assets/Scripts/Core/TurnPhase.cs` — Turn phases

