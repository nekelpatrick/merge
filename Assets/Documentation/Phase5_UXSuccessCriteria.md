# Phase 5 - UX Success Criteria

**Date:** December 2024  
**Goal:** Make Shield Wall intuitive, enjoyable, and playable by clarifying the rune core loop

---

## Problem Statement

Based on current state analysis and screenshot review, the game has these critical UX issues:

1. **Rune codes are cryptic** - "AX", "LO", "BR" mean nothing to new players
2. **No action preview** - Players can't see what locking dice will unlock
3. **No phase guidance** - Players don't know when to act or what to do
4. **No enemy intent** - Can't plan defense without knowing what's incoming
5. **No feedback loop** - Actions feel disconnected from results

These make the core loop **confusing and boring** instead of **strategic and tense**.

---

## Target User Experience

### The Ideal First Turn

1. **Player sees:** "Lock dice to ready actions" (clear CTA)
2. **Player locks 2 Shield runes** → Action preview updates: "Shield Wall (READY)"
3. **Player sees effect:** "Block 1 attack on you"
4. **Player sees enemy intent:** Red icon above enemy = "This one attacks YOU"
5. **Player clicks Shield Wall** → Visual feedback: shield flashes, attack blocked
6. **Player feels:** "I understand what happened and why"

### Design Pillars for Phase 5

| Pillar | Current State | Target State |
|--------|---------------|--------------|
| **Discoverability** | Hidden combos, no hints | Action preview shows what's possible |
| **Clarity** | Cryptic codes ([AX], [LO]) | Full names + colored icons |
| **Guidance** | Silent phases | Clear CTAs per phase |
| **Feedback** | Instant resolution, no juice | Visual/audio feedback on hits/blocks |
| **Planning** | Blind defense | Enemy intent visible before commit |

---

## Success Criteria Checklist

### 1. Rune Clarity ✓

**Criteria:**
- [ ] Rune full names visible (not codes): "Shield", "Axe", "Spear", "Brace", "Odin", "Loki"
- [ ] Rune colors match Visual Style System (Iron Gray, Blood Red, Bronze, Forest Green, Gold, Purple)
- [ ] Dice visuals use color + symbol, not text codes
- [ ] Action requirements show colored rune badges, not [AX][SP] text

**Validation:**
- New player can identify rune types without reading tutorial
- Action requirements are self-explanatory at a glance

---

### 2. Action Preview System ✓

**Criteria:**
- [ ] Action preview panel visible at all times during PlayerTurn
- [ ] Shows ALL available actions based on currently locked dice
- [ ] Displays action name, rune cost (colored badges), and effect description
- [ ] Updates in real-time as player locks/unlocks dice
- [ ] Shows "unmet requirements" state for locked-but-not-ready actions (e.g., "Need 1 more Shield")

**Validation:**
- Player can see what actions unlock BEFORE committing to dice locks
- Player understands cause-effect: "If I lock these 2, I get that action"

**Example Preview Layout:**
```
┌─────────────────────────────────┐
│ AVAILABLE ACTIONS               │
├─────────────────────────────────┤
│ ✓ Shield Wall                   │
│   [●●] (2 Shield)               │
│   Block 1 attack on you         │
├─────────────────────────────────┤
│ ✓ Spear Thrust                  │
│   [●●] (Axe + Spear)            │
│   Kill 1 enemy                  │
├─────────────────────────────────┤
│ ○ Testudo (NEED 1 MORE SHIELD)  │
│   [●●●] (3 Shield)              │
│   Block ALL attacks this turn   │
└─────────────────────────────────┘
```

---

### 3. Phase Guidance & CTAs ✓

**Criteria:**
- [ ] Phase banner visible at top or bottom of screen
- [ ] Clear text per phase:
  - `WaveStart`: "Enemies approach! Prepare to defend!"
  - `PlayerTurn`: "Lock dice to ready actions, then confirm"
  - `Resolution`: "Resolving actions..."
  - `WaveEnd`: "Turn complete. -1 Stamina"
- [ ] Button text matches phase context:
  - `PlayerTurn`: "Confirm Actions" (not "End Turn")
  - `Resolution`: Button disabled/hidden
- [ ] Phase transitions have 0.5s delay for readability (already implemented)

**Validation:**
- Player always knows which phase they're in
- Player knows what action is expected of them
- No "what do I do now?" confusion

---

### 4. Inline Tutorial Hints ✓

**Criteria:**
- [ ] Contextual hints appear ONCE on first occurrence (not modal blocking)
- [ ] Hints are dismissible (fade after 3s or on next action)
- [ ] Hint triggers:
  1. First roll: "Click dice to lock them for combos"
  2. First lock: "Locked dice unlock actions (see preview below)"
  3. First action select: "Select actions, then confirm to execute"
  4. First damage: "You took damage! Stamina is your doom clock"
  5. First brother wounded: "Shield your brothers or they fall"
- [ ] Hints use existing TutorialHintSO system (already implemented)

**Validation:**
- New player completes first turn without external help
- Hints guide without patronizing

---

### 5. Enemy Intent Display ✓

**Criteria:**
- [ ] Simple intent icons appear above enemies during PlayerTurn
- [ ] Icons indicate:
  - Target: Player (red icon) vs Brother (yellow icon)
  - Type: Normal attack (sword) vs Ignores Block (arrow)
- [ ] Icons clear after Resolution phase
- [ ] Prototype uses simple Unity UI sprites (not 3D)

**Validation:**
- Player can see WHO is being attacked before committing actions
- Player can plan defense strategy ("I need to block 2 attacks on me")

**Example Intent:**
```
    Enemy 1      Enemy 2      Enemy 3
      [⚔️]         [⚔️]         [➡️]
    (You)      (Brother)    (You, Unblock)
```

---

### 6. Combat Feedback ✓

**Criteria:**
- [ ] Hit flash when enemy killed (brief red flash on enemy)
- [ ] Shield flash when attack blocked (white pulse on shield visual)
- [ ] Stamina tick pulse when stamina decreases (blue pulse on stamina bar)
- [ ] Damage vignette when player wounded (red edges flash)
- [ ] All feedback uses existing ScreenEffectsController if available, or new minimal system
- [ ] Feedback is immediate but non-intrusive (< 0.3s duration)

**Validation:**
- Player sees cause-effect for every action
- Combat feels responsive, not instant/flat

**Events to Hook:**
- `OnEnemyKilled` → hit flash
- `OnAttackBlocked` → shield flash
- `OnStaminaChanged` → pulse
- `OnPlayerWounded` → vignette

---

## Non-Goals (Out of Scope for Phase 5)

### What We Are NOT Changing:

- ❌ Core combat resolution logic (order of resolution, damage calc)
- ❌ Dice rolling probabilities or rune weights
- ❌ Turn state machine flow (phase order)
- ❌ 3D art, animations, or particle effects (primitives only)
- ❌ Sound effects or music (Phase 3 Track D handled audio)
- ❌ Deep tutorial system (just lightweight inline hints)
- ❌ New game mechanics or action types

### Why These Are Out:

- Core systems work, presentation is the issue
- Phase 5 is about **clarity and feedback**, not new features
- Keep changes additive and reversible

---

## Validation Test Script

**Test with a new player (or pretend to be one):**

### Pre-Test Setup:
1. Fresh Unity scene with Battle loaded
2. Wave 1 spawned (3 enemies)
3. Player has 4 dice

### Test Flow:

#### Turn 1 - First Impressions
1. ✓ Player can identify rune types by name/color (not codes)
2. ✓ Player sees "Lock dice to ready actions" prompt
3. ✓ Player locks 2 dice → Action preview updates
4. ✓ Player reads action effect before selecting
5. ✓ Player sees enemy intent icons
6. ✓ Player selects action → Button says "Confirm Actions"
7. ✓ Player clicks confirm → Sees resolution feedback (flash/pulse)
8. ✓ Player sees phase change to "Turn complete"

#### Turn 2 - Understanding
1. ✓ Player experiments with different dice combos
2. ✓ Action preview updates in real-time
3. ✓ Player sees "Need X more runes" for incomplete actions
4. ✓ Player plans defense based on enemy intent

#### Turn 3 - Confidence
1. ✓ Player executes turn without confusion
2. ✓ Player feels strategic (not random)
3. ✓ Player wants to try "just one more turn"

### Pass/Fail Criteria:
- **Pass:** Player completes 3 turns without asking "What do I do?"
- **Fail:** Player confused, stuck, or bored after turn 1

---

## Alignment with Game Design Pillars

| GDD Pillar | Phase 5 Support |
|------------|-----------------|
| **Brotherhood** | Enemy intent shows when brothers are targeted → increases attachment |
| **Fate** | Clear rune display makes dice rolls feel impactful, not arbitrary |
| **Endurance** | Stamina feedback reinforces doom clock tension |
| **Sacrifice** | Action preview clarifies trade-offs (e.g., Berserker: kill 3, take wound) |

---

## Minimal Viable Implementation

If time/scope pressure hits, prioritize in this order:

1. **MUST HAVE:**
   - Action preview with real-time updates (Step 4-5)
   - Phase CTA text (Step 6)
   - Rune full names + colors (Step 3)

2. **SHOULD HAVE:**
   - Inline hints (Step 7)
   - Enemy intent icons (Step 9)

3. **NICE TO HAVE:**
   - Combat feedback hooks (Step 8)
   - Tuning pass (Step 10)

---

## Next Steps

This criteria document approved. Proceeding to Step 3: Define rune/action labels and colors mapping.
