# Phase 4 - Track D: UI Juice

## Assignment

You are implementing **Track D** of Phase 4: Polish & Juice.

Your focus is creating UI animations and feedback effects.

---

## Your Scope

### Files CREATED

| File | Purpose |
|------|---------|
| `Assets/Scripts/UI/UIAnimator.cs` | Shared UI animation utilities |
| `Assets/Scripts/UI/ButtonFeedback.cs` | Hover, click, disabled state animations |
| `Assets/Scripts/UI/HealthHeartAnimator.cs` | Heart beat, crack, shatter animations |
| `Assets/Scripts/UI/StaminaDrainEffect.cs` | Stamina bar pulse, critical warning |

---

## DO NOT TOUCH

- `Assets/Scripts/Core/*` — Core systems
- `Assets/Scripts/Combat/*` — Combat logic
- `Assets/Scripts/Visual/*` — Visual systems (except UI-related)

---

## Implementation Details

### D1: UIAnimator.cs

Static utility class with:
- PunchScale, Shake, FadeIn, FadeOut
- SlideIn, ScaleBounce, FlashColor, Pulse

### D2: ButtonFeedback.cs

- Hover scale animation
- Click punch animation
- Glow effect on hover
- IPointerEnterHandler, IPointerExitHandler, etc.

### D3: HealthHeartAnimator.cs

- Idle pulse animation
- Critical state (faster pulse, red color)
- Damage flash animation
- Shatter animation on death

### D4: StaminaDrainEffect.cs

- Drain punch animation
- Critical warning pulse
- Color transition to red when low

---

## Success Criteria

- [x] UI elements have hover and click feedback
- [x] Health hearts pulse and crack on damage
- [x] Stamina bar warns when critical
- [x] All animations use unscaled time

