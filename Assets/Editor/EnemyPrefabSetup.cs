using UnityEngine;
using UnityEditor;
using ShieldWall.Visual;
using ShieldWall.Data;

namespace ShieldWall.Editor
{
    public class EnemyPrefabSetup : EditorWindow
    {
        private GameObject _vikingModelPrefab;
        private ModularCharacterData _characterData;
        private string _prefabName = "Enemy_Viking";
        
        [MenuItem("ShieldWall/Setup/Create Enemy Prefab from Model")]
        public static void ShowWindow()
        {
            GetWindow<EnemyPrefabSetup>("Enemy Prefab Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Enemy Prefab Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            _vikingModelPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Viking Model (GLB)", 
                _vikingModelPrefab, 
                typeof(GameObject), 
                false
            );
            
            _characterData = (ModularCharacterData)EditorGUILayout.ObjectField(
                "Character Data (Optional)", 
                _characterData, 
                typeof(ModularCharacterData), 
                false
            );
            
            _prefabName = EditorGUILayout.TextField("Prefab Name", _prefabName);
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "This will create an enemy prefab from the Viking model with:\n" +
                "- Proper rotation for combat stance\n" +
                "- EnemyVisualInstance component\n" +
                "- ModularCharacterBuilder component\n" +
                "- DismembermentController component\n" +
                "- Collider for selection\n" +
                "- Outline for hover effects",
                MessageType.Info
            );
            
            EditorGUILayout.Space();
            
            GUI.enabled = _vikingModelPrefab != null;
            if (GUILayout.Button("Create Enemy Prefab", GUILayout.Height(40)))
            {
                CreateEnemyPrefab();
            }
            GUI.enabled = true;
        }
        
        private void CreateEnemyPrefab()
        {
            GameObject enemyRoot = new GameObject(_prefabName);
            enemyRoot.transform.position = Vector3.zero;
            enemyRoot.transform.rotation = Quaternion.identity;
            
            GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(_vikingModelPrefab);
            modelInstance.transform.SetParent(enemyRoot.transform);
            modelInstance.transform.localPosition = Vector3.zero;
            
            modelInstance.transform.localRotation = Quaternion.Euler(0, 180, 0);
            modelInstance.transform.localScale = Vector3.one;
            
            EnemyVisualInstance visualInstance = enemyRoot.AddComponent<EnemyVisualInstance>();
            
            ModularCharacterBuilder modularBuilder = enemyRoot.AddComponent<ModularCharacterBuilder>();
            if (_characterData != null)
            {
                SerializedObject so = new SerializedObject(modularBuilder);
                so.FindProperty("_characterData").objectReferenceValue = _characterData;
                so.ApplyModifiedProperties();
            }
            
            DismembermentController dismemberment = enemyRoot.AddComponent<DismembermentController>();
            
            CapsuleCollider collider = enemyRoot.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 1f, 0);
            collider.radius = 0.4f;
            collider.height = 2f;
            collider.isTrigger = true;
            
            string prefabPath = $"Assets/Prefabs/Characters/{_prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(enemyRoot, prefabPath);
            
            DestroyImmediate(enemyRoot);
            
            EditorUtility.DisplayDialog(
                "Success", 
                $"Enemy prefab created at:\n{prefabPath}\n\n" +
                "Next steps:\n" +
                "1. Assign the prefab to EnemySO assets in ScriptableObjects/Enemies/\n" +
                "2. Configure materials for different enemy types\n" +
                "3. Test in Battle scene",
                "OK"
            );
            
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            EditorGUIUtility.PingObject(Selection.activeObject);
        }
    }
}
