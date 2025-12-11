using UnityEngine;
using UnityEditor;
using ShieldWall.Visual;
using ShieldWall.Data;
using System.Collections.Generic;

namespace ShieldWall.Editor
{
    public class VisualSystemValidator : EditorWindow
    {
        private Vector2 _scrollPosition;
        private List<string> _validationResults = new List<string>();
        
        [MenuItem("Shield Wall/Validate Visual System")]
        public static void ShowWindow()
        {
            GetWindow<VisualSystemValidator>("Visual System Validator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("3D Model Upgrade Validation", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Run Full Validation", GUILayout.Height(30)))
            {
                RunValidation();
            }
            
            GUILayout.Space(10);
            
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            foreach (var result in _validationResults)
            {
                if (result.StartsWith("[PASS]"))
                {
                    GUI.color = Color.green;
                }
                else if (result.StartsWith("[FAIL]"))
                {
                    GUI.color = Color.red;
                }
                else if (result.StartsWith("[WARN]"))
                {
                    GUI.color = Color.yellow;
                }
                else
                {
                    GUI.color = Color.white;
                }
                
                GUILayout.Label(result);
            }
            
            GUI.color = Color.white;
            GUILayout.EndScrollView();
        }
        
        private void RunValidation()
        {
            _validationResults.Clear();
            _validationResults.Add("=== VISUAL SYSTEM VALIDATION ===");
            _validationResults.Add("");
            
            ValidateFolderStructure();
            ValidateScripts();
            ValidatePrefabs();
            ValidateMaterials();
            ValidateEditorTools();
            
            _validationResults.Add("");
            _validationResults.Add("=== VALIDATION COMPLETE ===");
        }
        
        private void ValidateFolderStructure()
        {
            _validationResults.Add("--- Folder Structure ---");
            
            CheckFolder("Assets/Prefabs/Characters");
            CheckFolder("Assets/Prefabs/Gore");
            CheckFolder("Assets/Prefabs/VFX");
            CheckFolder("Assets/Art/Models/Characters");
            CheckFolder("Assets/Art/Materials/Characters");
            CheckFolder("Assets/Art/Models/Environment");
            CheckFolder("Assets/Art/Materials/Environment");
            
            _validationResults.Add("");
        }
        
        private void ValidateScripts()
        {
            _validationResults.Add("--- Core Scripts ---");
            
            CheckScript("Assets/Scripts/Data/ModularCharacterData.cs");
            CheckScript("Assets/Scripts/Visual/ModularCharacterBuilder.cs");
            CheckScript("Assets/Scripts/Visual/SeveredLimb.cs");
            CheckScript("Assets/Scripts/Visual/DismembermentController.cs");
            CheckScript("Assets/Scripts/Visual/PrimitiveMeshGenerator.cs");
            CheckScript("Assets/Scripts/Visual/ActionDismembermentMapper.cs");
            CheckScript("Assets/Scripts/Data/ToonMaterialPalette.cs");
            CheckScript("Assets/Scripts/Visual/LODController.cs");
            
            _validationResults.Add("");
        }
        
        private void ValidatePrefabs()
        {
            _validationResults.Add("--- Prefabs ---");
            
            CheckPrefab("Assets/Prefabs/Gore/SeveredHead.prefab");
            CheckPrefab("Assets/Prefabs/Gore/SeveredArm.prefab");
            CheckPrefab("Assets/Prefabs/Gore/SeveredLeg.prefab");
            CheckPrefab("Assets/Prefabs/VFX/BloodBurst.prefab");
            
            _validationResults.Add("");
        }
        
        private void ValidateMaterials()
        {
            _validationResults.Add("--- Materials ---");
            
            CheckMaterial("Assets/Art/Materials/Characters/M_Character_Player.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Character_Brother.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Character_Thrall.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Character_Warrior.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Character_Berserker.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Character_Archer.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Blood.mat");
            CheckMaterial("Assets/Art/Materials/Characters/M_Gore.mat");
            
            _validationResults.Add("");
        }
        
        private void ValidateEditorTools()
        {
            _validationResults.Add("--- Editor Tools ---");
            
            CheckScript("Assets/Editor/PrimitivePrefabCreator.cs");
            CheckScript("Assets/Editor/MaterialCreator.cs");
            CheckScript("Assets/Editor/EnvironmentPropCreator.cs");
            CheckScript("Assets/Editor/BloodDecalCreator.cs");
            
            _validationResults.Add("");
        }
        
        private void CheckFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                _validationResults.Add($"[PASS] Folder exists: {path}");
            }
            else
            {
                _validationResults.Add($"[FAIL] Folder missing: {path}");
            }
        }
        
        private void CheckScript(string path)
        {
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null)
            {
                _validationResults.Add($"[PASS] Script exists: {path}");
            }
            else
            {
                _validationResults.Add($"[WARN] Script not found (may need creation): {path}");
            }
        }
        
        private void CheckPrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                _validationResults.Add($"[PASS] Prefab exists: {path}");
            }
            else
            {
                _validationResults.Add($"[WARN] Prefab not created yet (use editor tools): {path}");
            }
        }
        
        private void CheckMaterial(string path)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                bool hasInstancing = material.enableInstancing;
                if (hasInstancing)
                {
                    _validationResults.Add($"[PASS] Material exists with GPU instancing: {path}");
                }
                else
                {
                    _validationResults.Add($"[WARN] Material exists but GPU instancing disabled: {path}");
                }
            }
            else
            {
                _validationResults.Add($"[WARN] Material not created yet (use editor tools): {path}");
            }
        }
    }
}
