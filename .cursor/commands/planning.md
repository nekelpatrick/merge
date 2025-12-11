[1 tool called]

Here's an improved, game-context-aware version of your prompt:

---

**Improved Prompt:**

> Create a detailed implementation plan for **Shield Wall** (first-person turn-based Viking tactical survival game).
> 
> **Requirements:**
> 1. Break down into the smallest possible atomic steps — each step should accomplish exactly ONE thing
> 2. Every step MUST begin with a research phase before any implementation:
>    - Examine existing codebase structure
>    - Identify dependencies and integration points
>    - Review relevant ScriptableObject patterns and event conventions already in use
>    - Check the GDD specifications for that specific system
>
> 3. Order steps by dependency chain — never implement something that depends on code that doesn't exist yet
>
> 4. For each step, specify:
>    - **Research:** What files/patterns to examine first
>    - **Scope:** The ONE thing this step accomplishes
>    - **Validation:** How to verify it works in isolation
>    - **Integration:** What events or interfaces connect it to other systems
>
> 5. Start from the core and expand outward:
>    - Data layer (ScriptableObjects) → Logic layer → Presentation layer
>    - State machine skeleton → Individual states → Transitions
>    - Events defined → Events fired → Events subscribed
>
> 6. Each step should be testable in isolation before moving to the next
>
> **Focus area for this plan:** [SPECIFY: e.g., "Rune Dice System", "Shield Wall System", "Combat Resolution", "Turn State Machine", etc.]
>
> Use the project structure from `Assets/Documentation/GameDesignDocument.md` and follow the patterns defined in the workspace rules for event-driven architecture, ScriptableObjects for data, and clear separation between game logic and UI.

---

**Why this is better:**

| Original Problem | Improved Solution |
|------------------|-------------------|
| Vague "very small steps" | Defines "atomic" = ONE thing per step |
| "Do a research" is unclear | Specifies exactly WHAT to research (files, patterns, GDD) |
| No structure | Enforces data → logic → presentation order |
| No validation | Requires isolation testing per step |
| No context | Explicitly references Shield Wall systems and GDD |
| Generic | Asks for a specific focus area to avoid scope creep |