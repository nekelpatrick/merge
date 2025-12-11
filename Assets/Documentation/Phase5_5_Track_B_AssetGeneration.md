# Phase 5.5 - Track B: 3D Asset Generation Guide

**Task:** Track B Step 1 - Run 3D asset creator and assign materials  
**Time:** 35 minutes  
**Status:** Ready to Execute

---

## Step 1: Execute 3D Asset Creator (5 minutes)

### In Unity Editor:

1. **Open Unity Project**
   - Ensure you're in the Shield Wall project
   - Wait for compilation to complete

2. **Run Asset Creator:**
   - Menu: `Shield Wall Builder > 3D Assets > Create All 3D Assets (One-Click)`
   - Wait for console messages

3. **Verify Console Output:**
   ```
   --- Creating 3D Assets ---
   Creating limb prefabs...
   Created: SeveredHead.prefab
   Created: SeveredArm.prefab
   Created: SeveredLeg.prefab
   Creating toon materials...
   Created: M_Character_Player.mat
   Created: M_Character_Brother.mat
   Created: M_Character_Thrall.mat
   Created: M_Character_Warrior.mat
   Created: M_Character_Berserker.mat
   Created: M_Character_Archer.mat
   Created: M_Blood.mat
   Created: M_Gore.mat
   Creating environment props...
   Created ground plane prefab
   Creating blood VFX...
   Created blood decal prefabs
   --- 3D Assets Complete ---
   ```

4. **Verify Created Assets:**
   
   **Check Prefabs/Gore:**
   - Navigate to `Assets/Prefabs/Gore/`
   - Should contain: `SeveredHead.prefab`, `SeveredArm.prefab`, `SeveredLeg.prefab`
   
   **Check Materials:**
   - Navigate to `Assets/Art/Materials/Characters/`
   - Should contain: M_Character_*.mat files
   
   **Check Environment:**
   - Navigate to `Assets/Prefabs/` or `Assets/Art/`
   - Should contain ground plane and prop prefabs

---

## Step 2: Assign Materials to Characters (30 minutes)

### Open Battle Scene

1. **Load Scene:**
   - File > Open Scene
   - Navigate to `Assets/Scenes/Battle.unity`
   - Open scene

### Assign Brother Materials

2. **Find Brother GameObjects:**
   - In Hierarchy, search for "Brother" or look under formation/brothers
   - Typical names: `BrotherFarLeft`, `BrotherLeft`, `BrotherRight`, `BrotherFarRight`

3. **For Each Brother:**
   
   **Locate Body Mesh:**
   - Expand brother GameObject hierarchy
   - Find child with MeshRenderer (typically named "Body" or "Visual")
   
   **Assign Material:**
   - Select body GameObject
   - In Inspector, find MeshRenderer component
   - Materials array, Element 0:
     - Drag `Assets/Art/Materials/Characters/M_Character_Brother.mat`
   
   **Repeat for all 4 brothers:**
   - All brothers should use `M_Character_Brother.mat`
   - This gives them consistent blue-gray Viking look

### Assign Enemy Materials

4. **Find Enemy GameObjects:**
   - In Hierarchy, look under "EnemySpawnPoint" or "Enemies"
   - May be spawned at runtime - if so, modify enemy prefabs instead

5. **Locate Enemy Prefabs:**
   - Navigate to where enemies are stored (check EnemyWaveController references)
   - Typical location: `Assets/Prefabs/` or spawned dynamically

6. **Assign by Enemy Type:**
   
   **Thralls:**
   - Material: `M_Character_Thrall.mat` (brown/leather)
   
   **Warriors:**
   - Material: `M_Character_Warrior.mat` (gray/iron)
   
   **Berserkers:**
   - Material: `M_Character_Berserker.mat` (red/blood)
   
   **Archers:**
   - Material: `M_Character_Archer.mat` (green/forest)
   
   **Spearmen:**
   - Material: `M_Character_Warrior.mat` (same as warrior)
   
   **ShieldBreakers:**
   - Material: `M_Character_Warrior.mat` (dark variant)

### Assign Player Shield Material

7. **Find Player Shield:**
   - In Hierarchy, search for "Shield" or "PlayerShield"
   - Typically under Camera or PlayerView

8. **Assign Material:**
   - Select shield GameObject
   - MeshRenderer > Materials:
     - Element 0: `M_Character_Player.mat` (iron gray)

### Test Materials

9. **Enter Play Mode:**
   - Press Play button
   - Observe character colors:
     - Brothers: Blue-gray (distinct from enemies)
     - Enemies: Brown, gray, red, green (varied)
     - Player shield: Iron gray (metallic)

10. **Verify Toon Shader:**
    - Look for cel-shaded bands on characters
    - Should have visible light/shadow transitions
    - If not cel-shaded, materials may need shader assignment

---

## Step 3: Material Shader Configuration (If Needed)

If materials don't have toon shader applied:

1. **Open Material:**
   - Select `M_Character_Brother.mat` in Project view

2. **Assign Shader:**
   - Inspector > Shader dropdown
   - Select: `Universal Render Pipeline/Lit` or custom toon shader if available

3. **Configure Properties:**
   - Surface Type: Opaque
   - Workflow Mode: Metallic
   - Metallic: 0.1-0.3
   - Smoothness: 0.3-0.5

4. **Repeat for All Materials**

---

## Validation Checklist

After completing Step 2:

### Assets Created ✓
- [ ] `Assets/Prefabs/Gore/SeveredHead.prefab` exists
- [ ] `Assets/Prefabs/Gore/SeveredArm.prefab` exists
- [ ] `Assets/Prefabs/Gore/SeveredLeg.prefab` exists
- [ ] `Assets/Art/Materials/Characters/M_Character_Brother.mat` exists
- [ ] `Assets/Art/Materials/Characters/M_Character_Thrall.mat` exists
- [ ] `Assets/Art/Materials/Characters/M_Character_Warrior.mat` exists
- [ ] `Assets/Art/Materials/Characters/M_Character_Berserker.mat` exists
- [ ] `Assets/Art/Materials/Characters/M_Character_Archer.mat` exists

### Materials Assigned ✓
- [ ] All 4 brothers use M_Character_Brother.mat
- [ ] Enemies use type-specific materials
- [ ] Player shield uses M_Character_Player.mat

### Visual Verification ✓
- [ ] Brothers visually distinct from enemies (blue-gray vs varied)
- [ ] Enemies have color variety (brown, gray, red, green)
- [ ] Toon shader visible (cel-shaded look)
- [ ] No pink "missing material" indicators

### Performance ✓
- [ ] Play mode runs at 60 FPS (or acceptable framerate)
- [ ] No console errors related to materials

---

## Troubleshooting

### Issue: Assets Not Created

**Symptom:** Console shows errors, prefabs/materials missing

**Fix:**
- Check folder structure: `Assets/Prefabs/Gore/` and `Assets/Art/Materials/Characters/` must exist
- Create folders manually if missing
- Re-run `Create All 3D Assets` menu item

### Issue: Materials Show Pink

**Symptom:** Characters are bright pink/magenta

**Fix:**
- Materials missing shader assignment
- Open material, assign `Universal Render Pipeline/Lit` shader
- Ensure URP pipeline asset configured

### Issue: No Toon Shading Visible

**Symptom:** Characters look flat, no cel-shaded bands

**Fix:**
- Toon shader may need custom implementation
- For now, use URP/Lit shader with adjusted Smoothness (0.3-0.4)
- Phase 5.5 can proceed with basic lit materials

### Issue: Characters Still Look Ugly

**Symptom:** Materials applied but still brown blobs

**Expected:** This is normal! Materials improve colors but models are still primitives
- Track B Step 2 (character model replacement) will fix this
- Current goal: Make brothers DISTINCT from enemies via color

---

## Next Steps

After completing this task:

1. **Mark Todo Complete:**
   - Track B: Run 3D asset creator and assign materials ✅

2. **Proceed to:**
   - Track B Step 2: Source/create character models (4-5 hours)
   - OR continue Track A: Wire UI components in Battle scene
   - OR start Track C: Wire ScreenEffects

3. **Quick Win Gate Check:**
   - If combining with Track A Step 1 (dice labels fixed), validate Quick Win criteria
   - Characters should be slightly less ugly (colored materials vs default brown)

---

## Time Breakdown

- Run asset creator: 5 minutes
- Assign brother materials: 5 minutes
- Assign enemy materials: 10 minutes
- Assign player shield: 2 minutes
- Test in Play mode: 5 minutes
- Troubleshooting buffer: 8 minutes

**Total: ~35 minutes**

---

**Status:** Ready for Unity Editor execution

**Blocker:** Must be done in Unity Editor (cannot automate via code)

**Priority:** HIGH - Required for visual improvements

**Track:** B (Visual Upgrade)
