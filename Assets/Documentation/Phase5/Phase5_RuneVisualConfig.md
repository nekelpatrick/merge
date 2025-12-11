# Rune Visual Configuration - Phase 5

**Date:** December 2024  
**Purpose:** Define canonical rune labels, colors, and UI display rules

---

## Rune Visual Mapping

### Complete Rune Data

| Type | Full Name | Short Code | Symbol | Domain | Color Hex | RGB |
|------|-----------|------------|--------|--------|-----------|-----|
| **Thurs** | Shield | SH | ᚦ | Defense | `#5C5C5C` | (92, 92, 92) |
| **Tyr** | Axe | AX | ᛏ | Attack | `#8B2020` | (139, 32, 32) |
| **Gebo** | Spear | SP | ᚷ | Precision | `#8B6914` | (139, 105, 20) |
| **Berkana** | Brace | BR | ᛒ | Support | `#3D5C3D` | (61, 92, 61) |
| **Othala** | Odin | OD | ᛟ | Wild | `#C9A227` | (201, 162, 39) |
| **Laguz** | Loki | LO | ᛚ | Chaos | `#5C3D6E` | (92, 61, 110) |

---

## Display Rules by Context

### 1. Dice UI (Individual Die Display)

**Priority:** Symbol + Color background

```
┌─────────┐
│   ᚦ    │  ← Symbol in white
│         │     Background in rune color
└─────────┘
  (Gray BG)
```

**Fallback:** Short code if unicode not available
```
┌─────────┐
│   SH    │
└─────────┘
```

---

### 2. Action Requirements (Cost Display)

**Priority:** Colored badges with symbols

```
Action: Shield Wall
Cost: ●● (2x Shield)
      ^^ Both badges in Iron Gray (#5C5C5C)
```

**Implementation:**
- Use UI Image with colored sprite
- Arrange horizontally for multiple runes
- Tooltip on hover shows full name

---

### 3. Action Preview Panel

**Priority:** Full names with colored badges

```
┌──────────────────────────────────┐
│ Shield Wall                      │
│ [Shield] [Shield]                │
│ ^^^^^^^^^^^^^^^^^                │
│ Badges colored Iron Gray         │
│                                  │
│ Effect: Block 1 attack on you    │
└──────────────────────────────────┘
```

---

### 4. Tooltips / Help Text

**Priority:** Full name + domain + description

```
Shield (ᚦ)
Domain: Defense
"The rune of protection. Lock Shield runes to block attacks."
```

---

## UI Color Palette (from VisualStyleSystem.md)

### Text Colors
- **Primary Text:** Bone White `#D4C8B8` (212, 200, 184)
- **Secondary Text:** Muted Tan `#A89880` (168, 152, 128)
- **Accent:** Gold `#C9A227` (201, 162, 39)

### Background Colors
- **Panel Background:** Dark Brown `#2A1F1A` (42, 31, 26)
- **Panel:** Worn Leather `#6B4E3D` (107, 78, 61) at 80% alpha

### Status Colors
- **Ready/Available:** Gold `#C9A227`
- **Locked In:** Bone White `#D4C8B8`
- **Disabled/Unmet:** Muted Tan `#A89880` at 50% alpha

---

## Implementation Classes

### RuneDisplay.cs

Static utility class providing:
- `GetFullName(RuneType)` → "Shield", "Axe", etc.
- `GetShortCode(RuneType)` → "SH", "AX", etc.
- `GetSymbol(RuneType)` → "ᚦ", "ᛏ", etc.
- `GetDefaultColor(RuneType)` → Color struct
- `GetDomain(RuneType)` → "Defense", "Attack", etc.

### RuneSO (existing)

ScriptableObject with:
- `runeType: RuneType`
- `runeName: string`
- `icon: Sprite`
- `color: Color`
- `description: string`

**Note:** RuneSO can override default colors if needed per-instance.

---

## Rune Badge Sprite Creation

### Recommended Approach:

1. **Simple colored circles** (16x16 or 32x32)
   - Solid fill with rune color
   - Optional: Symbol overlay in white
   - Use Unity Sprite Creator or simple PNG

2. **Usage:**
   - Action requirement badges: Small (16x16)
   - Die faces: Medium (64x64)
   - Tutorial hints: Large (128x128)

### Asset Locations:
```
Art/
  UI/
    Runes/
      Badge_Shield.png
      Badge_Axe.png
      Badge_Spear.png
      Badge_Brace.png
      Badge_Odin.png
      Badge_Loki.png
```

---

## Code Usage Examples

### Display action cost with colors

```csharp
using ShieldWall.UI;
using ShieldWall.Dice;

// Get colored badge for Shield rune
Color shieldColor = RuneDisplay.GetDefaultColor(RuneType.Thurs);
string shieldName = RuneDisplay.GetFullName(RuneType.Thurs);

// Display: "Shield Shield" with colored badges
foreach (var rune in action.requiredRunes)
{
    var badge = CreateBadge(RuneDisplay.GetDefaultColor(rune));
    badge.tooltip = RuneDisplay.GetFullName(rune);
}
```

### Display rune in action button

```csharp
// Old (cryptic):
text.text = "[AX][SP]";

// New (clear):
var sb = new StringBuilder();
foreach (var rune in action.requiredRunes)
{
    sb.Append(RuneDisplay.GetFullName(rune)).Append(" ");
}
text.text = sb.ToString().Trim();
// Output: "Axe Spear"
```

---

## Migration Plan

### Phase 5 Step 3 Changes:

1. ✓ Create `RuneDisplay.cs` static helper (DONE)
2. Update `ActionButton.cs` to use full names instead of codes
3. Update `DieVisual.cs` (if exists) to show colored backgrounds
4. Create simple badge sprites (Unity Sprite Creator)
5. Update action requirement display to use badges + colors

### Files to Modify:
- `Scripts/UI/ActionButton.cs` - Replace `GetRuneSymbol()` with `RuneDisplay.GetFullName()`
- `Scripts/UI/DieVisual.cs` - Add colored background based on rune type
- `Prefabs/UI/ActionButton.prefab` - Add Image components for rune badges
- `Prefabs/UI/DieVisual.prefab` - Add background Image component

---

## Validation Checklist

- [ ] RuneDisplay.cs provides all lookup functions
- [ ] Action buttons show "Shield Shield" not "[SH][SH]"
- [ ] Rune colors match Visual Style System exactly
- [ ] Colors are readable on dark UI backgrounds
- [ ] Short codes ("SH", "AX") still available for compact displays if needed

---

## Next Steps

Step 3 complete. Proceeding to Step 4: Update ActionButton to use new display system.
