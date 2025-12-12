using UnityEngine;
using UnityEditor;
using System.IO;

namespace ShieldWall.Editor
{
    public class ForceModelImport
    {
        [MenuItem("ShieldWall/Setup/0. Force Import Viking Model (Run This First!)")]
        public static void ForceImportVikingModel()
        {
            string modelPath = "Assets/Art/Models/Characters/Viking_Player.glb";
            string metaPath = modelPath + ".meta";
            
            if (!File.Exists(modelPath))
            {
                EditorUtility.DisplayDialog(
                    "File Not Found",
                    $"Viking model file not found at:\n{modelPath}",
                    "OK"
                );
                return;
            }
            
            EditorUtility.DisplayProgressBar("Importing Model", "Deleting old meta file...", 0.2f);
            
            if (File.Exists(metaPath))
            {
                FileUtil.DeleteFileOrDirectory(metaPath);
                Debug.Log("Deleted old .meta file");
            }
            
            EditorUtility.DisplayProgressBar("Importing Model", "Refreshing asset database...", 0.4f);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
            EditorUtility.DisplayProgressBar("Importing Model", "Forcing reimport...", 0.6f);
            AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            
            EditorUtility.DisplayProgressBar("Importing Model", "Checking import status...", 0.8f);
            
            AssetImporter importer = AssetImporter.GetAtPath(modelPath);
            
            if (importer == null)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog(
                    "⚠️ Import Failed",
                    "Unity could not create an importer for this file.\n\n" +
                    "The GLB file may be corrupted.\n\n" +
                    "Try:\n" +
                    "1. Re-download the Viking model\n" +
                    "2. Or convert to FBX format",
                    "OK"
                );
                return;
            }
            
            string importerType = importer.GetType().Name;
            Debug.Log($"Importer type: {importerType}");
            
            if (importer is ModelImporter modelImporter)
            {
                modelImporter.isReadable = true;
                modelImporter.animationType = ModelImporterAnimationType.Generic;
                modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
                modelImporter.SaveAndReimport();
            }
            else
            {
                Debug.Log($"Using {importerType} (glTFast or other scripted importer)");
            }
            
            EditorUtility.ClearProgressBar();
            
            AssetDatabase.Refresh();
            
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            
            if (model != null)
            {
                int childCount = CountAllChildren(model.transform);
                
                EditorUtility.DisplayDialog(
                    "✅ Success!",
                    $"Viking model imported successfully!\n\n" +
                    $"Importer: {importerType}\n" +
                    $"Total objects: {childCount}\n\n" +
                    "Next steps:\n" +
                    "1. Run: ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab\n" +
                    "2. Run: ShieldWall > Setup > 3. Create Enemy Variant Prefabs",
                    "OK"
                );
                
                Selection.activeObject = model;
                EditorGUIUtility.PingObject(model);
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "⚠️ Import Issue",
                    $"Model imported with {importerType} but couldn't be loaded as GameObject.\n\n" +
                    "This may indicate:\n" +
                    "• GLB file structure incompatibility\n" +
                    "• Scene-based GLB (not model-based)\n\n" +
                    "Try converting to FBX format for better compatibility.",
                    "OK"
                );
            }
        }
        
        private static int CountAllChildren(Transform transform)
        {
            int count = transform.childCount;
            foreach (Transform child in transform)
            {
                count += CountAllChildren(child);
            }
            return count;
        }
    }
}
