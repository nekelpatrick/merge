# Shield Wall - Weekly Implementation Plan (15-20 Hours)

**Timeline:** Week of December 11-17, 2024  
**Budget:** 15-20 hours focused development  
**Priority:** UX Clarity First → Visual Upgrade Second → Polish Third

---

## Overview

This plan follows the **Phase-Track-Agent Model** with atomic, dependency-ordered steps. Each step includes:
- **Research:** What to examine before implementing
- **Scope:** The ONE thing this step accomplishes
- **Validation:** How to verify it works in isolation
- **Integration:** What events/interfaces connect it

---

## Day 1-2: Quick Win Path (2-4 hours total)

**Goal:** Make the game understandable and immediately more playable

---

### Step 1: Fix Dice Rune Label Display (5 min)

**Research:**
- Read `Assets/Scripts/UI/DieVisual.cs` lines 114-126
- Read `Assets/Scripts/UI/RuneDisplay.cs` lines 8-20
- Confirm DieVisual.GetRuneSymbol() returns codes ("SH", "AX")
- Confirm RuneDisplay.GetFullName() returns full names ("Shield", "Axe")

**Scope:**
Replace DieVisual's local GetRuneSymbol() method to call RuneDisplay.GetFullName()

**Implementation:**
```csharp
// File: Assets/Scripts/UI/DieVisual.cs
// Line 114-126: Replace entire method

private string GetRuneSymbol(RuneType type)
{
    return RuneDisplay.GetFullName(type); // Returns "Shield", "Axe", etc.
}
```

**Validation:**
1. Save script
2. Open Battle scene in Unity
3. Enter Play mode
4. Roll dice
5. Verify dice now show "Shield", "Axe", "Spear", "Brace" (not "SH", "AX", "SP", "BR")

**Integration:**
- No events changed
- DiceUI.cs already calls DieVisual.SetRune()
- Change is transparent to rest of system

**Estimated Time:** 5 minutes

---

### Step 2: Create ActionPreviewItem Prefab (20 min)

**Research:**
- Read `Assets/Scripts/UI/ActionPreviewItem.cs` (full file)
- Note required components: Image (icon), TextMeshProUGUI (name, description), Transform (rune badge container)
- Check existing UI prefabs in `Assets/Prefabs/UI/` for style reference

**Scope:**
Create ActionPreviewItem.prefab with proper UI layout

**Implementation:**

1. **Create GameObject hierarchy:**
   ```
   ActionPreviewItem (GameObject)
   ├─ Background (Image) - Panel style
   ├─ Icon (Image) - Left side, 48×48
   ├─ Content (GameObject) - Vertical layout
   │  ├─ NameText (TextMeshProUGUI) - Bold, 18pt
   │  ├─ RuneBadgeContainer (HorizontalLayoutGroup) - Auto-layout badges
   │  └─ DescriptionText (TextMeshProUGUI) - Regular, 14pt
   └─ ReadyCheckmark (Image) - Top-right, conditional visibility
   ```

2. **Add ActionPreviewItem component:**
   - Drag script onto root GameObject
   - Wire references in inspector:
     - `_iconImage` → Icon
     - `_nameText` → NameText
     - `_descriptionText` → DescriptionText
     - `_runeBadgeContainer` → RuneBadgeContainer
     - `_readyCheckmark` → ReadyCheckmark

3. **Set colors (match UIColorPalette):**
   - Background: #6B4E3D with 80% alpha (Worn Leather)
   - NameText: #D4C8B8 (Bone White)
   - DescriptionText: #A89880 (Muted Tan)

4. **Save prefab:**
   - Save to `Assets/Prefabs/UI/ActionPreviewItem.prefab`

**Validation:**
1. Create test ActionPreviewItem in scene
2. Assign a random ActionSO in inspector
3. Enter Play mode
4. Verify action name, description, rune badges display
5. Delete test instance

**Integration:**
- ActionPreviewUI.cs expects this prefab at runtime
- Will instantiate copies in _previewContainer

**Estimated Time:** 20 minutes

---

### Step 3: Create RuneBadge Prefab (10 min)

**Research:**
- Check ActionPreviewItem.cs for how rune badges are spawned
- Note: badges should show colored icon + rune name

**Scope:**
Create RuneBadge.prefab for displaying individual rune requirements

**Implementation:**

1. **Create GameObject:**
   ```
   RuneBadge (GameObject)
   ├─ Background (Image) - Rounded rect, colored by rune type
   └─ RuneText (TextMeshProUGUI) - Rune symbol or name
   ```

2. **Sizing:**
   - Width: 60-80px
   - Height: 30-40px
   - Padding: 5px

3. **Script (optional):**
   - If ActionPreviewItem needs script to set colors, add simple RuneBadgeUI.cs:
   ```csharp
   public void SetRune(RuneType type)
   {
       _background.color = RuneDisplay.GetDefaultColor(type);
       _text.text = RuneDisplay.GetSymbol(type); // or GetFullName()
   }
   ```

4. **Save prefab:**
   - Save to `Assets/Prefabs/UI/RuneBadge.prefab`

**Validation:**
1. Spawn 3 badges with different rune types
2. Verify colors match Visual Style System
3. Verify symbols/names readable

**Integration:**
- ActionPreviewItem.cs spawns these in RuneBadgeContainer
- Uses RuneDisplay helper for colors/text

**Estimated Time:** 10 minutes

---

### Step 4: Add ActionPreviewUI to Battle Scene (30 min)

**Research:**
- Read `Assets/Scripts/UI/ActionPreviewUI.cs` (full file)
- Note required references: _previewContainer, _previewItemPrefab, _headerText, _emptyStatePanel
- Find Canvas in Battle.unity scene

**Scope:**
Add ActionPreviewUI GameObject to Battle scene and wire references

**Implementation:**

1. **Open Battle scene:**
   - `Assets/Scenes/Battle.unity`

2. **Locate Canvas:**
   - Find existing Canvas GameObject in hierarchy

3. **Create ActionPreviewUI hierarchy:**
   ```
   Canvas/
   └─ ActionPreviewUI (GameObject)
      ├─ ActionPreviewUI component
      ├─ Panel Background (Image)
      ├─ Header (GameObject)
      │  └─ HeaderText (TextMeshProUGUI) - "AVAILABLE ACTIONS"
      ├─ PreviewContainer (GameObject + VerticalLayoutGroup)
      │  └─ (ActionPreviewItem instances spawn here)
      └─ EmptyStatePanel (GameObject)
         └─ EmptyStateText (TextMeshProUGUI) - "Lock dice to ready actions"
   ```

4. **Position ActionPreviewUI:**
   - Anchor: Top-left or center-right
   - Suggested: Right side of screen, below wave counter
   - RectTransform: Width ~300-400px, Height auto-fit

5. **Wire references in ActionPreviewUI inspector:**
   - `_previewContainer` → PreviewContainer
   - `_previewItemPrefab` → ActionPreviewItem prefab (from Step 2)
   - `_headerText` → HeaderText
   - `_emptyStatePanel` → EmptyStatePanel
   - `_emptyStateText` → EmptyStateText

6. **Save scene**

**Validation:**
1. Enter Play mode
2. Start battle
3. Lock 0 dice → Verify "Lock dice to ready actions" message visible
4. Lock 2 Shield dice → Verify action preview shows "Shield Wall" with effect description
5. Lock more dice → Verify preview updates in real-time

**Integration:**
- Subscribes to `GameEvents.OnAvailableActionsChanged`
- Fired by ComboResolver when dice locks change
- Hides during non-PlayerTurn phases automatically

**Estimated Time:** 30 minutes

---

### Step 5: Create PhaseBannerUI Prefab (15 min)

**Research:**
- Read `Assets/Scripts/UI/PhaseBannerUI.cs` (full file)
- Note required: _phaseText, _ctaText, _canvasGroup (for fades)

**Scope:**
Create PhaseBannerUI.prefab with fade-in/out animation

**Implementation:**

1. **Create GameObject hierarchy:**
   ```
   PhaseBannerUI (GameObject + CanvasGroup)
   ├─ PhaseBannerUI component
   ├─ Background (Image) - Dark panel with gradient
   └─ Content (GameObject - VerticalLayoutGroup)
      ├─ PhaseText (TextMeshProUGUI) - Bold, 24pt, uppercase
      └─ CTAText (TextMeshProUGUI) - Regular, 16pt
   ```

2. **Styling:**
   - Background: Dark brown (#2A1F1A) with 90% alpha
   - PhaseText: Gold (#C9A227), bold, centered
   - CTAText: Bone White (#D4C8B8), centered

3. **CanvasGroup:**
   - Set alpha to 0 by default
   - PhaseBannerUI script animates this

4. **Save prefab:**
   - `Assets/Prefabs/UI/PhaseBannerUI.prefab`

**Validation:**
1. Spawn in scene
2. Manually set alpha to 1
3. Verify readable and centered
4. Delete test instance

**Integration:**
- Will be added to Battle scene in next step
- Subscribes to `GameEvents.OnPhaseChanged`

**Estimated Time:** 15 minutes

---

### Step 6: Add PhaseBannerUI to Battle Scene (20 min)

**Research:**
- Confirm PhaseBannerUI prefab ready (Step 5)
- Find Canvas in Battle.unity

**Scope:**
Add PhaseBannerUI to Battle scene, position, wire references

**Implementation:**

1. **Open Battle scene**

2. **Add PhaseBannerUI to Canvas:**
   - Drag PhaseBannerUI prefab into Canvas hierarchy
   - Position: Top-center of screen
   - Anchor: Top stretch
   - RectTransform: Width 600-800px, Height 100-150px
   - Offset from top: 20-50px

3. **Wire references in PhaseBannerUI inspector:**
   - `_phaseText` → PhaseText child
   - `_ctaText` → CTAText child
   - `_canvasGroup` → CanvasGroup on root

4. **Adjust settings:**
   - `_fadeInDuration` = 0.3
   - `_displayDuration` = 2.0
   - Verify phase messages correct (or leave defaults)

5. **Save scene**

**Validation:**
1. Enter Play mode
2. Start battle
3. Verify banner fades in with "ENEMIES APPROACH - Prepare to defend!"
4. Wait for PlayerTurn
5. Verify banner shows "YOUR TURN - Lock dice to ready actions, then confirm"
6. Confirm action
7. Verify banner shows "RESOLVING - Actions executing..."

**Integration:**
- Subscribes to `GameEvents.OnPhaseChanged`
- Fired by TurnManager on every phase transition
- No other systems need to know about it

**Estimated Time:** 20 minutes

---

### Step 7: Wire ScreenEffectsController References (15 min)

**Research:**
- Read `Assets/Scripts/Visual/ScreenEffectsController.cs` lines 1-60
- Note required Image references: _vignetteImage, _flashImage, _staminaPulseImage
- Check Battle scene for existing ScreenEffectsController

**Scope:**
Ensure ScreenEffectsController has all UI Image references wired

**Implementation:**

1. **Open Battle scene**

2. **Find/Create ScreenEffects UI:**
   - Locate Canvas
   - Find or create GameObject: `ScreenEffects`
   - Add child Images:
     ```
     Canvas/
     └─ ScreenEffects (GameObject)
        ├─ VignetteImage (Image) - Fullscreen, vignette texture, alpha 0
        ├─ FlashImage (Image) - Fullscreen, white color, alpha 0
        └─ StaminaPulseImage (Image) - Fullscreen, blue tint, alpha 0
     ```

3. **Configure Images:**
   - All Images: Raycast Target = OFF (don't block clicks)
   - All Images: Anchor to full screen (stretch-stretch)
   - VignetteImage: Sprite = vignette texture (or use color gradient)
   - FlashImage: Color = white
   - StaminaPulseImage: Color = #3D5A6E (Iron Blue)

4. **Find ScreenEffectsController GameObject:**
   - Should already exist in scene (check hierarchy)
   - If missing: Create empty GameObject `ScreenEffectsController`, add script

5. **Wire references:**
   - `_cameraTransform` → Main Camera (auto-finds if null)
   - `_vignetteImage` → VignetteImage
   - `_flashImage` → FlashImage
   - `_staminaPulseImage` → StaminaPulseImage

6. **Save scene**

**Validation:**
1. Enter Play mode
2. Take damage → Verify red vignette flashes
3. Kill enemy → Verify white flash
4. Turn ends → Verify blue stamina pulse (if subscribed to OnStaminaChanged)
5. Block attack → Verify camera shake

**Integration:**
- Already subscribes to all combat events
- Images now referenced, effects will trigger

**Estimated Time:** 15 minutes

---

### Step 8: Run 3D Asset Creator (5 min execution + 30 min material assignment)

**Research:**
- Check `Assets/Editor/ShieldWallSceneBuilder.cs` has `Create All 3D Assets (One-Click)` menu item
- Confirm it creates: Limb prefabs, Toon materials, Environment props, Blood VFX

**Scope:**
Execute editor script to generate all 3D visual assets

**Implementation:**

1. **Open Unity Editor**

2. **Run menu item:**
   - Menu: `Shield Wall Builder > 3D Assets > Create All 3D Assets (One-Click)`
   - Wait for console messages confirming creation

3. **Verify outputs:**
   - Check `Assets/Prefabs/Gore/` now has:
     - SeveredHead.prefab
     - SeveredArm.prefab
     - SeveredLeg.prefab
   - Check `Assets/Art/Materials/Characters/` has:
     - M_Character_Player.mat
     - M_Character_Brother.mat
     - M_Character_Enemy.mat
   - Check `Assets/Prefabs/VFX/` has blood VFX prefabs

4. **Assign materials to existing characters:**
   - Open Battle scene
   - Find brother GameObjects
   - For each brother body mesh:
     - Assign M_Character_Brother material
   - Find enemy GameObjects
   - For each enemy body mesh:
     - Assign M_Character_Enemy material
   - Find player shield:
     - Assign M_Character_Player material

5. **Adjust material colors (if needed):**
   - M_Character_Brother: Earthy browns/grays
   - M_Character_Enemy: Darker browns, slightly red tint
   - M_Character_Player: Iron Gray for shield

6. **Save scene**

**Validation:**
1. Enter Play mode
2. Verify brothers no longer plain brown capsules
3. Verify enemies visually distinct from brothers
4. Verify toon shader visible (cel-shaded bands)

**Integration:**
- Materials applied to existing MeshRenderer components
- No code changes needed
- Visual upgrade is purely Inspector-level

**Estimated Time:** 5 min execution + 30 min tweaking = 35 minutes

---

### Step 9: Test & Validate Quick Win Path (30 min)

**Research:**
- Review Phase5_UXSuccessCriteria.md checklist
- Compare before/after screenshots

**Scope:**
Full playtest of 3 turns, verify all Quick Win fixes working

**Implementation:**

**Playtest Script:**
1. Open Battle scene
2. Enter Play mode
3. Start battle (Wave 1)

**Turn 1 - First Impressions:**
- ✅ Dice show "Shield", "Axe", "Spear", "Brace" (not codes)
- ✅ Phase banner shows "YOUR TURN - Lock dice to ready actions, then confirm"
- ✅ Lock 2 Shield dice
- ✅ Action preview panel appears showing "Shield Wall (READY)"
- ✅ Read effect: "Block 1 attack on you"
- ✅ Select action, confirm
- ✅ See hit flash when enemy killed (if applicable)
- ✅ See banner change to "TURN COMPLETE"

**Turn 2 - Understanding:**
- ✅ Try different dice combos
- ✅ Action preview updates in real-time
- ✅ Phase guidance clear at each step

**Turn 3 - Confidence:**
- ✅ Execute turn without confusion
- ✅ Game feels more strategic
- ✅ Want to continue playing

**Bug Reporting:**
- Note any issues in console
- Screenshot any visual glitches
- Document confusing moments

**Integration:**
- No integration in this step - pure validation

**Estimated Time:** 30 minutes

---

### Quick Win Path Total: **~2.5 hours**

**Deliverable:** Game is understandable, runes clear, action preview working, phase guidance visible, characters slightly less ugly.

---

## Day 3-4: Full Phase 5 Integration (6-8 hours total)

**Goal:** Complete all Phase 5 UX success criteria

---

### Step 10: Create EnemyIntentIndicator Prefab (10 min)

**Research:**
- Read `Assets/Scripts/UI/EnemyIntentIndicator.cs`
- Note required: Icon image, optional text

**Scope:**
Create EnemyIntentIndicator prefab for above-enemy-head icons

**Implementation:**

1. **Create World Space Canvas prefab:**
   ```
   EnemyIntentIndicator (GameObject + Canvas)
   ├─ Canvas component:
   │  - Render Mode: World Space
   │  - Sorting Layer: UI or Overlay
   ├─ EnemyIntentIndicator component
   ├─ Background (Image) - Circle or square, dark background
   └─ IntentIcon (Image) - Sword/arrow/shield icon
   ```

2. **Sizing:**
   - Canvas size: 1×1 world units
   - Icon: 0.8×0.8 within canvas
   - Scale: 0.5-1.0 depending on distance

3. **Icons needed:**
   - Sword icon (normal attack)
   - Arrow icon (unblockable attack)
   - Color coding: Red = targets player, Yellow = targets brother

4. **Add EnemyIntentIndicator component:**
   - Wire `_iconImage` reference

5. **Save prefab:**
   - `Assets/Prefabs/UI/EnemyIntentIndicator.prefab`

**Validation:**
1. Spawn in scene
2. Position above test enemy
3. Call SetIntent() manually
4. Verify icon displays

**Integration:**
- EnemyIntentManager spawns these at runtime
- Positioned via enemy.transform.position + Vector3.up

**Estimated Time:** 10 minutes

---

### Step 11: Add EnemyIntentManager to Battle Scene (30 min)

**Research:**
- Read `Assets/Scripts/UI/EnemyIntentManager.cs`
- Understand how it finds enemies and spawns indicators

**Scope:**
Add EnemyIntentManager GameObject to Battle scene, wire prefab reference

**Implementation:**

1. **Open Battle scene**

2. **Create EnemyIntentManager GameObject:**
   - Create empty GameObject: `EnemyIntentManager`
   - Add EnemyIntentManager component
   - Position: Doesn't matter (it's a manager, not visual)

3. **Wire references:**
   - `_intentIndicatorPrefab` → EnemyIntentIndicator prefab (Step 10)
   - `_iconSprites` → Array of icon sprites (sword, arrow, etc.)
   - If sprites don't exist, use simple colored squares for prototype

4. **Save scene**

**Validation:**
1. Enter Play mode
2. Wait for enemies to spawn
3. Enter PlayerTurn phase
4. Verify intent icons appear above enemies
5. Verify icons color-coded (red = player, yellow = brother)
6. Confirm action, enter Resolution
7. Verify icons disappear

**Integration:**
- Subscribes to `GameEvents.OnPhaseChanged`
- Subscribes to `GameEvents.OnEnemyWaveSpawned`
- Finds all Enemy instances via tag or manager
- Spawns indicators dynamically

**Estimated Time:** 30 minutes

---

### Step 12: Create Intent Icon Sprites (20 min)

**Research:**
- Check what icons EnemyIntentManager expects
- Typical intents: Attack (sword), Unblockable (arrow), Defend (shield)

**Scope:**
Create or source simple icon sprites for enemy intents

**Implementation:**

**Option A: Create in Unity (simple shapes):**
1. Create 64×64 textures in image editor or use Unity primitive textures
2. Sword: Triangle pointing down
3. Arrow: Arrow pointing forward
4. Shield: Circle or rounded square

**Option B: Use Asset Store:**
1. Search Unity Asset Store for "UI icons" (many free packs)
2. Import pack
3. Assign sword/arrow/shield sprites

**Option C: Quick prototype (colored squares):**
1. Use Unity default white square
2. Tint via Image.color:
   - Red = attack player
   - Yellow = attack brother
   - Blue = defend/other

4. **Save sprites to:**
   - `Assets/Art/UI/Icons/` folder

5. **Assign to EnemyIntentManager:**
   - Open Battle scene
   - Select EnemyIntentManager
   - Drag sprites into `_iconSprites` array

**Validation:**
1. Enter Play mode
2. Verify icons appear and are readable

**Integration:**
- EnemyIntentManager.SetIntent() uses these sprites

**Estimated Time:** 20 minutes (Option A or C)

---

### Step 13: Enable Dismemberment System (30 min)

**Research:**
- Grep for `DismembermentController` script
- Check if it exists and what it requires
- Review Visual Style System doc Section 7 (Dismemberment)

**Scope:**
Verify dismemberment triggers on enemy kills, spawns limbs and blood

**Implementation:**

1. **Check if DismembermentController exists:**
   - Search: `Assets/Scripts/Visual/DismembermentController.cs`
   - If missing: Implement basic version (see below)

2. **If exists:**
   - Open Battle scene
   - Find or create `DismembermentController` GameObject
   - Add component if not present
   - Wire references:
     - `_limbPrefabs` → SeveredHead, SeveredArm, SeveredLeg (from Step 8)
     - `_bloodBurstPrefab` → Blood particle prefab (from Step 8)
     - `_bloodDecalPrefab` → Blood decal prefab (from Step 8)

3. **Subscribe to events:**
   - If not already, ensure OnEnable subscribes to `GameEvents.OnEnemyKilled`

4. **Test kill types:**
   - Map ActionSO to dismemberment type:
     - Counter (Axe+Shield) → Decapitation (head)
     - Strike (Axe+Spear) → Random (head or arm)
     - Berserker (3×Axe) → Multiple limbs

5. **Fallback if DismembermentController missing:**
   - Simple version:
   ```csharp
   void OnEnable() {
       GameEvents.OnEnemyKilled += HandleEnemyKilled;
   }
   void HandleEnemyKilled(EnemySO enemy) {
       // Spawn limb at enemy position
       Instantiate(_limbPrefab, enemy.transform.position, Random.rotation);
       // Spawn blood burst
       Instantiate(_bloodBurst, enemy.transform.position, Quaternion.identity);
   }
   ```

6. **Save scene**

**Validation:**
1. Enter Play mode
2. Kill enemy with Strike action
3. Verify limb flies off with physics
4. Verify blood particle burst
5. Verify blood decal on ground

**Integration:**
- Subscribes to `GameEvents.OnEnemyKilled`
- Visual-only system, no gameplay impact

**Estimated Time:** 30 minutes

---

### Step 14: Tutorial Hints Integration (20 min)

**Research:**
- Check `Assets/Scripts/Tutorial/TutorialManager.cs`
- Confirm tutorial hint assets exist in `ScriptableObjects/Tutorial/`
- Review TutorialHintUI prefab in `Prefabs/UI/`

**Scope:**
Wire TutorialManager to show contextual hints on first occurrences

**Implementation:**

1. **Open Battle scene**

2. **Find/Create TutorialManager GameObject:**
   - Should already exist (check hierarchy)
   - If missing: Create empty GameObject `TutorialManager`, add component

3. **Assign hint assets:**
   - `_hints` array in inspector:
     - Hint_LockDice (triggers on first dice roll)
     - Hint_MatchRunes (triggers on first lock)
     - Hint_Stamina (triggers on first stamina change)
     - Hint_Brothers (triggers on first brother wounded)
     - Hint_Berserkers (triggers on first berserker spawn)

4. **Verify TutorialHintUI prefab wired:**
   - `_hintPrefab` → TutorialHintPanel.prefab
   - `_hintContainer` → Canvas child for spawning hints

5. **Adjust settings:**
   - `_hintDisplayDuration` = 3 seconds (or 0 = dismiss manually)
   - `_showOnlyOnce` = true

6. **Save scene**

**Validation:**
1. Enter Play mode (fresh, no PlayerPrefs set)
2. Roll dice → Verify "Click dice to lock them for combos" hint appears
3. Lock dice → Verify "Locked dice unlock actions" hint appears
4. Take damage → Verify stamina hint appears
5. Subsequent turns → Verify hints don't repeat

**Integration:**
- TutorialManager subscribes to relevant GameEvents
- Shows hints via TutorialHintUI.ShowHint()
- Stores shown state in PlayerPrefs

**Estimated Time:** 20 minutes

---

### Step 15: Visual Tuning Pass (1-2 hours)

**Research:**
- Review Visual Style System color palette
- Check current UI spacing, font sizes
- Test at different resolutions

**Scope:**
Adjust colors, spacing, font sizes for visual consistency and readability

**Implementation:**

1. **UI Color Pass:**
   - Verify all UI panels use #6B4E3D (Worn Leather) with 80% alpha
   - Verify all primary text uses #D4C8B8 (Bone White)
   - Verify all secondary text uses #A89880 (Muted Tan)
   - Verify action buttons highlight with #C9A227 (Gold) when selected

2. **Font Size Pass:**
   - Phase banner: 24pt (phase) + 16pt (CTA)
   - Action preview: 18pt (name) + 14pt (description)
   - Dice labels: 16-20pt (depending on space)
   - Health/stamina UI: 14-16pt

3. **Spacing Pass:**
   - Ensure 10-20px padding inside all panels
   - Ensure 5-10px spacing between UI elements
   - Ensure action preview items have 5px vertical gap

4. **Lighting Adjustment:**
   - Open Battle scene
   - Adjust Directional Light:
     - Angle: ~45° down, slightly angled for drama
     - Intensity: 0.8-1.0
     - Color: Slight warm tint (#FFF8E0)
   - Adjust fog:
     - Density: 0.01-0.02
     - Color: Match skybox low-end color

5. **Camera Adjustment:**
   - FOV: 70-75° (claustrophobic feel)
   - Position: Y = 1.7 (eye height)
   - Rotation: Slight downward tilt (5-10°)

6. **Test Resolutions:**
   - 1920×1080 (primary)
   - 1280×720 (common low-end)
   - 2560×1440 (high-end)
   - Verify UI readable at all sizes
   - Adjust anchors/scaling if needed

7. **Save scene**

**Validation:**
- Play 1 full turn at each resolution
- Verify no text cutoff
- Verify clickable areas not overlapping
- Screenshot each resolution for comparison

**Integration:**
- Visual only, no code changes

**Estimated Time:** 1-2 hours

---

### Step 16: Full Phase 5 Playtest Validation (1 hour)

**Research:**
- Read `Phase5_UXSuccessCriteria.md` full checklist
- Prepare to test all criteria systematically

**Scope:**
Complete validation of all Phase 5 success criteria

**Implementation:**

**Validation Checklist:**

**1. Rune Clarity ✓**
- [ ] Rune full names visible: "Shield", "Axe", "Spear", "Brace"
- [ ] Rune colors match Visual Style System
- [ ] Dice visuals use color + name (not codes)
- [ ] Action requirements show colored rune badges

**2. Action Preview System ✓**
- [ ] Action preview panel visible during PlayerTurn
- [ ] Shows ALL available actions from locked dice
- [ ] Displays action name, rune cost (badges), effect description
- [ ] Updates in real-time as dice locked/unlocked
- [ ] Shows "unmet" state for incomplete actions

**3. Phase Guidance & CTAs ✓**
- [ ] Phase banner visible at phase transitions
- [ ] WaveStart: "Enemies approach! Prepare to defend!"
- [ ] PlayerTurn: "Lock dice to ready actions, then confirm"
- [ ] Resolution: "Resolving actions..."
- [ ] WaveEnd: "Turn complete. -1 Stamina"

**4. Inline Tutorial Hints ✓**
- [ ] Hint on first roll: "Click dice to lock them"
- [ ] Hint on first lock: "Locked dice unlock actions"
- [ ] Hint on first action: "Select actions, then confirm"
- [ ] Hint on first damage: "Stamina is your doom clock"
- [ ] Hint on brother wounded: "Shield your brothers"
- [ ] Hints dismissible and don't repeat

**5. Enemy Intent Display ✓**
- [ ] Intent icons appear above enemies during PlayerTurn
- [ ] Icons show target (red = player, yellow = brother)
- [ ] Icons show type (sword = normal, arrow = unblockable)
- [ ] Icons clear after Resolution

**6. Combat Feedback ✓**
- [ ] Hit flash when enemy killed
- [ ] Camera shake when attack blocked
- [ ] Stamina pulse when stamina decreases
- [ ] Damage vignette when player wounded

**Bug Tracking:**
- Create list of any bugs encountered
- Note performance issues (FPS drops, stutters)
- Document unclear/confusing moments

**Integration:**
- This step purely validates, no implementation

**Estimated Time:** 1 hour

---

### Full Phase 5 Integration Total: **~6-8 hours**

**Deliverable:** All Phase 5 UX success criteria met, game is intuitive and strategic.

---

## Day 5-6: Visual Upgrade Path (6-8 hours total)

**Goal:** Replace primitives with low-poly models, add atmosphere, polish visuals

---

### Step 17: Character Model Research & Sourcing (1 hour)

**Research:**
- Search Unity Asset Store for:
  - "Low poly Viking"
  - "Low poly warrior"
  - "Stylized medieval characters"
- Criteria:
  - Low poly count (< 5000 tris per character)
  - Toon/stylized aesthetic compatible
  - Includes basic animations (idle, attack, death)
  - Preferably modular (swappable heads/limbs for dismemberment)

**Scope:**
Source or create low-poly character models for brothers and enemies

**Implementation:**

**Option A: Asset Store (Recommended):**
1. Find suitable pack (e.g., Synty Polygon Vikings, Quaternius Medieval)
2. Purchase/download (budget: $0-30)
3. Import into project
4. Extract models to `Assets/Art/Models/Characters/`

**Option B: Create in Blender (Advanced):**
1. Model simple capsule-based Viking characters
2. Add cube head, cylindrical limbs
3. UV unwrap for toon textures
4. Export as .fbx

**Option C: Upgrade Primitives (Quick):**
1. Keep capsule bodies
2. Add stylized heads (spheres with texture)
3. Add shield/weapon meshes
4. Apply toon materials
5. This is the fastest "better than primitives" option

**Deliverable:**
- Brother model prefab
- Enemy model prefab (or 3 variants)

**Validation:**
- Import test model into scene
- Apply toon material
- Verify renders correctly

**Integration:**
- Models will replace existing primitive capsules in next step

**Estimated Time:** 1 hour (Option A or C)

---

### Step 18: Replace Brother Primitives with Models (2 hours)

**Research:**
- Locate current brother GameObjects in Battle scene
- Note how BrotherVisualController currently creates primitives

**Scope:**
Replace primitive capsule brothers with low-poly models

**Implementation:**

1. **Open Battle scene**

2. **For each brother position:**
   - Delete primitive capsule body
   - Instantiate new brother model prefab
   - Position at same location
   - Assign M_Character_Brother material (from Step 8)

3. **Update BrotherVisualController.cs (if needed):**
   - If script spawns primitives procedurally, modify to spawn prefab instead:
   ```csharp
   // Old:
   GameObject bodyGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
   
   // New:
   GameObject bodyGO = Instantiate(_brotherModelPrefab, position, rotation);
   ```

4. **Assign individual brother customization:**
   - Bjorn (Far Left): Red cape or helmet
   - Erik (Left): Shield + spear
   - Gunnar (Right): Axe + no shield
   - Olaf (Far Right): Extra armor plates

5. **Adjust scale if needed:**
   - Target: 1.8-2.0 units tall (average human height)

6. **Test dismemberment compatibility:**
   - Ensure models have separable head/limbs
   - If not modular, limb prefabs spawn as separate objects (existing approach)

7. **Save scene**

**Validation:**
1. Enter Play mode
2. Verify all 4 brothers visible
3. Verify visually distinct from each other
4. Verify toon material applied correctly
5. Take damage to brother → verify visual stays correct

**Integration:**
- BrotherVisualController may need prefab reference
- ShieldWallManager.cs unchanged (only visuals)

**Estimated Time:** 2 hours

---

### Step 19: Replace Enemy Primitives with Models (2 hours)

**Research:**
- Similar to Step 18 but for enemies
- Note: 6 enemy types, may need variants

**Scope:**
Replace primitive sphere enemies with low-poly models

**Implementation:**

1. **Open Battle scene**

2. **Create enemy model variants:**
   - Thrall: Basic enemy, minimal armor
   - Warrior: Shield + sword
   - Spearman: Spear, leather armor
   - Berserker: Bare-chested, dual axes
   - Archer: Bow, quiver
   - ShieldBreaker: Large hammer

3. **If using single base model:**
   - Create prefab variants with different weapons/armor
   - Save as: Enemy_Thrall.prefab, Enemy_Warrior.prefab, etc.

4. **Apply M_Character_Enemy material:**
   - Darker brown/gray than brothers
   - Slight red or green tint to distinguish

5. **Update EnemyVisualInstance.cs:**
   - If spawns primitives, replace with prefab instantiation
   - Map EnemySO.enemyType to correct prefab variant

6. **Adjust enemy positions:**
   - Ensure enemies visible in first-person view
   - Typically Z = 4-6 units ahead of player

7. **Save scene**

**Validation:**
1. Enter Play mode
2. Spawn wave 1 (thralls)
3. Verify enemies visible and distinct from brothers
4. Kill enemy → verify dismemberment still works
5. Spawn wave 2+ → verify enemy variety visible

**Integration:**
- EnemyWaveController spawns EnemySO instances
- EnemyVisualInstance handles model instantiation

**Estimated Time:** 2 hours

---

### Step 20: Environment & Atmosphere Pass (1-2 hours)

**Research:**
- Review Visual Style System atmospheric settings
- Check current fog/lighting in Battle scene

**Scope:**
Add ground texture, adjust fog, tune lighting for oppressive Viking mood

**Implementation:**

1. **Ground Plane:**
   - If not exists: Create plane at Y = 0
   - Scale: 20×20 or larger
   - Material: M_Environment_Ground (from Step 8) or create mud texture
   - Texture: Dark brown, muddy, subtle normal map

2. **Fog Adjustment:**
   - Open Lighting window
   - Fog enabled: Yes
   - Fog Mode: Exponential
   - Fog Density: 0.015-0.025 (thick but not obscuring)
   - Fog Color: #1A1A1C (dark blue-gray)

3. **Skybox:**
   - If using gradient skybox (current):
     - Top: #3D5A6E (Iron Blue)
     - Bottom: #2A1F1A (Dark Brown)
   - Alternative: Find/create painted overcast sky texture

4. **Directional Light:**
   - Color: Slight warm (#FFF8E0)
   - Intensity: 0.7-0.9 (moody, not bright)
   - Angle: 45° from above, 30° from side (dramatic shadows)

5. **Post-Processing (if URP Volume exists):**
   - Color Grading:
     - Lift: Slightly blue/cold
     - Gamma: Neutral
     - Gain: Slightly warm (contrast)
   - Vignette:
     - Intensity: 0.2-0.3 (subtle edge darkening)
     - Smoothness: 0.4
   - Ambient Occlusion:
     - Enable if performance allows
     - Intensity: 0.5

6. **Test visibility:**
   - Ensure brothers/enemies still clearly visible
   - Fog should add mood, not obscure gameplay

7. **Save scene**

**Validation:**
1. Enter Play mode
2. Verify oppressive, Viking atmosphere
3. Verify characters stand out against environment
4. Check performance (fog can be expensive)

**Integration:**
- Scene-level settings, no code changes

**Estimated Time:** 1-2 hours

---

### Step 21: Blood & Gore Polish (1 hour)

**Research:**
- Check dismemberment working (Step 13)
- Review blood VFX prefabs (Step 8)

**Scope:**
Tune blood decals, particle bursts, screen blood for maximum impact

**Implementation:**

1. **Blood Decal Tuning:**
   - Open blood decal prefabs
   - Ensure 3 splatter texture variants
   - Adjust size: Random 0.5-2.0 scale
   - Adjust rotation: Random 0-360°
   - Lifetime: 30 seconds or persist until wave end

2. **Blood Burst Particle Tuning:**
   - Open blood burst prefab
   - Particle System settings:
     - Emission: Burst 50-100 particles
     - Shape: Cone, angle 30°, toward camera
     - Start color: #6B1010 (deep red)
     - Start size: Random 0.05-0.2
     - Start lifetime: 0.5-1.5 seconds
     - Gravity: -9.8 (realistic fall)
     - Collision: Enabled, spawn decal on impact

3. **Screen Blood Overlay:**
   - Verify ScreenEffectsController has blood vignette
   - Test on player damage
   - Adjust alpha: 0.3-0.5 (visible but not blinding)
   - Adjust fade duration: 3-5 seconds

4. **Gore Sound Integration:**
   - If AudioManager exists:
     - Assign flesh_slice.wav to OnEnemyKilled
     - Assign bone_crack.wav to dismemberment
     - Assign blood_splat.wav to blood burst
   - Volume: 0.5-0.7 (audible but not overwhelming)

5. **Save all prefabs and scene**

**Validation:**
1. Enter Play mode
2. Kill enemy with Counter (decapitation)
3. Verify:
   - Head flies off with physics
   - Blood burst sprays toward camera
   - Blood decals appear on ground
   - Flesh slice sound plays
   - No performance drop

**Integration:**
- DismembermentController triggers all effects
- ScreenEffectsController handles screen blood

**Estimated Time:** 1 hour

---

### Step 22: Camera & First-Person Composition (30 min)

**Research:**
- Review GDD Section 8 - First-Person View
- Check current camera position/FOV

**Scope:**
Adjust camera for optimal first-person shield wall framing

**Implementation:**

1. **Open Battle scene**

2. **Main Camera adjustments:**
   - Position: (0, 1.7, 0) - Eye height
   - Rotation: (5, 0, 0) - Slight downward tilt to see ground
   - FOV: 70-75° - Wider for claustrophobic feel
   - Near clip: 0.1 - Allow close weapon rendering
   - Far clip: 50-100 - Fog handles distance

3. **Player Shield Positioning:**
   - Shield mesh at (-0.3, 1.2, 0.5) - Lower-left screen
   - Rotation: (-10, 15, 0) - Angled naturally
   - Scale: Visible but not obscuring

4. **Brother Positioning (relative to camera):**
   - Far Left: (-2.5, 0, 2)
   - Left: (-1.2, 0, 1.5)
   - Right: (1.2, 0, 1.5)
   - Far Right: (2.5, 0, 2)
   - These should be at edge of screen, partially visible

5. **Enemy Grid Positioning:**
   - Z range: 4-6 units ahead
   - X range: -2 to 2
   - Spawn in visible cone

6. **Test composition:**
   - Brothers at screen edges (peripheral vision)
   - Enemies ahead (center focus)
   - Shield visible at screen bottom
   - Ground visible but not dominating

7. **Save scene**

**Validation:**
1. Enter Play mode
2. Verify first-person view feels claustrophobic
3. Verify all characters visible
4. Verify shield not blocking UI

**Integration:**
- Camera settings only, no code

**Estimated Time:** 30 minutes

---

### Step 23: Final Visual Playtest (1 hour)

**Research:**
- Review all visual upgrade goals
- Compare before/after screenshots

**Scope:**
Full visual validation playtest

**Implementation:**

**Playtest Script:**
1. Play 5 full turns (Wave 1-2)
2. Test all actions (Strike, Block, Cover, etc.)
3. Test enemy kills (verify dismemberment)
4. Test brother damage (verify visual states)
5. Test lighting at different times

**Validation Checklist:**

**Visual Quality ✓**
- [ ] Brothers visually distinct from enemies
- [ ] Characters use models (not bare primitives)
- [ ] Toon materials applied (cel-shaded look)
- [ ] Ground plane visible with texture
- [ ] Fog/atmosphere creates Viking mood

**Combat Feedback ✓**
- [ ] Hit flash on kill
- [ ] Camera shake on hit
- [ ] Blood spray on dismemberment
- [ ] Blood decals on ground
- [ ] Screen blood on player damage

**Composition ✓**
- [ ] First-person view immersive
- [ ] Brothers visible at screen edges
- [ ] Enemies clearly visible ahead
- [ ] Shield visible at screen bottom
- [ ] UI not blocked by 3D elements

**Performance ✓**
- [ ] 60 FPS on target hardware
- [ ] No stutters during combat
- [ ] Fog not killing performance
- [ ] VFX not lagging

**Aesthetic Match ✓**
- [ ] Matches Visual Style System doc
- [ ] Colors match palette (Iron Gray, Blood Red, etc.)
- [ ] Oppressive Darkest Dungeon mood
- [ ] Gritty Kingdom Come realism
- [ ] Stylized Banner Saga art

**Screenshot Comparison:**
- Take screenshot of same scene before/after
- Document improvements

**Bug Reporting:**
- List any visual glitches
- Note missing textures/materials
- Document performance issues

**Integration:**
- Validation only

**Estimated Time:** 1 hour

---

### Visual Upgrade Path Total: **~6-8 hours**

**Deliverable:** Game looks like Visual Style System mockups, characters are low-poly 3D models, atmosphere is oppressive Viking mood.

---

## Week Total Time Budget

| Path | Hours | Cumulative |
|------|-------|------------|
| Quick Win (Days 1-2) | 2-4 | 2-4 |
| Full Phase 5 (Days 3-4) | 6-8 | 8-12 |
| Visual Upgrade (Days 5-6) | 6-8 | 14-20 |

**Total:** 14-20 hours (within budget)

---

## Parallel Track Execution (Optional)

If you want to work in parallel (e.g., two AI agents or alternating tasks):

### Track A: UX Clarity (Steps 1-6, 10-16)
- Focus: Phase 5 integration
- Time: 8-10 hours
- Output: Game is intuitive

### Track B: Visual Upgrade (Steps 8, 17-23)
- Focus: 3D models and atmosphere
- Time: 6-8 hours
- Output: Game looks good

### Track C: Polish (Steps 7, 9, 15, 21-23)
- Focus: Feedback, tuning, validation
- Time: 3-5 hours
- Output: Game feels good

**These can run in parallel** since they touch different files/systems.

---

## Success Criteria (End of Week)

### Phase 5 UX Complete ✅
- All Phase5_UXSuccessCriteria.md items checked
- New player completes first turn without help
- Action preview working
- Phase guidance clear
- Enemy intent visible

### Visual Upgrade Complete ✅
- Characters use 3D models (not primitives)
- Brothers distinct from enemies
- Toon materials applied
- Atmospheric fog and lighting
- Blood/gore on kills

### Fun Factor ✅
- Player wants "just one more turn"
- Combat feels satisfying (feedback + gore)
- Strategy is clear (can plan defense)
- Game looks like a real game

---

## Next Steps After This Week

If time remains or next sprint:

1. **Content Expansion**
   - Add more enemy variants
   - Create more actions/combos
   - Build additional scenarios

2. **Audio Pass**
   - Add combat sounds (Phase 3 Track D)
   - Music loop
   - Voice lines (optional)

3. **Progression System**
   - Between-battle upgrades
   - Brother recruitment
   - Dice upgrades

4. **Balancing**
   - Playtest tuning
   - Difficulty curve adjustment
   - Action cost/effect balancing

---

## Emergency Scope Cuts

If running out of time, cut in this order:

1. ❌ EnemyIntentManager (Step 10-12) - Nice but not critical
2. ❌ Tutorial hints (Step 14) - Can add later
3. ❌ Character model replacement (Steps 17-19) - Keep upgraded primitives
4. ❌ Blood/gore polish (Step 21) - Basic dismemberment enough
5. ⚠️ Do NOT cut: ActionPreviewUI, PhaseBannerUI, dice label fix (core UX)

---

## Validation Gates

**Before proceeding to next day, verify:**

### After Day 2 (Quick Win):
- ✅ Dice show full rune names
- ✅ Action preview updates on dice lock
- ✅ Phase banner shows guidance
- ✅ Characters slightly less ugly

### After Day 4 (Full Phase 5):
- ✅ All Phase 5 UX criteria met
- ✅ Enemy intent visible
- ✅ Combat feedback working
- ✅ Tutorial hints triggering

### After Day 6 (Visual Upgrade):
- ✅ 3D models replacing primitives
- ✅ Atmosphere oppressive
- ✅ Blood/gore satisfying
- ✅ Screenshot looks like real game

---

**Status:** 📋 PLAN READY FOR EXECUTION

**Recommendation:** Start with Quick Win Path (Steps 1-9), validate, then proceed to Full Phase 5 if time allows.

**Estimated Completion:** End of week (December 17, 2024)

🎮 **Let's make Shield Wall playable and fun!**
