# Phase 3 - Track C: Battle Scenarios

## Assignment

You are implementing **Track C** of Phase 3: Content + Polish.

Your focus is creating the scenario system and 3 playable battle scenarios.

---

## Your Scope

### Files to CREATE

| File | Purpose |
|------|---------|
| `Assets/Scripts/Data/BattleScenarioSO.cs` | Scenario ScriptableObject definition |
| `Assets/Scripts/Data/Difficulty.cs` | Difficulty enum |
| `Assets/Scripts/Core/ScenarioManager.cs` | Loads and configures scenarios |

### Assets to CREATE

| Asset | Purpose |
|-------|---------|
| `Assets/ScriptableObjects/Scenarios/Scenario_TheBreach.asset` | Easy tutorial scenario |
| `Assets/ScriptableObjects/Scenarios/Scenario_HoldTheLine.asset` | Normal challenge |
| `Assets/ScriptableObjects/Scenarios/Scenario_TheLastStand.asset` | Hard survival |

### Wave Assets to CREATE (for new scenarios)

| Asset | Enemies |
|-------|---------|
| `Assets/ScriptableObjects/Waves/Wave_Easy_01.asset` | 2 Thralls |
| `Assets/ScriptableObjects/Waves/Wave_Easy_02.asset` | 3 Thralls |
| `Assets/ScriptableObjects/Waves/Wave_Easy_03.asset` | 2 Thralls, 1 Warrior |
| `Assets/ScriptableObjects/Waves/Wave_Hard_01.asset` | 2 Warriors, 1 Berserker |
| `Assets/ScriptableObjects/Waves/Wave_Hard_02.asset` | 3 Berserkers |
| `Assets/ScriptableObjects/Waves/Wave_Hard_03.asset` | 2 Archers, 2 Warriors |
| `Assets/ScriptableObjects/Waves/Wave_Hard_04.asset` | 4 Berserkers |

---

## DO NOT TOUCH

- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/UI/*` — UI components
- `Assets/Scripts/Visual/*` — Visual systems
- Existing wave assets (Wave_01 through Wave_05)

---

## Implementation Details

### C1: Difficulty Enum

```csharp
namespace ShieldWall.Data
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }
}
```

### C2: BattleScenarioSO

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Scenario_", menuName = "ShieldWall/BattleScenario")]
    public class BattleScenarioSO : ScriptableObject
    {
        public string scenarioName;
        [TextArea(2, 4)] public string description;
        public Difficulty difficulty;
        public List<WaveConfigSO> waves;
        
        [Header("Starting Conditions")]
        public int startingStamina = 12;
        public int startingPlayerHealth = 5;
        public int startingDiceCount = 4;
        
        [Header("Unlock")]
        public bool isUnlocked = true;
        public BattleScenarioSO prerequisite;
    }
}
```

### C3: ScenarioManager

```csharp
using UnityEngine;
using ShieldWall.Data;

namespace ShieldWall.Core
{
    public class ScenarioManager : MonoBehaviour
    {
        public static ScenarioManager Instance { get; private set; }
        
        [SerializeField] private BattleScenarioSO _defaultScenario;
        
        public BattleScenarioSO CurrentScenario { get; private set; }
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void SelectScenario(BattleScenarioSO scenario)
        {
            CurrentScenario = scenario;
        }
        
        public void LoadDefaultScenario()
        {
            CurrentScenario = _defaultScenario;
        }
        
        public BattleScenarioSO GetCurrentOrDefault()
        {
            return CurrentScenario ?? _defaultScenario;
        }
    }
}
```

### C4: Scenario Configurations

**Scenario_TheBreach (Easy - Tutorial)**
```yaml
scenarioName: "The Breach"
description: "Raiders have broken through the outer wall. Hold them off while the village evacuates. A good place to learn the basics."
difficulty: Easy
waves: [Wave_Easy_01, Wave_Easy_02, Wave_Easy_03]
startingStamina: 15
startingPlayerHealth: 6
startingDiceCount: 5
isUnlocked: true
```

**Scenario_HoldTheLine (Normal)**
```yaml
scenarioName: "Hold the Line"
description: "The enemy comes in force. Your shield wall is all that stands between them and your people."
difficulty: Normal
waves: [Wave_01, Wave_02, Wave_03, Wave_04, Wave_05]  # Existing waves
startingStamina: 12
startingPlayerHealth: 5
startingDiceCount: 4
isUnlocked: true
```

**Scenario_TheLastStand (Hard)**
```yaml
scenarioName: "The Last Stand"
description: "Berserkers and archers. Few supplies. No retreat. Only glory or death awaits."
difficulty: Hard
waves: [Wave_Hard_01, Wave_Hard_02, Wave_03, Wave_Hard_03, Wave_04, Wave_Hard_04, Wave_05]
startingStamina: 10
startingPlayerHealth: 4
startingDiceCount: 4
isUnlocked: false
prerequisite: Scenario_HoldTheLine
```

### C5: Wave Configurations

Reference existing enemy assets:
- `Assets/ScriptableObjects/Enemies/Enemy_Thrall.asset`
- `Assets/ScriptableObjects/Enemies/Enemy_Warrior.asset`
- `Assets/ScriptableObjects/Enemies/Enemy_Berserker.asset`
- `Assets/ScriptableObjects/Enemies/Enemy_Archer.asset`

**Wave_Easy_01:**
- waveNumber: 1
- enemies: [Thrall, Thrall]
- hasScriptedEvent: true
- scriptedEventId: "tutorial_dice"

**Wave_Easy_02:**
- waveNumber: 2
- enemies: [Thrall, Thrall, Thrall]
- hasScriptedEvent: false

**Wave_Easy_03:**
- waveNumber: 3
- enemies: [Thrall, Thrall, Warrior]
- hasScriptedEvent: false

**Wave_Hard_01:**
- waveNumber: 1
- enemies: [Warrior, Warrior, Berserker]

**Wave_Hard_02:**
- waveNumber: 2
- enemies: [Berserker, Berserker, Berserker]

**Wave_Hard_03:**
- waveNumber: 3
- enemies: [Archer, Archer, Warrior, Warrior]

**Wave_Hard_04:**
- waveNumber: 4
- enemies: [Berserker, Berserker, Berserker, Berserker]

---

## Integration Notes

The `ScenarioManager` should be used by `BattleManager` to get starting conditions:

```csharp
// In BattleManager.StartBattle():
var scenario = ScenarioManager.Instance.GetCurrentOrDefault();
_staminaManager.Initialize(scenario.startingStamina);
_playerWarrior.Initialize(scenario.startingPlayerHealth);
_waveController.Initialize(scenario.waves);
```

**Note:** Do NOT modify BattleManager directly. Document this integration point for the merge phase.

---

## Success Criteria

- [ ] BattleScenarioSO compiles without errors
- [ ] Difficulty enum exists
- [ ] ScenarioManager singleton works
- [ ] 3 scenario assets created with correct data
- [ ] 7 new wave assets created
- [ ] Scenarios reference correct waves
- [ ] Easy scenario has more starting resources
- [ ] Hard scenario is locked by default

---

## Test Steps

1. Open Unity
2. Navigate to `Assets/ScriptableObjects/Scenarios/`
3. Verify 3 scenario assets exist
4. Click each scenario, verify wave references are valid
5. Check wave counts: The Breach (3), Hold the Line (5), The Last Stand (7)
6. Verify starting stamina: 15, 12, 10 respectively

---

## Reference Files

- `Assets/Scripts/Data/WaveConfigSO.cs` — Wave structure
- `Assets/Scripts/Data/EnemySO.cs` — Enemy structure
- `Assets/ScriptableObjects/Waves/` — Existing wave assets
- `Assets/ScriptableObjects/Enemies/` — Existing enemy assets

