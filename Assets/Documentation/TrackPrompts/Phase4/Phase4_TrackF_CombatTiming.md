# Phase 4 - Track F: Combat Timing

## Assignment

You are implementing **Track F** of Phase 4: Polish & Juice.

Your focus is improving combat pacing and attack emphasis.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Scripts/Combat/CombatPacer.cs` | Configurable timing delays |
| `Assets/Scripts/Combat/AttackTelegraph.cs` | Enemy attack wind-up visual |
| `Assets/Scripts/Visual/HitEmphasis.cs` | Combined hit effects coordinator |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/UI/*` — UI components
- `Assets/Scripts/Dice/*` — Dice system

---

## Implementation Details

### F1: CombatPacer.cs

Singleton with configurable delays:
- Action anticipation delay (0.3s)
- Action execution delay (0.2s)
- Between actions delay (0.15s)
- Enemy telegraph duration (0.4s)
- Between enemy attacks delay (0.25s)
- Phase transition delay (0.5s)
- Wave start/end delays

### F2: AttackTelegraph.cs

- Enemy flashes red before attacking
- Pullback and strike forward motion
- Group telegraph for multiple enemies
- Audio cue on telegraph

### F3: HitEmphasis.cs

Coordinates all impact effects:
- Triggers CameraEffects shake/punch
- Triggers TimeController hit stop
- Triggers PostProcessController effects
- Triggers ImpactVFXController blood/block

---

## Success Criteria

- [x] Combat resolution is paced, not instant
- [x] Enemies telegraph attacks before striking
- [x] Hit emphasis combines all feedback systems
- [x] Timing is configurable via inspector

