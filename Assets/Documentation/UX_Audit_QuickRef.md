# UX Audit - Quick Reference Card

**Last Updated:** December 12, 2025  
**Status:** ✅ Audit Complete - Ready for Implementation

---

## 🎯 What You Have Now

**6 New Documents** in `Assets/Documentation/`:
1. **UX_Fun_Backlog.md** ⭐ (LIVING DOC - use this daily)
2. **Internal_Audit_Report_Dec2025.md** (snapshot - reference once)
3. **Micro_Playtest_Kit.md** (run after Wave 1)
4. **Issue_Prioritization_Matrix.md** (reference - see scoring)
5. **Track_Implementation_Guide.md** (roadmap - follow step-by-step)
6. **UX_Audit_Summary.md** (overview - read first)

---

## ⚡ Quick Start: Fix the Game in 2.5 Hours

### Wave 1: The Critical 5

| Order | ID | Fix | Time | File |
|-------|----|----|------|------|
| 1st | **UX-001** | Change DieVisual rune codes | 5 min | `Scripts/UI/DieVisual.cs` line 122 |
| 2nd | **UX-003** | Add PhaseBannerUI to scene | 20 min | Create prefab + add to `Battle.unity` |
| 3rd | **UX-002** | Add ActionPreviewUI to scene | 30 min | Create prefab + add to `Battle.unity` |
| 4th | **UX-011** | Add brother name labels | 20 min | Add TextMeshPro to brothers in `Battle.unity` |
| 5th | **UX-004** | Run 3D asset creator, assign colors | 1 hour | Menu > Shield Wall Builder > Create All 3D Assets |

**Total:** 2h 35min → Game becomes **playable** (not pretty, but understandable)

---

## 📋 After Each Fix - Validation Checklist

**Copy this for each issue:**

- [ ] Fix implemented
- [ ] Open Battle.unity, press Play
- [ ] Test steps from Implementation Guide pass
- [ ] No new errors in Console
- [ ] Update `UX_Fun_Backlog.md` status: 🟡 Known → 🔵 In Progress → ✅ Fixed
- [ ] Commit with message: "Fix UX-XXX: [description]"
- [ ] Take screenshot (before/after comparison)

---

## 🔄 Audit Cycle (Weekly Recommended)

```
1. Play game as "new player" (20 min)
   ↓
2. Note confusion points (use Observer Form)
   ↓
3. Add new issues to backlog
   ↓
4. Score issues (Impact × Freq × 1/Effort)
   ↓
5. Pick top 3 issues
   ↓
6. Implement fixes
   ↓
7. Validate (internal playtest)
   ↓
8. REPEAT until fun
```

---

## 📊 Success Metrics Dashboard

| Metric | Baseline | Target | Current |
|--------|----------|--------|---------|
| **Phase5 UX gates passed** | 0/6 | 5/6 | 0/6 |
| **Time-to-first-turn** | ∞ | <60s | ∞ |
| **Blocks issues** | 5 | 0 | 5 |
| **GDD pillars achieved** | 0/4 | 3/4 | 0/4 |

**Update this after Wave 1, Wave 2, and each playtest.**

---

## 🎮 When to Playtest

### Internal (You) - After Every Fix
- Quick 5-min check: "Does this work?"
- Full 20-min playthrough: After Wave 1

### External (Others) - After Wave 1+2
- Recruit 3-5 testers (use Micro_Playtest_Kit)
- Run 20-30 min sessions
- Analyze, prioritize new issues
- Implement top 3
- REPEAT

---

## 🚨 Red Flags (Stop and Fix)

If during playtest you observe:
- ❌ Player quits before Turn 1 complete
- ❌ Player asks "What do I do?" more than once
- ❌ Player can't name a single rune type
- ❌ Player takes damage and doesn't know why
- ❌ Player says "This isn't fun"

→ **STOP** implementing new features  
→ **FIX** the blocker immediately  
→ **RETEST** before continuing

---

## 💡 Pro Tips

### Prioritization
- Fix all 🔴 Blocks before adding features
- Fix 🟠 Hurts before polishing
- Defer 🟢 Polish until game is fun

### Testing
- Test on fresh Unity Editor restart (clears PlayerPrefs)
- Ask testers to "think aloud" (most valuable feedback)
- Record sessions (video or notes) - you'll forget details

### Iteration Speed
- Small fixes → test → commit (don't batch 10 fixes then test)
- "Done" = code works + player understands + feeling achieved
- Ship ugly but fun > ship pretty but confusing

---

## 📁 File Organization

```
Assets/Documentation/
├── Core/                          (reference - don't edit)
│   ├── GameDesignDocument.md
│   └── VisualStyleSystem.md
├── Phase5/                        (reference)
│   └── Phase5_UXSuccessCriteria.md
├── TrackPrompts/                  (reference)
│   ├── Phase3/
│   ├── Phase4/
│   └── Phase5/
├── UX_Fun_Backlog.md             ⭐ UPDATE DAILY
├── Internal_Audit_Report_Dec2025.md
├── Micro_Playtest_Kit.md
├── Issue_Prioritization_Matrix.md
├── Track_Implementation_Guide.md  ⭐ USE FOR IMPLEMENTATION
└── UX_Audit_Summary.md
```

---

## 🎯 Critical Path to Shippable

```
TODAY:      Wave 1 (Critical 5)         2.5h
            ↓
            Internal playtest           20m
            ↓
THIS WEEK:  Wave 2 (High Impact)        1.5h
            ↓
            External playtest (3-5)     3h
            ↓
            Fix top 3 new issues        2h
            ↓
NEXT WEEK:  Polish (Wave 3)             2h
            ↓
            Final validation            1h
            ↓
SHIP:       Demo / Alpha / Playtest Build
```

**Total to Shippable:** ~12 hours focused work

---

## ⚙️ Troubleshooting

### "I fixed UX-001 but dice still show 'BR'"
- Check: Did you save DieVisual.cs?
- Check: Did you reload Unity scene?
- Check: Is DieVisual actually using the method? (add Debug.Log)

### "I created prefab but it doesn't show in scene"
- Check: Did you add GameObject to Battle.unity Canvas?
- Check: Did you assign prefab reference in inspector?
- Check: Is GameObject active (checkbox in Hierarchy)?
- Check: Is Canvas render mode correct (Screen Space - Overlay)?

### "Playtester said 'still confusing'"
- Capture specific confusion point (timestamp, quote)
- Add to backlog as new issue (UX-016, etc.)
- Score and prioritize
- Don't dismiss as "they just don't get it"

---

## 🔗 External Resources

**Playtest Recruitment:**
- r/playmygame (Reddit)
- r/gamedev (Reddit)
- Game Dev Discord servers
- PlaytestCloud (paid, $99-299)

**Similar Games (Reference):**
- Dicey Dungeons (dice mechanics)
- Slay the Spire (combo discovery)
- Darkest Dungeon (brother management)
- Into the Breach (enemy intent)

---

## ✅ Validation Test (Run After Wave 1)

**New Player Test** (You or friend, 5 minutes):

1. Launch game
2. Can identify rune types in < 10 seconds? ✓ / ✗
3. Lock 2 dice, see action preview update? ✓ / ✗
4. See phase banner telling what to do? ✓ / ✗
5. Distinguish brothers from enemies? ✓ / ✗
6. Complete Turn 1 confidently? ✓ / ✗

**If ≥4 ✓ → Wave 1 SUCCESS → Continue to Wave 2**  
**If <4 ✓ → Fix failures before Wave 2**

---

## 📞 Need Help?

**Check these docs in order:**
1. This Quick Reference (overview)
2. Track_Implementation_Guide.md (detailed specs)
3. UX_Fun_Backlog.md (current state)
4. Internal_Audit_Report (evidence)

**Still stuck?** Review Phase5_UXSuccessCriteria for acceptance criteria details.

---

**Good luck! You've got a complete system now. Just execute. 🚀**
