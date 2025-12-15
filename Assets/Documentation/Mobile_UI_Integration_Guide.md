# Shield Wall - Mobile UI Integration Guide

## MOB-041 to MOB-044: UI Safe Area & Touch-Friendly Updates

This guide explains how to manually apply mobile UI updates to scenes.

---

## MOB-041: Apply SafeAreaFitter to MainMenu

### Manual Steps (Unity Editor)

1. **Open MainMenu scene**: `Assets/Scenes/MainMenu.unity`

2. **Find the root UI panel**:
   - In Hierarchy, expand the Canvas
   - Locate the main panel/group that contains all menu buttons
   - This is likely called "MainPanel" or similar

3. **Add SafeAreaFitter component**:
   - Select the root panel GameObject
   - Click "Add Component"
   - Search for "Safe Area Fitter"
   - Add the component

4. **Configure SafeAreaFitter**:
   - `Conform X`: ✅ Enabled
   - `Conform Y`: ✅ Enabled
   - `Update On Screen Change`: ✅ Enabled
   - `Log Safe Area`: ✅ Enabled (for testing, disable in release)

5. **Test in Device Simulator**:
   - Open Game view
   - Enable Device Simulator (Game view dropdown → select device)
   - Choose a notched device (e.g., iPhone 12, Pixel 4)
   - Verify menu buttons are not clipped by notch/cutout

6. **Save scene**

---

## MOB-042: Apply SafeAreaFitter to Battle HUD

### Manual Steps (Unity Editor)

1. **Open Battle scene**: `Assets/Scenes/Battle.unity`

2. **Find HUD root panels**:
   - Locate the main UI Canvas
   - Find these key panels (or their parent):
     - Dice container
     - Action buttons container
     - Health/Stamina display
     - Wave/enemy display
     - Phase banner

3. **Option A: Apply to individual panels** (more control)
   - Add SafeAreaFitter to each critical panel
   - Configure based on position:
     - Bottom panels (dice): `Conform Y: true, Conform X: false`
     - Top panels (wave/health): `Conform Y: true, Conform X: false`
     - Side panels (if any): `Conform X: true, Conform Y: false`

4. **Option B: Apply to single root HUD container** (simpler)
   - Find or create a single root panel containing all HUD elements
   - Add SafeAreaFitter to this root
   - Configure: `Conform X: true, Conform Y: true`

5. **Test in Device Simulator**:
   - Choose notched device
   - Play scene
   - Verify:
     - Dice are visible and tappable
     - Health/Stamina not clipped
     - All buttons reachable
     - Nothing overlaps notch/punch-hole

6. **Save scene**

---

## MOB-043: Normalize CanvasScaler Settings

### Recommended Mobile Canvas Scaler Settings

For **all** Canvas components in MainMenu and Battle:

1. **Select Canvas**
2. **Find CanvasScaler component**
3. **Configure**:
   - `UI Scale Mode`: Scale With Screen Size
   - `Reference Resolution`: `1920 x 1080` (16:9 landscape standard)
   - `Screen Match Mode`: Match Width Or Height
   - `Match`: `0.5` (balanced between width and height)
   - `Reference Pixels Per Unit`: `100`

### Why These Settings?

- **1920x1080 reference**: Common mobile resolution, scales well across devices
- **Match 0.5**: Ensures UI scales proportionally on various aspect ratios (16:9, 18:9, 19.5:9)
- **Scale With Screen Size**: Maintains relative sizes across screen sizes

### Testing Different Aspect Ratios

Use Device Simulator to test:
- 16:9 (standard tablets, older phones)
- 18:9 (modern mid-range phones)
- 19.5:9 (flagship phones with tall screens)

Verify:
- Buttons remain readable (not too small)
- Text is legible
- Touch targets meet minimum size (80x80 pixels)

---

## MOB-044: Add On-Screen Pause Button

### Manual Steps (Unity Editor)

1. **Open Battle scene**: `Assets/Scenes/Battle.unity`

2. **Create Pause Button**:
   - Right-click in Hierarchy (under Canvas)
   - UI → Button - TextMeshPro
   - Rename to "PauseButton"

3. **Position Button**:
   - Place in top-right or top-left corner
   - RectTransform settings:
     - Anchor: Top-Left or Top-Right corner anchor
     - Position: `X: 80, Y: -80` (adjust as needed)
     - Width: `100`, Height: `100`
   - Ensure it's within safe area (not too close to edge/notch)

4. **Style Button** (optional):
   - Set icon or text (e.g., "⏸" or "||" or "MENU")
   - Apply `ButtonFeedback` component if available
   - Use `UIColorPalette` for consistent styling

5. **Wire Button to PauseMenuUI**:
   - Find PauseMenuUI GameObject in scene
   - Select PauseButton
   - In Button component → OnClick()
   - Drag PauseMenuUI GameObject to the object slot
   - Select function: `PauseMenuUI.Pause()`

6. **Alternative: Create Dedicated Input Controller** (cleaner):
   - Create new script `MobilePauseButton.cs` if needed
   - Or directly call `GameEvents.RaiseApplicationPauseRequested()`

7. **Test**:
   - Play scene
   - Tap pause button
   - Verify pause menu appears
   - Verify resume/restart/quit work

8. **Save scene**

### Responsive Pause Button (Scripting Approach)

If you want the button to auto-hide on desktop and show on mobile:

```csharp
using UnityEngine;
using ShieldWall.Core;

public class MobilePauseButton : MonoBehaviour
{
    void Start()
    {
        // Only show on mobile platforms
        gameObject.SetActive(IsMobilePlatform());
    }

    public void OnPauseClicked()
    {
        GameEvents.RaiseApplicationPauseRequested();
    }

    private bool IsMobilePlatform()
    {
        return Application.platform == RuntimePlatform.Android ||
               Application.platform == RuntimePlatform.IPhonePlayer;
    }
}
```

---

## Touch Target Validation

### Minimum Touch Target Sizes

Verify all interactive elements meet minimum touch target requirements:

- **iOS HIG**: 44x44 points minimum
- **Material Design**: 48x48 dp minimum
- **Our Standard**: 80x80 pixels at 1080p reference resolution

### Elements to Check

- [ ] Dice (each die)
- [ ] Action buttons
- [ ] Reroll button
- [ ] End Turn button
- [ ] Pause button
- [ ] Menu buttons (MainMenu)
- [ ] Scenario cards

### How to Verify

1. Select UI element in Hierarchy
2. Check RectTransform Width/Height
3. If too small, increase size or add padding
4. Test on device (tap accuracy)

---

## Testing Checklist

### MainMenu
- [ ] SafeAreaFitter applied
- [ ] All buttons visible on notched devices
- [ ] Buttons are tappable
- [ ] CanvasScaler set to recommended settings
- [ ] Text is readable on small screens

### Battle
- [ ] SafeAreaFitter applied to critical UI
- [ ] Dice visible and tappable
- [ ] Health/Stamina not clipped
- [ ] Action buttons reachable
- [ ] On-screen pause button visible and works
- [ ] CanvasScaler set to recommended settings
- [ ] All touch targets meet minimum size

### Device Simulator Testing
- [ ] iPhone 12 (notch)
- [ ] Pixel 4 (punch-hole)
- [ ] iPad (16:9 no notch)
- [ ] Generic tall phone (19.5:9)

---

**Status**: ✅ Manual integration steps documented  
**Action Required**: Apply changes to MainMenu.unity and Battle.unity  
**Owner**: Track F (Mobile UI/UX)

