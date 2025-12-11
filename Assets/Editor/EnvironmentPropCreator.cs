using UnityEngine;
using UnityEditor;

namespace ShieldWall.Editor
{
    public class EnvironmentPropCreator : EditorWindow
    {
        [MenuItem("Shield Wall/Create Environment Props")]
        public static void CreateEnvironmentProps()
        {
            string propPath = "Assets/Art/Models/Environment/";
            string prefabPath = "Assets/Prefabs/Environment/";
            
            if (!AssetDatabase.IsValidFolder(propPath.TrimEnd('/')))
            {
                AssetDatabase.CreateFolder("Assets/Art/Models", "Environment");
            }
            
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Environment"))
            {
                AssetDatabase.CreateFolder("Assets/Prefabs", "Environment");
            }
            
            CreateGroundPlane(prefabPath);
            CreateStake(prefabPath);
            CreateRock(prefabPath);
            CreateDebris(prefabPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Successfully created environment props!");
        }
        
        private static void CreateGroundPlane(string path)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "GroundPlane";
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
            ground.transform.position = new Vector3(0f, 0f, 0f);
            
            var renderer = ground.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = new Color(0.29f, 0.22f, 0.16f);
                mat.SetFloat("_Smoothness", 0.2f);
                renderer.sharedMaterial = mat;
                
                AssetDatabase.CreateAsset(mat, "Assets/Art/Materials/Environment/M_Ground.mat");
            }
            
            string prefabPath = path + "GroundPlane.prefab";
            PrefabUtility.SaveAsPrefabAsset(ground, prefabPath);
            DestroyImmediate(ground);
            
            Debug.Log($"Created: {prefabPath}");
        }
        
        private static void CreateStake(string path)
        {
            GameObject stake = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stake.name = "WoodenStake";
            stake.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
            stake.transform.position = new Vector3(0f, 1f, 0f);
            
            var renderer = stake.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = new Color(0.35f, 0.25f, 0.15f);
                mat.SetFloat("_Smoothness", 0.1f);
                renderer.sharedMaterial = mat;
                
                AssetDatabase.CreateAsset(mat, "Assets/Art/Materials/Environment/M_Wood.mat");
            }
            
            string prefabPath = path + "WoodenStake.prefab";
            PrefabUtility.SaveAsPrefabAsset(stake, prefabPath);
            DestroyImmediate(stake);
            
            Debug.Log($"Created: {prefabPath}");
        }
        
        private static void CreateRock(string path)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "Rock";
            rock.transform.localScale = new Vector3(0.8f, 0.6f, 0.9f);
            rock.transform.position = new Vector3(0f, 0.3f, 0f);
            
            var renderer = rock.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = new Color(0.4f, 0.4f, 0.4f);
                mat.SetFloat("_Smoothness", 0.1f);
                renderer.sharedMaterial = mat;
                
                AssetDatabase.CreateAsset(mat, "Assets/Art/Materials/Environment/M_Stone.mat");
            }
            
            string prefabPath = path + "Rock.prefab";
            PrefabUtility.SaveAsPrefabAsset(rock, prefabPath);
            DestroyImmediate(rock);
            
            Debug.Log($"Created: {prefabPath}");
        }
        
        private static void CreateDebris(string path)
        {
            GameObject debris = new GameObject("Debris");
            
            for (int i = 0; i < 3; i++)
            {
                GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
                piece.name = $"DebrisPiece{i}";
                piece.transform.SetParent(debris.transform);
                piece.transform.localPosition = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    0.1f,
                    Random.Range(-0.3f, 0.3f)
                );
                piece.transform.localRotation = Random.rotation;
                piece.transform.localScale = new Vector3(
                    Random.Range(0.1f, 0.2f),
                    Random.Range(0.05f, 0.1f),
                    Random.Range(0.1f, 0.2f)
                );
                
                var renderer = piece.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.color = new Color(0.3f, 0.25f, 0.2f);
                    renderer.sharedMaterial = mat;
                }
            }
            
            string prefabPath = path + "Debris.prefab";
            PrefabUtility.SaveAsPrefabAsset(debris, prefabPath);
            DestroyImmediate(debris);
            
            Debug.Log($"Created: {prefabPath}");
        }
    }
}
