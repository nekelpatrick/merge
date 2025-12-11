using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using ShieldWall.UI;
using ShieldWall.Visual;

namespace ShieldWall.Editor
{
    /// <summary>
    /// Editor tool to integrate Phase 5.5 UI components into Battle scene.
    /// Run via menu: Shield Wall Builder > Phase 5.5 Setup > Integrate UI Into Scene
    /// </summary>
    public static class Phase5_5_SceneIntegrator
    {
        private const string PREFAB_PATH = "Assets/Prefabs/UI/";
        private const string BATTLE_SCENE_PATH = "Assets/Scenes/Battle.unity";
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Integrate All UI Into Scene", false, 320)]
        public static void IntegrateAllUI()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("=== Integrating Phase 5.5 UI Into Battle Scene ===");
            
            Canvas canvas = FindOrCreateCanvas();
            
            IntegrateActionPreviewUIInternal(canvas);
            IntegratePhaseBannerUIInternal(canvas);
            IntegrateEnemyIntentManagerInternal();
            IntegrateScreenEffectsUIInternal(canvas);
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            
            Debug.Log("=== Phase 5.5 UI Integration Complete ===");
            Debug.Log("Next: Test in Play mode to verify dice labels, action preview, and phase banner.");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Add ActionPreviewUI to Scene", false, 321)]
        public static void IntegrateActionPreviewUI()
        {
            if (!OpenBattleScene()) return;
            Canvas canvas = FindOrCreateCanvas();
            IntegrateActionPreviewUIInternal(canvas);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
        }
        
        private static void IntegrateActionPreviewUIInternal(Canvas canvas)
        {
            if (!OpenBattleScene()) return;
            if (canvas == null) canvas = FindOrCreateCanvas();
            
            if (GameObject.Find("ActionPreviewUI") != null)
            {
                Debug.LogWarning("ActionPreviewUI already exists in scene. Skipping.");
                return;
            }
            
            GameObject uiGO = new GameObject("ActionPreviewUI");
            uiGO.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = uiGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-20, 0);
            rect.sizeDelta = new Vector2(380, 400);
            
            Image background = uiGO.AddComponent<Image>();
            background.color = new Color(42f/255f, 31f/255f, 26f/255f, 0.9f);
            
            GameObject headerGO = new GameObject("Header");
            headerGO.transform.SetParent(uiGO.transform, false);
            RectTransform headerRect = headerGO.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.anchoredPosition = new Vector2(0, -10);
            headerRect.sizeDelta = new Vector2(-20, 30);
            
            var headerText = headerGO.AddComponent<TMPro.TextMeshProUGUI>();
            headerText.text = "AVAILABLE ACTIONS";
            headerText.fontSize = 18;
            headerText.fontStyle = TMPro.FontStyles.Bold;
            headerText.color = new Color(201f/255f, 162f/255f, 39f/255f, 1f);
            headerText.alignment = TMPro.TextAlignmentOptions.Center;
            
            GameObject containerGO = new GameObject("PreviewContainer");
            containerGO.transform.SetParent(uiGO.transform, false);
            RectTransform containerRect = containerGO.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0, 0);
            containerRect.anchorMax = new Vector2(1, 1);
            containerRect.offsetMin = new Vector2(10, 50);
            containerRect.offsetMax = new Vector2(-10, -50);
            
            VerticalLayoutGroup layout = containerGO.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            
            GameObject emptyStateGO = new GameObject("EmptyStatePanel");
            emptyStateGO.transform.SetParent(uiGO.transform, false);
            RectTransform emptyRect = emptyStateGO.AddComponent<RectTransform>();
            emptyRect.anchorMin = Vector2.zero;
            emptyRect.anchorMax = Vector2.one;
            emptyRect.sizeDelta = Vector2.zero;
            
            var emptyText = emptyStateGO.AddComponent<TMPro.TextMeshProUGUI>();
            emptyText.text = "Lock dice to ready actions";
            emptyText.fontSize = 16;
            emptyText.color = new Color(212f/255f, 200f/255f, 184f/255f, 0.7f);
            emptyText.alignment = TMPro.TextAlignmentOptions.Center;
            
            var script = uiGO.AddComponent<ActionPreviewUI>();
            
            string actionPreviewItemPath = $"{PREFAB_PATH}ActionPreviewItem.prefab";
            GameObject actionPreviewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(actionPreviewItemPath);
            
            SerializedObject so = new SerializedObject(script);
            so.FindProperty("_previewContainer").objectReferenceValue = containerGO.transform;
            so.FindProperty("_previewItemPrefab").objectReferenceValue = actionPreviewPrefab?.GetComponent<ActionPreviewItem>();
            so.FindProperty("_headerText").objectReferenceValue = headerText;
            so.FindProperty("_emptyStatePanel").objectReferenceValue = emptyStateGO;
            so.FindProperty("_emptyStateText").objectReferenceValue = emptyText;
            so.ApplyModifiedProperties();
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("✓ ActionPreviewUI added to scene (right side panel)");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Add PhaseBannerUI to Scene", false, 322)]
        public static void IntegratePhaseBannerUI()
        {
            if (!OpenBattleScene()) return;
            Canvas canvas = FindOrCreateCanvas();
            IntegratePhaseBannerUIInternal(canvas);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
        }
        
        private static void IntegratePhaseBannerUIInternal(Canvas canvas)
        {
            if (!OpenBattleScene()) return;
            if (canvas == null) canvas = FindOrCreateCanvas();
            
            if (GameObject.Find("PhaseBannerUI") != null)
            {
                Debug.LogWarning("PhaseBannerUI already exists in scene. Skipping.");
                return;
            }
            
            string prefabPath = $"{PREFAB_PATH}PhaseBannerUI.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"PhaseBannerUI prefab not found at {prefabPath}. Create it first.");
                return;
            }
            
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab, canvas.transform) as GameObject;
            instance.name = "PhaseBannerUI";
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("✓ PhaseBannerUI added to scene (top-center banner)");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Add EnemyIntentManager to Scene", false, 323)]
        public static void IntegrateEnemyIntentManager()
        {
            if (!OpenBattleScene()) return;
            IntegrateEnemyIntentManagerInternal();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
        }
        
        private static void IntegrateEnemyIntentManagerInternal()
        {
            if (!OpenBattleScene()) return;
            
            if (GameObject.Find("EnemyIntentManager") != null)
            {
                Debug.LogWarning("EnemyIntentManager already exists in scene. Skipping.");
                return;
            }
            
            GameObject managerGO = new GameObject("EnemyIntentManager");
            var script = managerGO.AddComponent<EnemyIntentManager>();
            
            string prefabPath = $"{PREFAB_PATH}EnemyIntentIndicator.prefab";
            GameObject indicatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (indicatorPrefab != null)
            {
                SerializedObject so = new SerializedObject(script);
                so.FindProperty("_intentIndicatorPrefab").objectReferenceValue = indicatorPrefab;
                so.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning($"EnemyIntentIndicator prefab not found at {prefabPath}");
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("✓ EnemyIntentManager added to scene");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Add ScreenEffects UI", false, 324)]
        public static void IntegrateScreenEffectsUI()
        {
            if (!OpenBattleScene()) return;
            Canvas canvas = FindOrCreateCanvas();
            IntegrateScreenEffectsUIInternal(canvas);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
        }
        
        private static void IntegrateScreenEffectsUIInternal(Canvas canvas)
        {
            if (canvas == null)
            {
                Debug.LogError("[Phase5_5_SceneIntegrator] Canvas is null, cannot integrate ScreenEffects UI");
                return;
            }
            
            ScreenEffectsController controller = Object.FindFirstObjectByType<ScreenEffectsController>();
            
            if (controller == null)
            {
                Debug.LogWarning("ScreenEffectsController not found in scene. Skipping ScreenEffects UI integration.");
                return;
            }
            
            Transform canvasTransform = canvas.transform;
            Transform screenEffects = canvasTransform.Find("ScreenEffects");
            if (screenEffects == null)
            {
                GameObject screenEffectsGO = new GameObject("ScreenEffects");
                screenEffectsGO.transform.SetParent(canvasTransform, false);
                screenEffects = screenEffectsGO.transform;
                
                RectTransform rect = screenEffectsGO.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
            }
            
            Image vignetteImage = CreateFullscreenImage(screenEffects, "VignetteImage", new Color(0.5f, 0, 0, 0));
            Image flashImage = CreateFullscreenImage(screenEffects, "FlashImage", new Color(1, 1, 1, 0));
            Image staminaPulseImage = CreateFullscreenImage(screenEffects, "StaminaPulseImage", new Color(61f/255f, 90f/255f, 110f/255f, 0));
            
            if (vignetteImage == null || flashImage == null || staminaPulseImage == null)
            {
                Debug.LogError("Failed to create one or more fullscreen images");
                return;
            }
            
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("_vignetteImage").objectReferenceValue = vignetteImage;
            so.FindProperty("_flashImage").objectReferenceValue = flashImage;
            so.FindProperty("_staminaPulseImage").objectReferenceValue = staminaPulseImage;
            so.ApplyModifiedProperties();
            
            Debug.Log("✓ ScreenEffects UI added and wired to ScreenEffectsController");
        }
        
        private static Image CreateFullscreenImage(Transform parent, string name, Color color)
        {
            if (parent == null)
            {
                Debug.LogError($"Cannot create fullscreen image '{name}': parent is null");
                return null;
            }
            
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                Image existingImage = existing.GetComponent<Image>();
                if (existingImage != null)
                {
                    return existingImage;
                }
            }
            
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            Image image = go.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            
            return image;
        }
        
        private static Canvas FindOrCreateCanvas()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                Debug.Log("Created new Canvas in scene");
            }
            
            return canvas;
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
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Validate Scene Integration", false, 330)]
        public static void ValidateIntegration()
        {
            if (!OpenBattleScene()) return;
            
            Debug.Log("=== Validating Phase 5.5 Scene Integration ===");
            
            ValidateComponentExists<ActionPreviewUI>("ActionPreviewUI");
            ValidateComponentExists<PhaseBannerUI>("PhaseBannerUI");
            ValidateComponentExists<EnemyIntentManager>("EnemyIntentManager");
            ValidateComponentExists<ScreenEffectsController>("ScreenEffectsController");
            
            Debug.Log("=== Validation Complete ===");
        }
        
        private static void ValidateComponentExists<T>(string componentName) where T : Component
        {
            T component = Object.FindFirstObjectByType<T>();
            
            if (component != null)
            {
                Debug.Log($"✓ {componentName} found in scene");
                
                SerializedObject so = new SerializedObject(component);
                SerializedProperty prop = so.GetIterator();
                int nullRefs = 0;
                
                while (prop.NextVisible(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null && prop.name.StartsWith("_"))
                    {
                        nullRefs++;
                    }
                }
                
                if (nullRefs > 0)
                {
                    Debug.LogWarning($"  ⚠ {componentName} has {nullRefs} null reference(s). Check Inspector.");
                }
            }
            else
            {
                Debug.LogWarning($"✗ {componentName} NOT FOUND in scene");
            }
        }
    }
}

