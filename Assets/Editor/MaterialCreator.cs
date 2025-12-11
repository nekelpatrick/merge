using UnityEngine;
using UnityEditor;

namespace ShieldWall.Editor
{
    public class MaterialCreator : EditorWindow
    {
        [MenuItem("Shield Wall/Create Toon Materials")]
        public static void CreateMaterials()
        {
            string materialPath = "Assets/Art/Materials/Characters/";
            
            if (!AssetDatabase.IsValidFolder(materialPath.TrimEnd('/')))
            {
                Debug.LogError($"Folder {materialPath} does not exist!");
                return;
            }
            
            CreateMaterial(materialPath, "M_Character_Player", new Color(0.42f, 0.31f, 0.24f));
            CreateMaterial(materialPath, "M_Character_Brother", new Color(0.3f, 0.3f, 0.4f));
            CreateMaterial(materialPath, "M_Character_Thrall", new Color(0.42f, 0.27f, 0.14f));
            CreateMaterial(materialPath, "M_Character_Warrior", new Color(0.35f, 0.35f, 0.35f));
            CreateMaterial(materialPath, "M_Character_Berserker", new Color(0.55f, 0.13f, 0.13f));
            CreateMaterial(materialPath, "M_Character_Archer", new Color(0.18f, 0.35f, 0.15f));
            CreateMaterial(materialPath, "M_Blood", new Color(0.55f, 0.13f, 0.13f));
            CreateMaterial(materialPath, "M_Gore", new Color(0.4f, 0.1f, 0.1f));
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Successfully created toon materials in {materialPath}");
        }
        
        private static void CreateMaterial(string path, string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }
            
            Material mat = new Material(shader);
            mat.name = name;
            mat.color = color;
            
            mat.SetFloat("_Smoothness", 0.3f);
            mat.SetFloat("_Metallic", 0.0f);
            
            if (mat.HasProperty("_Surface"))
            {
                mat.SetFloat("_Surface", 0);
            }
            
            mat.enableInstancing = true;
            
            string fullPath = path + name + ".mat";
            AssetDatabase.CreateAsset(mat, fullPath);
            
            Debug.Log($"Created material: {fullPath}");
        }
    }
}
