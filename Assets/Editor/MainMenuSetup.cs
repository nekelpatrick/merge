using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ShieldWall.Core;
using ShieldWall.UI;
using ShieldWall.Data;

namespace ShieldWall.Editor
{
    public static class MainMenuSetup
    {
        [MenuItem("Shield Wall/Setup/Build Main Menu Scene")]
        public static void BuildMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            CreateCamera();
            CreateEventSystem();
            CreateSceneLoader();
            CreateScenarioManager();
            CreateMainMenuCanvas();
            
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
            
            AddScenesToBuildSettings();
            
            Debug.Log("MainMenu scene created and saved! Run 'Create Menu Prefabs' next.");
        }

        [MenuItem("Shield Wall/Setup/Create Menu Prefabs")]
        public static void CreateMenuPrefabs()
        {
            CreateScenarioCardPrefab();
            CreatePauseMenuPrefab();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Menu prefabs created in Assets/Prefabs/UI/");
        }

        private static void CreateCamera()
        {
            var cameraGO = new GameObject("Main Camera");
            cameraGO.tag = "MainCamera";
            var camera = cameraGO.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.06f, 0.05f);
            cameraGO.AddComponent<AudioListener>();
        }

        private static void CreateEventSystem()
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        private static void CreateSceneLoader()
        {
            var loaderGO = new GameObject("SceneLoader");
            loaderGO.AddComponent<SceneLoader>();
        }

        private static void CreateScenarioManager()
        {
            var managerGO = new GameObject("ScenarioManager");
            managerGO.AddComponent<ScenarioManager>();
        }

        private static void CreateMainMenuCanvas()
        {
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();

            CreateBackground(canvasGO.transform);
            var mainPanel = CreateMainPanel(canvasGO.transform);
            var scenarioPanel = CreateScenarioPanel(canvasGO.transform);
            var settingsPanel = CreateSettingsPanel(canvasGO.transform);
            var fadePanel = CreateFadePanel(canvasGO.transform);

            var mainMenuUI = canvasGO.AddComponent<MainMenuUI>();
            SetPrivateField(mainMenuUI, "_mainPanel", mainPanel);
            SetPrivateField(mainMenuUI, "_scenarioPanel", scenarioPanel);
            SetPrivateField(mainMenuUI, "_settingsPanel", settingsPanel);
            
            SetPrivateField(mainMenuUI, "_playButton", mainPanel.transform.Find("PlayButton")?.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_scenariosButton", mainPanel.transform.Find("ScenariosButton")?.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_settingsButton", mainPanel.transform.Find("SettingsButton")?.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_quitButton", mainPanel.transform.Find("QuitButton")?.GetComponent<Button>());
            SetPrivateField(mainMenuUI, "_settingsBackButton", settingsPanel.transform.Find("BackButton")?.GetComponent<Button>());

            var scenarioSelectUI = scenarioPanel.GetComponent<ScenarioSelectUI>();
            if (scenarioSelectUI != null)
            {
                SetPrivateField(scenarioSelectUI, "_mainMenu", mainMenuUI);
            }

            var sceneLoader = Object.FindFirstObjectByType<SceneLoader>();
            if (sceneLoader != null)
            {
                var fadePanelCG = fadePanel.GetComponent<CanvasGroup>();
                SetPrivateField(sceneLoader, "_fadePanel", fadePanelCG);
            }
        }

        private static void CreateBackground(Transform parent)
        {
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(parent, false);
            
            var rect = bgGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            
            var image = bgGO.AddComponent<Image>();
            image.color = new Color(0.1f, 0.06f, 0.05f);
        }

        private static GameObject CreateMainPanel(Transform parent)
        {
            var panelGO = new GameObject("MainPanel");
            panelGO.transform.SetParent(parent, false);
            
            var rect = panelGO.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(400, 500);
            rect.anchoredPosition = Vector2.zero;

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(panelGO.transform, false);
            var titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.sizeDelta = new Vector2(400, 120);
            titleRect.anchoredPosition = new Vector2(0, 50);
            
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "SHIELD WALL";
            titleText.fontSize = 72;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.91f, 0.88f, 0.82f);
            titleText.outlineWidth = 0.2f;
            titleText.outlineColor = new Color(0.17f, 0.1f, 0.06f);

            var layout = panelGO.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.padding = new RectOffset(0, 0, 150, 0);

            CreateMenuButton("PlayButton", panelGO.transform, "PLAY", new Color(0.24f, 0.36f, 0.24f));
            CreateMenuButton("ScenariosButton", panelGO.transform, "SCENARIOS", new Color(0.23f, 0.16f, 0.1f));
            CreateMenuButton("SettingsButton", panelGO.transform, "SETTINGS", new Color(0.23f, 0.16f, 0.1f));
            CreateMenuButton("QuitButton", panelGO.transform, "QUIT", new Color(0.36f, 0.14f, 0.14f));

            return panelGO;
        }

        private static GameObject CreateScenarioPanel(Transform parent)
        {
            var panelGO = new GameObject("ScenarioPanel");
            panelGO.transform.SetParent(parent, false);
            
            var rect = panelGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var bgImage = panelGO.AddComponent<Image>();
            bgImage.color = new Color(0.08f, 0.05f, 0.04f, 0.95f);

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(panelGO.transform, false);
            var titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 1);
            titleRect.anchorMax = new Vector2(0.5f, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.sizeDelta = new Vector2(400, 80);
            titleRect.anchoredPosition = new Vector2(0, -40);
            
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "SELECT SCENARIO";
            titleText.fontSize = 48;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.91f, 0.88f, 0.82f);

            var cardContainer = new GameObject("CardContainer");
            cardContainer.transform.SetParent(panelGO.transform, false);
            var containerRect = cardContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(900, 300);
            containerRect.anchoredPosition = Vector2.zero;
            
            var containerLayout = cardContainer.AddComponent<HorizontalLayoutGroup>();
            containerLayout.spacing = 30;
            containerLayout.childAlignment = TextAnchor.MiddleCenter;
            containerLayout.childControlWidth = false;
            containerLayout.childControlHeight = false;

            var backButton = CreateMenuButton("BackButton", panelGO.transform, "BACK", new Color(0.36f, 0.14f, 0.14f));
            var backRect = backButton.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.5f, 0);
            backRect.anchorMax = new Vector2(0.5f, 0);
            backRect.pivot = new Vector2(0.5f, 0);
            backRect.anchoredPosition = new Vector2(0, 60);

            var scenarioSelect = panelGO.AddComponent<ScenarioSelectUI>();
            SetPrivateField(scenarioSelect, "_cardContainer", containerRect);
            SetPrivateField(scenarioSelect, "_backButton", backButton.GetComponent<Button>());

            panelGO.SetActive(false);
            return panelGO;
        }

        private static GameObject CreateSettingsPanel(Transform parent)
        {
            var panelGO = new GameObject("SettingsPanel");
            panelGO.transform.SetParent(parent, false);
            
            var rect = panelGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var bgImage = panelGO.AddComponent<Image>();
            bgImage.color = new Color(0.08f, 0.05f, 0.04f, 0.95f);

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(panelGO.transform, false);
            var titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.5f);
            titleRect.anchorMax = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(400, 80);
            
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "SETTINGS\n(Coming Soon)";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.7f, 0.65f, 0.6f);

            var backButton = CreateMenuButton("BackButton", panelGO.transform, "BACK", new Color(0.36f, 0.14f, 0.14f));
            var backRect = backButton.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0.5f, 0);
            backRect.anchorMax = new Vector2(0.5f, 0);
            backRect.pivot = new Vector2(0.5f, 0);
            backRect.anchoredPosition = new Vector2(0, 60);

            panelGO.SetActive(false);
            return panelGO;
        }

        private static GameObject CreateFadePanel(Transform parent)
        {
            var panelGO = new GameObject("FadePanel");
            panelGO.transform.SetParent(parent, false);
            
            var rect = panelGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            var image = panelGO.AddComponent<Image>();
            image.color = Color.black;

            var canvasGroup = panelGO.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            return panelGO;
        }

        private static GameObject CreateMenuButton(string name, Transform parent, string text, Color bgColor)
        {
            var buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            var rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 60);
            
            var image = buttonGO.AddComponent<Image>();
            image.color = bgColor;
            
            var button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;
            
            var colors = button.colors;
            colors.normalColor = bgColor;
            colors.highlightedColor = new Color(bgColor.r + 0.1f, bgColor.g + 0.1f, bgColor.b + 0.1f);
            colors.pressedColor = new Color(bgColor.r - 0.1f, bgColor.g - 0.1f, bgColor.b - 0.1f);
            button.colors = colors;

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 28;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.91f, 0.88f, 0.82f);

            var layoutElem = buttonGO.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = 300;
            layoutElem.preferredHeight = 60;

            return buttonGO;
        }

        private static void CreateScenarioCardPrefab()
        {
            string path = "Assets/Prefabs/UI/ScenarioCard.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                Debug.Log("ScenarioCard prefab already exists.");
                return;
            }

            var cardGO = new GameObject("ScenarioCard");
            
            var rect = cardGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280, 280);

            var image = cardGO.AddComponent<Image>();
            image.color = new Color(0.2f, 0.15f, 0.1f, 0.95f);

            var nameTextGO = new GameObject("NameText");
            nameTextGO.transform.SetParent(cardGO.transform, false);
            var nameRect = nameTextGO.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.75f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.sizeDelta = Vector2.zero;
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, -10);
            var nameText = nameTextGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Scenario Name";
            nameText.fontSize = 24;
            nameText.fontStyle = FontStyles.Bold;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = new Color(0.91f, 0.88f, 0.82f);

            var descTextGO = new GameObject("DescriptionText");
            descTextGO.transform.SetParent(cardGO.transform, false);
            var descRect = descTextGO.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.35f);
            descRect.anchorMax = new Vector2(1, 0.75f);
            descRect.sizeDelta = Vector2.zero;
            descRect.offsetMin = new Vector2(10, 5);
            descRect.offsetMax = new Vector2(-10, -5);
            var descText = descTextGO.AddComponent<TextMeshProUGUI>();
            descText.text = "Scenario description goes here.";
            descText.fontSize = 16;
            descText.alignment = TextAlignmentOptions.Center;
            descText.color = new Color(0.7f, 0.65f, 0.6f);

            var diffTextGO = new GameObject("DifficultyText");
            diffTextGO.transform.SetParent(cardGO.transform, false);
            var diffRect = diffTextGO.AddComponent<RectTransform>();
            diffRect.anchorMin = new Vector2(0, 0.2f);
            diffRect.anchorMax = new Vector2(0.5f, 0.35f);
            diffRect.sizeDelta = Vector2.zero;
            diffRect.offsetMin = new Vector2(10, 0);
            diffRect.offsetMax = new Vector2(-5, 0);
            var diffText = diffTextGO.AddComponent<TextMeshProUGUI>();
            diffText.text = "Normal";
            diffText.fontSize = 18;
            diffText.fontStyle = FontStyles.Bold;
            diffText.alignment = TextAlignmentOptions.Left;
            diffText.color = new Color(0.79f, 0.64f, 0.15f);

            var waveTextGO = new GameObject("WaveCountText");
            waveTextGO.transform.SetParent(cardGO.transform, false);
            var waveRect = waveTextGO.AddComponent<RectTransform>();
            waveRect.anchorMin = new Vector2(0.5f, 0.2f);
            waveRect.anchorMax = new Vector2(1, 0.35f);
            waveRect.sizeDelta = Vector2.zero;
            waveRect.offsetMin = new Vector2(5, 0);
            waveRect.offsetMax = new Vector2(-10, 0);
            var waveText = waveTextGO.AddComponent<TextMeshProUGUI>();
            waveText.text = "3 Waves";
            waveText.fontSize = 16;
            waveText.alignment = TextAlignmentOptions.Right;
            waveText.color = new Color(0.7f, 0.65f, 0.6f);

            var selectBtnGO = new GameObject("SelectButton");
            selectBtnGO.transform.SetParent(cardGO.transform, false);
            var btnRect = selectBtnGO.AddComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.1f, 0.02f);
            btnRect.anchorMax = new Vector2(0.9f, 0.18f);
            btnRect.sizeDelta = Vector2.zero;
            
            var btnImage = selectBtnGO.AddComponent<Image>();
            btnImage.color = new Color(0.24f, 0.36f, 0.24f);
            
            var selectBtn = selectBtnGO.AddComponent<Button>();
            selectBtn.targetGraphic = btnImage;

            var btnTextGO = new GameObject("Text");
            btnTextGO.transform.SetParent(selectBtnGO.transform, false);
            var btnTextRect = btnTextGO.AddComponent<RectTransform>();
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.sizeDelta = Vector2.zero;
            var btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
            btnText.text = "SELECT";
            btnText.fontSize = 20;
            btnText.fontStyle = FontStyles.Bold;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.white;

            var lockedOverlay = new GameObject("LockedOverlay");
            lockedOverlay.transform.SetParent(cardGO.transform, false);
            var lockRect = lockedOverlay.AddComponent<RectTransform>();
            lockRect.anchorMin = Vector2.zero;
            lockRect.anchorMax = Vector2.one;
            lockRect.sizeDelta = Vector2.zero;
            var lockImage = lockedOverlay.AddComponent<Image>();
            lockImage.color = new Color(0, 0, 0, 0.7f);
            
            var lockTextGO = new GameObject("LockText");
            lockTextGO.transform.SetParent(lockedOverlay.transform, false);
            var lockTextRect = lockTextGO.AddComponent<RectTransform>();
            lockTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            lockTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            lockTextRect.sizeDelta = new Vector2(200, 50);
            var lockText = lockTextGO.AddComponent<TextMeshProUGUI>();
            lockText.text = "LOCKED";
            lockText.fontSize = 32;
            lockText.fontStyle = FontStyles.Bold;
            lockText.alignment = TextAlignmentOptions.Center;
            lockText.color = new Color(0.5f, 0.5f, 0.5f);
            
            lockedOverlay.SetActive(false);

            var scenarioCard = cardGO.AddComponent<ScenarioCardUI>();
            SetPrivateField(scenarioCard, "_nameText", nameText);
            SetPrivateField(scenarioCard, "_descriptionText", descText);
            SetPrivateField(scenarioCard, "_difficultyText", diffText);
            SetPrivateField(scenarioCard, "_waveCountText", waveText);
            SetPrivateField(scenarioCard, "_selectButton", selectBtn);
            SetPrivateField(scenarioCard, "_lockedOverlay", lockedOverlay);

            PrefabUtility.SaveAsPrefabAsset(cardGO, path);
            Object.DestroyImmediate(cardGO);
            
            Debug.Log("ScenarioCard prefab created.");
        }

        private static void CreatePauseMenuPrefab()
        {
            string path = "Assets/Prefabs/UI/PauseMenuCanvas.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                Debug.Log("PauseMenuCanvas prefab already exists.");
                return;
            }

            var canvasGO = new GameObject("PauseMenuCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();

            var pausePanel = new GameObject("PausePanel");
            pausePanel.transform.SetParent(canvasGO.transform, false);
            var panelRect = pausePanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            
            var panelImage = pausePanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.85f);

            var contentPanel = new GameObject("ContentPanel");
            contentPanel.transform.SetParent(pausePanel.transform, false);
            var contentRect = contentPanel.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.5f, 0.5f);
            contentRect.anchorMax = new Vector2(0.5f, 0.5f);
            contentRect.sizeDelta = new Vector2(400, 400);
            
            var contentImage = contentPanel.AddComponent<Image>();
            contentImage.color = new Color(0.15f, 0.1f, 0.08f, 0.95f);

            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(contentPanel.transform, false);
            var titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.75f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = Vector2.zero;
            titleRect.offsetMin = new Vector2(0, 0);
            titleRect.offsetMax = new Vector2(0, -20);
            
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "PAUSED";
            titleText.fontSize = 48;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.91f, 0.88f, 0.82f);

            var buttonLayout = new GameObject("ButtonLayout");
            buttonLayout.transform.SetParent(contentPanel.transform, false);
            var layoutRect = buttonLayout.AddComponent<RectTransform>();
            layoutRect.anchorMin = new Vector2(0.5f, 0.1f);
            layoutRect.anchorMax = new Vector2(0.5f, 0.7f);
            layoutRect.sizeDelta = new Vector2(300, 0);
            
            var layout = buttonLayout.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            var resumeBtn = CreatePauseButton("ResumeButton", buttonLayout.transform, "RESUME", new Color(0.24f, 0.36f, 0.24f));
            var restartBtn = CreatePauseButton("RestartButton", buttonLayout.transform, "RESTART", new Color(0.36f, 0.28f, 0.15f));
            var menuBtn = CreatePauseButton("MainMenuButton", buttonLayout.transform, "MAIN MENU", new Color(0.36f, 0.14f, 0.14f));

            var pauseMenuUI = canvasGO.AddComponent<PauseMenuUI>();
            SetPrivateField(pauseMenuUI, "_pausePanel", pausePanel);
            SetPrivateField(pauseMenuUI, "_resumeButton", resumeBtn.GetComponent<Button>());
            SetPrivateField(pauseMenuUI, "_restartButton", restartBtn.GetComponent<Button>());
            SetPrivateField(pauseMenuUI, "_mainMenuButton", menuBtn.GetComponent<Button>());

            pausePanel.SetActive(false);

            PrefabUtility.SaveAsPrefabAsset(canvasGO, path);
            Object.DestroyImmediate(canvasGO);
            
            Debug.Log("PauseMenuCanvas prefab created.");
        }

        private static GameObject CreatePauseButton(string name, Transform parent, string text, Color bgColor)
        {
            var buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            
            var rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280, 55);
            
            var image = buttonGO.AddComponent<Image>();
            image.color = bgColor;
            
            var button = buttonGO.AddComponent<Button>();
            button.targetGraphic = image;

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            var textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = new Color(0.91f, 0.88f, 0.82f);

            var layoutElem = buttonGO.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = 280;
            layoutElem.preferredHeight = 55;

            return buttonGO;
        }

        private static void AddScenesToBuildSettings()
        {
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Battle.unity", true)
            };
            
            EditorBuildSettings.scenes = scenes;
            Debug.Log("Build Settings updated: MainMenu (0), Battle (1)");
        }

        [MenuItem("Shield Wall/Setup/Add Pause Menu to Battle Scene")]
        public static void AddPauseMenuToBattle()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            if (!scene.IsValid())
            {
                Debug.LogError("Could not open Battle.unity!");
                return;
            }

            var existing = GameObject.Find("PauseMenuCanvas");
            if (existing != null)
            {
                Debug.Log("PauseMenuCanvas already exists in Battle scene.");
                return;
            }

            var prefabPath = "Assets/Prefabs/UI/PauseMenuCanvas.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogWarning("PauseMenuCanvas prefab not found. Creating it first...");
                CreatePauseMenuPrefab();
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }

            if (prefab != null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = "PauseMenuCanvas";
                
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                
                Debug.Log("PauseMenuCanvas added to Battle scene!");
            }
        }

        [MenuItem("Shield Wall/Setup/Wire ScenarioSelect Prefab")]
        public static void WireScenarioSelectPrefab()
        {
            var scenarioSelect = Object.FindFirstObjectByType<ScenarioSelectUI>();
            if (scenarioSelect == null)
            {
                Debug.LogWarning("ScenarioSelectUI not found in scene.");
                return;
            }

            var prefabPath = "Assets/Prefabs/UI/ScenarioCard.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab != null)
            {
                var cardComponent = prefab.GetComponent<ScenarioCardUI>();
                SetPrivateField(scenarioSelect, "_cardPrefab", cardComponent);
                EditorUtility.SetDirty(scenarioSelect);
                Debug.Log("ScenarioCardUI prefab wired to ScenarioSelectUI.");
            }

            var scenarios = LoadAllAssets<BattleScenarioSO>("Assets/ScriptableObjects/Waves");
            if (scenarios.Length > 0)
            {
                var scenarioList = new System.Collections.Generic.List<BattleScenarioSO>(scenarios);
                SetPrivateField(scenarioSelect, "_scenarios", scenarioList);
                EditorUtility.SetDirty(scenarioSelect);
                Debug.Log($"Assigned {scenarios.Length} scenarios to ScenarioSelectUI.");
            }
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            if (obj == null) return;
            
            var field = obj.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(obj, value);
                if (obj is Object unityObj)
                    EditorUtility.SetDirty(unityObj);
            }
        }

        private static T[] LoadAllAssets<T>(string folderPath) where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });
            var assets = new T[guids.Length];
            
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            
            return assets;
        }
    }
}

