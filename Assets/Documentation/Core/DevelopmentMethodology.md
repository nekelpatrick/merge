# Shield Wall - AI-Assisted Development Methodology

## Overview

This document describes the parallel-track development system used to accelerate AI-assisted game development while maintaining code quality and avoiding conflicts.

---

## The Phase-Track-Agent Model

```
┌─────────────────────────────────────────────────────────────┐
│                     PHASE DOCUMENT                          │
│   (High-level goals, architecture diagrams, success criteria)│
└─────────────────────────┬───────────────────────────────────┘
                          │
          ┌───────────────┼───────────────┐
          │               │               │
          ▼               ▼               ▼
    ┌──────────┐    ┌──────────┐    ┌──────────┐
    │ Track A  │    │ Track B  │    │ Track C  │    ...
    │ Agent 1  │    │ Agent 2  │    │ Agent 3  │
    └──────────┘    └──────────┘    └──────────┘
          │               │               │
          └───────────────┼───────────────┘
                          │
                          ▼
                    ┌──────────┐
                    │ MERGE &  │
                    │ INTEGRATE│
                    └──────────┘
```

---

## Step 1: Create Phase Document

A **Phase** represents a major milestone in development (e.g., "Greybox Prototype", "Content + Polish", "Visual Style").

### Phase Document Structure

```markdown
# Phase N: [Name]

## Overview
- What this phase delivers
- High-level goals

## Architecture Diagram
- Mermaid flowcharts showing system relationships
- Event flow between components

## Track A: [Feature Area]
### A1: Specific task
### A2: Specific task
...

## Track B: [Feature Area]
...

## Implementation Order
| Step | Track | Deliverable | Dependencies |

## Success Criteria
- [ ] Checkbox list of must-have features
```

### Key Principles for Phase Documents

1. **Self-contained** - Each phase doc has all context needed
2. **Dependency-aware** - Implementation order considers what depends on what
3. **Testable** - Success criteria are concrete and verifiable
4. **Architecture-first** - Diagrams before code

---

## Step 2: Design Independent Tracks

**Tracks** are parallel workstreams that can be developed independently with minimal merge conflicts.

### Track Design Rules

| Rule | Why |
|------|-----|
| **One folder/domain per track** | Avoids file conflicts |
| **No cross-track dependencies within phase** | Enables parallel work |
| **Clear interfaces** | Tracks communicate via events, not direct references |
| **Numbered sub-tasks (A1, A2...)** | Preserves implementation order within track |

### Example Track Breakdown (Phase 3)

| Track | Domain | Files Modified |
|-------|--------|----------------|
| A | 3D Visual Primitives | `Scripts/Visual/*` |
| B | Scene Atmosphere | `Editor/BattleSceneSetup.cs`, Lighting |
| C | Battle Scenarios | `Scripts/Data/`, `ScriptableObjects/Scenarios/` |
| D | Audio Integration | `Scripts/Audio/`, `Audio/*` |
| E | Tutorial System | `Scripts/Tutorial/`, `Scripts/UI/TutorialHintUI.cs` |
| F | Menus | `Scenes/MainMenu.unity`, `Scripts/UI/Menu*.cs` |
| G | Editor Automation | `Editor/*.cs` |

### Avoiding Conflicts

- Tracks should NOT modify the same files
- If shared file must be touched (e.g., `router.go`), designate ONE track as owner
- Use events/interfaces for cross-track communication
- Integration happens AFTER tracks complete

---

## Step 3: Assign Tracks to Agents

Each **Track** is given to a separate AI agent session for implementation.

### Agent Assignment Protocol

1. **Open new agent session** (new chat/context)
2. **Provide context:**
   - Full phase document
   - Specific track assignment (e.g., "Implement Track A only")
   - Existing codebase patterns (via `.cursorrules` or workspace rules)
3. **Agent works independently** on assigned track
4. **Agent commits/PRs** when track complete

### Agent Instructions Template

```
You are implementing Track [X] of Phase [N].

## Your Scope
- [List of files to create/modify]
- [Specific tasks A1, A2, etc.]

## DO NOT TOUCH
- Files owned by other tracks
- Core systems unless explicitly listed

## Integration Points
- Subscribe to these events: [list]
- Fire these events: [list]

## When Complete
- All tasks A1-An implemented
- No compiler errors
- Manual test: [specific test steps]
```

---

## Step 4: Merge & Integrate

After all tracks complete, integration phase begins.

### Integration Checklist

1. **Merge all track branches** (resolve any conflicts)
2. **Verify event wiring** - Do systems respond to each other?
3. **Run full test suite** - Unit + Integration
4. **Manual playtest** - Success criteria from phase doc
5. **Fix integration bugs** - Often cross-track communication issues

### Common Integration Issues

| Issue | Cause | Fix |
|-------|-------|-----|
| Events not firing | Subscriber not registered | Check `OnEnable`/`OnDisable` |
| Null references | Initialization order | Use `Awake` for refs, `Start` for deps |
| Missing assets | Track created but didn't commit | Check `.meta` files |
| Scene not updated | Track modified prefab, not scene | Re-add prefabs to scene |

---

## Benefits of This Approach

### Parallelization
- N agents working simultaneously = N× throughput
- No waiting for one feature to complete before starting another

### Reduced Context Switching
- Each agent focuses on ONE domain
- Deep understanding of assigned track

### Minimized Conflicts
- Tracks own specific folders/files
- Git merges are clean

### Clear Accountability
- Each track has defined deliverables
- Easy to identify which track caused issues

### Scalable
- Add more tracks = add more agents
- Works for solo dev (sequential) or team (parallel)

---

## When NOT to Use This Approach

| Scenario | Why Not | Alternative |
|----------|---------|-------------|
| Tightly coupled refactor | Everything touches everything | Single agent, sequential |
| Exploration/prototyping | Scope unclear | Single agent, iterative |
| Bug fixing | Often cross-cutting | Single agent with full context |
| Small features | Overhead not worth it | Just do it |

---

## Template: Track Assignment Prompt

```markdown
# Track [X] Implementation

## Context
[Paste relevant section of phase document]

## Your Deliverables
1. [File 1] - [description]
2. [File 2] - [description]
...

## Constraints
- Follow existing patterns in [reference file]
- Use events from `GameEvents.cs` for communication
- Do NOT modify files outside your track

## Success Criteria
- [ ] Criteria 1
- [ ] Criteria 2

## Test Steps
1. Open scene [X]
2. Press Play
3. Verify [behavior]
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2025-01-XX | Initial methodology documentation |

---

## Related Documents

- [`GameDesignDocument.md`](GameDesignDocument.md) - Game design reference
- [`VisualStyleSystem.md`](VisualStyleSystem.md) - Visual implementation guide
- Phase documents in `.cursor/plans/`

