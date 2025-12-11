# Phase 4 - Track G: Polish Testing Tools

## Assignment

You are implementing **Track G** of Phase 4: Polish & Juice.

Your focus is creating tools for testing and tuning polish effects.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Editor/PolishTestWindow.cs` | EditorWindow for triggering effects |
| `Assets/Scripts/Debug/PolishDebugger.cs` | Runtime debug commands |

### Files MODIFIED

| File | Changes |
|------|---------|
| `Assets/Editor/ShieldWallSceneBuilder.cs` | Added SetupPolishComponents menu item |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic (except adding components)

---

## Implementation Details

### G1: PolishTestWindow.cs

EditorWindow with sections:
- Camera Effects (shake light/medium/heavy, punch)
- Time Control (hit stop, slow motion, reset)
- VFX (blood, block effect, post process)
- Game Events (simulate player wounded, enemy killed, etc.)

### G2: PolishDebugger.cs

Runtime MonoBehaviour:
- F1: Camera Shake
- F2: Camera Punch
- F3: Hit Stop
- F4: Slow Motion
- F5: Blood VFX
- F6: Block VFX
- F7: Post Process
- F8-F10: Game Events
- F12: Toggle debug display

### G3: ShieldWallSceneBuilder Integration

- Added `SetupPolishComponents()` method
- Creates PolishEffects, AudioControllers, DebugTools GameObjects
- Adds all Phase 4 components to scene

---

## Success Criteria

- [x] Polish Test Window accessible from menu
- [x] All effects triggerable from Editor
- [x] Runtime debug keys work in Play mode
- [x] Polish components auto-added during scene build

