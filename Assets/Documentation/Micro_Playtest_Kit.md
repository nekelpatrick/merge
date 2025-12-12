# Shield Wall - Micro-Playtest Kit

**Purpose:** Conduct 3-5 short playtests (20-30 min) to validate UX fixes and capture player confusion/fun moments  
**When to Use:** After Critical 5 UX fixes implemented (UX-001 to UX-005)  
**Target Participants:** Friends, colleagues, online playtest services, /r/playmygame

---

## Quick Start Checklist

Before running playtests:

- [ ] Critical 5 fixes implemented (see Internal_Audit_Report)
- [ ] Build exported (Windows/Mac standalone)
- [ ] Recording setup ready (OBS, screen capture, or observer notes)
- [ ] Observer form printed/open (see Section 3)
- [ ] Participant lined up (scheduled or recruited)
- [ ] Quiet environment (no distractions)

---

## Section 1: Recruiter Script

Use this to find participants (online forums, Discord, friends):

### Recruitment Post Template

```
**[Playtest Request] Shield Wall - Viking Dice Tactics (20 min)**

I'm developing a turn-based Viking game where you roll rune dice to fight enemies and protect your shield brothers. Looking for 3-5 people to playtest for **UX feedback** (not your skill!).

**What I need:**
- 20-30 minutes of your time
- Think aloud while playing (say what you see, what confuses you)
- Short survey after (5 questions)
- No experience needed - fresh eyes preferred!

**What you get:**
- Early access to the game
- Credit in game (if you want)
- Good karma :)

**Platform:** Windows/Mac (link to build)

**When:** [Your availability]

Interested? Reply or DM me!
```

### Screening (Optional)

If you get many volunteers, screen for:
- ✅ **NEW** to turn-based dice games (best for UX testing)
- ✅ Willing to think aloud
- ✅ Reliable (won't no-show)
- ❌ Avoid close friends/family (too biased)
- ❌ Avoid hardcore strategy gamers (too experienced, won't reflect new player)

---

## Section 2: Session Script

### Pre-Session (2 min)

**Read this to participant verbatim:**

> "Hi! Thanks for helping test Shield Wall. I'm testing **UX clarity**, not your skills, so there's no way to do this 'wrong.'
>
> Here's how this works:
> 1. I'll give you the game and **one sentence** to start
> 2. Please **think aloud** - say what you see, what you're trying to do, what confuses you
> 3. I'll stay **mostly silent** and just observe - I may take notes
> 4. If you're stuck for more than 30 seconds, I'll give a small hint
> 5. After 15-20 minutes, I'll stop you and ask a few questions
>
> Any questions before we start?"

**Answer questions briefly**, then:

> "Okay, here's the build. Your goal: **Survive as long as you can.** Good luck!"

**Start recording/note-taking now.**

---

### During Session (15-20 min)

**Observer Role:**
- 🔇 **Stay silent** (unless tester stuck > 30s)
- 👀 **Watch intently** - note facial expressions, hesitations, moments of joy/frustration
- ✍️ **Take timestamped notes** using Observer Form (Section 3)
- 📷 **Screen record if possible** (or manual notes)

**When to Intervene:**
- Tester stuck > 30 seconds → Give **minimal hint** ("Try clicking a die")
- Tester about to quit → Ask "What's making you want to stop?"
- Tester silent for > 30 seconds → Prompt: "What are you thinking?"

**What NOT to Do:**
- ❌ Don't explain mechanics before they encounter them
- ❌ Don't defend design decisions ("Well actually...")
- ❌ Don't tell them they're "doing it wrong"
- ❌ Don't take it personally if they hate it

**Observation Priorities:**

1. **Time-to-first-confident-turn** (start stopwatch at dice roll, stop when they click "Confirm Actions" intentionally)
2. **Confusion moments** ("Huh?", "What?", "I don't get it", long pauses)
3. **First utterances** (first thing they say reveals first impression)
4. **Emotional reactions** (laughs, groans, "oh no!", "YES!")
5. **Strategic thinking** (do they explain choices? or click randomly?)

---

### Post-Session Interview (5 min)

After 15-20 min (or 3 turns, whichever comes first), stop them:

> "Okay, I'm going to stop you there. Thanks for playing! I have a few quick questions."

**Ask these 5 questions verbatim:**

#### Q1: Goal Understanding
> "What did you think the goal was?"

**Look for:**
- Clear answer ("Survive waves", "Protect brothers", "Don't run out of stamina")
- Confused answer ("Uh... click dice?")
- Wrong answer ("Get high score?")

---

#### Q2: Turn 1 Decision
> "Think back to your first turn. What did you do, and why?"

**Look for:**
- Intentional strategy ("I locked shields to block attacks")
- Exploratory ("I clicked things to see what happens")
- Random ("I don't know, I just clicked")

---

#### Q3: Strength/Weakness
> "What felt strong about the game? What felt weak or confusing?"

**Listen for:**
- Positives (what worked)
- Negatives (what didn't)
- Balance (constructive feedback)

---

#### Q4: Smart/Helpless Moments
> "Was there a moment where you felt smart or clever? A moment where you felt helpless or frustrated?"

**Look for:**
- Smart moment = **good UX** (discovery, mastery)
- Helpless moment = **bad UX** (confusion, unfair)

---

#### Q5: Likelihood to Continue
> "On a scale of 1-10, how likely are you to play this again?"
>
> - 1 = Never
> - 5 = Maybe
> - 10 = Can't wait

**Follow-up if < 7:** "What would make it higher?"

---

### Post-Interview Wrap-Up (1 min)

> "Thank you so much! This is super helpful. Do you have any other thoughts or suggestions?"

**Let them rant/praise** - don't interrupt

**If tester wants credit:**
> "Do you want to be credited in the game? What name should I use?"

**Thank them again** and end session.

---

## Section 3: Observer Form

Copy this template for each session. Fill out during + immediately after session.

---

### **Playtest Session Report**

**Date:** _____________  
**Tester ID:** _________ (T1, T2, T3, etc. - keep anonymous)  
**Experience Level:** [ ] New to genre [ ] Casual [ ] Experienced [ ] Hardcore  
**Platform:** [ ] Windows [ ] Mac [ ] Linux  
**Observer:** _____________

---

### **Timeline & Observations**

| Timestamp | Event | Tester Reaction | Notes |
|-----------|-------|-----------------|-------|
| 0:00 | Game starts | | |
| | First utterance | | |
| | First die locked | | |
| | First confusion | | |
| | First action selected | | |
| | Turn 1 complete | | ✅ Stopwatch: Time-to-first-confident-turn = _____ seconds |
| | Turn 2 complete | | |
| | Turn 3 complete | | |
| | Game Over / Quit | | |

**Total Play Time:** _____ minutes

---

### **Phase5 UX Gate Checklist**

Observe whether tester demonstrates understanding (don't ask directly):

| Gate | Pass? | Evidence |
|------|-------|----------|
| **Rune Clarity** | [ ] Y [ ] N | Could they identify rune types? |
| **Action Preview** | [ ] Y [ ] N | Did they look at preview panel before locking? |
| **Phase Guidance** | [ ] Y [ ] N | Did they know what phase they were in? |
| **Tutorial Hints** | [ ] Y [ ] N | Did hints appear and help? |
| **Enemy Intent** | [ ] Y [ ] N | Did they mention seeing enemy targets? |
| **Combat Feedback** | [ ] Y [ ] N | Did they react to visual feedback? |

**Gates Passed:** ___/6

---

### **Confusion Points**

List every moment where tester said "Huh?", "What?", paused > 10s, or looked lost:

1. **[Timestamp]** _____________________________________________
2. **[Timestamp]** _____________________________________________
3. **[Timestamp]** _____________________________________________
4. **[Timestamp]** _____________________________________________
5. **[Timestamp]** _____________________________________________

**Most Critical Confusion:** ___________________________________________

---

### **Fun/Delight Moments**

List every moment where tester smiled, laughed, said "nice!", "oh cool", or showed excitement:

1. **[Timestamp]** _____________________________________________
2. **[Timestamp]** _____________________________________________
3. **[Timestamp]** _____________________________________________

**Best Moment:** ___________________________________________

---

### **Frustration/Pain Moments**

List every moment where tester groaned, said "ugh", "this is annoying", or showed irritation:

1. **[Timestamp]** _____________________________________________
2. **[Timestamp]** _____________________________________________
3. **[Timestamp]** _____________________________________________

**Worst Moment:** ___________________________________________

---

### **Post-Interview Responses**

**Q1: Goal Understanding**  
Answer: _____________________________________________________________

**Q2: Turn 1 Decision**  
Answer: _____________________________________________________________

**Q3: Strength/Weakness**  
Strengths: ___________________________________________________________  
Weaknesses: __________________________________________________________

**Q4: Smart/Helpless Moments**  
Smart: _______________________________________________________________  
Helpless: ____________________________________________________________

**Q5: Likelihood to Continue (1-10)**  
Score: _____  
Reason: ______________________________________________________________

---

### **Observer Notes**

**First Impression (0-30 seconds):**
___________________________________________________________________
___________________________________________________________________

**Strategic Thinking (Did they plan? Or click randomly?):**
___________________________________________________________________
___________________________________________________________________

**Emotional Attachment (Did they care about brothers?):**
___________________________________________________________________
___________________________________________________________________

**"Just One More Turn" Test (Did they want to continue?):**
[ ] YES - Wanted to keep playing  
[ ] MAYBE - Neutral  
[ ] NO - Wanted to quit

---

### **Top 3 Issues for This Tester**

1. **Issue:** _____________________________________________ (Blocks/Hurts/Polish)
2. **Issue:** _____________________________________________ (Blocks/Hurts/Polish)
3. **Issue:** _____________________________________________ (Blocks/Hurts/Polish)

---

### **Recommended Changes (Observer's Opinion)**

___________________________________________________________________
___________________________________________________________________
___________________________________________________________________

---

**Observer Signature:** _______________ **Date:** _______________

---

## Section 4: Analysis Template

After completing 3-5 sessions, aggregate results using this template.

---

### **Aggregate Playtest Analysis**

**Date Range:** _______________  
**Total Sessions:** _____  
**Total Play Time:** _____ minutes

---

### **Quantitative Metrics**

| Metric | T1 | T2 | T3 | T4 | T5 | Average |
|--------|----|----|----|----|----|----|
| **Time-to-first-confident-turn (sec)** | | | | | | |
| **Phase5 gates passed (0-6)** | | | | | | |
| **Turns completed** | | | | | | |
| **Likelihood to continue (1-10)** | | | | | | |

**Target Benchmarks:**
- Time-to-first-confident-turn: < 60s
- Phase5 gates: ≥ 5/6
- Turns completed: ≥ 3
- Likelihood: ≥ 7/10

**How We Did:**
- [ ] Met all benchmarks ✅
- [ ] Met most benchmarks 🟡
- [ ] Failed benchmarks ❌

---

### **Common Confusion Points**

Cluster confusion moments from all testers. Count frequency:

| Confusion Point | Frequency | Severity | Existing Issue? |
|----------------|-----------|----------|----------------|
| Example: "What do these dice symbols mean?" | 4/5 | 🔴 Blocks | UX-001 |
| | | | |
| | | | |
| | | | |

**Top 3 Confusions:**
1. _______________________________________________________
2. _______________________________________________________
3. _______________________________________________________

---

### **Common Delight Moments**

Cluster fun moments. Count frequency:

| Delight Moment | Frequency | Why It Worked |
|----------------|-----------|---------------|
| Example: "Unlocking 3-axe combo felt badass" | 3/5 | Action preview showed possibility |
| | | |
| | | |

**Keep Doing:**
- _______________________________________________________
- _______________________________________________________

---

### **Common Frustrations**

Cluster pain moments. Count frequency:

| Frustration | Frequency | Severity | Solution |
|-------------|-----------|----------|----------|
| Example: "I died and don't know why" | 5/5 | 🟠 Hurts | Add defeat reason to Game Over |
| | | | |
| | | | |

**Fix Immediately:**
- _______________________________________________________
- _______________________________________________________

---

### **Goal Understanding (Qualitative)**

**How many correctly understood the goal?**
- ___/5 said "Survive waves" or similar ✅
- ___/5 said unclear/wrong ❌

**Common Misunderstandings:**
- _______________________________________________________

---

### **Strategic Thinking (Qualitative)**

**How many showed intentional strategy?**
- ___/5 explained choices ("I blocked because...") ✅
- ___/5 clicked randomly ❌

**Sample Strategic Quote:**
> "_____________________________________________________________________"

---

### **GDD Pillar Validation**

Based on observation, did testers demonstrate:

| Pillar | Evidence? | Quote/Example |
|--------|-----------|---------------|
| **Brotherhood** (cared about brothers) | ___/5 | |
| **Fate** (dice felt impactful) | ___/5 | |
| **Endurance** (felt stamina pressure) | ___/5 | |
| **Sacrifice** (made tradeoffs) | ___/5 | |

**Pillars Achieved:** ___/4

---

### **New Issues Discovered**

List any issues NOT already in backlog:

| ID | Symptom | Impact | Frequency | Proposed Fix |
|----|---------|--------|-----------|--------------|
| UX-016 | | | | |
| UX-017 | | | | |
| UX-018 | | | | |

---

### **Validation Summary**

**Phase5 UX Gates:**
- Rune Clarity: ___/5 passed
- Action Preview: ___/5 passed
- Phase Guidance: ___/5 passed
- Tutorial Hints: ___/5 passed
- Enemy Intent: ___/5 passed
- Combat Feedback: ___/5 passed

**Overall UX Health:** ___/30 points (target: ≥25)

---

### **Recommendations**

**Critical Fixes (Before Next Playtest):**
1. _______________________________________________________
2. _______________________________________________________
3. _______________________________________________________

**Important Improvements:**
1. _______________________________________________________
2. _______________________________________________________

**Polish (Later):**
1. _______________________________________________________
2. _______________________________________________________

---

### **Next Steps**

- [ ] Update UX_Fun_Backlog.md with new issues
- [ ] Prioritize fixes using severity rubric
- [ ] Implement top 3 fixes
- [ ] Schedule follow-up playtest in ___ weeks

---

**Analysis Complete By:** _______________ **Date:** _______________

---

## Section 5: Online Playtest Services

If you don't have 3-5 friends/colleagues, use these services:

### Free Options

**r/playmygame (Reddit)**
- Post your recruitment script
- Free, but quality varies
- Expect 0-3 volunteers

**Game Dev Discord Servers**
- Post in #playtesting channels
- Free, dev community (experienced players)

**Twitter/Social Media**
- Post request with #gamedev #indiedev #playtesting
- Free, but reach depends on followers

### Paid Options

**PlaytestCloud** (playtestcloud.com)
- $99-299 per test (5 sessions)
- Professional testers, video recordings included
- Best quality, fastest turnaround

**UserTesting** (usertesting.com)
- $49 per tester
- General audience (not gamers specifically)
- Good for UX-focused tests

**BetaFamily** (betafamily.com)
- Free tier available
- Mobile-focused but supports PC
- Community of testers

### Recommended: Start Free, Then Paid

1. First playtest: Friends (free, fast feedback)
2. Second playtest: Reddit/Discord (free, wider audience)
3. Third playtest: PlaytestCloud (paid, professional)

---

## Section 6: Troubleshooting

### "No one volunteered for my playtest"

**Solutions:**
- Offer incentive (Steam key, game credit, $5 gift card)
- Post in multiple subreddits (/r/gamedev, /r/playmygame, /r/incremental_games)
- DM friends directly (don't mass-post)
- Use paid service (PlaytestCloud)

---

### "Tester quit after 2 minutes"

**This is GOOD feedback!**

**Do:**
- Ask why they wanted to quit (most important data)
- Note exactly when they gave up (timestamp)
- Thank them for honest feedback

**Don't:**
- Convince them to continue
- Take it personally
- Dismiss as "they just don't get it"

---

### "Tester found a game-breaking bug"

**Do:**
- Apologize, end session early
- Fix bug immediately
- Invite them back after fix (if willing)

**Don't:**
- Continue session (wastes their time)
- Blame Unity/engine/laptop

---

### "All testers confused by the same thing"

**This is CRITICAL data!**

**Do:**
- Mark as 🔴 Blocks issue (100% frequency)
- Fix immediately before next playtest
- Update backlog with evidence ("5/5 testers confused")

---

### "Testers disagree on what's fun"

**This is NORMAL!**

**Do:**
- Look for patterns (3/5 agree = trend)
- Weight feedback by target audience (new players > experts)
- Consider multiple solutions

**Don't:**
- Try to please everyone
- Average contradictory feedback

---

## Section 7: Playtest Log

Track all sessions in one place:

| Date | Tester ID | Platform | Time | Gates (0-6) | Likelihood (1-10) | Top Issue | Status |
|------|-----------|----------|------|-------------|-------------------|-----------|--------|
| Dec 12 | T1 | Win | 22m | 3/6 | 6/10 | No action preview | Needs fix |
| | | | | | | | |
| | | | | | | | |

---

**Playtest Kit Status:** ✅ READY TO USE

**Next Step:** After Critical 5 fixes, recruit 3-5 testers and run sessions

**Expected Outcomes:**
- Identify remaining UX blockers
- Validate Phase5 fixes work
- Discover 3-5 new issues
- Establish baseline metrics

---

**Document Maintenance:** Update this kit after each playtest round with learnings
