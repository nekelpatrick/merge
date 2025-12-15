# Shield Wall - Mobile Quality Profiles Guide

## MOB-051 & MOB-052: Creating Quality Profile Assets

This guide explains how to create mobile quality profile assets for Android and iOS.

---

## Quick Setup

### 1. Create Profile Assets Directory

1. In Unity Project window, navigate to `Assets/ScriptableObjects/`
2. Create new folder: `Settings` (if it doesn't exist)
3. Inside Settings, create folder: `MobileQuality`

Final path: `Assets/ScriptableObjects/Settings/MobileQuality/`

---

## 2. Create Low Quality Profile

**Right-click in `MobileQuality` folder → Create → ShieldWall → Mobile → Quality Profile**

**Asset Name**: `MobileQualityProfile_Low`

**Configuration**:
```
Profile Info:
  Profile Name: "Low"
  Description: "Optimized for low-end devices (Snapdragon 660, iPhone 8). Target: 30 FPS stable."
  Target Platform: Android (or iOS)

Rendering:
  Render Scale: 0.7
  MSAA Quality: 0
  Enable HDR: ❌ Disabled

Shadows:
  Enable Shadows: ❌ Disabled
  Shadow Resolution: _512 (ignored if disabled)
  Shadow Distance: 15

Post-Processing:
  Enable Post Processing: ✅ Enabled
  Enable Bloom: ❌ Disabled
  Enable Vignette: ✅ Enabled (damage feedback only)
  Enable Chromatic Aberration: ❌ Disabled
  Enable Color Grading: ✅ Enabled (minimal)

VFX:
  Particle Multiplier: 0.5
  Max Particles Per Effect: 25
  Enable Blood VFX: ✅ Enabled (reduced count)
  Enable Dismemberment: ❌ Disabled (expensive)

Performance:
  Target Frame Rate: 30
  Enable VSync: ❌ Disabled
  Pixel Light Count: 1

Audio:
  Force Mono: ✅ Enabled
  Audio Quality Scale: 0.7
```

---

## 3. Create Medium Quality Profile

**Asset Name**: `MobileQualityProfile_Medium`

**Configuration**:
```
Profile Info:
  Profile Name: "Medium"
  Description: "Balanced for mid-tier devices (Snapdragon 730, iPhone 11). Target: 60 FPS."
  Target Platform: Android (or iOS)

Rendering:
  Render Scale: 0.85
  MSAA Quality: 2
  Enable HDR: ❌ Disabled

Shadows:
  Enable Shadows: ✅ Enabled
  Shadow Resolution: _1024
  Shadow Distance: 20

Post-Processing:
  Enable Post Processing: ✅ Enabled
  Enable Bloom: ❌ Disabled (still expensive)
  Enable Vignette: ✅ Enabled
  Enable Chromatic Aberration: ✅ Enabled
  Enable Color Grading: ✅ Enabled

VFX:
  Particle Multiplier: 0.75
  Max Particles Per Effect: 50
  Enable Blood VFX: ✅ Enabled
  Enable Dismemberment: ✅ Enabled (limited)

Performance:
  Target Frame Rate: 60
  Enable VSync: ❌ Disabled
  Pixel Light Count: 1

Audio:
  Force Mono: ❌ Disabled
  Audio Quality Scale: 0.85
```

---

## 4. Create High Quality Profile

**Asset Name**: `MobileQualityProfile_High`

**Configuration**:
```
Profile Info:
  Profile Name: "High"
  Description: "Full quality for high-end devices (Snapdragon 865+, iPhone 12+). Target: 60 FPS."
  Target Platform: Android (or iOS)

Rendering:
  Render Scale: 1.0
  MSAA Quality: 4
  Enable HDR: ❌ Disabled (mobile limitation)

Shadows:
  Enable Shadows: ✅ Enabled
  Shadow Resolution: _2048
  Shadow Distance: 30

Post-Processing:
  Enable Post Processing: ✅ Enabled
  Enable Bloom: ✅ Enabled (subtle)
  Enable Vignette: ✅ Enabled
  Enable Chromatic Aberration: ✅ Enabled
  Enable Color Grading: ✅ Enabled

VFX:
  Particle Multiplier: 1.0
  Max Particles Per Effect: 100
  Enable Blood VFX: ✅ Enabled
  Enable Dismemberment: ✅ Enabled (full)

Performance:
  Target Frame Rate: 60
  Enable VSync: ❌ Disabled
  Pixel Light Count: 2

Audio:
  Force Mono: ❌ Disabled
  Audio Quality Scale: 1.0
```

---

## 5. Create Platform-Specific Defaults

### Android Default

**Asset Name**: `MobileQualityProfile_Android_Default`

- **Copy** the **Medium** profile settings
- Set `Target Platform: Android`
- This will be auto-selected on Android devices if no user preference exists

### iOS Default

**Asset Name**: `MobileQualityProfile_iOS_Default`

- **Copy** the **Medium** profile settings (or **High** if targeting newer iPhones)
- Set `Target Platform: IPhonePlayer`
- This will be auto-selected on iOS devices

---

## 6. Wire Profiles to MobilePlatformBootstrapper

1. **Open any persistent scene** (e.g., create a "MobileBootstrap" scene or add to MainMenu)
2. Create empty GameObject: `MobilePlatformBootstrapper`
3. Add component: `MobilePlatformBootstrapper`
4. Configure in Inspector:

```
Quality Profiles:
  Android Default Profile: [Drag] MobileQualityProfile_Android_Default
  iOS Default Profile: [Drag] MobileQualityProfile_iOS_Default
  Allow Profile Override: ✅ Enabled

Frame Rate Settings (Fallback):
  Target Frame Rate: 60
  Disable VSync: ✅ Enabled

Screen Settings:
  Prevent Sleep During Gameplay: ✅ Enabled
  Sleep Timeout: -1

Debug:
  Log Settings: ✅ Enabled (for testing)
```

5. **Save scene**

---

## 7. Optional: Create Resources Folder for Runtime Switching

If you want users to switch profiles at runtime (via MobileSettingsUI):

1. Create folder: `Assets/Resources/MobileQuality/`
2. **Copy** all profile assets to this folder:
   - MobileQualityProfile_Low
   - MobileQualityProfile_Medium
   - MobileQualityProfile_High
3. MobileSettingsUI will be able to load these via `Resources.Load()`

---

## 8. Wire Profiles to MobileSettingsUI (Optional)

If creating in-game settings UI:

1. Find or create Settings panel in MainMenu scene
2. Add `MobileSettingsUI` component
3. Configure:

```
References:
  Platform Bootstrapper: [Drag] MobilePlatformBootstrapper GameObject

Quality Profile Selection:
  Available Profiles (Array):
    Element 0: MobileQualityProfile_Low
    Element 1: MobileQualityProfile_Medium
    Element 2: MobileQualityProfile_High
  Profile Dropdown: [Drag] TMP_Dropdown UI element
  Profile Description Text: [Drag] TextMeshProUGUI for description

Individual Toggles:
  Reduce VFX Toggle: [Drag] Toggle UI element
  Disable Post Process Toggle: [Drag] Toggle UI element
  Reduce Shadows Toggle: [Drag] Toggle UI element

Frame Rate Selection:
  FPS Dropdown: [Drag] TMP_Dropdown (30 FPS / 60 FPS options)

Debug:
  Current Profile Text: [Drag] TextMeshProUGUI for status display
```

---

## Testing Profiles

### Via Context Menu (Inspector)

1. Select profile asset
2. In Inspector, right-click script name
3. Select "Apply" from context menu
4. Profile settings applied to current Unity session

### Via Bootstrapper

1. Enter Play Mode
2. Check Console for log:
   ```
   [MobilePlatform] Applied profile: Medium
     Render Scale: 0.85
     MSAA: 2x
     ...
   ```
3. Verify settings in Quality Settings window

### Via Device Testing

1. Build to device (Android/iOS)
2. Run app
3. Open device console (adb logcat or Xcode console)
4. Look for `[MobilePlatform]` logs
5. Verify correct profile is loaded

---

## Profile Selection Logic

```
MobilePlatformBootstrapper.Awake()
  ↓
Check PlayerPrefs for saved profile name
  ↓ (if saved)
Load profile from Resources/MobileQuality/{name}
  ↓ (if not saved or load failed)
Load default profile for platform (Android/iOS)
  ↓
profile.Apply()
  ↓
Update QualitySettings + URP settings
```

---

## Common Issues

### Issue: Profile not applying
- **Fix**: Ensure profile is assigned in MobilePlatformBootstrapper Inspector
- **Fix**: Check Console for errors during Apply()

### Issue: Settings revert after restart
- **Fix**: Verify `PlayerPrefs.Save()` is called in MobileSettingsUI
- **Fix**: Check platform has persistent storage permissions

### Issue: Runtime switching doesn't work
- **Fix**: Ensure profiles are in `Assets/Resources/MobileQuality/` folder
- **Fix**: Profile file name must match `profile.profileName` field

---

**Status**: ✅ Profile creation guide complete  
**Action Required**: Create 5 profile assets + wire to bootstrapper  
**Owner**: Track G (Mobile Platform) + Track F (Mobile UI)

