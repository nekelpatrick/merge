# Phase 4 - Track C: VFX Enhancement

## Assignment

You are implementing **Track C** of Phase 4: Polish & Juice.

Your focus is creating impact particle effects and visual feedback.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Scripts/Visual/ImpactVFXController.cs` | Central VFX spawner with pooling |
| `Assets/Scripts/Visual/ShieldBlockVFX.cs` | Shield impact sparks + ripple |
| `Assets/Scripts/Visual/AttackTrailVFX.cs` | Weapon swing trails |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/UI/*` — UI components

---

## Implementation Details

### C1: ImpactVFXController.cs

- Object pooling for blood and block effects
- Subscribes to GameEvents for auto-spawning
- SpawnBlood and SpawnBlockEffect public methods

### C2: ShieldBlockVFX.cs

- Spark particle system
- Expanding ripple quad
- Fade out over duration

### C3: AttackTrailVFX.cs

- LineRenderer-based arc trail
- Configurable arc angle and radius
- Right-to-left or left-to-right slash

---

## Success Criteria

- [x] Blood spawns on damage events
- [x] Shield block shows sparks and ripple
- [x] Attack trails render properly
- [x] VFX pooling prevents garbage collection

