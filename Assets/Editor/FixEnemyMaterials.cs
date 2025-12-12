using UnityEngine;
using UnityEditor;

namespace ShieldWall.Editor
{
    public class FixEnemyMaterials
    {
        [MenuItem("ShieldWall/Setup/5. Fix Enemy Materials (Pink Models)")]
        public static void FixMaterials()
        {
            string[] prefabPaths = new string[]
            {
                "Assets/Prefabs/Characters/Enemy_Viking.prefab",
                "Assets/Prefabs/Characters/Enemy_Thrall.prefab",
                "Assets/Prefabs/Characters/Enemy_Warrior.prefab",
                "Assets/Prefabs/Characters/Enemy_Berserker.prefab",
                "Assets/Prefabs/Characters/Enemy_Archer.prefab",
                "Assets/Prefabs/Characters/Enemy_Spearman.prefab",
                "Assets/Prefabs/Characters/Enemy_ShieldBreaker.prefab"
            };
            
            Material existingMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Materials/Characters/M_Enemy_Viking.mat");
            
            if (existingMaterial == null)
            {
                existingMaterial = CreateDefaultMaterial();
            }
            
            int fixedCount = 0;
            
            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null) continue;
                
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                
                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                
                foreach (Renderer renderer in renderers)
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] == null || materials[i].shader.name.Contains("Hidden"))
                        {
                            materials[i] = existingMaterial;
                        }
                    }
                    renderer.sharedMaterials = materials;
                }
                
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.AutomatedAction);
                Object.DestroyImmediate(instance);
                
                fixedCount++;
                Debug.Log($"<color=green>✓</color> Fixed materials for {prefabPath}");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog(
                "✅ Materials Fixed!",
                $"Fixed materials for {fixedCount} enemy prefabs!\n\n" +
                "All enemies now use:\n" +
                $"• Shader: {existingMaterial.shader.name}\n" +
                $"• Color: {existingMaterial.color}\n\n" +
                "Press Play again - enemies should now have proper brown Viking appearance! ⚔️",
                "OK"
            );
        }
        
        private static Material CreateDefaultMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }
            
            Material mat = new Material(shader);
            mat.name = "M_Enemy_Viking";
            mat.color = new Color(0.7f, 0.4f, 0.3f, 1f);
            
            string materialPath = "Assets/Art/Materials/Characters/M_Enemy_Viking.mat";
            AssetDatabase.CreateAsset(mat, materialPath);
            
            Debug.Log($"Created default enemy material at {materialPath}");
            
            return mat;
        }
    }
}
