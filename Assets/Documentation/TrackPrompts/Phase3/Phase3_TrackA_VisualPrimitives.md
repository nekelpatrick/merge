# Phase 3 - Track A: 3D Visual Primitives

## Assignment

You are implementing **Track A** of Phase 3: Content + Polish.

Your focus is creating 3D visual representations of enemies and brothers using Unity primitives.

---

## Your Scope

### Files to CREATE

| File | Purpose |
|------|---------|
| `Assets/Scripts/Visual/EnemyVisualController.cs` | Spawns/manages enemy 3D representations |
| `Assets/Scripts/Visual/EnemyVisualInstance.cs` | Individual enemy visual with animations |
| `Assets/Scripts/Visual/BrotherVisualController.cs` | Side-positioned brother visuals |
| `Assets/Scripts/Visual/BloodBurstVFX.cs` | Blood particle effect controller |
| `Assets/Prefabs/VFX/BloodBurst.prefab` | Blood particle prefab |

### Files to MODIFY

| File | Changes |
|------|---------|
| `Assets/Scripts/Visual/FirstPersonArms.cs` | Add shield animation, damage shake |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/UI/*` — UI components
- `Assets/Scripts/Dice/*` — Dice system
- Any files outside `Assets/Scripts/Visual/` and `Assets/Prefabs/VFX/`

---

## Implementation Details

### A1: EnemyVisualController

```csharp
namespace ShieldWall.Visual
{
    public class EnemyVisualController : MonoBehaviour
    {
        // Subscribe to GameEvents.OnEnemyWaveSpawned
        // Spawn EnemyVisualInstance for each enemy
        // Position in front of camera (Z: 5-10 units, spread on X)
        // Subscribe to GameEvents.OnEnemyKilled to trigger death
    }
}
```

**Enemy Visual Setup (primitives):**
- Body: Capsule (height 2, radius 0.3)
- Head: Cube (0.4 units)
- Material: Unlit color based on enemy type

**Colors by EnemySO name:**
| Enemy Type | Color |
|------------|-------|
| Thrall | Brown (#6B4423) |
| Warrior | Gray (#5A5A5A) |
| Berserker | Red (#8B2020) |
| Archer | Green (#2D5A27) |

### A2: EnemyVisualInstance

```csharp
namespace ShieldWall.Visual
{
    public class EnemyVisualInstance : MonoBehaviour
    {
        public EnemySO EnemyData { get; private set; }
        
        public void Initialize(EnemySO enemy, Vector3 position);
        public void PlayDeathAnimation(); // Shrink + fall, then destroy
        public void PlayHitReaction();    // Brief flash/shake
    }
}
```

**Death Animation:**
1. Scale to 0.1 over 0.3s (ease in)
2. Rotate X by 90 degrees (fall forward)
3. Spawn BloodBurst at position
4. Destroy after 0.5s

### A3: BrotherVisualController

```csharp
namespace ShieldWall.Visual
{
    public class BrotherVisualController : MonoBehaviour
    {
        // Subscribe to GameEvents.OnBrotherWounded, OnBrotherDied
        // Create brother visuals at fixed positions
        // Update visual state based on health
    }
}
```

**Brother Positions (relative to camera):**
| WallPosition | Local Position |
|--------------|----------------|
| FarLeft | (-3, 0, 1) |
| Left | (-1.5, 0, 1) |
| Right | (1.5, 0, 1) |
| FarRight | (3, 0, 1) |

**Visual States:**
- Healthy: Full opacity, upright
- Wounded: Red tint, lean 15 degrees
- Dead: Fall over (rotate X 90), fade out

### A4: FirstPersonArms Enhancement

Modify existing `FirstPersonArms.cs`:

```csharp
// Add methods:
public void PlayBlockAnimation();  // Shield raises briefly
public void PlayDamageShake();     // Camera/arms shake
```

**Block Animation:**
- Move shield forward 0.2 units over 0.1s
- Return to original position over 0.2s

**Damage Shake:**
- Random position offset (±0.05 units) for 0.3s
- Ease back to original

### A5: BloodBurstVFX

```csharp
namespace ShieldWall.Visual
{
    public class BloodBurstVFX : MonoBehaviour
    {
        public void Play(); // Trigger particle burst
        // Auto-destroy after particles complete
    }
}
```

**ParticleSystem Settings:**
- Emission: Burst, 30-50 particles
- Shape: Cone, 45 degree angle
- Start Color: Blood red (#8B2020)
- Start Size: 0.05-0.15 (random)
- Start Lifetime: 0.5-1s
- Gravity Modifier: 1
- Simulation Space: World
- Stop Action: Destroy

---

## Events to SUBSCRIBE TO

```csharp
// In OnEnable:
GameEvents.OnEnemyWaveSpawned += SpawnEnemyVisuals;
GameEvents.OnEnemyKilled += HandleEnemyKilled;
GameEvents.OnBrotherWounded += HandleBrotherWounded;
GameEvents.OnBrotherDied += HandleBrotherDied;
GameEvents.OnPlayerWounded += HandlePlayerWounded;
GameEvents.OnAttackBlocked += HandleAttackBlocked;
```

---

## Success Criteria

- [ ] Enemy primitives spawn when wave starts
- [ ] Enemies positioned in front of camera (visible in Game view)
- [ ] Enemy death plays shrink/fall animation
- [ ] Blood burst spawns on enemy death
- [ ] Brother silhouettes visible at screen edges
- [ ] Brothers visually react to damage
- [ ] First-person arms visible at screen bottom
- [ ] Shield animates on block
- [ ] Screen shakes on player damage
- [ ] No compiler errors

---

## Test Steps

1. Open `Assets/Scenes/Battle.unity`
2. Press Play
3. Verify enemies appear as colored capsules
4. Execute a Strike action → enemy should die with animation + blood
5. Let an attack hit a brother → brother should react
6. Let an attack hit player → arms should shake
7. Execute a Block action → shield should animate

---

## Reference Files

- `Assets/Scripts/Core/GameEvents.cs` — Event definitions
- `Assets/Scripts/Data/EnemySO.cs` — Enemy data structure
- `Assets/Scripts/ShieldWall/WallPosition.cs` — Position enum

