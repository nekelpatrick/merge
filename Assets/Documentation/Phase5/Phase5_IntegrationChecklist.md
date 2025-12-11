# Phase 5 - Integration & Tuning Checklist

**Use this checklist when integrating Phase 5 changes into Unity**

---

## Pre-Integration

- [ ] Pull latest code changes
- [ ] Verify no compile errors in Unity console
- [ ] Backup current Battle.unity scene
- [ ] Review `Phase5_ImplementationSummary.md` for context

---

## Prefab Creation (30-45 min)

### ActionPreviewUI
- [ ] Create parent GameObject "ActionPreviewUI"
- [ ] Add CanvasGroup, VerticalLayoutGroup (optional)
- [ ] Add child "HeaderText" (TMP) - Text: "AVAILABLE ACTIONS"
- [ ] Add child "EmptyStatePanel" with "EmptyStateText" (TMP)
- [ ] Add child "PreviewContainer" with VerticalLayoutGroup
- [ ] Attach `ActionPreviewUI.cs` component
- [ ] Assign all references in inspector
- [ ] Create ActionPreviewItem prefab (see below)
- [ ] Assign _previewItemPrefab reference

### ActionPreviewItem
- [ ] Create parent GameObject "ActionPreviewItem" with Image (background)
- [ ] Add child "ActionNameText" (TMP - bold, size 18-24)
- [ ] Add child "EffectText" (TMP - regular, size 14-16)
- [ ] Add child "RuneContainer" with HorizontalLayoutGroup
- [ ] Add child "StatusIcon" (Image - checkmark or indicator)
- [ ] Attach `ActionPreviewItem.cs` component
- [ ] Assign all references
- [ ] Create simple RuneBadge prefab (16x16 Image, child TMP for symbol)
- [ ] Assign _runeBadgePrefab reference
- [ ] Save as prefab

### PhaseBannerUI
- [ ] Create parent GameObject "PhaseBannerUI" with CanvasGroup
- [ ] Add child "PhaseText" (TMP - large bold, size 32-48)
- [ ] Add child "CTAText" (TMP - regular, size 18-24)
- [ ] Attach `PhaseBannerUI.cs` component
- [ ] Assign references
- [ ] Set colors: Bone White (#D4C8B8)
- [ ] Configure timing (defaults OK: 0.3s fade, 2s display)
- [ ] Position at top-center or bottom-center of screen

### EnemyIntentIndicator
- [ ] Create parent GameObject "EnemyIntentIndicator" with Image (circle background)
- [ ] Add child "IconImage" (Image)
- [ ] Attach `EnemyIntentIndicator.cs` component
- [ ] Create 3 simple icon sprites (or use text):
    - Sword icon (⚔) for attack
    - Shield icon for brother target
    - Arrow icon (➡) for unblockable
- [ ] Assign icon sprites
- [ ] Set default colors (Red: #8B2020, Gold: #C9A227, Orange: #C45C26)
- [ ] Save as prefab

### EnemyIntentManager
- [ ] Create GameObject "EnemyIntentManager" in scene
- [ ] Add empty child "IntentContainer" with layout component
- [ ] Attach `EnemyIntentManager.cs` component
- [ ] Assign _intentContainer reference
- [ ] Assign EnemyIntentIndicator prefab
- [ ] Position container above/near enemies in scene

---

## Scene Integration (30 min)

### Battle Scene Hierarchy
- [ ] Add ActionPreviewUI to Canvas (bottom-left or right panel)
- [ ] Add PhaseBannerUI to Canvas (top or bottom center)
- [ ] Add EnemyIntentManager to scene root (or UI root)
- [ ] Verify all UI elements have correct sorting order
- [ ] Test layout at different resolutions (anchors set correctly)

### ScreenEffectsController Update
- [ ] Find existing ScreenEffectsController in scene
- [ ] Add new child "StaminaPulseImage" (Image, fullscreen)
- [ ] Set color: Iron Blue (#3D5A6E) with 30% alpha (0, 0, 0, 0)
- [ ] Assign to _staminaPulseImage field
- [ ] Test context menu: "Test Vignette" should work

### TurnManager Integration
- [ ] Find TurnManager in scene
- [ ] Add reference to EnemyIntentManager (if needed)
- [ ] OR add code snippet (see Implementation Summary) to pass attacks

---

## Tutorial Hints (15 min)

### Create Hint Assets
- [ ] Right-click in ScriptableObjects/Tutorial → Create > ShieldWall > TutorialHint
- [ ] Create "Hint_FirstRoll.asset":
    - hintId: "first_dice_roll"
    - hintText: "Dice rolled! Click dice to lock them for combos."
    - triggerPhase: PlayerTurn
    - triggerWave: 1
- [ ] Create "Hint_FirstLock.asset":
    - hintId: "first_dice_locked"
    - hintText: "Locked dice unlock actions. Check the action preview!"
    - triggerPhase: PlayerTurn
- [ ] Create "Hint_FirstDamage.asset":
    - hintId: "first_damage_taken"
    - hintText: "You took damage! Stamina is your doom clock."
- [ ] Create "Hint_BrotherWounded.asset":
    - hintId: "first_brother_wounded"
    - hintText: "Shield your brothers or they fall!"

### Assign Hints
- [ ] Find TutorialManager in scene
- [ ] Add all 4 hints to _hints list
- [ ] Verify TutorialHintUI reference assigned

---

## Visual Tuning (30-45 min)

### Font & Text Readability
- [ ] ActionPreviewUI: Adjust text sizes for readability
- [ ] PhaseBannerUI: Ensure bold/large text stands out
- [ ] Test on 1920x1080 and 1280x720 resolutions
- [ ] Check text contrast on dark backgrounds

### Color Adjustments
- [ ] Verify rune colors match Visual Style System
- [ ] Check action preview badges are visible
- [ ] Test enemy intent icon colors (distinct at a glance)
- [ ] Ensure phase banner text is high contrast

### Layout & Spacing
- [ ] ActionPreviewUI: Items not too crowded
- [ ] Dice UI: Enough space around buttons
- [ ] Phase banner: Not covering critical gameplay elements
- [ ] Enemy intents: Positioned near enemies but not obscuring

### Animation Timing
- [ ] Phase banner fade: Not too fast (0.3s is good)
- [ ] Phase banner display: Long enough to read (2s default)
- [ ] Tutorial hints: Auto-dismiss after 5s (good default)
- [ ] Screen effects: Pulses/flashes quick but visible (0.2-0.3s)

---

## Playtest Validation (1 hour)

### Turn 1 - First Impressions
- [ ] Start fresh battle (or reset tutorial)
- [ ] Dice roll: Can identify rune types?
- [ ] Lock dice: Action preview updates?
- [ ] Action preview: Shows full names, not codes?
- [ ] Enemy intents: Visible and understandable?
- [ ] Phase banner: Shows "YOUR TURN" message?
- [ ] Tutorial hint: Appears and is helpful?
- [ ] Confirm actions: Resolution feedback visible?

### Turn 2 - Understanding
- [ ] Lock different dice combos
- [ ] Action preview updates in real-time?
- [ ] Can see what actions require (colored badges)?
- [ ] Enemy intents change per turn?
- [ ] Stamina pulse triggers on turn end?
- [ ] No tutorial hints repeat (already shown)?

### Turn 3 - Confidence
- [ ] Can plan turn strategy without confusion?
- [ ] Feedback makes combat feel responsive?
- [ ] Phase transitions clear?
- [ ] No "what do I do now?" moments?

### New Player Simulation
- [ ] Pretend you've never seen the game
- [ ] Can complete 3 turns without asking questions?
- [ ] Rune system feels intuitive?
- [ ] Combat feels strategic, not random?

---

## Bug Checks

### Common Issues
- [ ] No null reference errors in console
- [ ] Action preview doesn't break with 0 actions
- [ ] Phase banner doesn't overlap critical UI
- [ ] Enemy intent doesn't show when no enemies
- [ ] Tutorial hints dismiss properly
- [ ] Screen effects don't stack/overlap badly

### Edge Cases
- [ ] Test with 2 dice (alone)
- [ ] Test with 5 dice (full wall)
- [ ] Test with 0 available actions
- [ ] Test with many available actions (UI scrolls?)
- [ ] Test stamina at 0 (defeat condition)

---

## Performance Check

- [ ] Profiler: No GC spikes during gameplay
- [ ] Profiler: UI updates not causing lag
- [ ] 60fps maintained during turn flow
- [ ] No continuous Update() calls (should be event-driven)

---

## Polish (Optional, if time permits)

- [ ] Add simple fade-in for action preview items
- [ ] Add hover tooltip on rune badges (show full name)
- [ ] Add subtle glow to "ready" actions
- [ ] Add pulse animation to enemy intent icons
- [ ] Add subtle background to action preview panel

---

## Final Checks

- [ ] All prefabs saved in Prefabs/UI/
- [ ] All tutorial hints saved in ScriptableObjects/Tutorial/
- [ ] Battle scene saved with new UI integrated
- [ ] No console warnings or errors
- [ ] Git commit with clear message: "Phase 5: UX improvements for rune clarity"

---

## Success Criteria (from Phase5_UXSuccessCriteria.md)

- [ ] ✅ Players can see which actions are ready from current dice
- [ ] ✅ Clear CTA text per phase
- [ ] ✅ Inline hints guide the first turn
- [ ] ✅ Immediate visual feedback for actions
- [ ] ✅ Enemy intent is visible before commit
- [ ] ✅ No changes to core logic (additive only)

---

## Estimated Time Breakdown

| Task | Time |
|------|------|
| Prefab creation | 30-45 min |
| Scene integration | 30 min |
| Tutorial hints setup | 15 min |
| Visual tuning | 30-45 min |
| Playtest validation | 60 min |
| **Total** | **2.5-3.5 hours** |

---

## Notes

- Keep prefabs simple and functional first, polish later
- Test frequently (press Play after each major change)
- Use F12 debug shortcuts already in game for testing effects
- If stuck, refer to `Phase5_ImplementationSummary.md` for details
- All code is complete, this is just wiring/tuning

---

## When Complete

1. Playtest one full battle (3-5 waves)
2. Document any remaining issues
3. Consider asking a friend to try (fresh eyes)
4. Celebrate! Phase 5 makes the game actually playable! 🎉
