# Shield Wall - Android Build Configuration Guide

## MOB-010: Android PlayerSettings Configuration

### Required Settings (Manual Configuration in Unity Editor)

Navigate to **Edit → Project Settings → Player → Android**

#### 1. Company & Product
- **Company Name**: `[Your Company]`
- **Product Name**: `Shield Wall`
- **Version**: `0.1.0` (mobile alpha)
- **Bundle Version Code**: `1`

#### 2. Icon & Splash
- **Override for Android**: ✅ Enabled
- **Adaptive Icon**: Provide launcher icons (currently using defaults)

#### 3. Resolution and Presentation
- **Default Orientation**: `Landscape Left`
- **Allowed Orientations for Auto Rotation**:
  - Portrait: ❌ Disabled
  - Portrait Upside Down: ❌ Disabled
  - Landscape Right: ✅ Enabled
  - Landscape Left: ✅ Enabled
- **32-bit Display Buffer**: ✅ Enabled (better color on OLED screens)
- **Render Outside Safe Area**: ❌ Disabled (we'll handle safe area manually)

#### 4. Other Settings

**Rendering**
- **Color Space**: Linear
- **Auto Graphics API**: ❌ Disabled
- **Graphics APIs** (ordered):
  1. Vulkan
  2. OpenGL ES 3
- **Multithreaded Rendering**: ✅ Enabled
- **Static Batching**: ✅ Enabled
- **Dynamic Batching**: ✅ Enabled (limited benefit but no harm)
- **GPU Skinning**: ✅ Enabled

**Identification**
- **Package Name**: `com.[yourcompany].shieldwall`
- **Minimum API Level**: Android 7.0 'Nougat' (API level 24)
- **Target API Level**: Automatic (Highest Installed)

**Configuration**
- **Scripting Backend**: IL2CPP
- **API Compatibility Level**: .NET Standard 2.1
- **Target Architectures**:
  - ARMv7: ❌ Disabled (deprecated)
  - ARM64: ✅ Enabled

**Optimization**
- **Managed Stripping Level**: Minimal (safe for first mobile build)
- **Strip Engine Code**: ✅ Enabled
- **Optimize Mesh Data**: ✅ Enabled

**Logging** (Development builds only)
- **Development Build**: ✅ Enabled for initial testing
- **Script Debugging**: ✅ Enabled for initial testing
- **Wait For Managed Debugger**: ❌ Disabled

#### 5. Publishing Settings
- **Build System**: Gradle
- **Custom Gradle Properties Template**: ❌ Not needed initially
- **Custom Launcher Gradle Template**: ❌ Not needed initially

---

## MOB-011: Android Quality Settings

### Default Quality Level for Android
Navigate to **Edit → Project Settings → Quality**

1. Set **Android** platform override to use **"Medium"** quality level (or create "Mobile" preset)
2. Recommended mobile quality settings:
   - **Pixel Light Count**: 1
   - **Texture Quality**: Full Res (we'll use asset import settings instead)
   - **Anisotropic Textures**: Per Texture
   - **Anti Aliasing**: 2x Multi Sampling (adjust based on device testing)
   - **Soft Particles**: ✅ Enabled
   - **Realtime Reflection Probes**: ❌ Disabled
   - **Shadows**:
     - Shadow Quality: Medium
     - Shadow Resolution: Medium (1024)
     - Shadow Distance: 20
     - Shadow Cascades: No Cascades

### URP Asset for Android
Navigate to **Assets/Settings/** and locate the active URP pipeline asset

Create or adjust mobile-specific URP settings:
- **Render Scale**: 0.85
- **MSAA**: 2x
- **HDR**: ❌ Disabled (mobile GPUs)
- **Depth Texture**: Only if Required (saves bandwidth)
- **Opaque Downsampling**: None
- **Terrain Holes**: ❌ Disabled

---

## MOB-012: Build & Deploy Checklist

### Before Building
1. ✅ All scenes added to **Build Settings** (File → Build Settings)
   - MainMenu.unity
   - Battle.unity
2. ✅ Android platform selected
3. ✅ Development Build enabled (for initial testing)
4. ✅ USB debugging enabled on test device
5. ✅ ADB drivers installed

### Build Process
1. **File → Build Settings**
2. Switch to **Android** platform (if not already)
3. Click **Player Settings** and verify all above settings
4. Click **Build** or **Build and Run**
5. Choose output location (e.g., `Builds/Android/`)
6. Wait for IL2CPP compilation (first build will be slow, 10-20 minutes)

### Testing on Device
1. Install APK via ADB or Build and Run
2. Launch app
3. **Smoke Test Checklist**:
   - [ ] App launches without crash
   - [ ] MainMenu loads and UI is visible
   - [ ] Can tap "Play" button
   - [ ] Battle scene loads
   - [ ] Dice appear and can be tapped
   - [ ] Can lock/unlock dice
   - [ ] Can reroll dice
   - [ ] Can select action
   - [ ] Can complete one turn
   - [ ] Can pause and resume
   - [ ] Orientation locks to landscape

### Common Issues & Fixes

#### Issue: "IL2CPP error: Unity.IL2CPP.Building.BuilderFailedException"
- **Fix**: Ensure Android NDK and SDK are correctly installed via Unity Hub

#### Issue: Black screen on launch
- **Fix**: Check Graphics API order (Vulkan first, then GLES3)
- **Fix**: Ensure all scenes are in build settings

#### Issue: App crashes immediately
- **Fix**: Check logcat via `adb logcat | grep Unity`
- **Fix**: Disable "Strip Engine Code" temporarily

#### Issue: UI is clipped or off-screen
- **Fix**: Verify Canvas Scaler settings (will be fixed in Gate 4)

---

## Next Steps (Gate 2: iOS)
After Android smoke test passes, configure iOS build settings.

---

**Status**: ✅ Configuration guide complete  
**Action Required**: Manual configuration in Unity Editor  
**Owner**: Track G (Mobile Platform)

