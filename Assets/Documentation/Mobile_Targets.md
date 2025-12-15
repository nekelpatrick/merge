# Shield Wall - Mobile Platform Targets

## Unity Environment
- **Unity Version**: 6000.0.x (Unity 6)
- **URP Version**: 17.0.4
- **Input System**: 1.14.2
- **Platform Feature Set**: `com.unity.feature.mobile` 1.0.0

## Target Platforms

### Android
- **Minimum OS**: Android 7.0 (API 24)
- **Target OS**: Android 13+ (API 33+)
- **Architecture**: ARM64 (IL2CPP)
- **Graphics API**: Vulkan primary, OpenGL ES 3.0 fallback
- **Orientation**: Landscape only (sensor landscape for rotation)

### iOS
- **Minimum OS**: iOS 13.0
- **Target OS**: iOS 16+
- **Architecture**: ARM64 (IL2CPP)
- **Graphics API**: Metal
- **Orientation**: Landscape only (sensor landscape for rotation)

## Device Matrix

### Low-Tier Baseline
- **Android**: Snapdragon 660 / Mali-G72 equivalent (2GB RAM)
- **iOS**: iPhone 8 / A11 (2GB RAM)
- **Target Performance**: 30 FPS stable, 45-50ms frame time
- **Memory Budget**: 512MB gameplay footprint

### Mid-Tier Target
- **Android**: Snapdragon 730 / Mali-G76 equivalent (4GB RAM)
- **iOS**: iPhone 11 / A13 (4GB RAM)
- **Target Performance**: 60 FPS stable, 16.6ms frame time
- **Memory Budget**: 768MB gameplay footprint

### High-Tier Aspirational
- **Android**: Snapdragon 865+ / Mali-G78 equivalent (6GB+ RAM)
- **iOS**: iPhone 12 Pro+ / A14+ (6GB+ RAM)
- **Target Performance**: 60 FPS with all effects, <16ms frame time
- **Memory Budget**: 1GB gameplay footprint

## Performance Budgets (Mid-Tier Target)

### Frame Time Budget (60 FPS = 16.6ms)
- **CPU Main Thread**: 10ms
  - Game Logic: 2ms
  - UI Updates: 2ms
  - Scripts/Events: 1ms
  - Rendering/Culling: 2ms
  - Physics: 1ms
  - Other: 2ms
- **GPU Render**: 12ms
  - Geometry: 3ms
  - Lighting/Shadows: 3ms
  - Post-processing: 2ms
  - VFX/Particles: 2ms
  - UI: 2ms

### Memory Budget
- **Total Runtime**: 768MB
  - Textures: 256MB
  - Meshes: 64MB
  - Audio: 32MB
  - Code/Scripts: 48MB
  - UI: 32MB
  - Render Targets: 128MB
  - Overhead/Misc: 208MB

### Draw Call Budget
- **Opaque**: 250-400
- **Transparent**: 50-100
- **UI**: 50-100
- **Total**: <600 per frame

### Battery/Thermal
- **Target**: 3+ hours gameplay on mid-tier device
- **Thermal**: No throttling within 20 minutes of gameplay

## Quality Profiles

### Low (for low-tier devices)
- Render Scale: 0.7x
- MSAA: Off
- Shadows: Disabled or low-res (512px)
- Post-processing: Minimal (vignette only)
- VFX: Reduced particle counts (50% cap)
- Audio: Mono, lower sample rates

### Medium (default for mid-tier)
- Render Scale: 0.85x
- MSAA: 2x
- Shadows: Medium-res (1024px)
- Post-processing: Selective (damage effects, no bloom)
- VFX: Standard particle counts
- Audio: Stereo, standard sample rates

### High (for high-tier devices)
- Render Scale: 1.0x
- MSAA: 4x
- Shadows: High-res (2048px)
- Post-processing: Full effects
- VFX: Full particle counts + extra effects
- Audio: Stereo, high sample rates

## Touch UX Targets

### Minimum Touch Target Size
- **Buttons**: 44x44 pt (iOS HIG), 48x48 dp (Material Design)
- **Unity UI**: Minimum 80x80 pixels at 1080p reference resolution

### Safe Area Compliance
- Support notched devices (iPhone X+, Android notch/punch-hole)
- All interactive elements within safe area bounds
- Critical info (health, stamina, dice) never clipped

### Gesture Requirements
- **Tap**: Dice lock/unlock, action select, menu buttons
- **No swipes/gestures required**: Keep to simple taps for accessibility

## Build Size Targets
- **Android APK/AAB**: <150MB (before Play Asset Delivery)
- **iOS IPA**: <200MB (before on-demand resources)

## Success Metrics
- [ ] Mid-tier device reaches 60 FPS in Battle with 4 enemies + VFX
- [ ] Low-tier device reaches 30 FPS stable in same scenario
- [ ] No frame drops during dice roll animation
- [ ] Battle scene loads in <3 seconds on mid-tier
- [ ] Memory stays under budget (no OOM crashes in 1-hour session)
- [ ] Battery drain <30% per hour on mid-tier device

## Known Constraints
1. Turn-based gameplay reduces frame time pressure vs real-time action
2. First-person camera = no rotating/moving camera overhead
3. Limited simultaneous VFX (max 5 enemies attacking per turn)
4. UI is mostly static during turn phases (minimize per-frame UI cost)

---

**Last Updated**: 2025-01-12  
**Owner**: Track G (Mobile Platform)

