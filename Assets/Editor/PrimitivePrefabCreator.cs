using UnityEngine;
using UnityEditor;
using ShieldWall.Visual;

namespace ShieldWall.Editor
{
    public class PrimitivePrefabCreator : EditorWindow
    {
        [MenuItem("Shield Wall/Create Primitive Limb Prefabs")]
        public static void CreateLimbPrefabs()
        {
            string prefabPath = "Assets/Prefabs/Gore/";
            
            if (!AssetDatabase.IsValidFolder(prefabPath.TrimEnd('/')))
            {
                Debug.LogError($"Folder {prefabPath} does not exist!");
                return;
            }
            
            CreateSeveredHeadPrefab(prefabPath);
            CreateSeveredArmPrefab(prefabPath);
            CreateSeveredLegPrefab(prefabPath);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Successfully created primitive limb prefabs in " + prefabPath);
        }
        
        private static void CreateSeveredHeadPrefab(string path)
        {
            GameObject headObj = new GameObject("SeveredHead");
            
            MeshFilter meshFilter = headObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = PrimitiveMeshGenerator.CreateCubeMesh(0.4f);
            
            MeshRenderer renderer = headObj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = CreateDefaultMaterial();
            
            BoxCollider collider = headObj.AddComponent<BoxCollider>();
            collider.size = Vector3.one * 0.4f;
            
            Rigidbody rb = headObj.AddComponent<Rigidbody>();
            rb.mass = 2f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            
            SeveredLimb limbScript = headObj.AddComponent<SeveredLimb>();
            
            string prefabPath = path + "SeveredHead.prefab";
            PrefabUtility.SaveAsPrefabAsset(headObj, prefabPath);
            DestroyImmediate(headObj);
        }
        
        private static void CreateSeveredArmPrefab(string path)
        {
            GameObject armObj = new GameObject("SeveredArm");
            
            MeshFilter meshFilter = armObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = PrimitiveMeshGenerator.CreateCapsuleMesh(0.6f, 0.08f);
            
            MeshRenderer renderer = armObj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = CreateDefaultMaterial();
            
            CapsuleCollider collider = armObj.AddComponent<CapsuleCollider>();
            collider.height = 0.6f;
            collider.radius = 0.08f;
            collider.direction = 1;
            
            Rigidbody rb = armObj.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            
            SeveredLimb limbScript = armObj.AddComponent<SeveredLimb>();
            
            string prefabPath = path + "SeveredArm.prefab";
            PrefabUtility.SaveAsPrefabAsset(armObj, prefabPath);
            DestroyImmediate(armObj);
        }
        
        private static void CreateSeveredLegPrefab(string path)
        {
            GameObject legObj = new GameObject("SeveredLeg");
            
            MeshFilter meshFilter = legObj.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = PrimitiveMeshGenerator.CreateCapsuleMesh(0.8f, 0.1f);
            
            MeshRenderer renderer = legObj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = CreateDefaultMaterial();
            
            CapsuleCollider collider = legObj.AddComponent<CapsuleCollider>();
            collider.height = 0.8f;
            collider.radius = 0.1f;
            collider.direction = 1;
            
            Rigidbody rb = legObj.AddComponent<Rigidbody>();
            rb.mass = 3f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;
            
            SeveredLimb limbScript = legObj.AddComponent<SeveredLimb>();
            
            string prefabPath = path + "SeveredLeg.prefab";
            PrefabUtility.SaveAsPrefabAsset(legObj, prefabPath);
            DestroyImmediate(legObj);
        }
        
        private static Material CreateDefaultMaterial()
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.6f, 0.3f, 0.3f);
            return mat;
        }
    }
}
