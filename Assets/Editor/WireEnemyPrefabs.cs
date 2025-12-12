using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using ShieldWall.Visual;

namespace ShieldWall.Editor
{
    public class WireEnemyPrefabs
    {
        [MenuItem("ShieldWall/Setup/4. Wire Enemy Prefabs to Scene")]
        public static void WireEnemyPrefabsToScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            
            if (activeScene.name != "Battle")
            {
                bool switchScene = EditorUtility.DisplayDialog(
                    "Wrong Scene",
                    $"Current scene: {activeScene.name}\n\n" +
                    "This script needs to run in the Battle scene.\n\n" +
                    "Open Battle scene and try again?",
                    "Open Battle Scene",
                    "Cancel"
                );
                
                if (switchScene)
                {
                    EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
                    EditorApplication.delayCall += WireEnemyPrefabsToScene;
                }
                return;
            }
            
            GameObject visualControllerGO = GameObject.Find("EnemyVisualController");
            
            if (visualControllerGO == null)
            {
                EditorUtility.DisplayDialog(
                    "EnemyVisualController Not Found",
                    "Could not find EnemyVisualController in the Battle scene.\n\n" +
                    "Make sure you're in the Battle scene.",
                    "OK"
                );
                return;
            }
            
            var visualController = visualControllerGO.GetComponent<EnemyVisualController>();
            
            if (visualController == null)
            {
                EditorUtility.DisplayDialog(
                    "Component Missing",
                    "EnemyVisualController component not found on GameObject.\n\n" +
                    "Make sure the component is attached.",
                    "OK"
                );
                return;
            }
            
            GameObject thrallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_Thrall.prefab");
            GameObject warriorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_Warrior.prefab");
            GameObject berserkerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_Berserker.prefab");
            GameObject archerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_Archer.prefab");
            GameObject spearmanPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_Spearman.prefab");
            GameObject shieldbreakerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_ShieldBreaker.prefab");
            GameObject defaultPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Enemy_Viking.prefab");
            
            SerializedObject so = new SerializedObject(visualController);
            
            so.FindProperty("_enemyThrallPrefab").objectReferenceValue = thrallPrefab;
            so.FindProperty("_enemyWarriorPrefab").objectReferenceValue = warriorPrefab;
            so.FindProperty("_enemyBerserkerPrefab").objectReferenceValue = berserkerPrefab;
            so.FindProperty("_enemyArcherPrefab").objectReferenceValue = archerPrefab;
            so.FindProperty("_enemySpearmanPrefab").objectReferenceValue = spearmanPrefab;
            so.FindProperty("_enemyShieldBreakerPrefab").objectReferenceValue = shieldbreakerPrefab;
            so.FindProperty("_defaultEnemyPrefab").objectReferenceValue = defaultPrefab;
            
            so.ApplyModifiedProperties();
            
            EditorSceneManager.MarkSceneDirty(activeScene);
            
            int assignedCount = 0;
            if (thrallPrefab != null) assignedCount++;
            if (warriorPrefab != null) assignedCount++;
            if (berserkerPrefab != null) assignedCount++;
            if (archerPrefab != null) assignedCount++;
            if (spearmanPrefab != null) assignedCount++;
            if (shieldbreakerPrefab != null) assignedCount++;
            if (defaultPrefab != null) assignedCount++;
            
            EditorUtility.DisplayDialog(
                "✅ Prefabs Wired!",
                $"Successfully assigned {assignedCount}/7 enemy prefabs to EnemyVisualController!\n\n" +
                "Assigned:\n" +
                $"• Thrall: {(thrallPrefab != null ? "✓" : "✗")}\n" +
                $"• Warrior: {(warriorPrefab != null ? "✓" : "✗")}\n" +
                $"• Berserker: {(berserkerPrefab != null ? "✓" : "✗")}\n" +
                $"• Archer: {(archerPrefab != null ? "✓" : "✗")}\n" +
                $"• Spearman: {(spearmanPrefab != null ? "✓" : "✗")}\n" +
                $"• ShieldBreaker: {(shieldbreakerPrefab != null ? "✓" : "✗")}\n" +
                $"• Default: {(defaultPrefab != null ? "✓" : "✗")}\n\n" +
                "Press Play to see Viking models in action! ⚔️",
                "OK"
            );
            
            Selection.activeGameObject = visualControllerGO;
            EditorGUIUtility.PingObject(visualControllerGO);
        }
    }
}
