# Phase 5 Research Findings - Current Rune/Action UI Flow

**Date:** December 2024  
**Purpose:** Document the existing dice and action UI system before Phase 5 improvements

---

## Current Architecture Overview

### Event Flow Chain

```
1. TurnManager sets phase to PlayerTurn
   ↓
2. DicePoolManager.Roll() → fires GameEvents.OnDiceRolled
   ↓
3. DiceUI updates visuals (die faces shown)
   ↓
4. Player clicks dice → DiceUI.HandleDieClicked → DicePoolManager.ToggleDieLock
   ↓
5. DicePoolManager fires GameEvents.OnDieLockToggled
   ↓
6. ComboManager.RecalculateCombos → fires GameEvents.OnAvailableActionsChanged
   ↓
7. ActionUI updates action buttons
   ↓
8. Player clicks actions → ActionSelectionManager.ToggleAction
   ↓
9. Player confirms → ActionSelectionManager fires OnActionsConfirmed
   ↓
10. TurnManager.ExecuteResolution
```

---

## Key Components

### 1. Dice System

**Files:**
- `DicePoolManager.cs` - Manages dice pool, rolling, locking
- `DicePool.cs` - Core dice logic (not reviewed but referenced)
- `RuneDie.cs` - Individual die state
- `DiceUI.cs` - Visual representation of dice

**Current Flow:**
1. Dice roll automatically when PlayerTurn phase starts
2. Player clicks individual dice to lock/unlock
3. Lock state triggers combo recalculation
4. Reroll button available (max 2 rerolls/turn)

**Pain Points Identified:**
- No visual indication of WHAT locking dice will unlock (action preview missing)
- Rune symbols shown as text codes: [AX], [LO], [BR], [SP], [OD] instead of full names
- No clear call-to-action text ("What should I do now?")
- Locked vs unlocked state is subtle

---

### 2. Combo Resolution System

**Files:**
- `ComboResolver.cs` - Static combo matching logic
- `ComboManager.cs` - Listens to dice events, fires action updates

**Current Logic:**
- `ComboResolver.Resolve()` - Returns ALL possible actions (non-greedy)
- `ComboResolver.ResolveGreedy()` - Spends runes to maximize action count
- Actions sorted by rune cost (descending) for priority

**Pain Points Identified:**
- Combo detection is hidden from player
- No feedback on "why" an action isn't available
- No preview of "if I lock these 2 dice, what actions unlock?"

---

### 3. Action UI System

**Files:**
- `ActionUI.cs` - Manages action button list
- `ActionButton.cs` - Individual action button with selection state
- `ActionSelectionManager.cs` - Tracks selected actions (not reviewed but referenced)

**Current Display:**
- Action name (e.g., "Shield Wall", "Spear Thrust")
- Rune requirement shown as text codes: `[SH][SH]`, `[AX][SP]`
- Icon (if assigned to ActionSO)
- Selected state (yellow tint)

**Pain Points Identified:**
- Rune codes are cryptic ([AX], [SP], etc.) - not intuitive for new players
- No "ready vs locked vs unmet" state visualization
- No effect preview (player can't see what action DOES without clicking)
- No affordance for "confirm actions" vs "keep selecting"

---

### 4. Turn Phase System

**Files:**
- `TurnPhase.cs` - Enum defining phases
- `TurnManager.cs` - State machine controlling battle flow

**Current Phases:**
1. `WaveStart` - Enemy wave spawns
2. `PlayerTurn` - Dice roll, action selection
3. `Resolution` - Actions execute
4. `WaveEnd` - Stamina tick, check victory

**Pain Points Identified:**
- No UI feedback for current phase (player doesn't know WHEN to act)
- Phase transitions are silent (no "Now it's your turn!" message)
- Resolution phase happens instantly with no buildup

---

## Events Used

### Dice Events
- `OnDiceRolled(RuneDie[])` - Fired when dice roll
- `OnDieLockToggled(int index, bool isLocked)` - Fired when die lock state changes

### Action Events
- `OnAvailableActionsChanged(List<ActionSO>)` - Fired when combo recalculated

### Turn Events
- `OnPhaseChanged(TurnPhase)` - Fired when turn phase changes

### Combat Events (for future feedback hooks)
- `OnEnemyKilled(EnemySO)`
- `OnAttackBlocked(Attack)`
- `OnAttackLanded(Attack)`
- `OnStaminaChanged(int)`

---

## Current User Experience Flow (from screenshot analysis)

### What Player Sees:
1. First-person view with shield visible at bottom
2. Two viking brothers visible in peripheral vision
3. Dice bar at bottom center showing rune codes: **AX LO BR SP OD**
4. "END TURN" button (red) in bottom right
5. Wave indicator: "11/12"
6. Debug text in upper left (F-key shortcuts)

### What's Confusing:
1. **Rune codes are cryptic** - "AX" = Axe (Tyr), "LO" = Loki (Laguz), "BR" = Brace (Berkana), etc.
2. **No action preview visible** - Player must memorize combos or guess
3. **No phase indicator** - Is it my turn? Should I be doing something?
4. **No enemy intent shown** - Can't see what attacks are incoming
5. **No feedback loop** - Click dice → ??? → actions appear somewhere?

---

## Data Structures

### RuneType Enum
```csharp
public enum RuneType
{
    Thurs,    // Shield - [SH]
    Tyr,      // Axe - [AX]
    Gebo,     // Spear - [SP]
    Berkana,  // Brace - [BR]
    Othala,   // Odin - [OD]
    Laguz     // Loki - [LO]
}
```

### ActionSO ScriptableObject
```csharp
- actionName: string (e.g., "Shield Wall")
- requiredRunes: RuneType[] (e.g., [Thurs, Thurs])
- icon: Sprite
- description: string
- effectType: ActionEffectType
- effectPower: int
```

---

## Integration Points for Phase 5

### Step 3: Labels/Colors Mapping
- Need to extend RuneType → full name mapping
- Need to define rune color palette (already exists in VisualStyleSystem.md)
- Expose via ScriptableObject or const lookup

### Step 4-5: Action Preview UI
- Subscribe to `OnAvailableActionsChanged`
- Display list with:
  - Full action names
  - Colored rune badges (not text codes)
  - "READY" vs "Need X more runes" state
  - Effect description tooltip

### Step 6: Phase CTAs
- Subscribe to `OnPhaseChanged`
- Show banner text:
  - `WaveStart` → "Enemies approach!"
  - `PlayerTurn` → "Lock dice to ready actions"
  - `Resolution` → "Resolving actions..."
  - `WaveEnd` → "Turn complete"

### Step 7: Inline Hints
- First roll → "Click dice to lock them for actions"
- First lock → "Locked dice unlock actions (see preview)"
- First action select → "Select actions, then confirm"
- First damage → "You took damage! Manage stamina!"

### Step 8: Combat Feedback
- Subscribe to existing events (already defined, not yet visually hooked)
- Add minimal visual hooks (flash, pulse, shake)

### Step 9: Enemy Intent
- Hook into enemy reveal phase (TurnManager.GenerateEnemyAttacks)
- Display simple icons above enemies

---

## Files NOT to Touch (Track Boundaries)

- Core combat resolution logic (`CombatResolver.cs`)
- Dice rolling math (`DicePool.cs`, `RuneDie.cs`)
- Turn state machine flow (`TurnManager.cs` phases)
- Event definitions (`GameEvents.cs` events)

Only UI presentation and feedback layers should change.

---

## Validation Checklist (for Step 2)

This research complete. Moving to Step 2: Define UX success criteria.
