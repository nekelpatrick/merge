using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace ShieldWall.Editor
{
    /// <summary>
    /// Helper utilities for creating and configuring UI components programmatically in Editor scripts.
    /// Used by Phase5_5_PrefabCreator.cs and Phase5_5_SceneIntegrator.cs.
    /// </summary>
    public static class UIComponentFactory
    {
        public static GameObject CreateActionPreviewItemTemplate(Transform parent)
        {
            GameObject root = new GameObject("ActionPreviewItem");
            if (parent != null) root.transform.SetParent(parent, false);
            
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(350, 90);
            
            Image background = root.AddComponent<Image>();
            background.color = new Color(107f/255f, 78f/255f, 61f/255f, 204f/255f);
            
            GameObject nameTextGO = new GameObject("ActionNameText");
            nameTextGO.transform.SetParent(root.transform, false);
            RectTransform nameRect = nameTextGO.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.pivot = new Vector2(0.5f, 1);
            nameRect.anchoredPosition = new Vector2(0, -10);
            nameRect.sizeDelta = new Vector2(-20, 25);
            
            TextMeshProUGUI nameText = nameTextGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Action Name";
            nameText.fontSize = 18;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = new Color(201f/255f, 162f/255f, 39f/255f, 1f);
            nameText.alignment = TextAlignmentOptions.TopLeft;
            
            GameObject runeContainerGO = new GameObject("RuneContainer");
            runeContainerGO.transform.SetParent(root.transform, false);
            RectTransform runeRect = runeContainerGO.AddComponent<RectTransform>();
            runeRect.anchorMin = new Vector2(0, 1);
            runeRect.anchorMax = new Vector2(1, 1);
            runeRect.pivot = new Vector2(0, 1);
            runeRect.anchoredPosition = new Vector2(10, -40);
            runeRect.sizeDelta = new Vector2(-20, 30);
            
            HorizontalLayoutGroup runeLayout = runeContainerGO.AddComponent<HorizontalLayoutGroup>();
            runeLayout.spacing = 5;
            runeLayout.childAlignment = TextAnchor.MiddleLeft;
            runeLayout.childForceExpandWidth = false;
            runeLayout.childForceExpandHeight = false;
            
            GameObject effectTextGO = new GameObject("EffectText");
            effectTextGO.transform.SetParent(root.transform, false);
            RectTransform effectRect = effectTextGO.AddComponent<RectTransform>();
            effectRect.anchorMin = new Vector2(0, 0);
            effectRect.anchorMax = new Vector2(1, 0);
            effectRect.pivot = new Vector2(0.5f, 0);
            effectRect.anchoredPosition = new Vector2(0, 10);
            effectRect.sizeDelta = new Vector2(-20, 20);
            
            TextMeshProUGUI effectText = effectTextGO.AddComponent<TextMeshProUGUI>();
            effectText.text = "Effect description";
            effectText.fontSize = 14;
            effectText.color = new Color(212f/255f, 200f/255f, 184f/255f, 1f);
            effectText.alignment = TextAlignmentOptions.BottomLeft;
            
            GameObject statusIconGO = new GameObject("StatusIcon");
            statusIconGO.transform.SetParent(root.transform, false);
            RectTransform statusRect = statusIconGO.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(1, 1);
            statusRect.anchorMax = new Vector2(1, 1);
            statusRect.pivot = new Vector2(1, 1);
            statusRect.anchoredPosition = new Vector2(-10, -10);
            statusRect.sizeDelta = new Vector2(24, 24);
            
            Image statusIcon = statusIconGO.AddComponent<Image>();
            statusIcon.color = Color.white;
            
            return root;
        }
        
        public static GameObject CreateRuneBadgeTemplate(Transform parent)
        {
            GameObject root = new GameObject("RuneBadge");
            if (parent != null) root.transform.SetParent(parent, false);
            
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(60, 30);
            
            Image background = root.AddComponent<Image>();
            background.color = Color.gray;
            
            GameObject textGO = new GameObject("RuneSymbol");
            textGO.transform.SetParent(root.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            TextMeshProUGUI text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = "ᚦ";
            text.fontSize = 16;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            
            return root;
        }
        
        public static GameObject CreatePhaseBannerTemplate(Transform parent)
        {
            GameObject root = new GameObject("PhaseBannerUI");
            if (parent != null) root.transform.SetParent(parent, false);
            
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 1);
            rootRect.anchorMax = new Vector2(0.5f, 1);
            rootRect.pivot = new Vector2(0.5f, 1);
            rootRect.anchoredPosition = new Vector2(0, -50);
            rootRect.sizeDelta = new Vector2(700, 110);
            
            CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            GameObject backgroundGO = new GameObject("Background");
            backgroundGO.transform.SetParent(root.transform, false);
            RectTransform bgRect = backgroundGO.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            
            Image background = backgroundGO.AddComponent<Image>();
            background.color = new Color(42f/255f, 31f/255f, 26f/255f, 230f/255f);
            
            GameObject contentGO = new GameObject("Content");
            contentGO.transform.SetParent(root.transform, false);
            RectTransform contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = new Vector2(-20, -20);
            
            VerticalLayoutGroup verticalLayout = contentGO.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = 5;
            verticalLayout.childAlignment = TextAnchor.MiddleCenter;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.padding = new RectOffset(10, 10, 10, 10);
            
            GameObject phaseTextGO = new GameObject("PhaseText");
            phaseTextGO.transform.SetParent(contentGO.transform, false);
            RectTransform phaseRect = phaseTextGO.AddComponent<RectTransform>();
            phaseRect.sizeDelta = new Vector2(680, 35);
            
            TextMeshProUGUI phaseText = phaseTextGO.AddComponent<TextMeshProUGUI>();
            phaseText.text = "YOUR TURN";
            phaseText.fontSize = 24;
            phaseText.fontStyle = FontStyles.Bold;
            phaseText.color = new Color(201f/255f, 162f/255f, 39f/255f, 1f);
            phaseText.alignment = TextAlignmentOptions.Center;
            
            GameObject ctaTextGO = new GameObject("CTAText");
            ctaTextGO.transform.SetParent(contentGO.transform, false);
            RectTransform ctaRect = ctaTextGO.AddComponent<RectTransform>();
            ctaRect.sizeDelta = new Vector2(680, 25);
            
            TextMeshProUGUI ctaText = ctaTextGO.AddComponent<TextMeshProUGUI>();
            ctaText.text = "Lock dice to ready actions, then confirm";
            ctaText.fontSize = 16;
            ctaText.color = new Color(212f/255f, 200f/255f, 184f/255f, 1f);
            ctaText.alignment = TextAlignmentOptions.Center;
            
            return root;
        }
        
        public static void LogComponentStructure(GameObject go, int indent = 0)
        {
            string indentStr = new string(' ', indent * 2);
            Component[] components = go.GetComponents<Component>();
            
            Debug.Log($"{indentStr}{go.name} ({components.Length} components)");
            
            foreach (Component comp in components)
            {
                Debug.Log($"{indentStr}  - {comp.GetType().Name}");
            }
            
            foreach (Transform child in go.transform)
            {
                LogComponentStructure(child.gameObject, indent + 1);
            }
        }
    }
}

