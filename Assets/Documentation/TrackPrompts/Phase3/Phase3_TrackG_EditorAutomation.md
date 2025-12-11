# Phase 3 - Track G: Editor Automation

## Assignment

You are implementing **Track G** of Phase 3: Content + Polish.

Your focus is updating editor scripts to automate asset creation and scene setup.

---

## Your Scope

### Files to MODIFY

| File | Changes |
|------|---------|
| `Assets/Editor/ShieldWallSetup.cs` | Add scenario/tutorial/wave asset creators |
| `Assets/Editor/BattleSceneSetup.cs` | Add atmosphere/visual setup methods |

---

## DO NOT TOUCH

- Any runtime scripts (`Assets/Scripts/*`)
- Existing assets in `Assets/ScriptableObjects/`
- Scene files directly (setup via editor scripts)

---

## Implementation Details

### G1: ShieldWallSetup.cs Additions

Add these menu items:

```csharp
using UnityEngine;
using UnityEditor;
using ShieldWall.Data;
using System.IO;

public static partial class ShieldWallSetup
{
    // ===== SCENARIOS =====
    
    [MenuItem("ShieldWall/Create Assets/Create Scenario Assets")]
    static void CreateScenarioAssets()
    {
        string path = "Assets/ScriptableObjects/Scenarios";
        EnsureDirectoryExists(path);
        
        // The Breach (Easy)
        var breach = ScriptableObject.CreateInstance<BattleScenarioSO>();
        breach.scenarioName = "The Breach";
        breach.description = "Raiders have broken through the outer wall. Hold them off while the village evacuates.";
        breach.difficulty = Difficulty.Easy;
        breach.startingStamina = 15;
        breach.startingPlayerHealth = 6;
        breach.startingDiceCount = 5;
        breach.isUnlocked = true;
        AssetDatabase.CreateAsset(breach, $"{path}/Scenario_TheBreach.asset");
        
        // Hold the Line (Normal)
        var hold = ScriptableObject.CreateInstance<BattleScenarioSO>();
        hold.scenarioName = "Hold the Line";
        hold.description = "The enemy comes in force. Your shield wall is all that stands between them and your people.";
        hold.difficulty = Difficulty.Normal;
        hold.startingStamina = 12;
        hold.startingPlayerHealth = 5;
        hold.startingDiceCount = 4;
        hold.isUnlocked = true;
        AssetDatabase.CreateAsset(hold, $"{path}/Scenario_HoldTheLine.asset");
        
        // The Last Stand (Hard)
        var last = ScriptableObject.CreateInstance<BattleScenarioSO>();
        last.scenarioName = "The Last Stand";
        last.description = "Berserkers and archers. Few supplies. No retreat. Only glory or death awaits.";
        last.difficulty = Difficulty.Hard;
        last.startingStamina = 10;
        last.startingPlayerHealth = 4;
        last.startingDiceCount = 4;
        last.isUnlocked = false;
        last.prerequisite = hold;
        AssetDatabase.CreateAsset(last, $"{path}/Scenario_TheLastStand.asset");
        
        AssetDatabase.SaveAssets();
        Debug.Log("Created 3 scenario assets in " + path);
    }
    
    // ===== TUTORIAL HINTS =====
    
    [MenuItem("ShieldWall/Create Assets/Create Tutorial Hint Assets")]
    static void CreateTutorialHintAssets()
    {
        string path = "Assets/ScriptableObjects/Tutorial";
        EnsureDirectoryExists(path);
        
        CreateHint(path, "Hint_LockDice", "lock_dice",
            "Click on a die to LOCK it. Locked dice won't re-roll.",
            TurnPhase.DiceRoll, 1, false, true, 6f);
            
        CreateHint(path, "Hint_MatchRunes", "match_runes",
            "Match rune symbols to unlock powerful ACTIONS. Try locking matching dice!",
            TurnPhase.DiceRoll, 1, true, false, 6f);
            
        CreateHint(path, "Hint_Brothers", "brothers_block",
            "Your shield brothers will try to BLOCK attacks for you. Keep them alive!",
            TurnPhase.EnemyReveal, 2, false, false, 5f);
            
        CreateHint(path, "Hint_Stamina", "stamina_drain",
            "STAMINA drains each turn. When it runs out, you lose. Strike fast!",
            TurnPhase.StaminaTick, 3, false, false, 5f);
            
        CreateHint(path, "Hint_Berserkers", "berserkers",
            "BERSERKERS ignore blocks! Kill them quickly or suffer.",
            TurnPhase.EnemyReveal, 4, false, false, 5f);
        
        AssetDatabase.SaveAssets();
        Debug.Log("Created 5 tutorial hint assets in " + path);
    }
    
    static void CreateHint(string path, string fileName, string hintId, string text,
        TurnPhase phase, int wave, bool requiresLocked, bool requiresNoLocked, float duration)
    {
        var hint = ScriptableObject.CreateInstance<TutorialHintSO>();
        hint.hintId = hintId;
        hint.hintText = text;
        hint.triggerPhase = phase;
        hint.triggerWave = wave;
        hint.requiresDiceLocked = requiresLocked;
        hint.requiresNoDiceLocked = requiresNoLocked;
        hint.displayDuration = duration;
        hint.autoDismiss = true;
        hint.pauseGame = false;
        AssetDatabase.CreateAsset(hint, $"{path}/{fileName}.asset");
    }
    
    // ===== EASY WAVES =====
    
    [MenuItem("ShieldWall/Create Assets/Create Easy Wave Assets")]
    static void CreateEasyWaveAssets()
    {
        string path = "Assets/ScriptableObjects/Waves";
        EnsureDirectoryExists(path);
        
        var thrall = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Thrall.asset");
        var warrior = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Warrior.asset");
        
        if (thrall == null || warrior == null)
        {
            Debug.LogError("Could not find enemy assets. Create enemies first.");
            return;
        }
        
        CreateWave(path, "Wave_Easy_01", 1, new[] { thrall, thrall }, true, "tutorial_dice");
        CreateWave(path, "Wave_Easy_02", 2, new[] { thrall, thrall, thrall }, false, "");
        CreateWave(path, "Wave_Easy_03", 3, new[] { thrall, thrall, warrior }, false, "");
        
        AssetDatabase.SaveAssets();
        Debug.Log("Created 3 easy wave assets in " + path);
    }
    
    // ===== HARD WAVES =====
    
    [MenuItem("ShieldWall/Create Assets/Create Hard Wave Assets")]
    static void CreateHardWaveAssets()
    {
        string path = "Assets/ScriptableObjects/Waves";
        EnsureDirectoryExists(path);
        
        var warrior = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Warrior.asset");
        var berserker = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Berserker.asset");
        var archer = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Archer.asset");
        
        if (warrior == null || berserker == null || archer == null)
        {
            Debug.LogError("Could not find enemy assets. Create enemies first.");
            return;
        }
        
        CreateWave(path, "Wave_Hard_01", 1, new[] { warrior, warrior, berserker }, false, "");
        CreateWave(path, "Wave_Hard_02", 2, new[] { berserker, berserker, berserker }, false, "");
        CreateWave(path, "Wave_Hard_03", 3, new[] { archer, archer, warrior, warrior }, false, "");
        CreateWave(path, "Wave_Hard_04", 4, new[] { berserker, berserker, berserker, berserker }, false, "");
        
        AssetDatabase.SaveAssets();
        Debug.Log("Created 4 hard wave assets in " + path);
    }
    
    static void CreateWave(string path, string fileName, int waveNum, EnemySO[] enemies, 
        bool hasEvent, string eventId)
    {
        var wave = ScriptableObject.CreateInstance<WaveConfigSO>();
        wave.waveNumber = waveNum;
        wave.enemies = new System.Collections.Generic.List<EnemySO>(enemies);
        wave.hasScriptedEvent = hasEvent;
        wave.scriptedEventId = eventId;
        AssetDatabase.CreateAsset(wave, $"{path}/{fileName}.asset");
    }
    
    // ===== UTILITIES =====
    
    static void EnsureDirectoryExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path);
            string folder = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
    
    [MenuItem("ShieldWall/Create Assets/Create All Phase 3 Assets")]
    static void CreateAllPhase3Assets()
    {
        CreateScenarioAssets();
        CreateTutorialHintAssets();
        CreateEasyWaveAssets();
        CreateHardWaveAssets();
        Debug.Log("=== All Phase 3 assets created! ===");
    }
}
```

### G2: BattleSceneSetup.cs Additions

Add these menu items:

```csharp
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static partial class BattleSceneSetup
{
    // ===== VOLUME PROFILE =====
    
    [MenuItem("ShieldWall/Scene Setup/Create Volume Profile")]
    static void CreateVolumeProfile()
    {
        string path = "Assets/Settings/BattleVolumeProfile.asset";
        
        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        
        // Color Grading
        var colorGrading = profile.Add<ColorAdjustments>(true);
        colorGrading.saturation.Override(-15f);
        colorGrading.contrast.Override(10f);
        
        // Vignette
        var vignette = profile.Add<Vignette>(true);
        vignette.intensity.Override(0.25f);
        vignette.smoothness.Override(0.4f);
        vignette.color.Override(new Color(0.16f, 0.09f, 0.06f)); // #2A1810
        
        // Bloom
        var bloom = profile.Add<Bloom>(true);
        bloom.intensity.Override(0.5f);
        bloom.threshold.Override(1f);
        bloom.scatter.Override(0.6f);
        
        // Film Grain
        var grain = profile.Add<FilmGrain>(true);
        grain.type.Override(FilmGrainLookup.Medium1);
        grain.intensity.Override(0.1f);
        
        AssetDatabase.CreateAsset(profile, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Created Volume Profile at " + path);
    }
    
    // ===== LIGHTING =====
    
    [MenuItem("ShieldWall/Scene Setup/Setup Battle Lighting")]
    static void SetupBattleLighting()
    {
        // Find or create directional light
        var sun = GameObject.Find("Directional Light");
        if (sun == null)
        {
            sun = new GameObject("Directional Light");
            sun.AddComponent<Light>();
        }
        
        var light = sun.GetComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        light.intensity = 0.8f;
        light.color = new Color(1f, 0.91f, 0.82f); // #FFE8D0
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 0.6f;
        
        // Ambient
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);
        RenderSettings.ambientIntensity = 0.3f;
        
        Debug.Log("Battle lighting configured");
    }
    
    // ===== GROUND PLANE =====
    
    [MenuItem("ShieldWall/Scene Setup/Create Ground Plane")]
    static void CreateGroundPlane()
    {
        // Check for existing
        var existing = GameObject.Find("Ground");
        if (existing != null)
        {
            Debug.LogWarning("Ground already exists. Delete it first to recreate.");
            return;
        }
        
        // Create plane
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(5f, 1f, 5f); // 50x50 units
        
        // Create material
        string matPath = "Assets/Art/Materials/Ground.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.29f, 0.22f, 0.16f); // #4A3728
            mat.SetFloat("_Smoothness", 0.1f);
            mat.SetFloat("_Metallic", 0f);
            
            EnsureDirectoryExists("Assets/Art/Materials");
            AssetDatabase.CreateAsset(mat, matPath);
        }
        
        ground.GetComponent<Renderer>().material = mat;
        
        Debug.Log("Ground plane created");
    }
    
    // ===== VOLUME COMPONENT =====
    
    [MenuItem("ShieldWall/Scene Setup/Add Volume to Scene")]
    static void AddVolumeToScene()
    {
        // Find or create volume
        var volumeObj = GameObject.Find("Global Volume");
        if (volumeObj == null)
        {
            volumeObj = new GameObject("Global Volume");
        }
        
        var volume = volumeObj.GetComponent<Volume>();
        if (volume == null)
            volume = volumeObj.AddComponent<Volume>();
            
        volume.isGlobal = true;
        
        // Load profile
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>("Assets/Settings/BattleVolumeProfile.asset");
        if (profile != null)
        {
            volume.profile = profile;
            Debug.Log("Volume added with BattleVolumeProfile");
        }
        else
        {
            Debug.LogWarning("BattleVolumeProfile not found. Create it first.");
        }
    }
    
    // ===== LAYERS =====
    
    [MenuItem("ShieldWall/Scene Setup/Setup Layers")]
    static void SetupLayers()
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        
        SetLayer(layers, 6, "PlayerView");
        SetLayer(layers, 7, "Brothers");
        SetLayer(layers, 8, "Enemies");
        SetLayer(layers, 9, "Environment");
        
        tagManager.ApplyModifiedProperties();
        
        Debug.Log("Layers configured: PlayerView(6), Brothers(7), Enemies(8), Environment(9)");
    }
    
    static void SetLayer(SerializedProperty layers, int index, string name)
    {
        var layer = layers.GetArrayElementAtIndex(index);
        if (string.IsNullOrEmpty(layer.stringValue))
            layer.stringValue = name;
    }
    
    // ===== FOG =====
    
    [MenuItem("ShieldWall/Scene Setup/Setup Fog")]
    static void SetupFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10f;
        RenderSettings.fogEndDistance = 50f;
        RenderSettings.fogColor = new Color(0.1f, 0.1f, 0.12f);
        
        Debug.Log("Fog configured");
    }
    
    // ===== FULL SETUP =====
    
    [MenuItem("ShieldWall/Scene Setup/Apply Full Atmosphere")]
    static void ApplyFullAtmosphere()
    {
        CreateVolumeProfile();
        SetupBattleLighting();
        CreateGroundPlane();
        AddVolumeToScene();
        SetupLayers();
        SetupFog();
        
        Debug.Log("=== Full battle atmosphere applied! ===");
    }
    
    // ===== VISUAL SPAWN POSITIONS =====
    
    [MenuItem("ShieldWall/Scene Setup/Create Visual Spawn Markers")]
    static void CreateVisualSpawnMarkers()
    {
        // Enemy spawn area marker
        var enemySpawn = new GameObject("EnemySpawnArea");
        enemySpawn.transform.position = new Vector3(0, 1, 7);
        
        // Brother position markers
        CreateMarker("BrotherPos_FarLeft", new Vector3(-3, 0, 1));
        CreateMarker("BrotherPos_Left", new Vector3(-1.5f, 0, 1));
        CreateMarker("BrotherPos_Right", new Vector3(1.5f, 0, 1));
        CreateMarker("BrotherPos_FarRight", new Vector3(3, 0, 1));
        
        Debug.Log("Visual spawn markers created");
    }
    
    static void CreateMarker(string name, Vector3 position)
    {
        var marker = new GameObject(name);
        marker.transform.position = position;
    }
    
    // ===== UTILITIES =====
    
    static void EnsureDirectoryExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = System.IO.Path.GetDirectoryName(path);
            string folder = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
```

---

## Menu Structure

After implementation, the menu will look like:

```
ShieldWall
├── Create Assets
│   ├── Create Scenario Assets
│   ├── Create Tutorial Hint Assets
│   ├── Create Easy Wave Assets
│   ├── Create Hard Wave Assets
│   └── Create All Phase 3 Assets
└── Scene Setup
    ├── Create Volume Profile
    ├── Setup Battle Lighting
    ├── Create Ground Plane
    ├── Add Volume to Scene
    ├── Setup Layers
    ├── Setup Fog
    ├── Apply Full Atmosphere
    └── Create Visual Spawn Markers
```

---

## Dependencies

Track G depends on Track C and Track E definitions:
- `BattleScenarioSO` (Track C)
- `Difficulty` (Track C)
- `TutorialHintSO` (Track E)

These scripts must exist before editor scripts can reference them.

---

## Success Criteria

- [ ] All menu items appear under "ShieldWall" menu
- [ ] "Create Scenario Assets" creates 3 scenarios
- [ ] "Create Tutorial Hint Assets" creates 5 hints
- [ ] "Create Easy Wave Assets" creates 3 waves
- [ ] "Create Hard Wave Assets" creates 4 waves
- [ ] "Create Volume Profile" creates profile with effects
- [ ] "Setup Battle Lighting" configures directional light
- [ ] "Create Ground Plane" adds plane with material
- [ ] "Apply Full Atmosphere" runs all setup steps
- [ ] No errors in console after running menu items

---

## Test Steps

1. Open Unity
2. Ensure Track C and E scripts exist (or stub them)
3. Click `ShieldWall > Create Assets > Create All Phase 3 Assets`
4. Verify assets in `Assets/ScriptableObjects/`
5. Open Battle scene
6. Click `ShieldWall > Scene Setup > Apply Full Atmosphere`
7. Check Scene view for ground, lighting changes
8. Press Play, verify vignette visible

---

## Reference Files

- `Assets/Editor/ShieldWallSetup.cs` — Existing setup code
- `Assets/Editor/BattleSceneSetup.cs` — Existing scene setup
- `Assets/Scripts/Data/*.cs` — ScriptableObject definitions

