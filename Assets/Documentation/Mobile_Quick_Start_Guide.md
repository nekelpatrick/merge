# Shield Wall Mobile - Quick Start Guide

**Status**: ✅ Code Implementation Complete  
**Next Step**: Manual Unity Editor Configuration + Scene Integration

---

## What's Been Done (Automatic)

✅ **18 new files created** (scripts + documentation)  
✅ **3 files modified** (GameEvents, PauseMenuUI, PostProcessController)  
✅ **No linting errors**  
✅ **All code follows Phase-Track methodology**  
✅ **Event-driven architecture maintained**

---

## What You Need to Do (Manual - ~2 hours)

### Step 1: Configure Unity Build Settings (30 min)

**Android**:
1. File → Build Settings → Switch to Android
2. Player Settings → Follow ALL steps in `Mobile_Android_Build_Guide.md`
   - Set orientation to Landscape
   - Set IL2CPP + ARM64
   - Configure package name
   - Set quality defaults

**iOS**:
1. File → Build Settings → Switch to iOS (or add both)
2. Player Settings → Follow ALL steps in `Mobile_iOS_Build_Guide.md`
   - Set orientation to Landscape
   - Set IL2CPP + ARM64
   - Configure bundle identifier
   - Set signing team

### Step 2: Add Mobile Components to Scenes (15 min)

**MainMenu Scene**:
1. Open `Assets/Scenes/MainMenu.unity`
2. Create empty GameObject: `MobileBootstrap`
3. Add components:
   - `MobilePlatformBootstrapper`
   - `MobileLifecycleHandler`
4. Find main UI panel (contains all buttons)
5. Add component: `SafeAreaFitter`
6. Save scene

**Battle Scene**:
1. Open `Assets/Scenes/Battle.unity`
2. Find Battle HUD root panel
3. Add component: `SafeAreaFitter`
4. (Optional) Add on-screen pause button - see `Mobile_UI_Integration_Guide.md` MOB-044
5. Save scene

### Step 3: Create Quality Profile Assets (15 min)

1. Create folder: `Assets/ScriptableObjects/Settings/MobileQuality/`
2. Follow `Mobile_Quality_Profiles_Guide.md` to create 5 assets:
   - Low, Medium, High
   - Android_Default
   - iOS_Default
3. Select `MobileBootstrap` GameObject in MainMenu scene
4. Assign Android/iOS default profiles to `MobilePlatformBootstrapper` component
5. Save

### Step 4: Build & Test (1-2 hours)

**Android**:
```
File → Build Settings → Build (or Build and Run)
Install on device
Run Mobile_Device_QA_Checklist.md test passes
```

**iOS**:
```
File → Build Settings → Build
Open Xcode project
Sign & deploy to device
Run Mobile_Device_QA_Checklist.md test passes
```

---

## Optional Optimizations (Later)

After initial device testing, if performance is not acceptable:

1. **URP Settings**: Follow `Mobile_Performance_Optimization_Guide.md` MOB-060
2. **VFX Limits**: Follow guide MOB-062
3. **Audio Settings**: Follow guide MOB-064 (batch import settings)
4. **UI Overdraw**: Follow guide MOB-063

---

## Documentation Roadmap

| Document | Purpose | When to Use |
|----------|---------|-------------|
| `Mobile_Targets.md` | Device specs, budgets | Reference during optimization |
| `Mobile_Android_Build_Guide.md` | Android PlayerSettings | Before first Android build |
| `Mobile_iOS_Build_Guide.md` | iOS PlayerSettings | Before first iOS build |
| `Mobile_UI_Integration_Guide.md` | Scene setup (safe area, pause button) | Step 2 above |
| `Mobile_Quality_Profiles_Guide.md` | Quality asset creation | Step 3 above |
| `Mobile_Performance_Optimization_Guide.md` | Optimization techniques | After profiling identifies bottlenecks |
| `Mobile_Device_QA_Checklist.md` | Testing protocol | Step 4 above |
| `Mobile_Implementation_Summary.md` | What was done + full file list | Overview/handoff |
| `Mobile_Quick_Start_Guide.md` | This file | Start here |

---

## Key Scripts Created

**Platform Bootstrap**:
- `MobilePlatformBootstrapper.cs` - FPS, sleep, quality profile loading
- `MobileLifecycleHandler.cs` - Pause/resume on background

**UI**:
- `SafeAreaFitter.cs` - Notch/cutout safe area
- `MobileSettingsUI.cs` - In-game quality/perf settings

**Data**:
- `MobileQualityProfileSO.cs` - Quality profile definition

**Utilities**:
- `DebugLogger.cs` - Conditional logging (stripped in release)

---

## Common Issues

**Issue**: Build fails with IL2CPP errors  
**Fix**: Ensure NDK/SDK installed via Unity Hub

**Issue**: Black screen on device  
**Fix**: Check Graphics API order (Vulkan first for Android, Metal for iOS)

**Issue**: UI clipped by notch  
**Fix**: Ensure `SafeAreaFitter` is on the ROOT panel, not child elements

**Issue**: Frame rate is low  
**Fix**: Profile first (see `Mobile_Performance_Baseline.md`), then optimize using `Mobile_Performance_Optimization_Guide.md`

**Issue**: App crashes on background  
**Fix**: Check console for errors; verify `MobileLifecycleHandler` is active

---

## Success Criteria Checklist

Before considering mobile support "complete":

- [ ] Android APK installs and launches
- [ ] iOS app installs and launches
- [ ] Can complete one full turn on both platforms
- [ ] UI is not clipped on notched devices
- [ ] App pauses when backgrounded
- [ ] App resumes when foregrounded
- [ ] Frame rate is acceptable (30+ FPS low-tier, 60 FPS mid-tier)
- [ ] No crashes in 10-minute session
- [ ] Quality profiles can be switched and persist

---

## Get Help

If stuck, check:
1. Relevant guide in `Assets/Documentation/Mobile_*.md`
2. Console logs (especially `[MobilePlatform]` and `[SafeAreaFitter]` tags)
3. Unity Profiler (for performance issues)
4. Device console (adb logcat for Android, Xcode console for iOS)

---

**START HERE** → Step 1 → Step 2 → Step 3 → Step 4 → Test → Optimize (if needed)

Good luck! 🎮📱

