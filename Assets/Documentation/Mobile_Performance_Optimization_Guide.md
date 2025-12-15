# Shield Wall - Mobile Performance Optimization Guide

## Gate 6: Performance & Memory Passes

This document provides implementation guidance for mobile-specific performance optimizations across all subsystems.

---

## MOB-060: URP Mobile Pass

### Settings to Adjust

**File**: `Assets/Settings/[YourURPAsset].asset` (URP Renderer asset)

Navigate to **Edit → Project Settings → Graphics** to find active URP asset.

### Mobile URP Asset Configuration

Create a mobile-specific URP asset or adjust existing:

**Rendering:**
- **Rendering Path**: Forward
- **Depth Texture**: Auto (only when needed)
- **Opaque Downsampling**: None
- **Terrain Holes**: ❌ Disabled

**Quality:**
- **HDR**: ❌ Disabled (mobile doesn't benefit, costs bandwidth)
- **MSAA**: 2x (driven by quality profile)
- **Render Scale**: 0.85 (driven by quality profile)
- **LOD Cross Fade**: ❌ Disabled

**Lighting:**
- **Main Light**: Per Pixel
- **Main Light Shadows**: ✅ Enabled (toggleable via profile)
- **Shadow Resolution**: 1024 (mobile default)
- **Shadow Distance**: 20 (mobile default)
- **Cascade Count**: 1 (no cascades on mobile)
- **Additional Lights**: ❌ Disabled (or max 1 if needed)

**Shadows:**
- **Soft Shadows**: ❌ Disabled (expensive filtering)
- **Conservative Enclosing Sphere**: ✅ Enabled
- **Working Unit**: Metric

**Post-Processing:**
- **Grading Mode**: Low Dynamic Range (LDR)
- **LUT Size**: 16 (smaller lookup table)

**Adaptive Performance** (if available):
- Consider enabling Unity's Adaptive Performance package for automatic quality scaling

---

## MOB-061: Post-Processing Cost Gating

### Modify PostProcessController to Respect Settings

**File**: `Assets/Scripts/Visual/PostProcessController.cs`

Add mobile settings check in `Awake()`:

```csharp
private void Awake()
{
    // ... existing code ...
    
    // Check mobile settings
    CheckMobileSettings();
}

private void OnEnable()
{
    GameEvents.OnPlayerWounded += HandlePlayerWounded;
    GameEvents.OnEnemyKilled += HandleEnemyKilled;
    GameEvents.OnMobileSettingChanged += HandleMobileSettingChanged;
}

private void OnDisable()
{
    GameEvents.OnPlayerWounded -= HandlePlayerWounded;
    GameEvents.OnEnemyKilled -= HandleEnemyKilled;
    GameEvents.OnMobileSettingChanged -= HandleMobileSettingChanged;
}

private void CheckMobileSettings()
{
    #if UNITY_ANDROID || UNITY_IOS
    if (MobileSettingsUI.GetDisablePostProcessing())
    {
        DisablePostProcessing();
    }
    #endif
}

private void HandleMobileSettingChanged(string settingName, bool value)
{
    if (settingName == "DisablePost")
    {
        if (value)
            DisablePostProcessing();
        else
            EnablePostProcessing();
    }
}

private void DisablePostProcessing()
{
    if (_volume != null)
    {
        _volume.enabled = false;
        Debug.Log("[PostProcess] Post-processing disabled for mobile performance");
    }
}

private void EnablePostProcessing()
{
    if (_volume != null)
    {
        _volume.enabled = true;
        Debug.Log("[PostProcess] Post-processing enabled");
    }
}
```

**Manual Steps:**
1. Open `PostProcessController.cs`
2. Add the above methods
3. Wire up the mobile setting event
4. Test: Toggle "Disable Post" in mobile settings

---

## MOB-062: VFX Pooling + Limits

### Files to Modify

1. `Assets/Scripts/Visual/ImpactVFXController.cs`
2. `Assets/Scripts/Visual/BloodBurstVFX.cs` (if exists)
3. Any other VFX spawners

### Add VFX Cap System

```csharp
// Add to ImpactVFXController or create new VFXLimiter.cs

private Queue<GameObject> _activeVFXPool = new Queue<GameObject>();
private int _maxActiveVFX = 10; // Mobile limit

private void Awake()
{
    #if UNITY_ANDROID || UNITY_IOS
    if (MobileSettingsUI.GetReduceVFX())
    {
        _maxActiveVFX = 5; // Reduced for low-end devices
    }
    #endif
}

public void SpawnBlood(Vector3 position, Vector3 direction, int intensity)
{
    // Limit active VFX count
    if (_activeVFXPool.Count >= _maxActiveVFX)
    {
        // Destroy oldest VFX
        var oldest = _activeVFXPool.Dequeue();
        if (oldest != null)
            Destroy(oldest);
    }
    
    // Spawn new VFX
    GameObject vfx = Instantiate(bloodPrefab, position, Quaternion.identity);
    _activeVFXPool.Enqueue(vfx);
    
    // Auto-cleanup after duration
    Destroy(vfx, 2f);
}
```

**Key Changes:**
- Cap simultaneous VFX instances
- Destroy oldest when limit reached
- Respect mobile "Reduce VFX" toggle
- Use object pooling for frequently spawned effects

---

## MOB-063: UI Overdraw + Animation Performance

### Reduce UI Overdraw

**Canvas Optimization Checklist:**

1. **Minimize overlapping UI elements**
   - Check Frame Debugger for overdraw (Window → Analysis → Frame Debugger)
   - Red areas = high overdraw

2. **Disable Raycast Target on non-interactive elements**
   - Select Image/Text components
   - Uncheck "Raycast Target" if not clickable
   - Reduces UI event system overhead

3. **Use Canvas Groups efficiently**
   - Don't nest too deeply
   - Disable `Interactable` on hidden panels

4. **Sprite Atlas for UI**
   - Create Sprite Atlas for UI sprites
   - Reduces draw calls from 50+ to 1-5

### Optimize UI Animations

**File**: `Assets/Scripts/UI/ButtonFeedback.cs`

Add mobile-specific animation reduction:

```csharp
private void Awake()
{
    // ... existing code ...
    
    #if UNITY_ANDROID || UNITY_IOS
    // Reduce animation complexity on mobile
    _hoverDuration = Mathf.Max(_hoverDuration * 1.5f, 0.15f); // Slower = fewer frames
    _clickPunchDuration = Mathf.Max(_clickPunchDuration * 1.2f, 0.2f);
    _enableGlow = false; // Disable per-frame glow pulse on mobile
    #endif
}
```

**File**: `Assets/Scripts/UI/UIAnimator.cs`

Cap coroutine count and simplify easing on mobile:

```csharp
private static int _activeAnimations = 0;
private const int MAX_MOBILE_ANIMATIONS = 5;

public static IEnumerator PunchScale(RectTransform target, float strength, float duration)
{
    #if UNITY_ANDROID || UNITY_IOS
    if (_activeAnimations >= MAX_MOBILE_ANIMATIONS)
    {
        yield break; // Skip animation if too many active
    }
    _activeAnimations++;
    #endif
    
    // ... existing animation code ...
    
    #if UNITY_ANDROID || UNITY_IOS
    _activeAnimations--;
    #endif
}
```

---

## MOB-064: Audio Import Settings

### Batch Audio Configuration

**Manual Steps in Unity:**

1. Select all audio files in `Assets/Audio/Music/`
2. In Inspector, configure for **Android**:
   - Load Type: **Streaming**
   - Compression Format: **Vorbis**
   - Quality: **70**
   - Sample Rate Setting: **Preserve Sample Rate** (or Optimize)

3. Configure for **iOS**:
   - Load Type: **Streaming**
   - Compression Format: **MP3** (or AAC if available)
   - Quality: **70**

4. Select all audio files in `Assets/Audio/SFX/`
5. Configure for **Android**:
   - Load Type: **Decompress On Load**
   - Compression Format: **ADPCM** (good balance)
   - Sample Rate Setting: **Override Sample Rate** → 22050 Hz (or 32000)

6. Configure for **iOS**:
   - Load Type: **Decompress On Load**
   - Compression Format: **ADPCM**
   - Sample Rate Setting: **Override Sample Rate** → 22050 Hz

**Rationale:**
- **Music**: Stream from disk (don't load all into memory)
- **SFX**: Decompress at load (fast playback, small files)
- **Lower sample rates**: 22-32kHz sufficient for game SFX (44.1kHz overkill)

### Script to Auto-Apply (Optional)

Create `Assets/Editor/MobileAudioImporter.cs`:

```csharp
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class MobileAudioImporter : AssetPostprocessor
{
    void OnPreprocessAudio()
    {
        AudioImporter audioImporter = (AudioImporter)assetImporter;
        
        // Music files
        if (assetPath.Contains("Audio/Music"))
        {
            var androidSettings = audioImporter.GetOverrideSampleSettings("Android");
            androidSettings.loadType = AudioClipLoadType.Streaming;
            androidSettings.compressionFormat = AudioCompressionFormat.Vorbis;
            androidSettings.quality = 0.7f;
            audioImporter.SetOverrideSampleSettings("Android", androidSettings);
            
            var iOSSettings = audioImporter.GetOverrideSampleSettings("iOS");
            iOSSettings.loadType = AudioClipLoadType.Streaming;
            iOSSettings.compressionFormat = AudioCompressionFormat.MP3;
            iOSSettings.quality = 0.7f;
            audioImporter.SetOverrideSampleSettings("iOS", iOSSettings);
        }
        
        // SFX files
        if (assetPath.Contains("Audio/SFX"))
        {
            var androidSettings = audioImporter.GetOverrideSampleSettings("Android");
            androidSettings.loadType = AudioClipLoadType.DecompressOnLoad;
            androidSettings.compressionFormat = AudioCompressionFormat.ADPCM;
            androidSettings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            androidSettings.sampleRateOverride = 22050;
            audioImporter.SetOverrideSampleSettings("Android", androidSettings);
            
            var iOSSettings = audioImporter.GetOverrideSampleSettings("iOS");
            iOSSettings.loadType = AudioClipLoadType.DecompressOnLoad;
            iOSSettings.compressionFormat = AudioCompressionFormat.ADPCM;
            iOSSettings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
            iOSSettings.sampleRateOverride = 22050;
            audioImporter.SetOverrideSampleSettings("iOS", iOSSettings);
        }
    }
}
#endif
```

---

## MOB-065: Reduce Log Spam

### Gate Debug Logs Behind Compile Symbols

**Files to modify** (examples):

1. `Assets/Scripts/Core/TurnManager.cs`
2. `Assets/Scripts/UI/DiceUI.cs`
3. `Assets/Scripts/Combat/CombatResolver.cs`
4. Any file with frequent `Debug.Log()` calls

**Find all `Debug.Log` calls and wrap:**

```csharp
// BEFORE:
Debug.Log($"TurnManager: Phase changed to {phase}");

// AFTER:
#if UNITY_EDITOR || DEVELOPMENT_BUILD
Debug.Log($"TurnManager: Phase changed to {phase}");
#endif
```

**Or create a logging wrapper:**

```csharp
// Assets/Scripts/Core/DebugLogger.cs
public static class DebugLogger
{
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message)
    {
        Debug.Log(message);
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(string message)
    {
        Debug.LogWarning(message);
    }
}

// Usage:
DebugLogger.Log($"TurnManager: Phase changed to {phase}");
```

**Benefits:**
- Release builds have zero logging overhead
- Development builds retain full logging
- No runtime if-checks needed (compiler strips code)

---

## Performance Testing Checklist

After applying optimizations:

- [ ] Profile Battle scene on device (Unity Profiler remote connection)
- [ ] Capture GPU frame time (should improve 20-30%)
- [ ] Check memory footprint (should be under budget)
- [ ] Verify VFX caps are working (count active particles)
- [ ] Test post-processing toggle (immediate FPS gain when disabled)
- [ ] Verify UI animations don't spike frame time
- [ ] Check audio memory usage in Profiler
- [ ] Run 10-minute session (check for memory leaks/GC spikes)

---

**Status**: ✅ Optimization guide complete  
**Action Required**: Apply changes to respective Track files  
**Owner**: Tracks B (Render), A (VFX), D (Audio), F (UI)

