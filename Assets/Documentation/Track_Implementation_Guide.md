# Track-Assigned Implementation Guide - Top 10 UX Issues

**Purpose:** Complete implementation specs for top 10 prioritized issues  
**Format:** Track owner, acceptance criteria, test steps, integration notes  
**Status:** Ready for implementation

---

## Wave 1: Critical 5 (Must Fix Before Playtesting)

### UX-001: Fix Cryptic Rune Codes

**Priority:** 10.0 (CRITICAL)  
**Track Owner:** Phase5 (UI)  
**Estimated Time:** 5 minutes  
**Risk Level:** 🟢 Low

#### Problem
Dice display "BR", "AX", "SP" instead of "Shield", "Axe", "Spear" - cryptic to new players.

#### Root Cause
`Assets/Scripts/UI/DieVisual.cs` lines 116-126 has hardcoded `GetRuneSymbol()` method that returns 2-letter codes, ignoring the proper `RuneDisplay.GetFullName()` helper.

#### Proposed Fix
Replace `GetRuneSymbol()` implementation to call `RuneDisplay.GetFullName()`.

```csharp
// BEFORE (lines 116-126):
private string GetRuneSymbol(RuneType type)
{
    return type switch
    {
        RuneType.Thurs => "SH",
        RuneType.Tyr => "AX",
        RuneType.Gebo => "SP",
        RuneType.Berkana => "BR",
        RuneType.Othala => "OD",
        RuneType.Laguz => "LO",
        _ => "?"
    };
}

// AFTER (simple replacement):
private string GetRuneSymbol(RuneType type)
{
    return RuneDisplay.GetFullName(type);
}
```

#### Files to Modify
- `Assets/Scripts/UI/DieVisual.cs` (1 line change)

#### Acceptance Criteria
- [ ] Dice in game show "Shield" instead of "SH"
- [ ] Dice show "Axe" instead of "AX"
- [ ] Dice show "Spear" instead of "SP"
- [ ] Dice show "Brace" instead of "BR"
- [ ] Dice show "Odin" instead of "OD"
- [ ] Dice show "Loki" instead of "LO"
- [ ] Text fits in die visual (not cut off)
- [ ] Colors match Visual Style System (if using RuneDisplay colors)

#### Test Steps
1. Open Unity, load `Battle.unity` scene
2. Press Play
3. Wait for dice roll (first turn)
4. **Verify:** Each die shows full name, not 2-letter code
5. Lock a die, confirm text doesn't change
6. Reroll dice (if available), verify new faces show full names
7. Take screenshot for documentation

#### Integration Notes
- **Depends on:** `RuneDisplay.cs` existing (already present)
- **Impacts:** Dice UI only, no other systems affected
- **Rollback:** Revert 1-line change if font/layout breaks

#### Validation
✅ **Pass:** New player can name runes without tutorial  
❌ **Fail:** Text cut off, still shows codes, or wrong names

---

### UX-002: Add Action Preview System

**Priority:** 5.0 (CRITICAL)  
**Track Owner:** Phase5 (UI)  
**Estimated Time:** 30 minutes  
**Risk Level:** 🟡 Medium

#### Problem
Players don't know what locking dice will unlock. Combo system is hidden.

#### Root Cause
1. `ActionPreviewItem.prefab` doesn't exist
2. `ActionPreviewUI` GameObject not added to Battle scene
3. Component exists in code but never instantiated

#### Proposed Fix
1. Create `ActionPreviewItem.prefab` with UI layout (icon, name, rune badges, description)
2. Add `ActionPreviewUI` GameObject to Battle scene Canvas
3. Wire prefab reference in inspector
4. Test real-time updates

#### Files to Create
- `Assets/Prefabs/UI/ActionPreviewItem.prefab`

#### Files to Modify
- `Assets/Scenes/Battle.unity` (add GameObject to Canvas)

#### Prefab Layout (ActionPreviewItem)
```
ActionPreviewItem (Prefab)
├── Panel (Image - background)
├── Icon (Image - action icon)
├── NameText (TextMeshProUGUI - "Shield Wall")
├── RuneBadgesContainer (Horizontal Layout Group)
│   └── RuneBadge (Image - colored circles) × N
├── DescriptionText (TextMeshProUGUI - "Block 1 attack on you")
└── ReadyIndicator (Image - checkmark or "NEED 1 MORE")
```

#### Scene Integration Steps
1. In Battle.unity, locate Canvas
2. Create empty GameObject → name "ActionPreviewUI"
3. Add `ActionPreviewUI.cs` component
4. Set anchors: Bottom-left corner, above dice UI
5. Assign prefab reference: `ActionPreviewItem.prefab`
6. Set container (Vertical Layout Group)
7. Test in Play mode

#### Acceptance Criteria
- [ ] Action preview panel visible during PlayerTurn phase
- [ ] Panel hidden during other phases
- [ ] Shows ALL available actions based on locked dice
- [ ] Displays action name (e.g., "Shield Wall")
- [ ] Displays rune cost as colored badges (not text codes like [SH][SH])
- [ ] Displays effect description (e.g., "Block 1 attack on you")
- [ ] Updates in real-time when player locks/unlocks dice
- [ ] Shows "READY" indicator when requirements met
- [ ] Shows "NEED 1 MORE [rune]" when incomplete
- [ ] Panel layout doesn't overlap dice UI

#### Test Steps
1. Open Battle.unity, press Play
2. Wave starts, dice roll
3. **Verify:** Action preview panel appears (may be empty if no dice locked)
4. Lock 1 Shield die
5. **Verify:** Panel updates, shows partial actions (e.g., "Shield Wall - NEED 1 MORE")
6. Lock 2nd Shield die
7. **Verify:** "Shield Wall" shows "READY" indicator
8. Unlock 1 die
9. **Verify:** Panel updates to "NEED 1 MORE"
10. Lock different combo (e.g., Axe + Spear)
11. **Verify:** Shows different action (e.g., "Spear Thrust")
12. Confirm turn, verify panel hidden during Resolution

#### Integration Notes
- **Depends on:** 
  - `ActionPreviewUI.cs` (exists)
  - `ActionPreviewItem.cs` (exists)
  - `GameEvents.OnAvailableActionsChanged` (exists, fired by ComboManager)
- **Impacts:** UI only, doesn't change game logic
- **Rollback:** Disable GameObject if breaks UI

#### Validation
✅ **Pass:** Player can see what actions unlock BEFORE committing dice  
❌ **Fail:** Panel doesn't appear, doesn't update, or shows wrong info

---

### UX-003: Add Phase Guidance Banner

**Priority:** 5.0 (CRITICAL)  
**Track Owner:** Phase5 (UI)  
**Estimated Time:** 20 minutes  
**Risk Level:** 🟡 Medium

#### Problem
Players don't know what phase they're in or what to do. No guidance = paralysis.

#### Root Cause
1. `PhaseBannerUI.prefab` doesn't exist
2. `PhaseBannerUI` GameObject not added to Battle scene
3. Component exists but never instantiated

#### Proposed Fix
1. Create `PhaseBannerUI.prefab` with fade animation
2. Add to Battle scene (top or bottom of screen)
3. Wire up to `GameEvents.OnPhaseChanged`
4. Test phase transitions

#### Files to Create
- `Assets/Prefabs/UI/PhaseBannerUI.prefab`

#### Files to Modify
- `Assets/Scenes/Battle.unity`

#### Prefab Layout (PhaseBannerUI)
```
PhaseBannerUI (Prefab)
├── Panel (Image - semi-transparent background)
├── PhaseText (TextMeshProUGUI - "YOUR TURN" - large, bold)
├── CTAText (TextMeshProUGUI - "Lock dice to ready actions, then confirm" - smaller)
└── CanvasGroup (component for fade in/out)
```

#### Phase Messages
- **WaveStart:** "ENEMIES APPROACH!" / "Prepare to defend!"
- **PlayerTurn:** "YOUR TURN" / "Lock dice to ready actions, then confirm"
- **Resolution:** "RESOLVING ACTIONS..." / ""
- **WaveEnd:** "TURN COMPLETE" / "-1 Stamina"

#### Scene Integration Steps
1. In Battle.unity, locate Canvas
2. Create empty GameObject → name "PhaseBannerUI"
3. Add `PhaseBannerUI.cs` component
4. Set anchors: Top-center, stretch horizontally
5. Configure fade duration (0.3s in, 2s hold, 0.2s out)
6. Test phase transitions in Play mode

#### Acceptance Criteria
- [ ] Phase banner visible at top (or bottom) of screen
- [ ] Shows clear phase name (e.g., "YOUR TURN")
- [ ] Shows contextual CTA (e.g., "Lock dice to ready actions")
- [ ] Fades in smoothly (0.3s)
- [ ] Holds for 2 seconds (readable)
- [ ] Fades out smoothly (0.2s)
- [ ] Updates on every phase change
- [ ] Text matches expected messages per phase
- [ ] Doesn't block critical UI (dice, actions)

#### Test Steps
1. Open Battle.unity, press Play
2. Wave 1 starts
3. **Verify:** Banner shows "ENEMIES APPROACH!" (fades in)
4. Wait 2 seconds
5. **Verify:** Banner fades out
6. Dice roll phase
7. **Verify:** Banner shows "YOUR TURN - Lock dice..." (fades in)
8. Lock dice, select action, confirm turn
9. **Verify:** Banner shows "RESOLVING ACTIONS..." (brief)
10. Resolution completes
11. **Verify:** Banner shows "TURN COMPLETE - -1 Stamina"
12. Repeat for Wave 2, confirm consistent behavior

#### Integration Notes
- **Depends on:**
  - `PhaseBannerUI.cs` (exists)
  - `GameEvents.OnPhaseChanged` (exists, fired by TurnManager)
- **Impacts:** UI only, no game logic changes
- **Rollback:** Disable GameObject if intrusive

#### Validation
✅ **Pass:** Player always knows current phase and what to do  
❌ **Fail:** Banner doesn't appear, wrong messages, or blocks UI

---

### UX-004: Visual Distinction (Enemies vs Brothers)

**Priority:** 5.0 (CRITICAL)  
**Track Owner:** Track A (Visuals)  
**Estimated Time:** 1 hour (includes asset generation + color assignment)  
**Risk Level:** 🟡 Medium

#### Problem
All characters are brown capsules - can't distinguish enemies from brothers. Battlefield unreadable.

#### Root Cause
1. 3D asset creator tools exist but never executed
2. All meshes use Unity default material (brown)
3. No color coding per Visual Style System

#### Proposed Fix
1. Run `Shield Wall Builder > 3D Assets > Create All 3D Assets (One-Click)`
2. Assign materials per Visual Style System:
   - Brothers: Iron Gray (#5C5C5C)
   - Enemies: Blood Red (#8B2020) or dark variant
   - Player shield: Iron Gray with blue accent
3. Verify visual hierarchy in Battle scene

#### Files Generated (Automated)
- `Assets/Art/Materials/M_Character_Player.mat`
- `Assets/Art/Materials/M_Character_Brother.mat`
- `Assets/Art/Materials/M_Character_Enemy.mat`
- `Assets/Prefabs/Characters/Brother_Placeholder.prefab` (updated)
- `Assets/Prefabs/Characters/Enemy_Placeholder.prefab` (updated)

#### Files to Modify
- `Assets/Scenes/Battle.unity` (assign new materials to existing character GameObjects)

#### Color Assignments (from Visual Style System)
| Character Type | Base Color | Material Name |
|----------------|------------|---------------|
| Player Shield | Iron Gray (#5C5C5C) | M_Character_Player |
| Brothers | Iron Gray (#5C5C5C) + slight variation per brother | M_Character_Brother |
| Enemies | Blood Red (#8B2020) or Dark Brown (#3A2A1A) | M_Character_Enemy |

#### Implementation Steps
1. Unity menu → `Shield Wall Builder > 3D Assets > Create All 3D Assets`
2. Wait for assets to generate (~30 seconds)
3. Verify materials created in `Assets/Art/Materials/`
4. Open Battle.unity scene
5. Select all Brother GameObjects
6. Assign `M_Character_Brother.mat` to MeshRenderer
7. Select all Enemy GameObjects
8. Assign `M_Character_Enemy.mat` to MeshRenderer
9. Select PlayerShield GameObject
10. Assign `M_Character_Player.mat`
11. Test in Play mode

#### Acceptance Criteria
- [ ] Brothers visually distinct from enemies (different colors)
- [ ] Brothers use Iron Gray or similar (per Visual Style System)
- [ ] Enemies use Blood Red or dark variant
- [ ] Player shield visible and distinct
- [ ] Can identify enemies at a glance (< 2 seconds)
- [ ] Can identify brothers at a glance
- [ ] Colors match Visual Style System palette
- [ ] Toon shader applied (if available)
- [ ] No rendering artifacts or broken materials

#### Test Steps
1. Run asset creator (Unity menu)
2. Open Battle.unity
3. Press Play
4. **Verify:** Brothers appear gray/metallic
5. **Verify:** Enemies appear red/dark
6. **Verify:** Can distinguish at a glance (ask someone)
7. Kill an enemy
8. **Verify:** Color change noticeable (or enemy disappears)
9. Take screenshot showing clear visual hierarchy

#### Integration Notes
- **Depends on:**
  - `ShieldWallSceneBuilder.cs` (exists in Editor/)
  - Visual Style System color palette defined
- **Impacts:** All character visuals
- **Risk:** May break existing prefabs if references lost
- **Rollback:** Revert materials to Unity defaults

#### Validation
✅ **Pass:** Can distinguish enemies from brothers instantly  
❌ **Fail:** Still look same, wrong colors, or rendering broken

---

### UX-011: Brother Identity (Names + Distinction)

**Priority:** 5.0 (CRITICAL)  
**Track Owner:** Track A (Visuals)  
**Estimated Time:** 20 minutes  
**Risk Level:** 🟡 Medium

#### Problem
All brothers look identical, no names visible. Player can't identify Bjorn from Erik from Gunnar.

#### Root Cause
1. No name labels above brothers
2. All use same material/color
3. Brotherhood pillar fails without identity

#### Proposed Fix
1. Add TextMeshPro world-space name labels above each brother
2. Optionally: slight color variation per brother (within Iron Gray palette)
3. Optionally: health bar UI above each brother

#### Files to Modify
- `Assets/Scenes/Battle.unity` (add name labels to brother GameObjects)
- Optionally: `Assets/Prefabs/Characters/Brother_Placeholder.prefab`

#### Implementation Steps
1. Open Battle.unity
2. For each Brother GameObject:
   a. Create child GameObject → name "NameLabel"
   b. Add TextMeshPro - Text (World Space) component
   c. Set text to brother name ("BJORN", "ERIK", etc.)
   d. Position above brother head (~0.5-1.0 units up)
   e. Rotate to face camera (LookAt player camera, or Billboard component)
   f. Set font size (3-5 in world space)
   g. Set color (Bone White #D4C8B8 per Visual Style)
   h. Add outline (dark brown for readability)
3. Test in Play mode, verify labels visible and readable
4. Optionally: add health bar (simple Image slider above name)

#### Name Label Layout
```
Brother GameObject
├── BrotherVisual (MeshRenderer)
└── NameLabel (TextMeshPro - World Space)
    ├── Text: "BJORN" (Bone White #D4C8B8)
    ├── Font Size: 4
    ├── Outline: Dark Brown, thickness 0.2
    └── Alignment: Center
```

#### Acceptance Criteria
- [ ] Each brother has name visible above head
- [ ] Names match brother ScriptableObjects (Bjorn, Erik, Gunnar, Olaf)
- [ ] Text readable from player camera angle
- [ ] Text doesn't clip through geometry
- [ ] Text color contrasts with background (use outline)
- [ ] Names persist throughout battle
- [ ] Wounded/dead brothers still show names (or hide appropriately)
- [ ] Can distinguish brothers by name at a glance

#### Test Steps
1. Open Battle.unity, press Play
2. **Verify:** See "BJORN" above leftmost brother
3. **Verify:** See "ERIK" above next brother
4. **Verify:** See "GUNNAR" and "OLAF" on remaining brothers
5. Move camera (if possible), verify names stay readable
6. Wound a brother (via debug or gameplay)
7. **Verify:** Name remains visible
8. Brother dies
9. **Verify:** Name disappears (or fades)

#### Integration Notes
- **Depends on:**
  - Brother GameObjects exist in Battle.unity
  - TextMeshPro installed (already present)
- **Impacts:** Brother visuals only
- **Rollback:** Delete NameLabel child objects

#### Validation
✅ **Pass:** Player can name each brother after seeing them once  
❌ **Fail:** Names not visible, wrong names, or unreadable

---

## Wave 2: High-Impact (Fix Soon)

### UX-006: Wire Combat Feedback (ScreenEffectsController)

**Priority:** 4.0 (HIGH)  
**Track Owner:** Track D (UI Juice)  
**Estimated Time:** 15 minutes  
**Risk Level:** 🟢 Low

#### Problem
Actions feel flat - no visual feedback on hits, blocks, damage. Cause-effect disconnected.

#### Root Cause
`ScreenEffectsController` exists and subscribes to events, but UI Image references not wired in inspector.

#### Proposed Fix
1. Verify `ScreenEffectsController` in Battle scene
2. Assign UI Image references in inspector:
   - Vignette Image (for damage flash)
   - Flash Image (for hit flash)
   - Camera reference (for shake)
3. Test each effect triggers correctly

#### Files to Modify
- `Assets/Scenes/Battle.unity` (inspector references only)

#### Required UI Images in Canvas
Ensure these exist in Battle.unity Canvas:
- `VignetteImage` (full-screen Image, red, 0% alpha)
- `FlashImage` (full-screen Image, white, 0% alpha)

If missing, create them:
1. Right-click Canvas → UI > Image
2. Name "VignetteImage"
3. Set color: Red (#8B2020), Alpha: 0
4. Set anchor: Stretch both (full screen)
5. Repeat for FlashImage (White, Alpha: 0)

#### Inspector Wiring Steps
1. Open Battle.unity
2. Locate `ScreenEffectsController` GameObject (or add if missing)
3. In Inspector, assign:
   - `Vignette Image` → VignetteImage
   - `Flash Image` → FlashImage
   - `Camera` → Main Camera
4. Save scene
5. Test in Play mode

#### Acceptance Criteria
- [ ] Hit flash triggers when enemy killed (brief red flash on enemy)
- [ ] Shield flash triggers when attack blocked (white pulse)
- [ ] Damage vignette triggers when player wounded (red edges)
- [ ] Stamina pulse triggers when stamina decreases (blue pulse - if implemented)
- [ ] Camera shake triggers on heavy impact (medium shake)
- [ ] Effects are immediate (< 0.1s delay)
- [ ] Effects are non-intrusive (< 0.3s duration)
- [ ] Multiple effects can overlap

#### Test Steps
1. Open Battle.unity, press Play
2. Complete turn, kill an enemy
3. **Verify:** Brief red flash or shake when enemy dies
4. Next turn, use Block action
5. **Verify:** White flash or pulse when block succeeds
6. Let enemy attack player (take damage)
7. **Verify:** Red vignette flashes at screen edges
8. End turn, stamina decreases
9. **Verify:** Blue pulse or similar feedback (if implemented)
10. Test multiple triggers in one turn (kill + block)
11. **Verify:** Effects stack/overlap appropriately

#### Integration Notes
- **Depends on:**
  - `ScreenEffectsController.cs` (exists)
  - `GameEvents` (OnEnemyKilled, OnAttackBlocked, OnPlayerWounded, etc.)
- **Impacts:** Visual feedback only, no gameplay changes
- **Rollback:** Unassign references in inspector

#### Validation
✅ **Pass:** Player sees clear visual response to every action  
❌ **Fail:** No feedback, or feedback doesn't match events

---

### UX-007: Verify Tutorial Hints Working

**Priority:** 3.0 (HIGH)  
**Track Owner:** Track E (Tutorial)  
**Estimated Time:** 20 minutes  
**Risk Level:** 🟢 Low

#### Problem
New players struggle without hints. Tutorial system exists but may not be wired.

#### Root Cause
`TutorialManager` may not be in scene, or hint assets not assigned, or events not firing.

#### Proposed Fix
1. Verify `TutorialManager` exists in Battle scene
2. Verify hint assets assigned (5 hint ScriptableObjects)
3. Verify TutorialHintUI prefab assigned
4. Test hint triggers in Play mode
5. If broken, fix event subscriptions

#### Files to Verify
- `Assets/Scenes/Battle.unity` (TutorialManager present?)
- `Assets/ScriptableObjects/Tutorial/` (hint assets exist?)
- `Assets/Prefabs/UI/TutorialHintPanel.prefab` (exists?)

#### Verification Steps
1. Open Battle.unity
2. Check hierarchy for `TutorialManager` GameObject
   - If missing, create one and add `TutorialManager.cs` component
3. In Inspector, verify:
   - Hints list has 5 entries (Hint_LockDice, Hint_MatchRunes, etc.)
   - HintUI reference assigned to TutorialHintPanel prefab
4. Press Play
5. **Expected:** "Click dice to lock them" hint appears on first dice roll
6. Lock a die
7. **Expected:** "Locked dice unlock actions" hint appears
8. If hints don't appear, check:
   - Events firing (debug log GameEvents.OnDiceRolled)
   - TutorialManager.OnEnable() subscribed
   - Hint trigger conditions met

#### Hint Trigger Checklist
| Hint | Trigger Event | Trigger Condition | Expected Text |
|------|---------------|-------------------|---------------|
| Hint_LockDice | OnPhaseChanged (DiceRoll) | Wave 1, no dice locked | "Click dice to lock them" |
| Hint_MatchRunes | OnDieLockToggled | Wave 1, first die locked | "Locked dice unlock actions" |
| Hint_Brothers | OnPhaseChanged (EnemyReveal) | Wave 2 | "Brothers will try to block" |
| Hint_Stamina | OnStaminaChanged | Wave 3 | "Stamina drains each turn" |
| Hint_Berserkers | OnPhaseChanged (EnemyReveal) | Wave 4, berserker present | "Berserkers ignore blocks" |

#### Acceptance Criteria
- [ ] TutorialManager exists in Battle scene
- [ ] 5 hint assets assigned in inspector
- [ ] TutorialHintUI prefab assigned
- [ ] First hint appears on Turn 1 (dice roll)
- [ ] Second hint appears on first die lock
- [ ] Hints fade in smoothly (0.3s)
- [ ] Hints auto-dismiss after 5 seconds
- [ ] Hints save to PlayerPrefs (don't repeat on retry)
- [ ] Hints are non-blocking (can play while visible)
- [ ] Can manually dismiss hints (button or timeout)

#### Test Steps
1. Clear PlayerPrefs (Edit > Clear All PlayerPrefs)
2. Open Battle.unity, press Play
3. Wave 1 starts, dice roll
4. **Verify:** "Click dice to lock" hint appears (top or bottom)
5. Wait 5 seconds
6. **Verify:** Hint fades out automatically
7. Lock a die
8. **Verify:** "Locked dice unlock actions" hint appears
9. Manually dismiss (click X or wait)
10. Continue to Wave 2
11. **Verify:** "Brothers block" hint appears
12. Stop Play mode, Press Play again
13. **Verify:** Hints do NOT reappear (saved to PlayerPrefs)

#### Integration Notes
- **Depends on:**
  - `TutorialManager.cs` (exists)
  - `TutorialHintUI.cs` (exists)
  - `TutorialHintSO` assets (5 total in ScriptableObjects/Tutorial/)
  - GameEvents firing correctly
- **Impacts:** Tutorial UX only
- **Rollback:** Disable TutorialManager GameObject

#### Validation
✅ **Pass:** New player sees helpful hints at right moments  
❌ **Fail:** No hints appear, or hints block gameplay, or repeat annoyingly

---

### UX-005: Add Enemy Intent Indicators

**Priority:** 2.5 (HIGH)  
**Track Owner:** Phase5 (UI)  
**Estimated Time:** 30 minutes  
**Risk Level:** 🟡 Medium

#### Problem
Player can't plan defense - no idea who enemy is targeting. Strategy impossible.

#### Root Cause
1. `EnemyIntentIndicator.prefab` doesn't exist
2. `EnemyIntentManager` GameObject not in scene
3. Component code exists but never instantiated

#### Proposed Fix
1. Create `EnemyIntentIndicator.prefab` (simple icon + animation)
2. Add `EnemyIntentManager` to Battle scene
3. Wire to enemy spawn points
4. Test intent displays during PlayerTurn

#### Files to Create
- `Assets/Prefabs/UI/EnemyIntentIndicator.prefab`

#### Files to Modify
- `Assets/Scenes/Battle.unity`

#### Prefab Layout (EnemyIntentIndicator)
```
EnemyIntentIndicator (Prefab - World Space UI)
├── Canvas (World Space, Billboard)
├── IconImage (Image - sword icon)
├── TargetIndicator (Image - red/yellow circle background)
└── Animation (Optional: pulse, bob)
```

#### Intent Icons
- **Target Player:** Red icon, sword symbol
- **Target Brother:** Yellow icon, sword symbol
- **Unblockable:** Red icon, arrow symbol (different from normal)

#### Implementation Steps
1. Create prefab:
   a. Create UI Canvas (World Space)
   b. Add Image for icon (sword sprite - can use Unity default or create)
   c. Add background Image (red/yellow circle)
   d. Position above enemy head (~1.0 units up)
   e. Add Billboard script (always face camera)
2. Add EnemyIntentManager to Battle scene:
   a. Create empty GameObject → name "EnemyIntentManager"
   b. Add `EnemyIntentManager.cs` component
   c. Assign prefab reference
   d. Assign enemy spawn container (parent of enemy GameObjects)
3. Test in Play mode

#### Acceptance Criteria
- [ ] Simple intent icons appear above enemies during PlayerTurn
- [ ] Red icon = targeting player
- [ ] Yellow icon = targeting brother
- [ ] Arrow icon = unblockable attack (if enemy type has this)
- [ ] Icons visible from player camera angle
- [ ] Icons clear after Resolution phase
- [ ] Icons don't overlap with names/health bars
- [ ] Prototype uses simple Unity UI (not 3D models)
- [ ] Icons readable at a glance (< 2 seconds to understand)

#### Test Steps
1. Open Battle.unity, press Play
2. Wave starts, enemies spawn
3. PlayerTurn phase begins
4. **Verify:** Icons appear above enemies
5. **Verify:** Icon color indicates target (red = you, yellow = brother)
6. Hover/inspect enemy
7. **Verify:** Can identify target before locking dice
8. Select actions, confirm turn
9. Resolution phase
10. **Verify:** Icons disappear
11. Next turn
12. **Verify:** Icons reappear with updated targets

#### Integration Notes
- **Depends on:**
  - `EnemyIntentManager.cs` (exists)
  - `EnemyIntentIndicator.cs` (exists)
  - `GameEvents.OnPhaseChanged` (exists)
  - Enemy GameObjects have consistent structure
- **Impacts:** UI only, no gameplay changes
- **Rollback:** Disable EnemyIntentManager GameObject

#### Validation
✅ **Pass:** Player can plan defense based on visible enemy intent  
❌ **Fail:** Icons don't appear, wrong targets, or unreadable

---

### UX-013: Improve Game Over Screen

**Priority:** 2.5 (HIGH)  
**Track Owner:** Track F (Menus)  
**Estimated Time:** 30 minutes  
**Risk Level:** 🟢 Low

#### Problem
Player dies, doesn't know why. Game Over screen lacks context.

#### Root Cause
Game Over UI may not explain defeat reason (stamina exhausted vs health depleted).

#### Proposed Fix
1. Update GameOverUI to show:
   - Defeat reason ("Stamina Exhausted" or "Defeated in Combat")
   - Stats (turns survived, enemies killed, brothers lost)
   - Clear retry/menu buttons
2. Test both defeat conditions

#### Files to Modify
- `Assets/Scripts/UI/GameOverUI.cs` (likely exists, enhance)
- `Assets/Scenes/Battle.unity` (GameOverCanvas prefab/GameObject)

#### Game Over Screen Layout
```
GameOverUI
├── Background (semi-transparent black)
├── TitleText ("DEFEAT" or "VICTORY")
├── ReasonText ("Stamina Exhausted" - large, clear)
├── StatsPanel
│   ├── "Turns Survived: X"
│   ├── "Enemies Killed: X"
│   ├── "Brothers Lost: X"
│   └── "Waves Cleared: X/12"
├── RetryButton ("TRY AGAIN")
├── MainMenuButton ("MAIN MENU")
└── QuitButton ("QUIT")
```

#### Implementation Steps
1. Locate GameOverUI GameObject/prefab
2. Add ReasonText field (TextMeshProUGUI)
3. In `GameOverUI.cs`, enhance `Show()` method:
   ```csharp
   public void Show(DefeatReason reason, BattleStats stats)
   {
       _reasonText.text = reason == DefeatReason.StaminaExhausted 
           ? "STAMINA EXHAUSTED" 
           : "DEFEATED IN COMBAT";
       _turnsSurvivedText.text = $"Turns Survived: {stats.turns}";
       // etc...
   }
   ```
4. Wire up defeat reason detection in TurnManager/BattleManager
5. Test both defeat paths

#### Acceptance Criteria
- [ ] Game Over screen appears on defeat
- [ ] Shows clear defeat reason (stamina vs combat)
- [ ] Shows turns survived count
- [ ] Shows enemies killed count
- [ ] Shows brothers lost count
- [ ] Shows waves cleared count
- [ ] "Try Again" button works (reloads scene)
- [ ] "Main Menu" button works (loads MainMenu)
- [ ] Screen readable (not too dark, good contrast)
- [ ] Player understands why they lost

#### Test Steps
1. Open Battle.unity, press Play
2. Play until stamina reaches 0 (or use debug to set stamina = 0)
3. **Verify:** Game Over appears
4. **Verify:** Shows "STAMINA EXHAUSTED"
5. **Verify:** Shows stats (turns, kills, etc.)
6. Click "Try Again"
7. **Verify:** Scene reloads, starts fresh
8. Play again, let health reach 0 (get hit repeatedly)
9. **Verify:** Game Over shows "DEFEATED IN COMBAT"
10. Click "Main Menu"
11. **Verify:** Loads MainMenu scene

#### Integration Notes
- **Depends on:**
  - `GameOverUI.cs` (likely exists)
  - TurnManager/BattleManager tracking defeat reason
- **Impacts:** Game Over UX only
- **Rollback:** Revert to simpler message

#### Validation
✅ **Pass:** Player can explain why they lost after seeing Game Over  
❌ **Fail:** Reason unclear, stats missing, or buttons broken

---

### UX-012: Brother Block Feedback

**Priority:** 2.0 (HIGH)  
**Track Owner:** Track D (UI Juice)  
**Estimated Time:** 30 minutes  
**Risk Level:** 🟡 Medium

#### Problem
Brother auto-defense is invisible - player doesn't know when brother blocks attack. Brotherhood pillar weakened.

#### Root Cause
No visual or audio feedback when brother successfully auto-defends.

#### Proposed Fix
1. Subscribe to brother block event (create if doesn't exist)
2. Flash brother white when blocks (similar to ScreenEffectsController)
3. Show "BJORN BLOCKED!" text popup (optional)
4. Play block sound effect (deferred to Audio track)

#### Files to Modify
- `Assets/Scripts/Visual/ScreenEffectsController.cs` (add brother flash method)
- OR create new `BrotherFeedbackController.cs`
- `Assets/Scenes/Battle.unity` (add GameObject if new controller)

#### Implementation Steps
1. Check if brother block event exists:
   - Look for `GameEvents.OnBrotherBlocked` or similar
   - If missing, add to `GameEvents.cs`:
     ```csharp
     public static event Action<ShieldBrother> OnBrotherBlocked;
     ```
2. In ShieldBrother.cs `AttemptAutoDefense()`, fire event:
   ```csharp
   if (Random.value < autoDefendChance)
   {
       GameEvents.OnBrotherBlocked?.Invoke(this);
       return true;
   }
   ```
3. In ScreenEffectsController (or new controller), subscribe:
   ```csharp
   void OnEnable() 
   {
       GameEvents.OnBrotherBlocked += FlashBrother;
   }
   void FlashBrother(ShieldBrother brother)
   {
       // Find brother GameObject, flash material white, or
       // Instantiate "BLOCKED!" text popup above brother
   }
   ```
4. Test brother blocking in Play mode

#### Acceptance Criteria
- [ ] Visual feedback triggers when brother blocks (flash white)
- [ ] Can identify which brother blocked (name or position flash)
- [ ] Feedback immediate (< 0.1s)
- [ ] Feedback distinct from other effects
- [ ] Optional: "BLOCKED!" text popup above brother
- [ ] Optional: shield impact sound effect
- [ ] Player notices when brother saves them

#### Test Steps
1. Open Battle.unity, press Play
2. Play turn, let enemies attack
3. If brother auto-defends (50% chance):
   **Verify:** Brother flashes white briefly
   **Verify:** Can see which brother blocked
4. If no block happens, retry (or increase auto-defend chance for testing)
5. Compare brother block to player block (ScreenEffects)
6. **Verify:** Both have feedback but distinguishable

#### Integration Notes
- **Depends on:**
  - `ShieldBrother.cs` (exists)
  - `GameEvents` (may need new event)
  - Brother GameObjects in scene
- **Impacts:** Visual feedback only
- **Risk:** Finding brother GameObject dynamically may fail
- **Rollback:** Remove event subscription

#### Validation
✅ **Pass:** Player says "Oh, my brother saved me!" after block  
❌ **Fail:** Player doesn't notice brother blocking

---

## Summary: Track Assignments

| Track | Issues Assigned | Total Time | Priority |
|-------|-----------------|------------|----------|
| **Phase5 (UI)** | UX-001, UX-002, UX-003, UX-005 | 1h 25min | 🔴 Critical |
| **Track A (Visuals)** | UX-004, UX-011 | 1h 20min | 🔴 Critical |
| **Track D (UI Juice)** | UX-006, UX-012 | 45min | 🟠 High |
| **Track E (Tutorial)** | UX-007 | 20min | 🟠 High |
| **Track F (Menus)** | UX-013 | 30min | 🟠 High |

**Total Top 10 Fix Time:** ~4 hours

**Implementation Order:**
1. **Wave 1 (Critical 5):** UX-001 → UX-002 → UX-003 → UX-004 → UX-011 (~2.5h)
2. **Validate:** Run internal playtest, verify Phase5 gates pass
3. **Wave 2 (High Impact):** UX-006 → UX-007 → UX-005 → UX-013 → UX-012 (~1.5h)
4. **Validate:** Run external micro-playtests (3-5 sessions)
5. **Iterate:** Fix new issues discovered, polish as time allows

---

**Document Status:** ✅ COMPLETE - Ready for Implementation

**Next Step:** Begin Wave 1 fixes (start with UX-001 - easiest/fastest win)

**Validation:** After each wave, update `UX_Fun_Backlog.md` with status changes (🟡 Known → 🔵 In Progress → ✅ Fixed)
