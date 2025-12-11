# Phase 3 - Track B: Scene Atmosphere

## Assignment

You are implementing **Track B** of Phase 3: Content + Polish.

Your focus is enhancing the Battle scene with lighting, post-processing, and atmosphere.

---

## Your Scope

### Files to MODIFY

| File | Changes |
|------|---------|
| `Assets/Editor/BattleSceneSetup.cs` | Add atmosphere setup methods |
| `Assets/Scenes/Battle.unity` | Apply atmosphere (via editor script) |

### Assets to CREATE

| Asset | Purpose |
|-------|---------|
| `Assets/Settings/BattleVolumeProfile.asset` | URP Volume Profile |
| `Assets/Art/Materials/Ground.mat` | Ground plane material |
| `Assets/Art/Materials/Skybox.mat` | Custom skybox (optional) |

---

## DO NOT TOUCH

- `Assets/Scripts/*` — No script changes (except Editor/)
- `Assets/Prefabs/*` — No prefab changes
- Any runtime code

---

## Implementation Details

### B1: Volume Profile (URP Post-Processing)

Create `BattleVolumeProfile.asset` with these overrides:

**Color Grading:**
- Mode: LDR
- Saturation: -15
- Temperature: -5 (slight cool shift)
- Contrast: 10

**Vignette:**
- Intensity: 0.25
- Smoothness: 0.4
- Color: Dark brown (#2A1810)

**Bloom:**
- Intensity: 0.5
- Threshold: 1.0
- Scatter: 0.6

**Film Grain (subtle):**
- Type: Medium
- Intensity: 0.1

### B2: Lighting Setup

**Directional Light (Sun):**
- Rotation: (50, -30, 0)
- Intensity: 0.8
- Color: Desaturated warm (#FFE8D0)
- Shadow Type: Soft Shadows
- Shadow Strength: 0.6

**Ambient:**
- Source: Color
- Ambient Color: Dark gray (#1A1A1A)
- Intensity: 0.3

### B3: Skybox

**Option A: Procedural Skybox**
- Atmosphere Thickness: 0.5
- Sky Tint: Stormy gray (#4A4A4A)
- Ground: Dark (#2A2A2A)
- Exposure: 0.5

**Option B: Gradient Skybox (preferred)**
- Top Color: Storm gray (#3A3A3A)
- Middle Color: Desaturated blue-gray (#4A4A5A)
- Bottom Color: Dark brown (#2A2010)

### B4: Ground Plane

Create ground in scene:
- Primitive: Plane
- Scale: (50, 1, 50)
- Position: (0, 0, 0)
- Material: `Ground.mat`

**Ground Material:**
- Shader: URP/Lit
- Base Color: Mud brown (#4A3728)
- Smoothness: 0.1
- Metallic: 0

### B5: Camera Layer Setup

Configure layers in `TagManager.asset`:
- Layer 6: "PlayerView" (arms, shield)
- Layer 7: "Brothers"
- Layer 8: "Enemies"
- Layer 9: "Environment"

**Main Camera Settings:**
- Clear Flags: Skybox
- Culling Mask: Everything
- Field of View: 60
- Near: 0.1
- Far: 100

### B6: Fog (Optional)

**Fog Settings:**
- Mode: Linear
- Start: 10
- End: 50
- Color: Match ambient (#1A1A1A with slight blue)

---

## BattleSceneSetup.cs Updates

Add these menu items:

```csharp
[MenuItem("ShieldWall/Scene Setup/Create Volume Profile")]
static void CreateVolumeProfile()
{
    // Create VolumeProfile asset
    // Add Color Grading, Vignette, Bloom, Film Grain
    // Save to Assets/Settings/BattleVolumeProfile.asset
}

[MenuItem("ShieldWall/Scene Setup/Setup Battle Lighting")]
static void SetupBattleLighting()
{
    // Find or create Directional Light
    // Apply settings from B2
    // Configure RenderSettings for ambient
}

[MenuItem("ShieldWall/Scene Setup/Create Ground Plane")]
static void CreateGroundPlane()
{
    // Create plane primitive
    // Apply ground material
    // Position at origin
}

[MenuItem("ShieldWall/Scene Setup/Setup Layers")]
static void SetupLayers()
{
    // Configure layer names via SerializedObject
}

[MenuItem("ShieldWall/Scene Setup/Apply Full Atmosphere")]
static void ApplyFullAtmosphere()
{
    // Run all above in sequence
}
```

---

## Success Criteria

- [ ] Volume Profile exists with correct settings
- [ ] Battle scene has Volume component using profile
- [ ] Directional light configured correctly
- [ ] Ground plane visible in scene
- [ ] Vignette visible during play
- [ ] Colors feel desaturated and moody
- [ ] No errors in editor or play mode

---

## Test Steps

1. Run `ShieldWall > Scene Setup > Apply Full Atmosphere`
2. Open `Assets/Scenes/Battle.unity`
3. Check Scene view — should see ground, proper lighting
4. Press Play
5. Verify vignette visible at screen edges
6. Verify overall mood is dark/stormy Viking aesthetic

---

## Reference Files

- `Assets/Documentation/VisualStyleSystem.md` — Full color palette
- `Assets/Settings/` — Existing URP settings

