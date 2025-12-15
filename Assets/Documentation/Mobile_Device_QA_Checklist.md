# Shield Wall - Mobile Device QA Checklist

## Gate 7: Device Matrix Testing & Regression Fixes

This checklist ensures Shield Wall runs correctly across the target device matrix (Android + iOS, landscape).

---

## Pre-Testing Setup

### Required Tools
- [ ] Unity Remote app (for quick iteration)
- [ ] adb (Android Debug Bridge) installed
- [ ] Xcode (for iOS testing)
- [ ] Unity Profiler configured for remote profiling

### Test Build Configuration
- [ ] Development Build: ✅ Enabled
- [ ] Script Debugging: ✅ Enabled (optional)
- [ ] Deep Profiling Support: ✅ Enabled (optional, slower)
- [ ] Compression: LZ4 (faster deploy during testing)

### Device Setup
- [ ] USB debugging enabled (Android)
- [ ] Developer certificate trusted (iOS)
- [ ] Devices fully charged
- [ ] Close background apps (clean test environment)

---

## Device Matrix

### Android Devices (Minimum 2)

**Low-Tier Device** (Baseline):
- [ ] Device: _____________ (e.g., Samsung Galaxy A32, Snapdragon 660)
- [ ] OS Version: _____________
- [ ] GPU: _____________
- [ ] RAM: _____________

**Mid-Tier Device** (Target):
- [ ] Device: _____________ (e.g., Google Pixel 5, Snapdragon 730)
- [ ] OS Version: _____________
- [ ] GPU: _____________
- [ ] RAM: _____________

### iOS Devices (Minimum 2)

**Older Device** (Baseline):
- [ ] Device: _____________ (e.g., iPhone 8, A11)
- [ ] OS Version: _____________

**Modern Device** (Target):
- [ ] Device: _____________ (e.g., iPhone 11, A13)
- [ ] OS Version: _____________

---

## Test Pass 1: Smoke Test (All Devices)

**Goal**: App launches and core loop works

### Install & Launch
- [ ] APK/IPA installs without errors
- [ ] App launches to MainMenu (no black screen)
- [ ] No immediate crash on launch
- [ ] Splash screen displays correctly (if present)

### MainMenu Flow
- [ ] All menu buttons visible (not clipped)
- [ ] Buttons are tappable (touch targets work)
- [ ] "Play" button loads Battle scene
- [ ] "Scenarios" button shows scenario cards (if implemented)
- [ ] "Settings" button opens settings panel
- [ ] "Quit" button exits app (Android) or returns to home (iOS)

### Battle Core Loop
- [ ] Battle scene loads successfully
- [ ] Camera view is correct (first-person, landscape)
- [ ] Dice appear and are visible
- [ ] **Tap to lock dice** works (visual feedback shown)
- [ ] **Reroll button** works
- [ ] **Action buttons** appear when combos are formed
- [ ] **Select action** → tap action button works
- [ ] **End Turn button** triggers combat resolution
- [ ] Combat resolution completes (no hang)
- [ ] **One full turn completes** without crash

### Pause & Resume
- [ ] On-screen pause button is visible and tappable
- [ ] Tapping pause shows pause menu
- [ ] Pause menu has working buttons (Resume, Restart, Quit)
- [ ] Resume returns to Battle correctly
- [ ] Restart reloads Battle scene
- [ ] Quit returns to MainMenu

---

## Test Pass 2: UI & Safe Area (Notched Devices Only)

**Goal**: UI is not clipped by notches, punch-holes, or rounded corners

### MainMenu Safe Area
- [ ] Top menu elements not clipped by notch
- [ ] Bottom buttons not clipped by home indicator
- [ ] Side buttons not clipped by curved edges
- [ ] All text readable
- [ ] All buttons reachable

### Battle HUD Safe Area
- [ ] Dice UI not clipped (bottom of screen)
- [ ] Health/Stamina not clipped (top-left)
- [ ] Wave/Enemy UI not clipped (top-right)
- [ ] Action buttons not clipped
- [ ] Pause button not clipped (top corner)
- [ ] Phase banner not clipped

### Orientation Changes (Sensor Landscape)
- [ ] Rotate device 180° → UI adapts correctly
- [ ] No layout breakage
- [ ] Touch targets remain functional

---

## Test Pass 3: Performance & Frame Rate

**Goal**: Meet target frame rates on each device tier

### Performance Targets
- **Low-Tier**: Stable 30 FPS, no hitches
- **Mid-Tier**: Stable 60 FPS, minor drops acceptable
- **High-Tier**: Locked 60 FPS

### Test Scenarios

**Scenario 1: Idle Turn (Player Thinking)**
- [ ] Frame rate stable at target
- [ ] No unexpected GPU/CPU spikes
- [ ] UI animations smooth

**Scenario 2: Dice Roll Animation**
- [ ] Animation plays smoothly
- [ ] Frame rate dip <10 FPS (acceptable)
- [ ] Recovers to stable after animation

**Scenario 3: Combat Resolution (Multiple Enemies)**
- [ ] Multiple VFX spawns (blood, blocks)
- [ ] Post-processing effects (damage vignette)
- [ ] Frame rate remains above minimum target
- [ ] No visible stuttering

**Scenario 4: Wave Transition**
- [ ] Phase banner animates smoothly
- [ ] Enemy spawn doesn't cause hitch
- [ ] Turn state transitions cleanly

### Profiler Capture (Mid-Tier Device)
- [ ] Capture 5-minute Battle session with Unity Profiler
- [ ] CPU Main Thread average: < 10ms (for 60 FPS)
- [ ] GPU Frame Time average: < 12ms (for 60 FPS)
- [ ] GC allocations per turn: < 100 KB
- [ ] No GC spikes > 5ms

---

## Test Pass 4: Lifecycle & Backgrounding

**Goal**: App handles backgrounding/foregrounding gracefully

### Background/Foreground Cycle
- [ ] **Mid-Battle**: Tap home button (Android) or swipe up (iOS)
- [ ] App backgrounds without crash
- [ ] Audio stops/pauses correctly
- [ ] Return to app → Battle resumes in paused state
- [ ] Can resume gameplay normally

### Lock Screen Cycle
- [ ] Mid-Battle: Press power button to lock screen
- [ ] Return from lock screen
- [ ] Battle resumes in paused state
- [ ] No data loss

### Interruptions
- [ ] Incoming call/notification → app backgrounds → returns correctly
- [ ] Music app playing in background → game audio respects system settings

### Extended Background (Memory Pressure)
- [ ] Background app for 5+ minutes
- [ ] Return to app
- [ ] If scene reloads, no crash
- [ ] If scene preserved, state is intact

---

## Test Pass 5: Touch UX & Responsiveness

**Goal**: Touch interactions feel responsive and accurate

### Touch Target Sizes
- [ ] All buttons meet minimum size (80x80 pixels)
- [ ] Dice are easy to tap accurately
- [ ] Action buttons have adequate spacing (no mis-taps)
- [ ] Reroll button distinct from dice

### Touch Feedback
- [ ] Buttons show visual feedback on tap (scale/color change)
- [ ] Dice show lock/unlock state clearly
- [ ] Selected actions highlighted
- [ ] No "dead zones" where taps are ignored

### Gesture Conflicts
- [ ] Tapping dice doesn't accidentally trigger swipe
- [ ] No conflict with system gestures (back, home, recent apps)

---

## Test Pass 6: Memory & Stability

**Goal**: No crashes or memory issues in extended session

### 10-Minute Session
- [ ] Play continuously for 10 minutes (multiple waves)
- [ ] No crash
- [ ] No memory leak (check via Profiler if possible)
- [ ] No progressive slowdown
- [ ] No audio glitches over time

### Scene Transitions
- [ ] MainMenu → Battle → Restart → Battle (repeat 5 times)
- [ ] No crash during transitions
- [ ] No noticeable memory growth
- [ ] Scene loads remain fast

### Edge Cases
- [ ] Die on Wave 1 → Game Over → Restart
- [ ] Complete Wave 5 → Victory → Return to Menu
- [ ] Pause mid-turn → Restart → Should work
- [ ] Spam tap buttons rapidly → No crash or state corruption

---

## Test Pass 7: Quality Settings (If Implemented)

**Goal**: Quality profiles switch correctly and persist

### Profile Switching
- [ ] Open Settings → Select "Low" profile
- [ ] Visual quality degrades (shadows off, lower res)
- [ ] Frame rate improves
- [ ] Return to menu → Restart app → Profile persists

### Individual Toggles
- [ ] Toggle "Reduce VFX" → Less blood/particles
- [ ] Toggle "Disable Post" → No vignette/chromatic aberration
- [ ] Toggle "Reduce Shadows" → Shadows disabled
- [ ] FPS dropdown → Switch 30/60 → Frame rate changes

---

## Regression Tracking

### Issue Template

**Issue ID**: MOB-QA-XXX  
**Device**: [Device name]  
**OS Version**: [Version]  
**Build**: [Version/commit hash]  
**Priority**: 🔴 Critical / 🟠 High / 🟡 Medium / 🟢 Low

**Steps to Reproduce**:
1. Step 1
2. Step 2
3. ...

**Expected Behavior**:
...

**Actual Behavior**:
...

**Screenshots/Video**:
[Attach if available]

**Frequency**:
- [ ] Always reproducible
- [ ] Intermittent (XX% of time)

**Track Owner**: [Track A/B/C/D/E/F/G]

**Fix Required**: [Yes/No/Deferred]

---

## Triage & Fix Process

### Priority Classification

**🔴 Critical (Must Fix)**:
- Crashes on launch
- Can't complete one turn
- Data loss
- App rejects (store submission blockers)

**🟠 High (Should Fix)**:
- Frame rate < 20 FPS on mid-tier
- UI clipping on common devices
- Backgrounding broken
- Audio glitches

**🟡 Medium (Nice to Fix)**:
- Minor visual glitches
- Frame rate 20-30 FPS on mid-tier (target 60)
- Non-critical UX issues

**🟢 Low (Defer)**:
- Polish issues
- Edge case bugs
- Optimization opportunities

### Fix Assignment by Track

| Issue Type | Owner Track |
|------------|-------------|
| Crash/logic error | Track G (Platform) or original owner |
| UI clipping/scaling | Track F (UI/UX) |
| Rendering/shadows | Track B (Rendering) |
| VFX/visual perf | Track A (VFX) |
| Audio | Track D (Audio) |
| Combat/gameplay | Original track owner |

---

## Final Sign-Off Criteria

All critical success criteria from Mobile_Targets.md must pass:

- [ ] **Android**: Installs + runs (IL2CPP/ARM64), reaches MainMenu and Battle
- [ ] **iOS**: Runs on device, reaches MainMenu and Battle
- [ ] **Landscape UI**: No clipped UI on notched devices (safe area handled)
- [ ] **Touch loop**: Roll dice → lock → reroll → select action → end turn → pause/resume
- [ ] **Performance**: Stable frame pacing on baseline devices (60 mid-tier, 30 low-tier)
- [ ] **Battery/lifecycle**: App auto-pauses on background; returns safely; audio behaves

### Final QA Approval

**Tested By**: _____________  
**Date**: _____________  
**Approval**: ✅ PASS / ❌ FAIL (re-test required)

**Critical Issues Remaining**: ___  
**High Issues Remaining**: ___  
**Medium Issues Remaining**: ___

**Notes**:
...

---

**Status**: ✅ QA checklist complete  
**Action Required**: Execute test passes on device matrix  
**Owner**: All tracks (triage by area)

