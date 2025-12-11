# Phase 5.5 - QUICK START (30 Minutes to Working Game)

**Status:** ✅ Ready to Execute  
**Time:** 30 minutes total  
**Difficulty:** Easy (just click menu items!)

---

## 🚀 Quick Win Path (Everything You Need)

### Minute 0-2: Open Unity
```
1. Open Shield Wall project in Unity
2. Wait for compilation to finish
3. Check Console → Should show 0 errors ✅
```

**✅ Success:** No compilation errors in Console

---

### Minute 2-7: Create UI Prefabs (One Click!)
```
1. Menu Bar → Shield Wall Builder
2. Hover over: Phase 5.5 Setup
3. Click: Create All UI Prefabs
4. Wait ~5 seconds
5. Check Console for success messages
```

**✅ Success:** Console shows:
```
=== Creating Phase 5.5 UI Prefabs ===
Created: Assets/Prefabs/UI/ActionPreviewItem.prefab
Created: Assets/Prefabs/UI/RuneBadge.prefab
Created: Assets/Prefabs/UI/PhaseBannerUI.prefab
Created: Assets/Prefabs/UI/EnemyIntentIndicator.prefab
=== Phase 5.5 UI Prefabs Created Successfully ===
```

**🔍 Verify:** Project window → `Assets/Prefabs/UI/` → 4 new prefabs exist

---

### Minute 7-12: Integrate Into Scene (One Click!)
```
1. Ensure Battle.unity is open (or tool will prompt to open it)
2. Menu Bar → Shield Wall Builder
3. Hover over: Phase 5.5 Setup
4. Click: Integrate All UI Into Scene
5. Wait ~5 seconds
6. Check Console for success messages
```

**✅ Success:** Console shows:
```
=== Integrating Phase 5.5 UI Into Battle Scene ===
✓ ActionPreviewUI added to scene (right side panel)
✓ PhaseBannerUI added to scene (top-center banner)
✓ EnemyIntentManager added to scene
✓ ScreenEffects UI added and wired to ScreenEffectsController
=== Phase 5.5 UI Integration Complete ===
```

**🔍 Verify:** Battle.unity Hierarchy → See new GameObjects:
- ActionPreviewUI
- PhaseBannerUI
- EnemyIntentManager
- Canvas/ScreenEffects (with 3 child images)

---

### Minute 12-22: Test in Play Mode
```
1. Hierarchy → Select Battle scene
2. Click Play button
3. Watch for battle to start
4. Roll dice → Check dice labels
5. Lock 2 dice → Check right side panel
6. Watch top of screen for phase banner
```

**✅ Success Checklist:**
- [ ] Dice show "Shield", "Axe", "Spear" (not "SH", "AX", "SP")
- [ ] Locking 2+ dice makes ActionPreviewUI appear on right
- [ ] ActionPreviewUI shows available actions with rune badges
- [ ] PhaseBannerUI appears at top when phase changes
- [ ] No console errors during gameplay

**❌ If Failed:**
- Check Console for errors
- Read: `Phase5_5_FIX_SUMMARY.md`
- Run: `Shield Wall Builder > Phase 5.5 Setup > Validate Scene Integration`

---

### Minute 22-30: Optional - Generate 3D Assets
```
1. Menu Bar → Shield Wall Builder
2. Hover over: 3D Assets
3. Click: Create All 3D Assets (One-Click)
4. Wait ~10 seconds
5. Check Console for success messages
6. Manually assign materials (see below)
```

**Manual Material Assignment (5 min):**
```
1. Project → Assets/Art/Materials/
2. Find: M_Character_Brother.mat
3. Hierarchy → Find brother GameObjects
4. Drag material onto MeshRenderer components

5. Find: M_Character_Enemy.mat
6. Hierarchy → Find enemy GameObjects  
7. Drag material onto MeshRenderer components
```

**✅ Success:** Characters have toon shading instead of default Unity gray

---

## 🎯 Expected Results

### Phase 5.5 UX Goals Met
1. ✅ **Rune Clarity** → Dice show full names
2. ✅ **Action Preview** → Right panel shows available actions
3. ✅ **Phase Guidance** → Top banner shows current phase + instructions
4. ✅ **Enemy Intent** → System ready (will show when enemies assigned)
5. ✅ **Combat Feedback** → Screen effects wired (vignette, flash, pulse)

### Visual Upgrade (Optional Track B)
1. ✅ **3D Assets** → Limb prefabs, materials, blood VFX created
2. ⏳ **Material Assignment** → Requires manual drag-and-drop (5 min)
3. ⏳ **Atmosphere** → Fog, lighting, post-processing (can be tuned later)

---

## 🐛 Troubleshooting

### Issue: Compilation Errors
**Symptom:** Red errors in Console

**Fix:**
1. Read error message carefully
2. Check: `Phase5_5_FIX_SUMMARY.md`
3. Likely: Missing using directive or namespace issue
4. Nuclear option: Delete `Library` folder and reopen Unity

### Issue: Menu Items Missing
**Symptom:** "Shield Wall Builder" menu not found

**Fix:**
1. Check: `Assets/Editor/` folder exists
2. Check: `.cs` files are in `Assets/Editor/` (not `Assets/Scripts/Editor/`)
3. Wait for Unity compilation to finish
4. Restart Unity if menu still missing

### Issue: Prefabs Not Created
**Symptom:** Menu item runs but no prefabs appear

**Fix:**
1. Check Console for errors
2. Ensure `Assets/Prefabs/UI/` folder exists
3. Run: `Shield Wall Builder > Phase 5.5 Setup > Validate Prefabs`
4. If validation fails, read error messages

### Issue: Scene Integration Failed
**Symptom:** GameObjects not added to scene

**Fix:**
1. Ensure Battle.unity is open (tool should prompt)
2. Check Canvas exists in scene
3. Run integration menu item again (idempotent, safe to retry)
4. Run: `Shield Wall Builder > Phase 5.5 Setup > Validate Scene Integration`

### Issue: Dice Still Show Codes
**Symptom:** Dice show "SH", "AX" instead of "Shield", "Axe"

**Fix:**
1. Check: `DieVisual.cs` has been modified
2. Line 114 should read: `return RuneDisplay.GetFullName(type);`
3. Not: `return type switch { ... }` with hardcoded codes
4. If wrong, re-apply fix from `Phase5_5_FIX_SUMMARY.md`

---

## 📋 Validation Checklist

Run these checks to confirm everything works:

### Compilation ✅
- [ ] Unity Console shows 0 errors
- [ ] All scripts compile successfully
- [ ] No missing references warnings

### Prefabs ✅
- [ ] `ActionPreviewItem.prefab` exists in `Assets/Prefabs/UI/`
- [ ] `RuneBadge.prefab` exists
- [ ] `PhaseBannerUI.prefab` exists
- [ ] `EnemyIntentIndicator.prefab` exists

### Scene Integration ✅
- [ ] `ActionPreviewUI` GameObject in Battle.unity
- [ ] `PhaseBannerUI` GameObject in Battle.unity
- [ ] `EnemyIntentManager` GameObject in Battle.unity
- [ ] `Canvas/ScreenEffects` with 3 child images

### Runtime Behavior ✅
- [ ] Dice show full rune names
- [ ] ActionPreviewUI appears when dice locked
- [ ] PhaseBannerUI shows at phase changes
- [ ] No runtime errors in Console

---

## 🎉 Success!

If all checklist items are checked, **Phase 5.5 UX Quick Win is COMPLETE!**

### What Changed
- **Before:** Cryptic codes, no action preview, no guidance
- **After:** Clear names, action preview panel, phase banners

### Time Spent
- **Estimated:** 2.5 hours of manual Unity work
- **Actual:** 30 minutes with automation
- **Savings:** 2 hours!

### Next Steps
1. **Playtest 3 turns** → Verify UX improvements feel better
2. **Optional: Polish** → Tune colors, spacing, fonts
3. **Optional: 3D Upgrade** → Source character models, replace primitives
4. **Optional: Atmosphere** → Tune fog, lighting, camera effects

---

## 📞 Need Help?

**Read These (In Order):**
1. `Phase5_5_FIX_SUMMARY.md` → Compilation troubleshooting
2. `Phase5_5_AUTOMATION_COMPLETE.md` → Full automation guide
3. `Phase5_5_FINAL_STATUS.md` → Complete status report

**Or:** Just run the validation menu items!
- `Shield Wall Builder > Phase 5.5 Setup > Validate Prefabs`
- `Shield Wall Builder > Phase 5.5 Setup > Validate Scene Integration`

---

**Last Updated:** December 11, 2024  
**Status:** ✅ Ready for immediate execution!  
**Time to Working Game:** 30 minutes!

