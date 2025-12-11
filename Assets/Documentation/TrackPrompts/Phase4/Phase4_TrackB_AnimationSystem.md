# Phase 4 - Track B: Animation System

## Assignment

You are implementing **Track B** of Phase 4: Polish & Juice.

Your focus is creating the tweening utility and procedural animations.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Scripts/Core/Tweener.cs` | Lightweight tweening utility |
| `Assets/Scripts/Visual/DiceAnimator.cs` | Dice roll spin, bounce, lock animations |
| `Assets/Scripts/Visual/EnemyAnimator.cs` | Spawn rise, attack telegraph, death collapse |
| `Assets/Scripts/Visual/BrotherAnimator.cs` | Wound stagger, death fall, idle sway |

---

## DO NOT TOUCH

- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/Audio/*` — Audio systems
- `Assets/Editor/*` — Editor scripts

---

## Implementation Details

### B1: Tweener.cs

Static utility class with:
- EaseType enum (Linear, EaseInQuad, EaseOutQuad, EaseOutBounce, EaseOutElastic, Punch, etc.)
- TweenFloat, TweenVector3, TweenColor coroutines
- PunchScale, PunchPosition, Shake utilities

### B2: DiceAnimator.cs

- Roll animation with spin and scale punch
- Lock animation with golden glow pulse
- Hover animation with scale

### B3: EnemyAnimator.cs

- Spawn animation rising from ground
- Idle sway animation
- Attack telegraph with pullback
- Death animation with stagger and collapse

### B4: BrotherAnimator.cs

- Wound stagger with lean
- Death fall animation
- Idle breathing and sway

---

## Success Criteria

- [x] Tweener provides all standard easing curves
- [x] Dice animate on roll and lock
- [x] Enemies animate on spawn and death
- [x] Brothers animate on wound and death

