# Quick Start Guide - 3D Model Upgrade System

## For Immediate Use

### Step 1: Generate Assets (1-Click)
```
Unity Editor → Shield Wall Builder → 3D Assets → Create All 3D Assets (One-Click)
```
This creates all prefabs, materials, and environment props automatically.

### Step 2: Validate Installation
```
Unity Editor → Shield Wall Builder → Validation → Validate Visual System
```
Check the console output for any missing assets.

### Step 3: Test in Battle Scene
1. Open `Assets/Scenes/Battle.unity`
2. Locate `EnemyVisualController` GameObject
3. In Inspector, assign:
   - Blood Burst Prefab: `Assets/Prefabs/VFX/BloodBurst.prefab`
4. Press Play
5. Trigger enemy kill → observe dismemberment

---

## What You Get

### Modular Character System
- Characters can swap body parts at runtime
- Dismemberment-ready architecture
- Event-driven gore triggers

### Gore System
- **3 Limb Types:** Head, Arm, Leg
- **Physics-based:** Tumble, bounce, despawn
- **Blood VFX:** Particle burst + ground decals
- **Performance:** Auto-cleanup after 10-30 seconds

### Materials (8 types)
- Player, Brother, 4 enemy types, Blood, Gore
- GPU instancing enabled
- URP/Lit shader ready

### Environment Props
- Ground plane, Stakes, Rocks, Debris
- Ready to place in Battle scene

### Editor Tools (6 tools)
- One-Click Setup (all assets)
- Validation System (verify integrity)
- Individual asset creators (granular control)

---

## File Structure

```
Assets/
├── Scripts/
│   ├── Data/
│   │   ├── ModularCharacterData.cs
│   │   └── ToonMaterialPalette.cs
│   └── Visual/
│       ├── ModularCharacterBuilder.cs
│       ├── DismembermentController.cs
│       ├── SeveredLimb.cs
│       ├── PrimitiveMeshGenerator.cs
│       ├── ActionDismembermentMapper.cs
│       ├── LODController.cs
│       └── BloodBurstVFX.cs (updated)
├── Editor/
│   ├── OneClickSetup.cs ★ USE THIS FIRST
│   ├── VisualSystemValidator.cs ★ USE THIS SECOND
│   ├── PrimitivePrefabCreator.cs
│   ├── MaterialCreator.cs
│   ├── EnvironmentPropCreator.cs
│   └── BloodDecalCreator.cs
├── Prefabs/
│   ├── Characters/ (for future modular prefabs)
│   ├── Gore/
│   │   ├── SeveredHead.prefab
│   │   ├── SeveredArm.prefab
│   │   └── SeveredLeg.prefab
│   └── VFX/
│       ├── BloodBurst.prefab
│       ├── BloodDecal_01.prefab
│       ├── BloodDecal_02.prefab
│       └── BloodDecal_03.prefab
└── Art/
    └── Materials/
        ├── Characters/ (8 materials)
        ├── Environment/ (3 materials)
        └── Effects/ (3 blood decals)
```

---

## Integration with Existing Code

### No Changes Required To:
- `GameEvents.cs` - Already has all needed events
- `EnemySO.cs` - No modification needed
- `ActionSO.cs` - No modification needed
- Core combat systems - Fully decoupled

### Modified Files (3):
- `EnemyVisualInstance.cs` - Added dismemberment support
- `EnemyVisualController.cs` - Triggers dismemberment on kill
- `BloodBurstVFX.cs` - Enhanced to spec

### Usage Example:
```csharp
// The system automatically hooks into existing events:
GameEvents.RaiseEnemyKilled(enemy);
// → EnemyVisualController receives event
// → Determines dismemberment type
// → Calls EnemyVisualInstance.PlayDeathAnimationWithDismemberment()
// → DismembermentController spawns limb + blood VFX
// → Profit! 🩸
```

---

## Troubleshooting

### "Validation shows warnings"
- **Prefabs missing:** Run `One-Click Setup All Assets`
- **Materials missing:** Run `Create Toon Materials` separately
- **Scripts not compiling:** Check console for errors

### "Dismemberment not working"
1. Check `EnemyVisualInstance` has `DismembermentController` component
2. Verify limb prefabs assigned in Inspector
3. Ensure `BloodBurst` prefab reference set
4. Check console for null reference errors

### "Blood VFX not appearing"
- Verify URP is active (Project Settings → Graphics)
- Check `BloodBurst.prefab` exists in `Prefabs/VFX/`
- Ensure particle system is not culled by camera

### "Performance issues"
- Enable LOD culling: Add `LODController` to distant objects
- Verify materials have GPU instancing enabled
- Check severed limbs are despawning (10s lifetime)
- Reduce MAX_PARTICLES in `BloodBurstVFX.cs` if needed

---

## Upgrading to Real 3D Models

When you acquire Viking character models:

1. **Create ModularCharacterData asset:**
   ```
   Assets → Create → ShieldWall → Modular Character Data
   ```

2. **Assign your meshes:**
   - Torso, Head, Arms, Legs (intact)
   - Headless/Armless/Legless variants

3. **Add to enemy prefab:**
   - Add `ModularCharacterBuilder` component
   - Assign your `ModularCharacterData` asset
   - System will use custom meshes instead of primitives

4. **Update limb prefabs:**
   - Replace primitive meshes in `SeveredHead/Arm/Leg` prefabs
   - Adjust collider sizes to match
   - Test physics behavior

---

## Performance Specs

| System | Impact | Optimization |
|--------|--------|--------------|
| Dismemberment | Low | Pooling not needed (infrequent) |
| Blood Particles | Medium | 50-100 burst, 1.5s lifetime |
| Blood Decals | Low | 30s cleanup, max ~10 per wave |
| Severed Limbs | Low | 10s despawn, typically 1-3 active |
| Materials | Low | GPU instancing enabled |
| LOD Culling | Configurable | Distance-based disable |

**Estimated Cost Per Kill:**
- 1 dismemberment trigger
- 1 blood burst (50-100 particles, 1.5s)
- 1-3 blood decals (30s lifetime)
- 1 severed limb (10s physics)
- Total: ~200 particles/frame peak, < 5 draw calls

**Tested On:** Unity 2021+ with URP

---

## Next Steps

### Immediate (Ready Now)
- ✅ Run One-Click Setup
- ✅ Validate System
- ✅ Test in Battle scene

### Short Term (This Sprint)
- Add dismemberment to brother deaths
- Create blood overlay UI for player damage
- Add gore sound effects (use existing AudioManager)

### Long Term (Phase 6+)
- Replace primitives with Synty/Polygon models
- Add skeletal animation
- Implement ragdoll physics
- Custom toon shader with rim lighting

---

**Read full documentation:** `Assets/Documentation/Phase5_3DModelUpgrade_Complete.md`

**Need help?** Check `VisualSystemValidator` for asset status.
