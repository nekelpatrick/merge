using UnityEngine;
using UnityEditor;

namespace ShieldWall.Editor
{
    public class OneClickSetup : EditorWindow
    {
        [MenuItem("Shield Wall/One-Click Setup All Assets")]
        public static void SetupAllAssets()
        {
            if (!EditorUtility.DisplayDialog(
                "One-Click Setup",
                "This will create all prefabs, materials, and environment props.\n\nContinue?",
                "Yes",
                "Cancel"))
            {
                return;
            }
            
            Debug.Log("=== Starting One-Click Setup ===");
            
            EnsureFoldersExist();
            
            Debug.Log("Step 1/4: Creating materials...");
            MaterialCreator.CreateMaterials();
            
            Debug.Log("Step 2/4: Creating limb prefabs...");
            PrimitivePrefabCreator.CreateLimbPrefabs();
            
            Debug.Log("Step 3/4: Creating blood VFX...");
            BloodDecalCreator.CreateBloodDecals();
            
            Debug.Log("Step 4/4: Creating environment props...");
            EnvironmentPropCreator.CreateEnvironmentProps();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("=== One-Click Setup Complete! ===");
            Debug.Log("Use 'Shield Wall > Validate Visual System' to verify all assets.");
            
            EditorUtility.DisplayDialog(
                "Setup Complete",
                "All assets have been created successfully!\n\nNext steps:\n" +
                "1. Run 'Shield Wall > Validate Visual System'\n" +
                "2. Check Assets/Documentation/Phase5/Phase5_3DModelUpgrade_Complete.md\n" +
                "3. Assign prefab references in Battle scene",
                "OK");
        }
        
        private static void EnsureFoldersExist()
        {
            EnsureFolder("Assets/Prefabs", "Characters");
            EnsureFolder("Assets/Prefabs", "Gore");
            EnsureFolder("Assets/Prefabs", "VFX");
            EnsureFolder("Assets/Prefabs", "Environment");
            EnsureFolder("Assets/Art/Materials", "Characters");
            EnsureFolder("Assets/Art/Materials", "Environment");
            EnsureFolder("Assets/Art/Materials", "Effects");
        }
        
        private static void EnsureFolder(string parent, string folder)
        {
            string path = $"{parent}/{folder}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, folder);
                Debug.Log($"Created folder: {path}");
            }
        }
    }
}
