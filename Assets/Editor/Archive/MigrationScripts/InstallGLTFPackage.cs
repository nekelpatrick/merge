using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace ShieldWall.Editor
{
    public class InstallGLTFPackage
    {
        private static AddRequest _addRequest;
        
        [MenuItem("ShieldWall/Setup/Install glTFast Package (Required for GLB Import)")]
        public static void InstallGLTFast()
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Install glTFast Package",
                "This will install Unity's official glTFast package which adds support for GLB/GLTF 3D model formats.\n\n" +
                "This is required to import the Viking_Player.glb model.\n\n" +
                "Installation will take a few moments.\n\n" +
                "After installation completes:\n" +
                "1. Wait for Unity to finish compiling\n" +
                "2. Run: ShieldWall > Setup > 0. Force Import Viking Model\n" +
                "3. Continue with the setup",
                "Install Now",
                "Cancel"
            );
            
            if (!confirm) return;
            
            Debug.Log("Installing glTFast package from Unity Registry...");
            _addRequest = Client.Add("com.unity.cloud.gltfast");
            EditorApplication.update += CheckInstallProgress;
        }
        
        private static void CheckInstallProgress()
        {
            if (_addRequest == null || !_addRequest.IsCompleted)
                return;
            
            EditorApplication.update -= CheckInstallProgress;
            
            if (_addRequest.Status == StatusCode.Success)
            {
                Debug.Log($"<color=green>✓</color> Successfully installed: {_addRequest.Result.displayName}");
                
                EditorUtility.DisplayDialog(
                    "✅ Installation Complete!",
                    $"Successfully installed {_addRequest.Result.displayName}\n\n" +
                    "Unity can now import GLB/GLTF files!\n\n" +
                    "Next steps:\n" +
                    "1. Wait for Unity to finish compiling (watch bottom right)\n" +
                    "2. Run: ShieldWall > Setup > 0. Force Import Viking Model\n" +
                    "3. Run: ShieldWall > Setup > 2. Auto-Setup Viking Enemy Prefab\n" +
                    "4. Run: ShieldWall > Setup > 3. Create Enemy Variant Prefabs",
                    "OK"
                );
            }
            else
            {
                Debug.LogError($"Failed to install glTFast package: {_addRequest.Error.message}");
                
                EditorUtility.DisplayDialog(
                    "⚠️ Installation Failed",
                    $"Could not install glTFast package.\n\n" +
                    $"Error: {_addRequest.Error.message}\n\n" +
                    "Alternative solutions:\n" +
                    "1. Manually install via Window > Package Manager\n" +
                    "   - Search for 'glTFast' or 'gltf'\n" +
                    "   - Click Install\n\n" +
                    "2. Or convert the GLB file to FBX format\n" +
                    "   - Use Blender (free) to convert GLB → FBX\n" +
                    "   - Replace Viking_Player.glb with Viking_Player.fbx",
                    "OK"
                );
            }
        }
        
        [MenuItem("ShieldWall/Setup/Check GLB Import Support")]
        public static void CheckGLBSupport()
        {
            string[] gltfPackages = new string[]
            {
                "com.unity.cloud.gltfast",
                "com.unity.gltf",
                "com.atteneder.gltfast"
            };
            
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted) { }
            
            bool hasGLTFSupport = false;
            string installedPackage = "";
            
            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    foreach (var gltfPkg in gltfPackages)
                    {
                        if (package.name.Contains("gltf") || package.name == gltfPkg)
                        {
                            hasGLTFSupport = true;
                            installedPackage = package.displayName;
                            break;
                        }
                    }
                }
            }
            
            if (hasGLTFSupport)
            {
                EditorUtility.DisplayDialog(
                    "✅ GLB Support Available",
                    $"Detected: {installedPackage}\n\n" +
                    "Unity can import GLB/GLTF files!\n\n" +
                    "If the Viking model still won't import:\n" +
                    "1. The GLB file may be corrupted\n" +
                    "2. Try converting to FBX format instead\n" +
                    "3. Or try a different Viking GLB model",
                    "OK"
                );
            }
            else
            {
                bool install = EditorUtility.DisplayDialog(
                    "⚠️ No GLB Import Support",
                    "Unity cannot import GLB/GLTF files without an additional package.\n\n" +
                    "Would you like to install glTFast package now?\n\n" +
                    "Alternative: Convert the GLB file to FBX format using Blender (free).",
                    "Install glTFast",
                    "Cancel"
                );
                
                if (install)
                {
                    InstallGLTFast();
                }
            }
        }
    }
}
