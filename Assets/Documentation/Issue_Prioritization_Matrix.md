# Issue Prioritization Matrix - December 12, 2025

**Purpose:** Score all known UX/fun issues, cluster by theme, pick top 10 for immediate action  
**Input:** Internal_Audit_Report + ShieldWall_CurrentStateAudit + Phase5_UXSuccessCriteria  
**Method:** Impact × Frequency × (1/Effort) = Priority Score

---

## Scoring System

### Impact Values
- **Blocks (10 points):** Prevents core loop understanding OR game not fun at all
- **Hurts (5 points):** Degrades experience, causes confusion/frustration
- **Polish (2 points):** Nice to have, improves feel

### Frequency Values
- **100% (1.0):** Every player, every playthrough
- **80% (0.8):** Most players, most playthroughs
- **60% (0.6):** Many players, situational
- **40% (0.4):** Some players, optional content
- **20% (0.2):** Edge case, rare

### Effort Values
- **Low (1):** < 30 min, 1-line fix or simple prefab
- **Medium (2):** 30 min - 2 hours, requires prefab creation + scene wiring
- **High (4):** 2+ hours, complex system or multiple tracks

---

## All Issues with Scores

| ID | Symptom | Impact | Freq | Effort | Priority | Theme |
|----|---------|--------|------|--------|----------|-------|
| **UX-001** | "What do BR, AX, SP mean?" | 10 | 1.0 | 1 | **10.0** | Clarity |
| **UX-002** | "I don't know what locking dice does" | 10 | 1.0 | 2 | **5.0** | Discoverability |
| **UX-003** | "What phase am I in? What do I do?" | 10 | 1.0 | 2 | **5.0** | Guidance |
| **UX-004** | "Which ones are enemies? Brothers?" | 10 | 1.0 | 2 | **5.0** | Hierarchy |
| **UX-005** | "I can't tell who is attacking me" | 5 | 1.0 | 2 | **2.5** | Planning |
| **UX-006** | "Did my action work? Nothing happened" | 5 | 0.8 | 1 | **4.0** | Feedback |
| **UX-007** | "I wish there was a hint" | 5 | 0.6 | 1 | **3.0** | Guidance |
| **UX-008** | "Looks flat and boring" | 2 | 0.4 | 1 | **0.8** | Atmosphere |
| **UX-009** | "Actions resolve instantly, no buildup" | 2 | 0.3 | 2 | **0.3** | Pacing |
| **UX-010** | "Game is silent, no sounds" | 2 | 1.0 | 4 | **0.5** | Audio |
| **UX-011** | "Which brother is which? All same" | 10 | 1.0 | 2 | **5.0** | Hierarchy |
| **UX-012** | "Did my brother block? Can't tell" | 5 | 0.8 | 2 | **2.0** | Feedback |
| **UX-013** | "I died. Why? What happened?" | 5 | 1.0 | 2 | **2.5** | Guidance |
| **UX-014** | "Can I skip these hints?" | 2 | 0.2 | 1 | **0.4** | Polish |
| **UX-015** | "Locked wrong dice, can't undo" | 2 | 0.4 | 1 | **0.8** | Polish |

---

## Clustered by Theme

### Theme: **Clarity** (Understanding Core Mechanics)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-001** | Cryptic rune codes | **10.0** | 🟡 Known |

**Theme Impact:** 🔴 **CRITICAL** - Players cannot understand basic game language  
**Fix Approach:** 1-line code change in DieVisual.cs  
**Track Owner:** Phase5 (UI)

---

### Theme: **Discoverability** (Finding What's Possible)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-002** | No action preview | **5.0** | 🟡 Known |

**Theme Impact:** 🔴 **CRITICAL** - Players cannot learn combo system  
**Fix Approach:** Create ActionPreviewItem prefab, add ActionPreviewUI to scene  
**Track Owner:** Phase5 (UI)

---

### Theme: **Guidance** (Knowing What to Do)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-003** | No phase banner | **5.0** | 🟡 Known |
| **UX-007** | No tutorial hints | **3.0** | 🟡 Known |
| **UX-013** | No defeat explanation | **2.5** | 🟡 Known |

**Theme Impact:** 🔴 **CRITICAL** - Players paralyzed by lack of direction  
**Fix Approach:** PhaseBannerUI (20 min) + TutorialManager wiring (20 min) + GameOverUI improvement (30 min)  
**Track Owner:** Phase5 (PhaseBanner) + Track E (Tutorial) + Track F (Menus)

---

### Theme: **Hierarchy** (Visual Distinction)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-004** | Can't tell enemies from brothers | **5.0** | 🟡 Known |
| **UX-011** | Brothers all look same | **5.0** | 🟡 Known |

**Theme Impact:** 🔴 **CRITICAL** - Players cannot parse battlefield  
**Fix Approach:** Run 3D asset creator, assign colors per Visual Style System, add name labels  
**Track Owner:** Track A (Visuals)

---

### Theme: **Feedback** (Seeing Results)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-006** | Actions feel flat | **4.0** | 🟡 Known |
| **UX-012** | Brother defense invisible | **2.0** | 🟡 Known |

**Theme Impact:** 🟠 **HIGH** - Players don't feel cause-effect loop  
**Fix Approach:** Wire ScreenEffectsController (15 min) + brother block feedback (30 min)  
**Track Owner:** Track D (UI Juice)

---

### Theme: **Planning** (Strategic Decision-Making)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-005** | No enemy intent | **2.5** | 🟡 Known |

**Theme Impact:** 🟠 **HIGH** - Players cannot strategize defense  
**Fix Approach:** Create EnemyIntentIndicator prefab, add EnemyIntentManager to scene  
**Track Owner:** Phase5 (UI)

---

### Theme: **Polish** (Quality of Life)
| ID | Symptom | Priority | Status |
|----|---------|----------|--------|
| **UX-015** | Can't undo dice locks | **0.8** | 🟡 Known |
| **UX-008** | Flat visuals | **0.8** | 🟡 Known |
| **UX-010** | No audio | **0.5** | 🟡 Known |
| **UX-014** | Can't skip tutorial | **0.4** | 🟡 Known |
| **UX-009** | Instant combat | **0.3** | 🟡 Known |

**Theme Impact:** 🟢 **LOW** - Nice to have, but doesn't block fun  
**Fix Approach:** Defer until Critical/High issues resolved  
**Track Owner:** Various

---

## Top 10 Priority Issues

Sorted by Priority Score (descending):

| Rank | ID | Symptom | Priority | Impact | Effort | Fix Time |
|------|----|---------|----------|--------|--------|----------|
| **1** | **UX-001** | Cryptic rune codes | **10.0** | 🔴 Blocks | Low | 5 min |
| **2** | **UX-002** | No action preview | **5.0** | 🔴 Blocks | Medium | 30 min |
| **3** | **UX-003** | No phase banner | **5.0** | 🔴 Blocks | Medium | 20 min |
| **4** | **UX-004** | Can't tell enemies/brothers | **5.0** | 🔴 Blocks | Medium | 1 hour |
| **5** | **UX-011** | Brothers all look same | **5.0** | 🔴 Blocks | Medium | 20 min |
| **6** | **UX-006** | No action feedback | **4.0** | 🟠 Hurts | Low | 15 min |
| **7** | **UX-007** | No tutorial hints | **3.0** | 🟠 Hurts | Low | 20 min |
| **8** | **UX-005** | No enemy intent | **2.5** | 🟠 Hurts | Medium | 30 min |
| **9** | **UX-013** | No defeat explanation | **2.5** | 🟠 Hurts | Medium | 30 min |
| **10** | **UX-012** | Brother defense invisible | **2.0** | 🟠 Hurts | Medium | 30 min |

**Total Fix Time (Top 10):** ~4 hours

---

## Implementation Waves

### Wave 1: Critical 5 (Blocks Core Loop)

**Goal:** Make game playable and understandable  
**Time:** ~2.5 hours  
**Validate:** All Phase5 UX gates pass

| ID | Fix | Time | Track |
|----|-----|------|-------|
| UX-001 | Change DieVisual.GetRuneSymbol() | 5 min | Phase5 |
| UX-002 | Add ActionPreviewUI to scene | 30 min | Phase5 |
| UX-003 | Add PhaseBannerUI to scene | 20 min | Phase5 |
| UX-004 | Run 3D asset creator, assign colors | 1 hour | Track A |
| UX-011 | Add brother name labels | 20 min | Track A |

**After Wave 1:**
- [ ] Run internal playtest
- [ ] Verify time-to-first-confident-turn < 60s
- [ ] Verify Phase5 gates ≥ 4/6 passed

---

### Wave 2: High Impact (Improves Fun)

**Goal:** Make game satisfying and strategic  
**Time:** ~1.5 hours  
**Validate:** GDD pillars start working

| ID | Fix | Time | Track |
|----|-----|------|-------|
| UX-006 | Wire ScreenEffectsController | 15 min | Track D (Juice) |
| UX-007 | Verify TutorialManager wired | 20 min | Track E |
| UX-005 | Add EnemyIntentManager to scene | 30 min | Phase5 |
| UX-013 | Improve Game Over screen | 30 min | Track F |

**After Wave 2:**
- [ ] Run 3-5 external micro-playtests
- [ ] Verify likelihood-to-continue ≥ 7/10
- [ ] Verify GDD pillars ≥ 2/4 achieved

---

### Wave 3: Polish (QoL & Juice)

**Goal:** Make game feel polished  
**Time:** ~2 hours (deferred)  
**Validate:** Target feelings achieved

| ID | Fix | Time | Track |
|----|-----|------|-------|
| UX-012 | Brother block feedback | 30 min | Track D (Juice) |
| UX-015 | Verify dice unlock works | 10 min | Phase5 |
| UX-008 | Adjust fog/lighting | 10 min | Track B |
| UX-009 | Add combat pacing delays | 30 min | Track F (Timing) |
| UX-010 | Add core audio (dice, hit, block) | 1 hour | Track D (Audio) |
| UX-014 | Add tutorial skip option | 30 min | Track E + F |

**After Wave 3:**
- [ ] Run final playtest round
- [ ] Verify all Phase5 UX gates pass
- [ ] Verify all GDD target feelings achieved
- [ ] Prepare for wider alpha testing

---

## Minimum Viable Fun (MVF) Definition

These issues **must** be fixed before external playtesting:

### Must Fix (MVF Blockers):
- ✅ UX-001 (rune codes) - **Without this, players confused from second 1**
- ✅ UX-002 (action preview) - **Without this, combo system hidden**
- ✅ UX-003 (phase banner) - **Without this, players lost**
- ✅ UX-004 (visual distinction) - **Without this, battlefield unreadable**
- ✅ UX-011 (brother identity) - **Without this, Brotherhood pillar fails**

**MVF Threshold:** Top 5 issues fixed = game is playable (not fun yet, but testable)

### Should Fix (Fun Blockers):
- UX-006 (feedback) - **Without this, actions feel disconnected**
- UX-007 (tutorial) - **Without this, new players struggle**
- UX-005 (enemy intent) - **Without this, strategy impossible**

**Fun Threshold:** Top 8 issues fixed = game is fun (worth playtesting externally)

---

## Risk Assessment

### Low Risk (Safe to Implement Immediately)
- ✅ UX-001 (1-line code change)
- ✅ UX-006 (wiring existing component)
- ✅ UX-007 (verify existing system)
- ✅ UX-015 (likely already works, just verify)

### Medium Risk (Test Carefully)
- 🟡 UX-002 (new prefab + scene integration)
- 🟡 UX-003 (new prefab + scene integration)
- 🟡 UX-005 (new prefab + scene integration)
- 🟡 UX-011 (requires world-space UI, may clip)

### High Risk (May Introduce New Issues)
- 🔴 UX-004 (running 3D asset creator may break existing setup)
- 🔴 UX-009 (combat pacing may break turn flow)
- 🔴 UX-010 (audio integration may cause performance issues)

**Mitigation:** Implement low-risk fixes first, test, then tackle medium/high risk

---

## Validation Metrics

Track these after each wave:

| Metric | Baseline | After Wave 1 | After Wave 2 | Target |
|--------|----------|--------------|--------------|--------|
| **Time-to-first-confident-turn** | ∞ | TBD | TBD | < 60s |
| **Phase5 UX gates passed** | 0/6 | TBD | TBD | 5/6 |
| **GDD pillars achieved** | 0/4 | TBD | TBD | 3/4 |
| **Likelihood to continue (1-10)** | TBD | TBD | TBD | ≥ 7 |
| **"Just one more turn" %** | TBD | TBD | TBD | ≥ 60% |

---

## Issue Lifecycle

```
🟡 Known → 🔵 In Progress → ✅ Fixed → ✔️ Validated → 🗑️ Closed
```

**Update backlog after each fix:**
1. Mark issue status (🔵 In Progress when starting)
2. Mark ✅ Fixed when code committed
3. Run playtest to validate
4. Mark ✔️ Validated when playtest confirms fix works
5. Close issue (🗑️) when shipped in build

---

## Next Actions (Immediate)

### Today:
1. ✅ Prioritization complete (this document)
2. ⏳ Update UX_Fun_Backlog.md with scores
3. ⏳ Assign tracks to top 10 issues (Todo 5)

### This Week:
4. ⏳ Implement Wave 1 (Critical 5) - 2.5 hours
5. ⏳ Internal playtest - validate fixes work
6. ⏳ Implement Wave 2 (High Impact) - 1.5 hours
7. ⏳ External micro-playtests (3-5 sessions)

### Next Week:
8. ⏳ Analyze playtest results
9. ⏳ Adjust priorities based on findings
10. ⏳ Implement Wave 3 (Polish) if time allows

---

**Prioritization Status:** ✅ COMPLETE

**Confidence Level:** HIGH (internal audit + existing documentation provides strong evidence)

**Next Step:** Todo 5 (Assign tracks with acceptance criteria)
