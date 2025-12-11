# Phase 5.5 - Prefab Creation Guide

**Status:** Ready for Unity Editor Implementation  
**Time Required:** ~1 hour  
**Prerequisites:** All Phase 5 C# scripts complete

---

## Overview

This guide provides step-by-step instructions for creating the 4 required UI prefabs in Unity Editor. These prefabs are needed for Track A (UX Clarity Integration) to function.

**Prefabs to Create:**
1. ActionPreviewItem.prefab
2. RuneBadge.prefab  
3. PhaseBannerUI.prefab
4. EnemyIntentIndicator.prefab

---

## 1. ActionPreviewItem Prefab (20 minutes)

### Purpose
Displays a single action in the action preview panel, showing name, required runes, and effect description.

### Script Requirements
- **Script:** `Assets/Scripts/UI/ActionPreviewItem.cs`
- **Required Fields:**
  - `_actionNameText` (TextMeshProUGUI)
  - `_effectText` (TextMeshProUGUI)
  - `_runeContainer` (Transform)
  - `_runeBadgePrefab` (Image) - will be set to RuneBadge prefab
  - `_statusIcon` (Image)

### Creation Steps

1. **Create GameObject hierarchy:**
   ```
   ActionPreviewItem (GameObject)
   ├─ Background (Image)
   ├─ ActionNameText (TextMeshProUGUI)
   ├─ RuneContainer (GameObject + HorizontalLayoutGroup)
   ├─ EffectText (TextMeshProUGUI)
   └─ StatusIcon (Image)
   ```

2. **Configure Root (ActionPreviewItem):**
   - Add RectTransform
   - Add `ActionPreviewItem` component (script)
   - Size: Width 300-350px, Height 80-100px

3. **Configure Background:**
   - Add Image component
   - Color: #6B4E3D (Worn Leather) with 80% alpha (RGB: 107, 78, 61, A: 204)
   - Stretch to fill parent

4. **Configure ActionNameText:**
   - Add TextMeshProUGUI component
   - Font: Bold, 18pt
   - Color: #C9A227 (Gold - RGB: 201, 162, 39)
   - Alignment: Top-Left
   - Position: Top of prefab with 10px padding

5. **Configure RuneContainer:**
   - Add HorizontalLayoutGroup component:
     - Spacing: 5
     - Child Alignment: Middle-Left
     - Child Force Expand: Width OFF, Height OFF
   - Position: Below ActionNameText, 5px gap

6. **Configure EffectText:**
   - Add TextMeshProUGUI component
   - Font: Regular, 14pt
   - Color: #D4C8B8 (Bone White - RGB: 212, 200, 184)
   - Alignment: Top-Left
   - Position: Below RuneContainer, 5px gap
   - Text wrap: Enabled

7. **Configure StatusIcon:**
   - Add Image component
   - Size: 24×24px
   - Position: Top-right corner
   - Color: Will be set at runtime (gold or gray)

8. **Wire References in Inspector:**
   - Select ActionPreviewItem root
   - ActionPreviewItem component:
     - `_actionNameText` → Drag ActionNameText child
     - `_effectText` → Drag EffectText child
     - `_runeContainer` → Drag RuneContainer child
     - `_runeBadgePrefab` → Leave null (will set after creating RuneBadge prefab)
     - `_statusIcon` → Drag StatusIcon child
     - `_availableColor` → #C9A227 (201, 162, 39, 255)
     - `_textColor` → #D4C8B8 (212, 200, 184, 255)

9. **Save Prefab:**
   - Drag from Hierarchy to `Assets/Prefabs/UI/`
   - Name: `ActionPreviewItem.prefab`
   - Delete from scene hierarchy

---

## 2. RuneBadge Prefab (10 minutes)

### Purpose
Small badge showing a single rune requirement (colored background + rune symbol).

### Creation Steps

1. **Create GameObject hierarchy:**
   ```
   RuneBadge (Image)
   └─ RuneSymbol (TextMeshProUGUI)
   ```

2. **Configure Root (RuneBadge):**
   - Add Image component
   - Size: 60×30px
   - Color: Will be set at runtime via RuneDisplay.GetDefaultColor()
   - Sprite: Use Unity built-in "UI-Default" or rounded rect

3. **Configure RuneSymbol:**
   - Add TextMeshProUGUI component
   - Font: Bold, 16pt
   - Color: White
   - Alignment: Center-Middle
   - Text: "ᚦ" (placeholder - will be set at runtime)
   - Position: Centered in parent

4. **Save Prefab:**
   - Drag from Hierarchy to `Assets/Prefabs/UI/`
   - Name: `RuneBadge.prefab`
   - Delete from scene hierarchy

5. **Update ActionPreviewItem:**
   - Open ActionPreviewItem prefab
   - ActionPreviewItem component:
     - `_runeBadgePrefab` → Drag RuneBadge prefab here
   - Save prefab

---

## 3. PhaseBannerUI Prefab (15 minutes)

### Purpose
Displays phase guidance banner at top of screen showing current turn state and call-to-action.

### Script Requirements
- **Script:** `Assets/Scripts/UI/PhaseBannerUI.cs`
- **Required Fields:**
  - `_phaseText` (TextMeshProUGUI)
  - `_ctaText` (TextMeshProUGUI)
  - `_canvasGroup` (CanvasGroup)

### Creation Steps

1. **Create GameObject hierarchy:**
   ```
   PhaseBannerUI (GameObject + CanvasGroup)
   ├─ Background (Image)
   └─ Content (GameObject + VerticalLayoutGroup)
      ├─ PhaseText (TextMeshProUGUI)
      └─ CTAText (TextMeshProUGUI)
   ```

2. **Configure Root (PhaseBannerUI):**
   - Add RectTransform
   - Add CanvasGroup component
     - Alpha: 0 (will fade in at runtime)
     - Interactable: OFF
     - Block Raycasts: OFF
   - Add `PhaseBannerUI` component (script)
   - Anchor: Top-center
   - Size: Width 600-800px, Height 100-120px
   - Position: Y offset -50 to -100 from top

3. **Configure Background:**
   - Add Image component
   - Color: #2A1F1A (Dark Brown) with 90% alpha (RGB: 42, 31, 26, A: 230)
   - Stretch to fill parent

4. **Configure Content:**
   - Add VerticalLayoutGroup component:
     - Spacing: 5
     - Child Alignment: Middle-Center
     - Child Force Expand: Both OFF
   - Padding: 10px all sides

5. **Configure PhaseText:**
   - Add TextMeshProUGUI component
   - Font: Bold, 24pt, UPPERCASE
   - Color: #C9A227 (Gold - RGB: 201, 162, 39)
   - Alignment: Center-Middle
   - Text: "YOUR TURN" (placeholder)

6. **Configure CTAText:**
   - Add TextMeshProUGUI component
   - Font: Regular, 16pt
   - Color: #D4C8B8 (Bone White - RGB: 212, 200, 184)
   - Alignment: Center-Middle
   - Text: "Lock dice to ready actions" (placeholder)

7. **Wire References in Inspector:**
   - Select PhaseBannerUI root
   - PhaseBannerUI component:
     - `_phaseText` → Drag PhaseText child
     - `_ctaText` → Drag CTAText child
     - `_canvasGroup` → Drag CanvasGroup on root
     - `_fadeInDuration` → 0.3
     - `_displayDuration` → 2.0
     - Phase messages: (leave defaults or customize)

8. **Save Prefab:**
   - Drag from Hierarchy to `Assets/Prefabs/UI/`
   - Name: `PhaseBannerUI.prefab`
   - Delete from scene hierarchy

---

## 4. EnemyIntentIndicator Prefab (10 minutes)

### Purpose
World-space icon above enemy head showing attack intent (target + type).

### Script Requirements
- **Script:** `Assets/Scripts/UI/EnemyIntentIndicator.cs`
- **Required Fields:**
  - `_iconImage` (Image)

### Creation Steps

1. **Create GameObject hierarchy:**
   ```
   EnemyIntentIndicator (Canvas - World Space)
   └─ IntentIcon (Image)
   ```

2. **Configure Root (EnemyIntentIndicator):**
   - Add Canvas component:
     - Render Mode: World Space
     - Sorting Layer: UI or Overlay
     - Order in Layer: 10
   - Add CanvasScaler component (optional):
     - Dynamic Pixels Per Unit: 10
   - Add `EnemyIntentIndicator` component (script)
   - RectTransform:
     - Width: 1
     - Height: 1
     - Scale: 0.01 (adjust for visibility)

3. **Configure IntentIcon:**
   - Add Image component
   - Size: Fill parent (100×100 anchored)
   - Color: Will be set at runtime (red/yellow)
   - Sprite: Simple sword/arrow icon or colored square
   - Preserve Aspect: ON

4. **Icon Sprites (Create Simple Placeholders):**
   
   **Option A: Use colored squares (quick):**
   - IntentIcon sprite: Unity default white square
   - Color at runtime: Red = player target, Yellow = brother target

   **Option B: Create simple icons:**
   - Create 64×64 textures in image editor
   - Sword: Triangle pointing down
   - Arrow: Arrow shape
   - Import as sprites
   - Assign to IntentIcon

5. **Wire References in Inspector:**
   - Select EnemyIntentIndicator root
   - EnemyIntentIndicator component:
     - `_iconImage` → Drag IntentIcon child

6. **Save Prefab:**
   - Drag from Hierarchy to `Assets/Prefabs/UI/`
   - Name: `EnemyIntentIndicator.prefab`
   - Delete from scene hierarchy

---

## Validation Checklist

After creating all prefabs:

### ActionPreviewItem ✓
- [ ] Prefab exists at `Assets/Prefabs/UI/ActionPreviewItem.prefab`
- [ ] Has ActionPreviewItem component
- [ ] All 5 text/transform references wired
- [ ] Colors set to Visual Style System palette
- [ ] RuneBadge prefab assigned

### RuneBadge ✓
- [ ] Prefab exists at `Assets/Prefabs/UI/RuneBadge.prefab`
- [ ] Has Image (root) + TextMeshProUGUI (child)
- [ ] Size approximately 60×30px
- [ ] Referenced by ActionPreviewItem prefab

### PhaseBannerUI ✓
- [ ] Prefab exists at `Assets/Prefabs/UI/PhaseBannerUI.prefab`
- [ ] Has PhaseBannerUI component
- [ ] Has CanvasGroup (alpha 0)
- [ ] PhaseText and CTAText wired
- [ ] Colors match palette

### EnemyIntentIndicator ✓
- [ ] Prefab exists at `Assets/Prefabs/UI/EnemyIntentIndicator.prefab`
- [ ] Canvas set to World Space
- [ ] Has EnemyIntentIndicator component
- [ ] IconImage wired
- [ ] Scale set appropriately (0.01 or similar)

---

## Quick Test (Before Scene Integration)

1. **Test ActionPreviewItem:**
   - Drag prefab into scene
   - Create test ActionSO asset
   - In Play mode, call `SetAction(testAction, true)` in console
   - Verify name, runes, and effect display

2. **Test PhaseBannerUI:**
   - Drag prefab into scene
   - In Play mode, trigger phase change
   - Verify fade in/out animation

3. **Test EnemyIntentIndicator:**
   - Drag prefab into scene
   - Position above test enemy
   - Verify world-space positioning
   - Verify icon visible

---

## Next Steps

After creating these 4 prefabs:
1. Proceed to **Track A Task 3:** Wire all UI components in Battle.unity scene
2. Use these prefabs in:
   - ActionPreviewUI (needs ActionPreviewItem prefab)
   - PhaseBannerUI (already self-contained)
   - EnemyIntentManager (needs EnemyIntentIndicator prefab)

---

## Color Reference (Quick Copy)

Visual Style System colors for Unity Inspector:

```
Worn Leather:  RGB(107, 78, 61),  Alpha 204  = #6B4E3DCC
Dark Brown:    RGB(42, 31, 26),   Alpha 230  = #2A1F1AE6
Bone White:    RGB(212, 200, 184), Alpha 255  = #D4C8B8FF
Gold:          RGB(201, 162, 39),  Alpha 255  = #C9A227FF
Blood Red:     RGB(139, 32, 32),   Alpha 255  = #8B2020FF
Iron Gray:     RGB(92, 92, 92),    Alpha 255  = #5C5C5CFF
```

---

**Status:** Documentation complete - Ready for Unity Editor implementation

**Estimated Time:** 55 minutes total (20+10+15+10)

**Blocker:** These prefabs MUST be created in Unity Editor before proceeding with Battle.unity scene integration.
