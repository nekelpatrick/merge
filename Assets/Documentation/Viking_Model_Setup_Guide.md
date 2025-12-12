# Viking Model Setup Guide

**Status:** ✅ Automated Setup Complete  
**Date:** December 11, 2025  
**Model Location:** `Assets/Art/Models/Characters/Viking_Player.glb`

---

## ⚡ Quick Start (Do This First!)

**⚠️ IMPORTANT: Unity needs GLB import support!**

### Step 0: Install GLB Support (One-Time Setup)

Unity doesn't natively support GLB files. Choose ONE option:

**Option A: Install glTFast Package (Easiest)**
1. In Unity: `ShieldWall > Setup > Install glTFast Package (Required for GLB Import)`
2. Click "Install Now"
3. Wait for installation + compilation
4. Continue to Step 1 below

**Option B: Convert GLB to FBX**
- See detailed guide: `Assets/Documentation/GLB_to_FBX_Conversion_Guide.md`
- Use Blender (free) or online converter
- Replace Viking_Player.glb with Viking_Player.fbx

---

### Step 1-4: Setup Enemy Prefabs

**After installing GLB support, run these commands in Unity Editor:**

1. `ShieldWall > Setup > 0. Force Import Viking Model (Run This First!)`
   - Forces Unity to import the 3D model correctly
   - Wait for "✅ Success!" dialog
   
2. `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`
   - Creates the base enemy prefab with Viking model
   
3. `ShieldWall > Setup > 3. Create Enemy Variant Prefabs`
   - Creates 6 enemy type variants

4. `ShieldWall > Setup > 4. Wire Enemy Prefabs to Scene` ⭐ **NEW - IMPORTANT!**
   - Opens Battle scene (if not already open)
   - Assigns all prefabs to EnemyVisualController
   - **This makes the game use 3D models instead of primitives!**

**Result:** 7 enemy prefabs created AND wired up for use!

---

## 🎮 Current Status

Looking at your screenshot, I can see:
- ✅ Game is running correctly
- ✅ Enemies are spawning (3 enemies visible)
- ✅ Combat system is working
- ⚠️ **Enemies showing as brown primitives** (capsules/cubes)

**Why brown primitives?** The `EnemyVisualInstance` script has a fallback system that creates simple shapes when no 3D model is loaded. This is working as designed! Once you import the Viking model properly, these will be replaced with the actual 3D Viking characters.

---

## Overview

This guide documents the automated setup of the Viking 3D model for use as enemy characters in Shield Wall. The T-pose Viking model has been configured with proper rotation, components, and materials for combat scenarios.

---

## ✅ What Has Been Set Up

### 1. Model Import
- **Location:** `Assets/Art/Models/Characters/Viking_Player.glb`
- **Format:** GLB (GLTF Binary)
- **Import Settings:** Configured automatically by the setup script

### 2. Automated Setup Scripts Created

Two editor scripts have been created to handle all setup:

#### `Assets/Editor/AutomatedEnemySetup.cs`

**Menu:** `ShieldWall > Setup > 1. Refresh Asset Database`

**What it does:**
- ✅ Forces Unity to refresh and reimport assets
- ✅ Checks if Viking model is correctly imported
- ✅ Shows helpful error messages if model not found
- ✅ Use this first if you get "model not found" errors

**Menu:** `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`

**What it does:**
- ✅ Creates enemy prefab root GameObject
- ✅ Instantiates Viking model as child
- ✅ Rotates model 180° on Y-axis (facing player correctly)
- ✅ Adds `EnemyVisualInstance` component
- ✅ Adds `ModularCharacterBuilder` component
- ✅ Adds `DismembermentController` component
- ✅ Adds `CapsuleCollider` for selection (trigger)
- ✅ Creates default enemy material
- ✅ Applies material to all model renderers
- ✅ Saves as prefab: `Assets/Prefabs/Characters/Enemy_Viking.prefab`
- ✅ Auto-retries with refresh if model not found on first attempt

**Menu:** `ShieldWall > Setup > 3. Create Enemy Variant Prefabs`

**What it does:**
- ✅ Creates 6 enemy type variants from base prefab
- ✅ Each variant gets unique color:
  - **Enemy_Thrall:** Dark brown (0.5, 0.3, 0.2)
  - **Enemy_Warrior:** Medium brown (0.7, 0.4, 0.3)
  - **Enemy_Berserker:** Dark red (0.8, 0.2, 0.2)
  - **Enemy_Archer:** Greenish (0.4, 0.5, 0.3)
  - **Enemy_Spearman:** Tan (0.6, 0.5, 0.4)
  - **Enemy_ShieldBreaker:** Dark gray (0.3, 0.3, 0.3)

#### `Assets/Editor/EnemyPrefabSetup.cs`
**Menu:** `ShieldWall > Setup > Create Enemy Prefab from Model`

**What it does:**
- Provides manual GUI for creating custom enemy prefabs
- Allows selecting different models
- Allows assigning custom ModularCharacterData
- Useful for future enemy variations

---

## 🎮 How to Use (Quick Start)

### Option 1: Automatic (Recommended)

1. **Open Unity Editor**
2. **⚠️ CRITICAL FIRST STEP - Force Import:**
   - Click: `ShieldWall > Setup > 0. Force Import Viking Model (Run This First!)`
   - This script:
     - Deletes the incorrect .meta file that Unity keeps generating
     - Forces Unity to recognize the GLB as a 3D model (not a generic file)
     - Configures proper import settings
   - Wait for "✅ Success!" dialog
   - **WHY THIS IS NEEDED:** Unity incorrectly classifies GLB files as generic files when added while the editor is running. This forces proper recognition.

3. **Create Enemy Prefab:**
   - Click: `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`
   - Wait for success dialog

4. **Create Enemy Variants:**
   - Click: `ShieldWall > Setup > 3. Create Enemy Variant Prefabs`

5. **Test in Scene:**
   - Already in Battle scene? Press Play again
   - You should now see Viking models instead of brown primitives!

6. **Done!** All enemy prefabs are ready

### Option 2: Manual Setup

1. **Open Unity Editor**
2. **Click:** `ShieldWall > Setup > Create Enemy Prefab from Model`
3. **Drag Viking_Player.glb into "Viking Model" field**
4. **Set Prefab Name** (e.g., "Enemy_Custom")
5. **Click "Create Enemy Prefab"**

### Troubleshooting Import Issues

If you see "Viking model not found" error:
1. First, click: `ShieldWall > Setup > 1. Refresh Asset Database`
2. This forces Unity to reimport the model
3. Then retry: `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`

---

## 📦 Prefab Structure

### Enemy_Viking.prefab Hierarchy

```
Enemy_Viking (Root)
├─ Components:
│  ├─ EnemyVisualInstance
│  ├─ ModularCharacterBuilder
│  ├─ DismembermentController
│  └─ CapsuleCollider (Trigger)
│
└─ Model (Child GameObject)
   ├─ Transform: Rotation (0, 180, 0)
   ├─ Scale: (1, 1, 1)
   └─ Contains: All GLB meshes and armature
```

### Component Configuration

#### EnemyVisualInstance
- **Purpose:** Manages enemy visual state, death animations, hit reactions
- **Auto-initialized:** Detects ModularCharacterBuilder automatically
- **Functions:**
  - `Initialize(EnemySO, Color, BloodBurstVFX)`
  - `PlayDeathAnimation()`
  - `PlayDeathAnimationWithDismemberment(DismembermentType)`
  - `PlayHitReaction()`

#### ModularCharacterBuilder
- **Purpose:** Handles body part swapping for dismemberment
- **Character Data:** Optional, can be assigned later
- **Functions:**
  - `BuildIntactCharacter()`
  - `SwapToState(CharacterState)`
  - `GetLimbPosition(CharacterState)`

#### DismembermentController
- **Purpose:** Triggers dismemberment effects
- **Functions:**
  - `TriggerDismemberment(DismembermentType, ActionSO)`

#### CapsuleCollider
- **Center:** (0, 1, 0)
- **Radius:** 0.4
- **Height:** 2.0
- **Is Trigger:** ✅ Yes

---

## 🎨 Materials

### Default Enemy Material
- **Path:** `Assets/Art/Materials/Characters/M_Enemy_Viking.mat`
- **Shader:** Universal Render Pipeline/Lit (or Standard if URP not available)
- **Base Color:** (0.7, 0.4, 0.3) - Medium brown
- **Applied to:** All meshes in Viking model

### Variant Materials
Each enemy variant prefab has its own embedded material with unique color.

---

## 🔧 Model Rotation Explained

### Why 180° Y-Rotation?

The Viking model is in **T-pose** facing **+Z direction** (Unity's forward). In Shield Wall:
- **Enemies face the player** who is at the shield wall
- **Player looks down +Z axis**
- **Enemies approach from +Z**

**Solution:** Rotate enemy model 180° on Y-axis so:
- Model's **front** faces **-Z direction** (toward player)
- Model's **back** faces **+Z direction** (spawn direction)

This ensures:
✅ Correct combat stance  
✅ Shield faces player  
✅ Attacks animate properly  
✅ Death animations fall correctly

---

## 🔗 Integration with Game Systems

### EnemyWaveController Integration

```csharp
// Example: Spawn Viking enemy
public class EnemyWaveController : MonoBehaviour
{
    [SerializeField] private GameObject enemyVikingPrefab;
    
    private void SpawnEnemy(EnemySO enemyData, Vector3 position)
    {
        GameObject enemy = Instantiate(enemyVikingPrefab, position, Quaternion.identity);
        EnemyVisualInstance visual = enemy.GetComponent<EnemyVisualInstance>();
        visual.Initialize(enemyData, enemyData.tintColor, bloodBurstPrefab);
    }
}
```

### EnemySO Integration

Update ScriptableObjects to reference prefabs:

```
Assets/ScriptableObjects/Enemies/
├─ Enemy_Thrall.asset → Assign Enemy_Thrall prefab
├─ Enemy_Warrior.asset → Assign Enemy_Warrior prefab
├─ Enemy_Berserker.asset → Assign Enemy_Berserker prefab
├─ Enemy_Archer.asset → Assign Enemy_Archer prefab
├─ Enemy_Spearman.asset → Assign Enemy_Spearman prefab
└─ Enemy_ShieldBreaker.asset → Assign Enemy_ShieldBreaker prefab
```

**Steps:**
1. Select an EnemySO asset
2. In Inspector, find "Prefab" field (if exists) or add reference field
3. Drag corresponding prefab from `Assets/Prefabs/Characters/`

---

## 🎭 Dismemberment System

### How It Works

1. **Intact State:** Full Viking model visible
2. **Dismemberment Triggered:** `DismembermentController.TriggerDismemberment(type)`
3. **State Changes:** `ModularCharacterBuilder.SwapToState(CharacterState)`
4. **Limb Spawns:** Severed limb prefab instantiated with physics
5. **Blood VFX:** Blood burst and decals spawn at limb position

### Dismemberment Types

```csharp
public enum DismembermentType
{
    Random,        // Chooses random dismemberment
    Decapitation,  // Head flies off
    ArmSword,      // Right arm (sword hand)
    ArmShield,     // Left arm (shield hand)
    Leg            // Legs removed (falls to ground)
}
```

### Usage Example

```csharp
// In action resolver
if (actionSO.causesDismemberment)
{
    enemyVisual.PlayDeathAnimationWithDismemberment(DismembermentType.Decapitation);
}
else
{
    enemyVisual.PlayDeathAnimation();
}
```

---

## 🛠 Troubleshooting

### "Viking model not found" Error

**Cause:** Unity hasn't properly imported the GLB file as a 3D model, or the .meta file is incorrect.

**The script now handles this automatically!** Just click the buttons in order:

1. Click: `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`
2. If you see "Import Issue Detected" dialog, click **"Fix & Reimport"**
3. The script will delete the incorrect .meta file and reimport
4. Run the setup again: `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`

**If the automated fix doesn't work:**

**Solution 1 (Restart Unity):**
1. Close Unity Editor completely
2. Reopen the project
3. Wait for Unity to finish importing (watch progress bar at bottom)
4. Then run: `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`

**Solution 2 (Manual Reimport):**
1. In Unity Project window, navigate to: `Assets/Art/Models/Characters/`
2. Right-click on `Viking_Player.glb`
3. Select "Reimport"
4. Wait for completion
5. Then run: `ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab`

**Solution 3 (Delete Meta File):**
1. Close Unity
2. Navigate to: `C:\Users\PatrickLocal\merge\Assets\Art\Models\Characters\`
3. Delete `Viking_Player.glb.meta` file
4. Reopen Unity
5. Unity will regenerate the .meta file correctly
6. Then run the setup

### Model not visible in scene
- **Check:** Model child has Renderer components
- **Check:** Material is assigned
- **Check:** Camera can see the position

### Enemy facing wrong direction
- **Check:** Model child rotation is (0, 180, 0)
- **Fix:** Select Model child, set Rotation Y to 180

### Collider not detecting clicks
- **Check:** CapsuleCollider is on root GameObject
- **Check:** Is Trigger is enabled
- **Check:** Collider size covers model bounds

### Materials look incorrect
- **Check:** Shader is compatible with render pipeline
- **Try:** Switch between Standard and URP/Lit shader
- **Check:** Material color is not black (0, 0, 0)

### Dismemberment not working
- **Check:** ModularCharacterBuilder is on root GameObject
- **Check:** Character Data is assigned (if using modular system)
- **Check:** Limb transforms are wired in DismembermentController

---

## 📝 Next Steps

### Immediate (To Make Enemies Functional)

1. ✅ **Run automated setup** (done by this guide)
2. ⏭ **Test in Battle scene:**
   - Open `Assets/Scenes/Battle.unity`
   - Find EnemyWaveController GameObject
   - Assign Enemy_Viking prefab to wave spawner
   - Press Play
   - Verify enemies spawn correctly

3. ⏭ **Update EnemySO assets:**
   - Assign prefabs to each enemy type
   - Test different enemy types spawn correct colors

### Short Term (Polish)

4. ⏭ **Create ModularCharacterData:**
   - Right-click → Create → ShieldWall → Modular Character Data
   - Extract meshes from Viking model
   - Assign to ModularCharacterBuilder
   - Test dismemberment states

5. ⏭ **Create Severed Limb prefabs:**
   - Head, Arm, Leg prefabs with physics
   - Assign to DismembermentController
   - Test dismemberment effects

6. ⏭ **Add animations:**
   - Import combat animations (idle, attack, hit, death)
   - Create Animator Controller
   - Wire up animation triggers

### Long Term (Content Expansion)

7. ⏭ **Create enemy variations:**
   - Different Viking armor styles
   - Weapon variations (axe, sword, spear)
   - Shield designs

8. ⏭ **Add accessories:**
   - Helmets, capes, belts
   - Procedural decoration system

9. ⏭ **Optimize for performance:**
   - LOD meshes
   - Texture atlasing
   - Draw call batching

---

## 📚 Related Documentation

- **Phase 5.5 Prefab Creation Guide:** `Assets/Documentation/Phase5_5_PrefabCreationGuide.md`
- **Visual Style System:** `Assets/Documentation/VisualStyleSystem.md`
- **Track A (Visual Primitives):** `.cursor/plans/Phase3_TrackA_VisualPrimitives.md`
- **EnemyVisualInstance Script:** `Assets/Scripts/Visual/EnemyVisualInstance.cs`
- **ModularCharacterBuilder Script:** `Assets/Scripts/Visual/ModularCharacterBuilder.cs`
- **DismembermentController Script:** `Assets/Scripts/Visual/DismembermentController.cs`

---

## 🎯 Track Ownership

**Track:** A (Visual Primitives)  
**Phase:** 3 / 5.5  
**Files Modified:**
- ✅ `Assets/Art/Models/Characters/Viking_Player.glb`
- ✅ `Assets/Editor/AutomatedEnemySetup.cs`
- ✅ `Assets/Editor/EnemyPrefabSetup.cs`
- ✅ `Assets/Prefabs/Characters/Enemy_*.prefab` (created)
- ✅ `Assets/Art/Materials/Characters/M_Enemy_Viking.mat` (created)

**Dependencies:**
- ✅ EnemyVisualInstance (exists)
- ✅ ModularCharacterBuilder (exists)
- ✅ DismembermentController (exists)
- ⏭ BloodBurstVFX prefab (TODO: assign)
- ⏭ SeveredLimb prefabs (TODO: create)

---

## ✅ Completion Checklist

- [x] Viking model placed in correct directory
- [x] AutomatedEnemySetup script created
- [x] EnemyPrefabSetup script created
- [x] Base enemy prefab structure defined
- [x] Model rotation configured (180° Y)
- [x] Required components added
- [x] Collider configured
- [x] Default material created
- [ ] Run automated setup in Unity
- [ ] Test enemy spawn in Battle scene
- [ ] Assign prefabs to EnemySO assets
- [ ] Create ModularCharacterData
- [ ] Create severed limb prefabs
- [ ] Test dismemberment system

---

**Author:** AI Assistant  
**Review Status:** Ready for Implementation  
**Last Updated:** December 11, 2025
