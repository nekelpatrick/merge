using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using ShieldWall.UI;

namespace ShieldWall.Editor
{
    /// <summary>
    /// Editor tool to create Phase 5.5 UI prefabs programmatically.
    /// Run via menu: Shield Wall Builder > Phase 5.5 Setup > Create UI Prefabs
    /// </summary>
    public static class Phase5_5_PrefabCreator
    {
        private const string PREFAB_PATH = "Assets/Prefabs/UI/";
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Create All UI Prefabs", false, 300)]
        public static void CreateAllPrefabs()
        {
            Debug.Log("=== Creating Phase 5.5 UI Prefabs ===");
            
            EnsureFolderExists(PREFAB_PATH);
            
            CreateActionPreviewItemPrefab();
            CreateRuneBadgePrefab();
            CreatePhaseBannerPrefab();
            CreateEnemyIntentIndicatorPrefab();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("=== Phase 5.5 UI Prefabs Created Successfully ===");
            Debug.Log($"Prefabs saved to: {PREFAB_PATH}");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Create ActionPreviewItem Prefab", false, 301)]
        public static void CreateActionPreviewItemPrefab()
        {
            GameObject go = UIComponentFactory.CreateActionPreviewItemTemplate(null);
            
            var script = go.AddComponent<ActionPreviewItem>();
            
            var nameText = go.transform.Find("ActionNameText")?.GetComponent<TextMeshProUGUI>();
            var effectText = go.transform.Find("EffectText")?.GetComponent<TextMeshProUGUI>();
            var runeContainer = go.transform.Find("RuneContainer");
            var statusIcon = go.transform.Find("StatusIcon")?.GetComponent<Image>();
            
            SerializedObject so = new SerializedObject(script);
            so.FindProperty("_actionNameText").objectReferenceValue = nameText;
            so.FindProperty("_effectText").objectReferenceValue = effectText;
            so.FindProperty("_runeContainer").objectReferenceValue = runeContainer;
            so.FindProperty("_statusIcon").objectReferenceValue = statusIcon;
            so.ApplyModifiedProperties();
            
            string path = $"{PREFAB_PATH}ActionPreviewItem.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            
            Debug.Log($"Created: {path}");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Create RuneBadge Prefab", false, 302)]
        public static void CreateRuneBadgePrefab()
        {
            GameObject go = UIComponentFactory.CreateRuneBadgeTemplate(null);
            
            var script = go.AddComponent<RuneBadgeUI>();
            
            var background = go.GetComponent<Image>();
            var symbolText = go.transform.Find("RuneSymbol")?.GetComponent<TextMeshProUGUI>();
            
            SerializedObject so = new SerializedObject(script);
            so.FindProperty("_backgroundImage").objectReferenceValue = background;
            so.FindProperty("_symbolText").objectReferenceValue = symbolText;
            so.ApplyModifiedProperties();
            
            string path = $"{PREFAB_PATH}RuneBadge.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            
            Debug.Log($"Created: {path}");
            
            UpdateActionPreviewItemWithRuneBadge(path);
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Create PhaseBanner Prefab", false, 303)]
        public static void CreatePhaseBannerPrefab()
        {
            GameObject go = UIComponentFactory.CreatePhaseBannerTemplate(null);
            
            var script = go.AddComponent<PhaseBannerUI>();
            
            var canvasGroup = go.GetComponent<CanvasGroup>();
            var phaseText = go.transform.Find("Content/PhaseText")?.GetComponent<TextMeshProUGUI>();
            var ctaText = go.transform.Find("Content/CTAText")?.GetComponent<TextMeshProUGUI>();
            
            SerializedObject so = new SerializedObject(script);
            so.FindProperty("_phaseText").objectReferenceValue = phaseText;
            so.FindProperty("_ctaText").objectReferenceValue = ctaText;
            so.FindProperty("_canvasGroup").objectReferenceValue = canvasGroup;
            so.FindProperty("_fadeInDuration").floatValue = 0.3f;
            so.FindProperty("_displayDuration").floatValue = 2.0f;
            so.ApplyModifiedProperties();
            
            string path = $"{PREFAB_PATH}PhaseBannerUI.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            
            Debug.Log($"Created: {path}");
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Create EnemyIntentIndicator Prefab", false, 304)]
        public static void CreateEnemyIntentIndicatorPrefab()
        {
            GameObject go = new GameObject("EnemyIntentIndicator");
            
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 10;
            
            RectTransform canvasRect = go.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1, 1);
            go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;
            
            GameObject iconGO = new GameObject("IntentIcon");
            iconGO.transform.SetParent(go.transform, false);
            
            RectTransform iconRect = iconGO.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.sizeDelta = Vector2.zero;
            
            Image iconImage = iconGO.AddComponent<Image>();
            iconImage.color = Color.red;
            
            var script = go.AddComponent<EnemyIntentIndicator>();
            
            SerializedObject so = new SerializedObject(script);
            so.FindProperty("_iconImage").objectReferenceValue = iconImage;
            so.ApplyModifiedProperties();
            
            string path = $"{PREFAB_PATH}EnemyIntentIndicator.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            
            Debug.Log($"Created: {path}");
        }
        
        private static void UpdateActionPreviewItemWithRuneBadge(string runeBadgePath)
        {
            string actionPreviewPath = $"{PREFAB_PATH}ActionPreviewItem.prefab";
            
            if (!System.IO.File.Exists(actionPreviewPath))
            {
                Debug.LogWarning($"ActionPreviewItem prefab not found at {actionPreviewPath}. Create it first.");
                return;
            }
            
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(actionPreviewPath);
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            
            var script = instance.GetComponent<ActionPreviewItem>();
            if (script != null)
            {
                Image runeBadge = AssetDatabase.LoadAssetAtPath<GameObject>(runeBadgePath)?.GetComponent<Image>();
                
                SerializedObject so = new SerializedObject(script);
                so.FindProperty("_runeBadgePrefab").objectReferenceValue = runeBadge;
                so.ApplyModifiedProperties();
                
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                
                Debug.Log($"Updated ActionPreviewItem with RuneBadge reference.");
            }
            
            Object.DestroyImmediate(instance);
        }
        
        private static void EnsureFolderExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentFolder = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
                string newFolderName = System.IO.Path.GetFileName(path);
                
                if (!AssetDatabase.IsValidFolder(parentFolder))
                {
                    EnsureFolderExists(parentFolder);
                }
                
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
                Debug.Log($"Created folder: {path}");
            }
        }
        
        [MenuItem("Shield Wall Builder/Phase 5.5 Setup/Validate Prefabs", false, 310)]
        public static void ValidatePrefabs()
        {
            Debug.Log("=== Validating Phase 5.5 Prefabs ===");
            
            ValidatePrefabExists("ActionPreviewItem");
            ValidatePrefabExists("RuneBadge");
            ValidatePrefabExists("PhaseBannerUI");
            ValidatePrefabExists("EnemyIntentIndicator");
            
            Debug.Log("=== Validation Complete ===");
        }
        
        private static void ValidatePrefabExists(string prefabName)
        {
            string path = $"{PREFAB_PATH}{prefabName}.prefab";
            bool exists = System.IO.File.Exists(path);
            
            if (exists)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                int componentCount = prefab.GetComponents<Component>().Length;
                Debug.Log($"✓ {prefabName} exists with {componentCount} components");
            }
            else
            {
                Debug.LogWarning($"✗ {prefabName} NOT FOUND at {path}");
            }
        }
    }
}

