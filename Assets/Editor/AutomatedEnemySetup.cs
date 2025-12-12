using UnityEngine;
using UnityEditor;
using ShieldWall.Visual;
using ShieldWall.Data;
using System.IO;

namespace ShieldWall.Editor
{
    public class AutomatedEnemySetup
    {
        private const string VIKING_MODEL_PATH = "Assets/Art/Models/Characters/Viking_Player.glb";
        private const string PREFAB_OUTPUT_PATH = "Assets/Prefabs/Characters/Enemy_Viking.prefab";
        private const string MATERIAL_PATH = "Assets/Art/Materials/Characters/M_Enemy_Viking.mat";
        
        [MenuItem("ShieldWall/Setup/1. Refresh Asset Database")]
        public static void RefreshAssetDatabase()
        {
            Debug.Log("Refreshing Unity Asset Database...");
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            GameObject vikingModel = AssetDatabase.LoadAssetAtPath<GameObject>(VIKING_MODEL_PATH);
            
            if (vikingModel != null)
            {
                EditorUtility.DisplayDialog(
                    "Asset Database Refreshed",
                    $"✅ Viking model found at:\n{VIKING_MODEL_PATH}\n\n" +
                    "Ready to run Auto-Setup!",
                    "OK"
                );
            }
            else
            {
                string[] foundModels = AssetDatabase.FindAssets("Viking_Player");
                if (foundModels.Length > 0)
                {
                    string foundPath = AssetDatabase.GUIDToAssetPath(foundModels[0]);
                    EditorUtility.DisplayDialog(
                        "Model Found at Different Location",
                        $"Viking model found at:\n{foundPath}\n\n" +
                        $"Expected location:\n{VIKING_MODEL_PATH}\n\n" +
                        "Please move the file to the correct location.",
                        "OK"
                    );
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Model Not Found",
                        $"Viking model not found anywhere in project.\n\n" +
                        $"Expected location:\n{VIKING_MODEL_PATH}\n\n" +
                        "Please ensure the file is copied to the correct location.",
                        "OK"
                    );
                }
            }
        }
        
        [MenuItem("ShieldWall/Setup/2. Auto-Setup Viking Enemy Prefab")]
        public static void SetupVikingEnemyPrefab()
        {
            if (!System.IO.File.Exists(VIKING_MODEL_PATH))
            {
                EditorUtility.DisplayDialog(
                    "File Not Found",
                    $"The Viking model file does not exist at:\n{VIKING_MODEL_PATH}\n\n" +
                    "Please ensure the Viking_Player.glb file is in the correct location.",
                    "OK"
                );
                return;
            }
            
            AssetDatabase.ImportAsset(VIKING_MODEL_PATH, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            
            GameObject vikingModel = AssetDatabase.LoadAssetAtPath<GameObject>(VIKING_MODEL_PATH);
            
            if (vikingModel == null)
            {
                AssetImporter importer = AssetImporter.GetAtPath(VIKING_MODEL_PATH);
                
                if (importer == null)
                {
                    bool reimport = EditorUtility.DisplayDialog(
                        "Import Issue Detected",
                        $"The Viking model file exists but Unity hasn't imported it yet.\n\n" +
                        "Click 'Reimport' to force Unity to process the file.\n\n" +
                        "If this fails, you may need to:\n" +
                        "• Install glTFast package (for GLB support)\n" +
                        "• Or convert GLB to FBX format",
                        "Reimport",
                        "Cancel"
                    );
                    
                    if (reimport)
                    {
                        string metaPath = VIKING_MODEL_PATH + ".meta";
                        if (System.IO.File.Exists(metaPath))
                        {
                            System.IO.File.Delete(metaPath);
                        }
                        
                        AssetDatabase.ImportAsset(VIKING_MODEL_PATH, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                        AssetDatabase.Refresh();
                        
                        vikingModel = AssetDatabase.LoadAssetAtPath<GameObject>(VIKING_MODEL_PATH);
                        
                        if (vikingModel == null)
                        {
                            EditorUtility.DisplayDialog(
                                "Reimport Failed",
                                "Model could not be imported.\n\n" +
                                "Please run:\n" +
                                "ShieldWall > Setup > Install glTFast Package\n\n" +
                                "Or convert the GLB to FBX format.",
                                "OK"
                            );
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    vikingModel = AssetDatabase.LoadAssetAtPath<GameObject>(VIKING_MODEL_PATH);
                }
            }
            
            if (vikingModel == null)
            {
                EditorUtility.DisplayDialog(
                    "Cannot Load Model",
                    $"Unity imported the file but cannot load it as a GameObject.\n\n" +
                    "This GLB file may have compatibility issues.\n\n" +
                    "Recommended: Convert to FBX format\n" +
                    "See: Assets/Documentation/GLB_to_FBX_Conversion_Guide.md",
                    "OK"
                );
                return;
            }
            
            Debug.Log($"<color=green>✓</color> Viking model loaded successfully from {VIKING_MODEL_PATH}");
            
            GameObject enemyPrefab = CreateEnemyPrefab(vikingModel);
            
            EditorUtility.DisplayDialog(
                "Setup Complete", 
                $"Viking enemy prefab created successfully!\n\n" +
                $"Prefab: {PREFAB_OUTPUT_PATH}\n\n" +
                "You can now:\n" +
                "1. Use this prefab in EnemyWaveController\n" +
                "2. Create variants for different enemy types\n" +
                "3. Test in Battle scene",
                "OK"
            );
        }
        
        private static GameObject CreateEnemyPrefab(GameObject modelPrefab)
        {
            GameObject enemyRoot = new GameObject("Enemy_Viking");
            enemyRoot.transform.position = Vector3.zero;
            enemyRoot.transform.rotation = Quaternion.identity;
            
            GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab, enemyRoot.transform);
            modelInstance.name = "Model";
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.Euler(0, 180, 0);
            modelInstance.transform.localScale = Vector3.one;
            
            EnsureModelImportSettings(VIKING_MODEL_PATH);
            
            EnemyVisualInstance visualInstance = enemyRoot.AddComponent<EnemyVisualInstance>();
            
            ModularCharacterBuilder modularBuilder = enemyRoot.AddComponent<ModularCharacterBuilder>();
            
            DismembermentController dismemberment = enemyRoot.AddComponent<DismembermentController>();
            ConfigureDismemberment(dismemberment, modelInstance.transform);
            
            CapsuleCollider collider = enemyRoot.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 1f, 0);
            collider.radius = 0.4f;
            collider.height = 2f;
            collider.isTrigger = true;
            
            if (TryAddOutlineComponent(enemyRoot))
            {
                Debug.Log("Outline component added successfully");
            }
            else
            {
                Debug.LogWarning("Outline component not available - install QuickOutline from Unity Asset Store or GitHub");
            }
            
            ApplyEnemyMaterial(modelInstance);
            
            EnsureDirectoryExists(Path.GetDirectoryName(PREFAB_OUTPUT_PATH));
            
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(enemyRoot, PREFAB_OUTPUT_PATH);
            
            Object.DestroyImmediate(enemyRoot);
            
            Debug.Log($"<color=green>✓</color> Enemy prefab created at {PREFAB_OUTPUT_PATH}");
            
            Selection.activeObject = savedPrefab;
            EditorGUIUtility.PingObject(savedPrefab);
            
            return savedPrefab;
        }
        
        private static void EnsureModelImportSettings(string modelPath)
        {
            ModelImporter importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
            if (importer == null) return;
            
            bool needsReimport = false;
            
            if (!importer.isReadable)
            {
                importer.isReadable = true;
                needsReimport = true;
                Debug.Log("Enabled Read/Write for Viking model");
            }
            
            if (importer.importAnimation && importer.animationType == ModelImporterAnimationType.None)
            {
                importer.animationType = ModelImporterAnimationType.Generic;
                needsReimport = true;
                Debug.Log("Set animation type to Generic");
            }
            
            if (needsReimport)
            {
                importer.SaveAndReimport();
            }
        }
        
        private static void ConfigureDismemberment(DismembermentController dismemberment, Transform modelRoot)
        {
            Transform head = FindChildRecursive(modelRoot, "Head");
            Transform rightArm = FindChildRecursive(modelRoot, "RightArm");
            if (rightArm == null) rightArm = FindChildRecursive(modelRoot, "Arm_R");
            Transform leftArm = FindChildRecursive(modelRoot, "LeftArm");
            if (leftArm == null) leftArm = FindChildRecursive(modelRoot, "Arm_L");
            
            SerializedObject so = new SerializedObject(dismemberment);
            
            if (head != null)
            {
                so.FindProperty("_headTransform").objectReferenceValue = head;
                Debug.Log($"Found head bone: {head.name}");
            }
            if (rightArm != null)
            {
                so.FindProperty("_rightArmTransform").objectReferenceValue = rightArm;
                Debug.Log($"Found right arm bone: {rightArm.name}");
            }
            if (leftArm != null)
            {
                so.FindProperty("_leftArmTransform").objectReferenceValue = leftArm;
                Debug.Log($"Found left arm bone: {leftArm.name}");
            }
            
            so.ApplyModifiedProperties();
        }
        
        private static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent.name.Contains(name))
                return parent;
            
            foreach (Transform child in parent)
            {
                Transform result = FindChildRecursive(child, name);
                if (result != null)
                    return result;
            }
            
            return null;
        }
        
        private static bool TryAddOutlineComponent(GameObject target)
        {
            System.Type outlineType = System.Type.GetType("Outline, Assembly-CSharp");
            if (outlineType == null)
            {
                outlineType = System.Type.GetType("Outline, QuickOutline");
            }
            
            if (outlineType == null)
            {
                return false;
            }
            
            Component outline = target.AddComponent(outlineType);
            
            SerializedObject so = new SerializedObject(outline);
            so.FindProperty("outlineMode").intValue = 1;
            so.FindProperty("outlineColor").colorValue = new Color(1f, 0.8f, 0.2f, 1f);
            so.FindProperty("outlineWidth").floatValue = 5f;
            so.ApplyModifiedPropertiesWithoutUndo();
            
            MonoBehaviour mb = outline as MonoBehaviour;
            if (mb != null)
            {
                mb.enabled = false;
            }
            
            return true;
        }
        
        private static void ApplyEnemyMaterial(GameObject modelInstance)
        {
            Material enemyMaterial = AssetDatabase.LoadAssetAtPath<Material>(MATERIAL_PATH);
            
            if (enemyMaterial == null)
            {
                enemyMaterial = CreateDefaultEnemyMaterial();
            }
            
            Renderer[] renderers = modelInstance.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = enemyMaterial;
                }
                renderer.sharedMaterials = materials;
            }
            
            Debug.Log($"Applied material to {renderers.Length} renderers");
        }
        
        private static Material CreateDefaultEnemyMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }
            
            Material mat = new Material(shader);
            mat.name = "M_Enemy_Viking";
            mat.color = new Color(0.7f, 0.4f, 0.3f, 1f);
            
            EnsureDirectoryExists(Path.GetDirectoryName(MATERIAL_PATH));
            AssetDatabase.CreateAsset(mat, MATERIAL_PATH);
            
            Debug.Log($"Created default enemy material at {MATERIAL_PATH}");
            
            return mat;
        }
        
        private static void EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }
        }
        
        [MenuItem("ShieldWall/Setup/3. Create Enemy Variant Prefabs")]
        public static void CreateEnemyVariants()
        {
            GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_OUTPUT_PATH);
            
            if (basePrefab == null)
            {
                EditorUtility.DisplayDialog(
                    "Base Prefab Missing",
                    "Please run 'Auto-Setup Viking Enemy Prefab' first!",
                    "OK"
                );
                return;
            }
            
            CreateVariant(basePrefab, "Enemy_Thrall", new Color(0.5f, 0.3f, 0.2f));
            CreateVariant(basePrefab, "Enemy_Warrior", new Color(0.7f, 0.4f, 0.3f));
            CreateVariant(basePrefab, "Enemy_Berserker", new Color(0.8f, 0.2f, 0.2f));
            CreateVariant(basePrefab, "Enemy_Archer", new Color(0.4f, 0.5f, 0.3f));
            CreateVariant(basePrefab, "Enemy_Spearman", new Color(0.6f, 0.5f, 0.4f));
            CreateVariant(basePrefab, "Enemy_ShieldBreaker", new Color(0.3f, 0.3f, 0.3f));
            
            EditorUtility.DisplayDialog(
                "Variants Created",
                "Created 6 enemy variant prefabs:\n" +
                "- Enemy_Thrall\n" +
                "- Enemy_Warrior\n" +
                "- Enemy_Berserker\n" +
                "- Enemy_Archer\n" +
                "- Enemy_Spearman\n" +
                "- Enemy_ShieldBreaker\n\n" +
                "Check Assets/Prefabs/Characters/",
                "OK"
            );
        }
        
        private static void CreateVariant(GameObject basePrefab, string variantName, Color color)
        {
            string variantPath = $"Assets/Prefabs/Characters/{variantName}.prefab";
            
            GameObject variant = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            variant.name = variantName;
            
            Transform modelTransform = variant.transform.Find("Model");
            if (modelTransform != null)
            {
                Renderer[] renderers = modelTransform.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        Material newMat = new Material(materials[i]);
                        newMat.name = $"M_{variantName}";
                        newMat.color = color;
                        materials[i] = newMat;
                    }
                    renderer.sharedMaterials = materials;
                }
            }
            
            PrefabUtility.SaveAsPrefabAsset(variant, variantPath);
            Object.DestroyImmediate(variant);
            
            Debug.Log($"<color=green>✓</color> Created variant: {variantName}");
        }
    }
}
