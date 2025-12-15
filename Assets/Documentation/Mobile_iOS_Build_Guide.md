# Shield Wall - iOS Build Configuration Guide

## MOB-020: iOS PlayerSettings Configuration

### Required Settings (Manual Configuration in Unity Editor)

Navigate to **Edit → Project Settings → Player → iOS**

#### 1. Company & Product
- **Company Name**: `[Your Company]`
- **Product Name**: `Shield Wall`
- **Version**: `0.1.0` (mobile alpha)
- **Build Number**: `1`

#### 2. Icon & Splash
- **Override for iOS**: ✅ Enabled
- **App Icons**: Provide all required sizes (currently using defaults)
  - Required: 1024x1024 (App Store), 180x180 (iPhone), 167x167 (iPad Pro)

#### 3. Resolution and Presentation
- **Default Orientation**: `Portrait`
- **Allowed Orientations for Auto Rotation**:
  - Portrait: ✅ Enabled
  - Portrait Upside Down: ✅ Enabled
  - Landscape Right: ❌ Disabled
  - Landscape Left: ❌ Disabled
- **Requires Fullscreen**: ✅ Enabled (opt out of home indicator auto-hide)
- **Status Bar Hidden**: ✅ Enabled (portrait game, no status bar needed)
- **Defer System Gestures on Edges**: `None` (allow standard iOS gestures)
- **Hide Home Button**: ❌ Disabled (respect user control)

#### 4. Other Settings

**Rendering**
- **Color Space**: Linear
- **Auto Graphics API**: ✅ Enabled (Metal will be selected automatically)
- **Metal Editor Support**: ✅ Enabled (for editor testing if on Mac)
- **Memoryless Depth**: ✅ Enabled (A11+ optimization)
- **Static Batching**: ✅ Enabled
- **Dynamic Batching**: ✅ Enabled
- **GPU Skinning**: ✅ Enabled

**Identification**
- **Bundle Identifier**: `com.[yourcompany].shieldwall`
- **Signing Team ID**: `[Your Apple Developer Team ID]`
- **Automatically Sign**: ✅ Enabled (requires Xcode signing setup)
- **Version**: `0.1.0`
- **Build Number**: `1`

**Configuration**
- **Scripting Backend**: IL2CPP
- **API Compatibility Level**: .NET Standard 2.1
- **Target Minimum iOS Version**: iOS 13.0
- **Target SDK**: Device SDK
- **Architecture**: ARM64

**Optimization**
- **Managed Stripping Level**: Minimal (safe for first mobile build)
- **Strip Engine Code**: ✅ Enabled
- **Script Call Optimization**: Slow and Safe (first build)
- **Optimize Mesh Data**: ✅ Enabled

**Camera Usage Description** (required for iOS)
- **Camera Usage Description**: *(leave blank, not used)*
- **Microphone Usage Description**: *(leave blank, not used)*
- **Location Usage Description**: *(leave blank, not used)*

#### 5. Build Settings (Xcode Project)
- **Accelerometer Frequency**: Disabled (not used)
- **Requires Persistent WiFi**: ❌ Disabled
- **Exit on Suspend**: ❌ Disabled (support backgrounding)

---

## iOS-Specific Considerations

### Metal Shader Compilation
- Unity will compile shaders for Metal at build time
- First build includes shader compilation (adds 5-10 minutes)
- Consider using "Build and Run" to automatically deploy after build

### Xcode Signing
1. Open generated Xcode project (`.xcodeproj`)
2. Select Unity-iPhone target
3. Under **Signing & Capabilities**:
   - ✅ Automatically manage signing
   - Select your development team
   - Verify provisioning profile is valid

### Device Testing Requirements
- iOS device with iOS 13.0+ installed
- Device registered in Apple Developer portal
- Valid development provisioning profile

---

## MOB-021: Build & Deploy Checklist

### Before Building
1. ✅ All scenes added to **Build Settings**
   - MainMenu.unity
   - Battle.unity
2. ✅ iOS platform selected
3. ✅ Development Build enabled (for initial testing)
4. ✅ Signing configured (Team ID set)
5. ✅ Test device connected (or Build for later deployment)

### Build Process
1. **File → Build Settings**
2. Switch to **iOS** platform (if not already)
3. Click **Player Settings** and verify all above settings
4. Click **Build** (creates Xcode project, does NOT build final IPA)
5. Choose output location (e.g., `Builds/iOS/`)
6. Wait for IL2CPP compilation (first build: 15-25 minutes)

### Deploy from Xcode
1. Open generated `.xcodeproj` in Xcode
2. Connect iOS device via USB
3. Select device from target dropdown (top of Xcode window)
4. Click **Run** (▶️ button) or **Cmd+R**
5. Xcode will sign, build, and deploy to device

### Testing on Device
**Smoke Test Checklist**:
- [ ] App launches without crash
- [ ] MainMenu loads and UI is visible
- [ ] All buttons are within safe area (no notch clipping)
- [ ] Can tap "Play" button
- [ ] Battle scene loads
- [ ] Dice appear and can be tapped
- [ ] Can lock/unlock dice
- [ ] Can reroll dice
- [ ] Can select action
- [ ] Can complete one turn
- [ ] Can pause and resume
- [ ] Orientation locks to portrait
- [ ] Home button/swipe works (app backgrounds cleanly)
- [ ] Returning from background resumes correctly

### Common Issues & Fixes

#### Issue: "Signing for 'Unity-iPhone' requires a development team"
- **Fix**: Set Signing Team ID in Unity Player Settings
- **Fix**: Or manually set in Xcode under Signing & Capabilities

#### Issue: "Unable to install [app name]"
- **Fix**: Device not registered in Developer Portal
- **Fix**: Provisioning profile expired or invalid
- **Fix**: Trust developer certificate on device (Settings → General → Device Management)

#### Issue: Black screen on launch
- **Fix**: Check Metal shader compilation succeeded
- **Fix**: Verify all required scenes are in build settings

#### Issue: App crashes on startup
- **Fix**: View Xcode console for crash logs
- **Fix**: Check IL2CPP build logs for errors
- **Fix**: Temporarily disable "Strip Engine Code"

#### Issue: UI clipped by notch/safe area
- **Fix**: Will be addressed in Gate 4 (SafeAreaFitter)

#### Issue: Performance is poor
- **Fix**: This is expected pre-optimization; Gate 6 addresses performance

---

## Quality Settings for iOS

### Default Quality Level
Same as Android (see Mobile_Android_Build_Guide.md):
- Use **"Medium"** quality level or create iOS-specific preset
- URP settings: Render scale 0.85, MSAA 2x, HDR disabled

### iOS-Specific Optimizations
- Metal automatically uses tile-based rendering (more efficient than desktop)
- Memoryless depth enabled (A11+ feature, saves bandwidth)
- Consider higher quality on newer iPhones (A13+) in quality profiles (Gate 5)

---

## Next Steps (Gate 3: Runtime Bootstrap)
After iOS smoke test passes, implement runtime mobile platform code:
- FPS targets
- Sleep timeout
- Application lifecycle handling

---

**Status**: ✅ Configuration guide complete  
**Action Required**: Manual configuration in Unity Editor + Xcode signing  
**Owner**: Track G (Mobile Platform)

