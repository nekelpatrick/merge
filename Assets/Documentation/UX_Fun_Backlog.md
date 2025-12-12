# Shield Wall - UX & Fun Improvement Backlog

**Date Created:** December 12, 2025  
**Status:** 🟢 ACTIVE - Living Document  
**Purpose:** Track UX/fun issues, prioritize fixes, assign to tracks, validate improvements

---

## Quick Reference

| Section | Purpose |
|---------|---------|
| [Player Journeys](#player-journeys) | Key user flows to evaluate |
| [Issue Tracking Table](#issue-tracking-table) | All known UX/fun problems |
| [Severity Rubric](#severity-rubric) | How we score issues |
| [Track Ownership Guide](#track-ownership-guide) | Which track fixes what |
| [Audit Process](#audit-process) | How to run periodic audits |

---

## Player Journeys

These are the critical paths where UX clarity and "fun factor" must succeed.

### Journey 1: First Launch → Battle Start

**Entry:** Player launches game  
**Exit:** Player sees first wave of enemies, understands goal

**Key Moments:**
1. Main menu appears - clear "Play" button
2. Scene transition to Battle
3. Battle scene loads - player sees shield, brothers, dice UI
4. Wave 1 starts - player understands "survive the wave"

**Success Criteria:**
- [ ] Player finds "Play" button in < 5 seconds
- [ ] Scene transition feels intentional (fade, not jarring)
- [ ] Player can distinguish brothers from enemies visually
- [ ] Player understands goal without reading external docs

**Phase5 UX Gates:**
- Rune Clarity (can player identify runes?)
- Visual Hierarchy (brothers vs enemies distinct?)

---

### Journey 2: Turn 1 - First Confident Action

**Entry:** Dice roll, player sees rune results  
**Exit:** Player locks dice, selects action, confirms with confidence

**Key Moments:**
1. Dice roll - player sees rune faces
2. Player hovers/clicks dice - understands "lock" concept
3. Player locks 2+ dice - sees action preview update
4. Player reads action effect - understands outcome
5. Player sees enemy intent - knows who will be attacked
6. Player selects action - button text clear ("Confirm Actions")
7. Resolution happens - player sees feedback (flash, shake, vignette)
8. Player sees phase change - "Turn complete" banner

**Success Criteria:**
- [ ] Player locks first die in < 10 seconds (with/without hint)
- [ ] Player can name at least 1 rune type correctly
- [ ] Player selects an action intentionally (not randomly)
- [ ] Player explains why they chose that action (when asked)
- [ ] Player sees cause-effect (action → result)

**Phase5 UX Gates:**
- Rune Clarity ✓
- Action Preview ✓
- Phase Guidance ✓
- Enemy Intent ✓
- Combat Feedback ✓

---

### Journey 3: Turn 2-3 - Building Mastery

**Entry:** Player understands basics from Turn 1  
**Exit:** Player experiments with combos, plans strategy

**Key Moments:**
1. Player tries different dice combos intentionally
2. Player discovers new action (not seen in Turn 1)
3. Player chooses between 2+ available actions (tradeoff)
4. Player sees brother auto-defend or fail to defend
5. Player reacts to stamina drain ("oh, this is the doom clock")

**Success Criteria:**
- [ ] Player tries at least 2 different combos across turns
- [ ] Player shows strategic thinking ("I need to block THIS enemy")
- [ ] Player reacts to brother wounding ("oh no, protect them")
- [ ] Player feels tension as stamina drops

**GDD Pillar Alignment:**
- **Brotherhood:** Player cares when brother is wounded
- **Fate:** Dice rolls feel impactful, not arbitrary
- **Endurance:** Stamina pressure is real but fair
- **Sacrifice:** Player makes tradeoffs (offense vs defense)

---

### Journey 4: Loss/Victory → Retry Decision

**Entry:** Battle ends (defeat or victory)  
**Exit:** Player decides to retry or quit

**Key Moments:**
1. Game over screen appears - clear outcome
2. Player reflects on what went wrong/right
3. Player sees "Retry" or "Main Menu" options
4. Player decides: "one more turn" vs "done for now"

**Success Criteria:**
- [ ] Player can explain why they lost/won
- [ ] Player wants to retry (at least once)
- [ ] Player feels defeat was fair (not RNG screwed)
- [ ] Player feels victory was earned (not lucky)

**GDD Target Feelings:**
- "Just one more turn" compulsion
- Meaningful choice (player agency mattered)
- Tension curve (pressure built, then released)

---

## Issue Tracking Table

**Instructions:** Each issue gets a unique ID (e.g., UX-001). Update Status as fixes progress. **Last Updated:** December 12, 2025

### Critical Issues (Wave 1 - Fix Immediately)

| ID | Area | Player Symptom | Impact | Freq | Priority | Root Cause | Track Owner | Status |
|----|------|----------------|--------|------|----------|------------|-------------|--------|
| **UX-001** | UI/Clarity | "What do BR, AX, SP mean?" | 🔴 Blocks | 100% | **10.0** | DieVisual uses cryptic 2-letter codes | Phase5 (UI) | 🟡 Known |
| **UX-002** | UI/Discoverability | "I don't know what locking dice does" | 🔴 Blocks | 100% | **5.0** | No action preview panel visible | Phase5 (UI) | 🟡 Known |
| **UX-003** | UI/Guidance | "What phase am I in? What do I do?" | 🔴 Blocks | 100% | **5.0** | No phase banner in scene | Phase5 (UI) | 🟡 Known |
| **UX-004** | Visual/Hierarchy | "Which ones are enemies? Which are brothers?" | 🔴 Blocks | 100% | **5.0** | All characters same brown color, primitive shapes | Track A (Visuals) | 🟡 Known |
| **UX-011** | Visual/Identity | "Which brother is which? They all look the same" | 🔴 Blocks | 100% | **5.0** | No names, all brown capsules | Track A (Visuals) | 🟡 Known |

**Wave 1 Total Fix Time:** ~2.5 hours

---

### High-Impact Issues (Wave 2 - Fix Soon)

| ID | Area | Player Symptom | Impact | Freq | Priority | Root Cause | Track Owner | Status |
|----|------|----------------|--------|------|----------|------------|-------------|--------|
| **UX-006** | Feedback/Juice | "Did my action work? Nothing happened" | 🟠 Hurts | 80% | **4.0** | ScreenEffectsController not wired | Track D (UI Juice) | 🟡 Known |
| **UX-007** | Tutorial | "I wish there was a hint" | 🟠 Hurts | 60% | **3.0** | TutorialManager not wired | Track E (Tutorial) | 🟡 Known |
| **UX-005** | UI/Planning | "I can't tell who is attacking me" | 🟠 Hurts | 100% | **2.5** | No enemy intent indicators | Phase5 (UI) | 🟡 Known |
| **UX-013** | UI/Guidance | "I died. Why? What happened?" | 🟠 Hurts | 100% | **2.5** | Game Over screen lacks explanation | Track F (Menus) | 🟡 Known |
| **UX-012** | Feedback/Brotherhood | "Did my brother block? Can't tell" | 🟠 Hurts | 80% | **2.0** | No visual feedback on brother actions | Track D (UI Juice) | 🟡 Known |

**Wave 2 Total Fix Time:** ~1.5 hours

---

### Polish Issues (Wave 3 - Fix Later)

| ID | Area | Player Symptom | Impact | Freq | Priority | Root Cause | Track Owner | Status |
|----|------|----------------|--------|------|----------|------------|-------------|--------|
| **UX-015** | UI/QoL | "Locked wrong dice, can't undo" | 🟢 Polish | 40% | **0.8** | May already work, need to verify | Phase5 (UI) | 🟡 Known |
| **UX-008** | Visual/Atmosphere | "Looks flat and boring" | 🟢 Polish | 40% | **0.8** | No fog, flat lighting | Track B (Atmosphere) | 🟡 Known |
| **UX-010** | Audio | "Game is silent, no sounds" | 🟢 Polish | 100% | **0.5** | No audio implementation yet | Track D (Audio) | 🟡 Known |
| **UX-014** | Tutorial/QoL | "Can I skip these hints?" | 🟢 Polish | 20% | **0.4** | No skip option in settings | Track E + F | 🟡 Known |
| **UX-009** | Combat/Pacing | "Actions resolve instantly" | 🟢 Polish | 30% | **0.3** | No combat pacing delays | Track F (Combat Timing) | 🟡 Known |

**Wave 3 Total Fix Time:** ~2 hours

### Issue Severity Key

- 🔴 **Blocks** = Prevents core loop understanding or fun (fix immediately)
- 🟠 **Hurts** = Reduces enjoyment or clarity (fix soon)
- 🟢 **Polish** = Nice to have, improves feel (fix when blocking/hurting issues cleared)

---

## Severity Rubric

Use this to score new issues during audits.

### Impact Scoring

| Impact | Definition | Example |
|--------|------------|---------|
| 🔴 **Blocks** | Player cannot understand core loop OR game is not fun at all | Cryptic rune codes, no action preview, can't tell enemies from brothers |
| 🟠 **Hurts** | Player can play but experience is degraded, confusion/frustration present | No enemy intent, no feedback on actions, missing tutorial hints |
| 🟢 **Polish** | Player can play and have fun, but QoL/juice missing | No audio, flat visuals, instant combat resolution |

### Frequency Scoring

| Frequency | Definition |
|-----------|------------|
| **100%** | Every player, every playthrough |
| **80%** | Most players, most playthroughs |
| **60%** | Many players, situational |
| **40%** | Some players, optional content |
| **20%** | Edge case, rare |

### Priority Formula

```
Priority = Impact × Frequency × (1 / Effort)

Impact: Blocks = 10, Hurts = 5, Polish = 2
Frequency: 100% = 1.0, 80% = 0.8, etc.
Effort: Low = 1, Medium = 2, High = 4

Example:
  UX-001 (cryptic rune codes):
  Priority = 10 × 1.0 × (1/1) = 10 → CRITICAL
  
  UX-009 (combat pacing):
  Priority = 2 × 0.3 × (1/2) = 0.3 → LOW
```

**Rule:** Fix all **Blocks** before adding new features.

---

## Track Ownership Guide

Map issues to tracks to avoid cross-track conflicts. Each issue should have ONE owner track.

| Track | Domain | Typical Issues |
|-------|--------|----------------|
| **Phase5 (UI)** | Rune clarity, action preview, phase banner, enemy intent | UX-001, UX-002, UX-003, UX-005 |
| **Track A (Visuals)** | 3D models, materials, visual hierarchy | UX-004 |
| **Track B (Atmosphere)** | Lighting, fog, post-processing, ground | UX-008 |
| **Track C (Scenarios)** | Wave composition, difficulty tuning | (Balance issues) |
| **Track D (Audio)** | Sound effects, music | UX-010 |
| **Track E (Tutorial)** | Tutorial hints, onboarding | UX-007 |
| **Track F (Menus)** | Main menu, scene flow, pause menu | (Menu-specific issues) |
| **Track D (UI Juice)** | Animations, feedback effects, hover states | UX-006 |
| **Track F (Combat Timing)** | Pacing, delays, telegraphs | UX-009 |

**If unsure:** Assign to the track that owns the file/prefab that needs changing.

---

## Audit Process

### When to Run Audits

- **After meaningful UX changes** (e.g., implemented top 3 issues)
- **Weekly** during active development
- **Before playtests** (external or internal)
- **Before major milestones** (demo, release)

### Step 1: Baseline Pass (Internal)

**Time:** 15-20 minutes  
**Setup:** Fresh Battle scene, Wave 1, pretend you're a new player

**Capture:**
1. **Time-to-first-confident-turn** (stopwatch: start at dice roll, stop at "Confirm Actions")
2. **Confusion points** (note timestamps/moments of "what do I do?")
3. **Feedback gaps** (list actions where result was unclear)

**Checklist (Phase5 UX Gates):**
- [ ] Rune Clarity: Can identify rune types without tutorial?
- [ ] Action Preview: Can see what locking dice unlocks?
- [ ] Phase Guidance: Knows current phase and what to do?
- [ ] Enemy Intent: Knows who is being attacked?
- [ ] Combat Feedback: Sees visual response to actions?

**Any failed gate = new Blocks issue** (if not already in backlog)

---

### Step 2: Fun/Tension Pass (Internal)

**Time:** 30-40 minutes  
**Setup:** Play 5 full turns, focus on feelings not mechanics

**Evaluate (GDD Pillars):**

| Pillar | Question | Evidence |
|--------|----------|----------|
| **Brotherhood** | Do I care when a brother is wounded? | Note emotional reaction |
| **Fate** | Do dice rolls feel impactful (not arbitrary)? | Are outcomes interesting? |
| **Endurance** | Does stamina pressure build? | Feel rushed/tense? |
| **Sacrifice** | Are there real tradeoffs? | Can name 2+ competing choices |

**Target Feelings (from GDD):**
- [ ] "Tense, sweaty-palms decision making" (heart rate up?)
- [ ] "Just one more turn" compulsion (want to retry after loss?)
- [ ] "Satisfying dice combo discoveries" (felt clever?)
- [ ] "Genuine dread as wall crumbles" (worried about brothers?)

**Any failed feeling = new Hurts issue**

---

### Step 3: External Micro-Playtests

**Time:** 20-30 min per tester  
**Participants:** 3-5 people (friends, colleagues, online playtest services)

#### Micro-Playtest Script

**Introduction (2 min):**
> "This is Shield Wall, a turn-based Viking dice game. I'm testing UX clarity, not your skills. Please think aloud as you play - say what you see, what you're confused about, what you're trying to do. I'll stay silent and only answer questions after. Ready?"

**Session (15-20 min):**
- Observer takes notes, timestamps, screenshots
- Let player struggle briefly (< 30s) before helping
- Note: first utterance of confusion ("huh?", "what?", "I don't get it")

**Post-Session Questions (5 min):**
1. "What did you think the goal was?"
2. "What did you do on Turn 1? Why?"
3. "What felt strong? What felt weak?"
4. "What made you feel smart? What made you feel helpless?"
5. "On a scale 1-10, how likely are you to play again?"

**Collect:**
- Time-to-first-confident-turn (average across testers)
- Confusion points (common themes)
- Fun moments (what worked)
- Frustration moments (what didn't)

---

### Step 4: Synthesis & Prioritization

**Cluster issues into themes:**
- **Clarity** (runes, phase, intent)
- **Discoverability** (action preview, hints)
- **Feedback/juice** (hit/block/stamina)
- **Pacing** (combat timing/telegraphs)
- **Hierarchy** (enemy vs brother distinction, color palette)

**Score each issue:**
1. Assign Impact (Blocks/Hurts/Polish)
2. Estimate Frequency (% of players affected)
3. Estimate Effort (Low/Medium/High)
4. Calculate Priority = Impact × Frequency × (1/Effort)

**Pick top 10:**
- Sort by Priority score (descending)
- Select top 3-5 **Blocks** issues
- Select top 3-5 **Hurts** issues
- Defer **Polish** until Blocks/Hurts cleared

**Validation:**
- [ ] All Phase5 UX gates that fail are listed
- [ ] All GDD feelings that fail are listed
- [ ] All tester confusion points are captured

---

### Step 5: Assign Tracks & Acceptance Criteria

For each top issue:

**1. Assign Track Owner:**
- Use [Track Ownership Guide](#track-ownership-guide)
- Ensure only ONE track owns the fix (no split work)

**2. Write Acceptance Criteria:**
- Use Phase5 UX Success Criteria format (see Phase5_UXSuccessCriteria.md)
- Make it testable (checkbox, observable behavior)
- Example: "Dice show 'Shield' not 'SH'" ✓

**3. Define Test Steps:**
- Reuse Phase5 validation steps where possible
- Example: "1. Open Battle scene, 2. Press Play, 3. Verify dice labels"

**4. Update Status:**
- 🟡 Known → 🔵 In Progress → ✅ Fixed → ✔️ Validated

---

## Current Backlog Status

**Last Updated:** December 12, 2025

| Severity | Count | Status |
|----------|-------|--------|
| 🔴 Blocks | 4 | 🟡 Known (from ShieldWall_CurrentStateAudit.md) |
| 🟠 Hurts | 3 | 🟡 Known |
| 🟢 Polish | 3 | 🟡 Known |
| **Total** | **10** | **🟡 Backlog Populated** |

**Next Actions:**
1. ✅ Backlog template created (this document)
2. ⏳ Run internal audit (Todo 2)
3. ⏳ Run micro-playtests (Todo 3)
4. ⏳ Score & prioritize (Todo 4)
5. ⏳ Assign tracks (Todo 5)

---

## Success Metrics

Track these over time to see improvement:

| Metric | Baseline | Target | Current |
|--------|----------|--------|---------|
| **Time-to-first-confident-turn** | TBD | < 60s | TBD |
| **Phase5 UX gates passed** | 0/5 | 5/5 | 0/5 |
| **GDD feelings achieved** | 0/4 | 4/4 | 0/4 |
| **Blocks issues count** | 4 | 0 | 4 |
| **Hurts issues count** | 3 | 0-2 | 3 |
| **"Just one more turn" % (testers)** | TBD | > 60% | TBD |

---

## Appendix: Quick-Add Issue Template

Copy this when adding new issues:

```markdown
| **UX-XXX** | [Area] | "[Player symptom in quotes]" | [Impact] | [Freq] | [Root cause] | [Proposed fix 1-3 bullets] | [Track] | [Acceptance criteria] | 🟡 New |
```

**Example:**
```markdown
| **UX-011** | Combat/Balance | "I died on wave 2, felt unfair" | 🟠 Hurts | 60% | Wave 2 too hard for new players | Reduce Wave 2 enemies from 5 to 4, or increase starting stamina | Track C (Scenarios) | New players survive Wave 2 at least 50% of time | 🟡 New |
```

---

**Document Status:** ✅ READY FOR USE

**Cadence:** Update after every audit cycle (weekly during active dev)

**Owner:** Project lead (you!)

**Related Docs:**
- `Assets/Documentation/Phase5/Phase5_UXSuccessCriteria.md` - UX gates
- `Assets/Documentation/ShieldWall_CurrentStateAudit.md` - Known gaps
- `Assets/Documentation/Core/GameDesignDocument.md` - Target experience
