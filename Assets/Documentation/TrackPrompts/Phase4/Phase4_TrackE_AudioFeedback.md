# Phase 4 - Track E: Audio Feedback

## Assignment

You are implementing **Track E** of Phase 4: Polish & Juice.

Your focus is improving audio responsiveness and feedback.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Scripts/Audio/CombatSFXController.cs` | Combat sound triggering |
| `Assets/Scripts/Audio/UISFXController.cs` | UI interaction sounds |
| `Assets/Scripts/Audio/AmbientController.cs` | Environmental ambiance |

### Files MODIFIED

| File | Changes |
|------|---------|
| `Assets/Scripts/Audio/SFXPlayer.cs` | Added pitch variation, overlap limit, pooling |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/Visual/*` — Visual systems

---

## Implementation Details

### E1: SFXPlayer.cs Enhancements

- Multiple AudioSource pooling (8 sources)
- Pitch randomization (0.9-1.1)
- Minimum repeat delay to prevent stacking
- PlayRandom for array of clips

### E2: CombatSFXController.cs

- Subscribes to all combat events
- Hit, block, kill, wound, wave sounds
- Victory and defeat sounds

### E3: UISFXController.cs

- Button hover, click, disabled sounds
- Dice roll, lock, unlock sounds
- Action select and confirm sounds
- Combo discovery chime

### E4: AmbientController.cs

- Battle ambience loop
- Crowd murmur, cheer, gasp
- Dynamic volume based on battle state

---

## Success Criteria

- [x] Sound effects have pitch variation
- [x] No audio clipping from overlapping sounds
- [x] Combat events trigger appropriate sounds
- [x] UI interactions have audio feedback

