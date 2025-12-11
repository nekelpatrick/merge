using UnityEngine;
using UnityEditor;

namespace ShieldWall.Editor
{
    public class BloodDecalCreator : EditorWindow
    {
        [MenuItem("Shield Wall/Create Blood Decal Prefabs")]
        public static void CreateBloodDecals()
        {
            string prefabPath = "Assets/Prefabs/VFX/";
            
            if (!AssetDatabase.IsValidFolder(prefabPath.TrimEnd('/')))
            {
                Debug.LogError($"Folder {prefabPath} does not exist!");
                return;
            }
            
            CreateBloodDecalVariant(prefabPath, "BloodDecal_01", 0);
            CreateBloodDecalVariant(prefabPath, "BloodDecal_02", 1);
            CreateBloodDecalVariant(prefabPath, "BloodDecal_03", 2);
            
            CreateBloodBurstPrefab(prefabPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Successfully created blood VFX prefabs in {prefabPath}");
        }
        
        private static void CreateBloodDecalVariant(string path, string name, int variant)
        {
            GameObject decalObj = new GameObject(name);
            
            var decalProjector = decalObj.AddComponent<DecalProjector>();
            if (decalProjector != null)
            {
                decalProjector.size = new Vector3(1f, 1f, 0.5f);
                decalProjector.pivot = new Vector3(0f, 0f, 0.5f);
                
                Material decalMat = CreateBloodDecalMaterial(variant);
                decalProjector.material = decalMat;
                
                string matPath = $"Assets/Art/Materials/Effects/M_BloodDecal_{variant:00}.mat";
                AssetDatabase.CreateAsset(decalMat, matPath);
            }
            
            string prefabPath = path + name + ".prefab";
            PrefabUtility.SaveAsPrefabAsset(decalObj, prefabPath);
            DestroyImmediate(decalObj);
            
            Debug.Log($"Created: {prefabPath}");
        }
        
        private static Material CreateBloodDecalMaterial(int variant)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Decal");
            if (shader == null)
            {
                shader = Shader.Find("Shader Graphs/Decal");
            }
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Lit");
            }
            
            Material mat = new Material(shader);
            mat.name = $"M_BloodDecal_{variant:00}";
            
            Color bloodColor = new Color(0.42f, 0.063f, 0.063f, 0.8f);
            mat.color = bloodColor;
            
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", bloodColor);
            }
            
            if (mat.HasProperty("_Smoothness"))
            {
                mat.SetFloat("_Smoothness", 0.2f);
            }
            
            return mat;
        }
        
        private static void CreateBloodBurstPrefab(string path)
        {
            GameObject burstObj = new GameObject("BloodBurst");
            
            ParticleSystem ps = burstObj.AddComponent<ParticleSystem>();
            var bloodScript = burstObj.AddComponent<Visual.BloodBurstVFX>();
            
            string prefabPath = path + "BloodBurst.prefab";
            PrefabUtility.SaveAsPrefabAsset(burstObj, prefabPath);
            DestroyImmediate(burstObj);
            
            Debug.Log($"Created: {prefabPath}");
        }
    }
}
