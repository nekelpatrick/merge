# Phase 4 - Track A: Advanced Screen Effects

## Assignment

You are implementing **Track A** of Phase 4: Polish & Juice.

Your focus is creating advanced camera shake, hit stop, and screen distortion effects.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Scripts/Visual/CameraEffects.cs` | Directional shake, punch, recoil |
| `Assets/Scripts/Visual/TimeController.cs` | Hit stop, slow motion |
| `Assets/Scripts/Visual/PostProcessController.cs` | Chromatic aberration, vignette pulse |

### Files MODIFIED

| File | Changes |
|------|---------|
| `Assets/Scripts/Visual/ScreenEffectsController.cs` | Integrated with new effects |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/UI/*` — UI components
- `Assets/Scripts/Dice/*` — Dice system

---

## Implementation Details

### A1: CameraEffects.cs

- Directional shake toward impact source
- Camera punch (quick zoom + recoil) on player damage
- Subscribes to GameEvents automatically

### A2: TimeController.cs

- Hit stop (0.05s freeze on kill)
- Slow motion on wave clear (0.5x for 1 second)
- Uses unscaled time for proper UI response

### A3: PostProcessController.cs

- Vignette pulse on damage
- Chromatic aberration on damage
- Saturation boost on kills

---

## Success Criteria

- [x] Camera shakes directionally on damage
- [x] Hit stop occurs on enemy kills
- [x] Slow motion triggers on wave clear
- [x] Post-process effects respond to events
- [x] All effects testable via ContextMenu

---

## Test Steps

1. Open `Battle.unity`
2. Press Play
3. Take damage → Verify camera shake + punch + vignette
4. Kill enemy → Verify hit stop
5. Clear wave → Verify slow motion

