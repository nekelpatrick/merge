using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ShieldWall.Editor
{
    /// <summary>
    /// Automation script for Visual Polish & Combat Feel improvements.
    /// Run via menu: Shield Wall Builder > Visual Polish > Apply All Improvements
    /// </summary>
    public static class VisualPolishAutomation
    {
        private const string BATTLE_SCENE_PATH = "Assets/Scenes/Battle.unity";
        
        [MenuItem("Shield Wall Builder/Visual Polish/Apply All Improvements", false, 400)]
        public static void ApplyAllImprovements()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("=== Applying Visual Polish & Combat Feel Improvements ===");
            
            ApplyCharacterMaterials();
            AdjustAtmosphereLighting();
            EnsureCombatFeedbackController();
            SetupEnemyHealthDisplays();
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            
            Debug.Log("=== Visual Polish Complete ===");
            Debug.Log("Next: Enter Play mode to test screen shake, blood VFX, enemy health displays, and visual improvements!");
        }
        
        [MenuItem("Shield Wall Builder/Visual Polish/1. Apply Character Materials", false, 401)]
        public static void ApplyCharacterMaterials()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("--- Applying Color-Coded Character Materials ---");
            
            Material brotherMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Characters/M_Character_Brother.mat");
            Material enemyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Characters/M_Character_Thrall.mat");
            Material playerMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Characters/M_Character_Player.mat");
            
            if (brotherMat == null || enemyMat == null || playerMat == null)
            {
                Debug.LogError("Could not find character materials! Run '3D Assets > Create All 3D Assets' first.");
                return;
            }
            
            int brothersUpdated = 0;
            int enemiesUpdated = 0;
            int playerUpdated = 0;
            
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            
            foreach (GameObject go in allObjects)
            {
                string name = go.name.ToLower();
                
                if (name.Contains("brother") && !name.Contains("manager"))
                {
                    ApplyMaterialToObject(go, brotherMat);
                    brothersUpdated++;
                }
                else if (name.Contains("enemy") || name.Contains("thrall"))
                {
                    ApplyMaterialToObject(go, enemyMat);
                    enemiesUpdated++;
                }
                else if (name.Contains("player") && (name.Contains("shield") || name.Contains("warrior")))
                {
                    ApplyMaterialToObject(go, playerMat);
                    playerUpdated++;
                }
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log($"✓ Applied materials: {brothersUpdated} brothers (gray), {enemiesUpdated} enemies (red), {playerUpdated} player objects (iron)");
        }
        
        [MenuItem("Shield Wall Builder/Visual Polish/2. Adjust Atmosphere & Lighting", false, 402)]
        public static void AdjustAtmosphereLighting()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("--- Adjusting Atmosphere & Lighting ---");
            
            Light directionalLight = Object.FindFirstObjectByType<Light>();
            if (directionalLight != null && directionalLight.type == LightType.Directional)
            {
                directionalLight.intensity = 0.6f;
                directionalLight.color = new Color(0.9f, 0.92f, 1.0f);
                directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                Debug.Log("✓ Adjusted directional light (dimmer, cooler, Viking gloom)");
            }
            
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 10f;
            RenderSettings.fogEndDistance = 30f;
            RenderSettings.fogColor = new Color(0.35f, 0.38f, 0.42f);
            Debug.Log("✓ Enabled atmospheric fog (dark blue-gray)");
            
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);
            Debug.Log("✓ Reduced ambient light for darker atmosphere");
            
            string volumeProfilePath = "Assets/Settings/BattleVolumeProfile.asset";
            var volumeProfile = AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.VolumeProfile>(volumeProfilePath);
            
            if (volumeProfile != null)
            {
                Debug.Log("✓ Volume profile found (vignette/color adjustments already configured)");
            }
            else
            {
                Debug.LogWarning("Volume profile not found at " + volumeProfilePath);
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        
        [MenuItem("Shield Wall Builder/Visual Polish/3. Setup Combat Feedback", false, 403)]
        public static void EnsureCombatFeedbackController()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("--- Setting Up Combat Feedback Controller ---");
            
            var existing = Object.FindFirstObjectByType<ShieldWall.Visual.CombatFeedbackController>();
            if (existing != null)
            {
                Debug.Log("✓ CombatFeedbackController already exists");
                return;
            }
            
            GameObject managerGO = new GameObject("CombatFeedbackController");
            managerGO.AddComponent<ShieldWall.Visual.CombatFeedbackController>();
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("✓ Created CombatFeedbackController (blood effects on enemy kill)");
        }
        
        [MenuItem("Shield Wall Builder/Visual Polish/4. Setup Enemy Health Displays", false, 404)]
        public static void SetupEnemyHealthDisplays()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("--- Setting Up Enemy Health Displays ---");
            
            int enemiesFound = 0;
            int healthDisplaysAdded = 0;
            
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            
            foreach (GameObject go in allObjects)
            {
                if (go.name.ToLower().Contains("enemy") && !go.name.Contains("Manager") && !go.name.Contains("Controller"))
                {
                    enemiesFound++;
                    
                    var existingDisplay = go.GetComponentInChildren<ShieldWall.UI.EnemyHealthDisplay>();
                    if (existingDisplay != null)
                    {
                        continue;
                    }
                    
                    GameObject healthDisplayGO = new GameObject("HealthDisplay");
                    healthDisplayGO.transform.SetParent(go.transform, false);
                    healthDisplayGO.transform.localPosition = new Vector3(0, 1.5f, 0);
                    
                    Canvas canvas = healthDisplayGO.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    
                    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                    canvasRect.sizeDelta = new Vector2(200, 50);
                    canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    
                    GameObject textGO = new GameObject("HealthText");
                    textGO.transform.SetParent(healthDisplayGO.transform, false);
                    
                    RectTransform textRect = textGO.AddComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    textRect.sizeDelta = Vector2.zero;
                    
                    var text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
                    text.text = "5/5 HP";
                    text.fontSize = 36;
                    text.fontStyle = TMPro.FontStyles.Bold;
                    text.color = Color.white;
                    text.alignment = TMPro.TextAlignmentOptions.Center;
                    
                    var healthDisplay = healthDisplayGO.AddComponent<ShieldWall.UI.EnemyHealthDisplay>();
                    
                    SerializedObject so = new SerializedObject(healthDisplay);
                    so.FindProperty("_healthText").objectReferenceValue = text;
                    so.FindProperty("_canvas").objectReferenceValue = canvas;
                    so.ApplyModifiedProperties();
                    
                    healthDisplaysAdded++;
                }
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log($"✓ Found {enemiesFound} enemies, added {healthDisplaysAdded} health displays");
        }
        
        [MenuItem("Shield Wall Builder/Visual Polish/Validate Visual Setup", false, 410)]
        public static void ValidateVisualSetup()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("=== Validating Visual Polish Setup ===");
            
            Material brotherMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Characters/M_Character_Brother.mat");
            Material enemyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Characters/M_Character_Thrall.mat");
            
            Debug.Log(brotherMat != null ? "✓ Brother material exists" : "✗ Brother material missing");
            Debug.Log(enemyMat != null ? "✓ Enemy material exists" : "✗ Enemy material missing");
            
            int brothersWithMaterial = 0;
            int enemiesWithMaterial = 0;
            
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    if (go.name.ToLower().Contains("brother"))
                        brothersWithMaterial++;
                    else if (go.name.ToLower().Contains("enemy"))
                        enemiesWithMaterial++;
                }
            }
            
            Debug.Log($"Brothers with materials: {brothersWithMaterial}");
            Debug.Log($"Enemies with materials: {enemiesWithMaterial}");
            Debug.Log($"Fog enabled: {RenderSettings.fog}");
            
            Debug.Log("=== Validation Complete ===");
        }
        
        private static void ApplyMaterialToObject(GameObject go, Material material)
        {
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
            
            foreach (Transform child in go.transform)
            {
                MeshRenderer childRenderer = child.GetComponent<MeshRenderer>();
                if (childRenderer != null)
                {
                    childRenderer.sharedMaterial = material;
                }
            }
        }
        
        private static bool OpenBattleScene()
        {
            var currentScene = EditorSceneManager.GetActiveScene();
            
            if (currentScene.path != BATTLE_SCENE_PATH)
            {
                if (EditorUtility.DisplayDialog(
                    "Open Battle Scene?",
                    $"This tool requires Battle.unity to be open. Current scene: {currentScene.name}\n\nOpen Battle.unity now?",
                    "Yes", "No"))
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene(BATTLE_SCENE_PATH);
                    return true;
                }
                else
                {
                    Debug.LogWarning("Operation cancelled. Battle scene must be open.");
                    return false;
                }
            }
            
            return true;
        }
    }
}
