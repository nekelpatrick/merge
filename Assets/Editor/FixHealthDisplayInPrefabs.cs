using UnityEngine;
using UnityEditor;
using ShieldWall.Visual;
using ShieldWall.UI;

namespace ShieldWall.Editor
{
    public class FixHealthDisplayInPrefabs
    {
        [MenuItem("ShieldWall/Setup/6. Fix Health Display in Prefabs")]
        public static void FixHealthDisplays()
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
            
            int fixedCount = 0;
            
            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null) continue;
                
                var healthDisplays = prefab.GetComponentsInChildren<EnemyHealthDisplay>(true);
                
                if (healthDisplays.Length > 1)
                {
                    Debug.LogWarning($"{prefab.name} has {healthDisplays.Length} health displays. Removing duplicates...");
                    
                    for (int i = 1; i < healthDisplays.Length; i++)
                    {
                        Object.DestroyImmediate(healthDisplays[i].gameObject);
                    }
                }
                
                if (healthDisplays.Length == 0)
                {
                    Debug.Log($"{prefab.name} has no health display. Skipping...");
                    continue;
                }
                
                var healthDisplay = healthDisplays[0];
                var canvas = healthDisplay.GetComponent<Canvas>();
                
                if (canvas != null)
                {
                    canvas.renderMode = RenderMode.WorldSpace;
                    
                    var rectTransform = canvas.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.sizeDelta = new Vector2(200, 50);
                        rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                        rectTransform.localPosition = new Vector3(0, 2.5f, 0);
                    }
                }
                
                var textComponent = healthDisplay.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.fontSize = 36;
                    textComponent.fontStyle = TMPro.FontStyles.Bold;
                    textComponent.color = Color.white;
                    textComponent.alignment = TMPro.TextAlignmentOptions.Center;
                    textComponent.enableWordWrapping = false;
                    textComponent.overflowMode = TMPro.TextOverflowModes.Overflow;
                    
                    var textRect = textComponent.GetComponent<RectTransform>();
                    if (textRect != null)
                    {
                        textRect.anchorMin = Vector2.zero;
                        textRect.anchorMax = Vector2.one;
                        textRect.sizeDelta = Vector2.zero;
                        textRect.anchoredPosition = Vector2.zero;
                    }
                }
                
                EditorUtility.SetDirty(prefab);
                fixedCount++;
                Debug.Log($"<color=green>✓</color> Fixed health display for {prefab.name}");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog(
                "✅ Health Displays Fixed!",
                $"Fixed health displays in {fixedCount} enemy prefabs!\n\n" +
                "Changes made:\n" +
                "• Removed duplicate health displays\n" +
                "• Fixed canvas size and positioning\n" +
                "• Fixed text alignment and sizing\n" +
                "• Set proper text overflow settings\n\n" +
                "Note: The Enemy Visual reference will be\n" +
                "connected automatically at runtime when\n" +
                "enemies spawn.\n\n" +
                "Press Play to test!",
                "OK"
            );
        }
    }
}
