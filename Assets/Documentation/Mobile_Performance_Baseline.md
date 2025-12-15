# Shield Wall - Mobile Performance Baseline

## Profiling Methodology

### Pre-Mobile Baseline (Desktop/Editor)
This document captures the performance characteristics of Shield Wall **before** mobile optimizations, to measure impact of mobile-specific changes.

### Profiling Setup
1. **Scene**: Battle.unity
2. **Test Scenario**: Wave 2 (4 enemies)
3. **Actions Tested**:
   - Dice roll (6 dice)
   - Lock/unlock dice (3 toggles)
   - Select multi-rune action (3-rune combo)
   - Combat resolution (player attack + enemy attacks)
   - Post-processing effects (damage vignette)
   - VFX spawns (blood, block effects)

### Tools
- Unity Profiler (Deep Profiling OFF for realistic measurements)
- Frame Debugger for draw call analysis
- Memory Profiler for heap snapshots

---

## Baseline Measurements (Editor, Unity 6, URP 17)

### CPU Performance (Main Thread)
**Measured in Editor Play Mode at 1920x1080, V-Sync OFF**

| Phase | Average Frame Time | Peak Frame Time | Notes |
|-------|-------------------|-----------------|-------|
| Idle (player turn, no input) | TBD ms | TBD ms | Waiting for dice input |
| Dice Roll Animation | TBD ms | TBD ms | Includes UI updates |
| Combat Resolution | TBD ms | TBD ms | Damage calculations + VFX spawns |
| Turn Transition | TBD ms | TBD ms | Phase changes + coroutines |

**Top CPU Costs** (to be profiled):
1. TBD
2. TBD
3. TBD
4. TBD
5. TBD

### GPU Performance
| Metric | Value | Notes |
|--------|-------|-------|
| Average GPU Time | TBD ms | |
| Draw Calls (Opaque) | TBD | |
| Draw Calls (Transparent) | TBD | |
| Draw Calls (UI) | TBD | |
| SetPass Calls | TBD | |
| Triangles | TBD K | |
| Vertices | TBD K | |

**Render Pipeline Breakdown** (Frame Debugger):
- Shadows: TBD ms
- Opaque Geometry: TBD ms
- Post-Processing: TBD ms
- UI: TBD ms

### Memory Footprint
| Category | Size | Notes |
|----------|------|-------|
| Total Allocated | TBD MB | Unity Profiler "Total Reserved" |
| Textures | TBD MB | |
| Meshes | TBD MB | |
| Audio | TBD MB | |
| Managed Heap | TBD MB | |
| GC Allocated (per turn) | TBD KB | Ideally <100KB/turn |

### GC Pressure
- **Allocations per Dice Roll**: TBD bytes
- **Allocations per Turn**: TBD bytes
- **GC Collections (5 min session)**: TBD

---

## Hot Spots to Optimize for Mobile

### Expected Problem Areas (based on code review)
1. **Post-Processing**: `PostProcessController` applies effects per-frame via coroutines
2. **VFX Instantiation**: Blood/block effects may spike on multi-enemy hits
3. **UI Coroutines**: `ButtonFeedback`, `UIAnimator` run per-frame tweens
4. **Event System**: Many `GameEvents` subscribers firing simultaneously
5. **Debug Logging**: Verbose logs in `TurnManager`, `DiceUI`, etc.

### Mobile-Specific Risks
1. **Overdraw**: First-person view with overlapping UI canvases
2. **Shader Complexity**: URP toon shaders + post-processing
3. **Audio**: No compression settings specified for mobile
4. **Resources.Load**: Used in `BattleBootstrapper` (slow on mobile)

---

## Action Items After Baseline
Once baseline is captured:
1. Set target frame time budgets (from Mobile_Targets.md)
2. Identify top 3 CPU bottlenecks
3. Identify top 3 GPU bottlenecks
4. Create optimization priority list for Gate 6

---

**Status**: ⏳ Awaiting profiler capture  
**Next Step**: Run Unity Profiler session and fill in TBD values  
**Owner**: Track G (Mobile Platform)

