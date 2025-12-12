# How to Convert GLB to FBX (Unity-Compatible Format)

**Problem:** Unity doesn't have native GLB support installed.

**Solution:** Convert the GLB file to FBX format, which Unity supports natively.

---

## Option 1: Automated (Install glTFast Package)

**In Unity Editor:**

1. Click: `ShieldWall > Setup > Install glTFast Package (Required for GLB Import)`
2. Click "Install Now"
3. Wait for installation to complete
4. Wait for Unity to finish compiling
5. Continue with Viking setup steps

---

## Option 2: Manual Conversion (Using Blender - Free)

### Step 1: Install Blender (if not already installed)
- Download: https://www.blender.org/download/
- Install and open Blender

### Step 2: Import GLB
1. In Blender: `File > Import > glTF 2.0 (.glb/.gltf)`
2. Navigate to: `C:\Users\PatrickLocal\merge\Assets\Art\Models\Characters\`
3. Select: `Viking_Player.glb`
4. Click "Import glTF 2.0"

### Step 3: Export as FBX
1. In Blender: `File > Export > FBX (.fbx)`
2. Save location: `C:\Users\PatrickLocal\merge\Assets\Art\Models\Characters\`
3. Filename: `Viking_Player.fbx`
4. In export settings (right panel):
   - **Path Mode:** Copy
   - **Embed Textures:** ✓ Checked
   - **Apply Scalings:** FBX Units Scale
   - **Forward:** -Z Forward
   - **Up:** Y Up
5. Click "Export FBX"

### Step 4: Update Unity
1. Return to Unity Editor
2. Unity will automatically import the new FBX file
3. Delete the GLB file (no longer needed):
   - Right-click `Viking_Player.glb` → Delete
4. Update the setup script path:
   - The script will automatically find `Viking_Player.fbx` instead

---

## Option 3: Use Online Converter

### Step 1: Upload to Converter
- Website: https://products.aspose.app/3d/conversion/glb-to-fbx
- Or: https://www.online-convert.com/

### Step 2: Convert
1. Upload `Viking_Player.glb`
2. Select output format: FBX
3. Click "Convert"
4. Download the converted file

### Step 3: Move to Unity
1. Save as: `C:\Users\PatrickLocal\merge\Assets\Art\Models\Characters\Viking_Player.fbx`
2. Unity will import automatically

---

## Option 4: Update Script to Use FBX Path

If you already have an FBX version, just update the path:

**Edit:** `Assets/Editor/AutomatedEnemySetup.cs`

Change line 12:
```csharp
// OLD:
private const string VIKING_MODEL_PATH = "Assets/Art/Models/Characters/Viking_Player.glb";

// NEW:
private const string VIKING_MODEL_PATH = "Assets/Art/Models/Characters/Viking_Player.fbx";
```

---

## Recommended: Option 1 (Install glTFast)

This is the easiest solution - just run the menu command and Unity will handle everything!

```
ShieldWall > Setup > Install glTFast Package (Required for GLB Import)
```

After installation:
- Unity can import GLB files natively
- No need to convert
- Future GLB models will work automatically
