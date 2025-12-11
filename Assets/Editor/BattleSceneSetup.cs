using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Combat;
using ShieldWall.Dice;
using ShieldWall.ShieldWall;
using ShieldWall.UI;
using ShieldWall.Data;

namespace ShieldWall.Editor
{
    public static class BattleSceneSetup
    {
        private static BattleManager _battleManager;
        private static TurnManager _turnManager;
        private static StaminaManager _staminaManager;
        private static DicePoolManager _dicePoolManager;
        private static ComboManager _comboManager;
        private static EnemyWaveController _waveController;
        private static CombatResolver _combatResolver;
        private static ActionSelectionManager _actionSelectionManager;
        private static ShieldWallManager _shieldWallManager;
        private static PlayerWarrior _playerWarrior;

        private static DiceUI _diceUI;
        private static ActionUI _actionUI;
        private static HealthUI _healthUI;
        private static StaminaUI _staminaUI;
        private static WallStatusUI _wallStatusUI;
        private static WaveUI _waveUI;
        private static GameOverUI _gameOverUI;

        [MenuItem("Shield Wall/Setup/Build Complete Battle Scene")]
        public static void BuildCompleteBattleScene()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            if (!scene.IsValid())
            {
                Debug.LogError("Could not open Battle.unity!");
                return;
            }

            Debug.Log("Building complete battle scene...");

            CreateManagerHierarchy();
            CreateUIPrefabs();
            CreateCanvasHierarchy();
            CreateFirstPersonArms();
            CreateEnemyVisuals();
            WireManagerReferences();
            AssignScriptableObjects();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("Battle scene setup complete! Press Play to test.");
        }

        [MenuItem("Shield Wall/Setup/Create First Person Arms")]
        public static void CreateFirstPersonArms()
        {
            var existing = GameObject.Find("PlayerArms");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            var playerArms = new GameObject("PlayerArms");
            playerArms.layer = 6;

            var shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shield.name = "Shield";
            shield.transform.SetParent(playerArms.transform);
            shield.transform.localPosition = new Vector3(-0.3f, -0.3f, 0.8f);
            shield.transform.localScale = new Vector3(0.6f, 0.8f, 0.05f);
            shield.transform.localRotation = Quaternion.Euler(0, 15f, 0);
            shield.layer = 6;

            var leftArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            leftArm.name = "LeftArm";
            leftArm.transform.SetParent(playerArms.transform);
            leftArm.transform.localPosition = new Vector3(-0.25f, -0.4f, 0.6f);
            leftArm.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
            leftArm.transform.localRotation = Quaternion.Euler(45f, 0, 0);
            leftArm.layer = 6;

            var rightArm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            rightArm.name = "RightArm";
            rightArm.transform.SetParent(playerArms.transform);
            rightArm.transform.localPosition = new Vector3(0.3f, -0.5f, 0.5f);
            rightArm.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
            rightArm.transform.localRotation = Quaternion.Euler(60f, 0, 0);
            rightArm.layer = 6;

            var weapon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            weapon.name = "Axe";
            weapon.transform.SetParent(rightArm.transform);
            weapon.transform.localPosition = new Vector3(0, 0.5f, 0);
            weapon.transform.localScale = new Vector3(0.3f, 0.5f, 0.1f);
            weapon.layer = 6;

            Debug.Log("First-person arms created. Assign to PlayerView layer (6).");
        }

        [MenuItem("Shield Wall/Setup/Create Enemy Visuals")]
        public static void CreateEnemyVisuals()
        {
            var existing = GameObject.Find("EnemySpawnPoint");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            var spawnPoint = new GameObject("EnemySpawnPoint");
            spawnPoint.transform.position = new Vector3(0, 0, 5f);
            
            for (int i = 0; i < 5; i++)
            {
                var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemy.name = $"EnemyPlaceholder_{i}";
                enemy.transform.SetParent(spawnPoint.transform);
                enemy.transform.localPosition = new Vector3((i - 2) * 1.5f, 1f, 0);
                enemy.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
                enemy.layer = 8;
                enemy.SetActive(false);

                var renderer = enemy.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    mat.color = new Color(0.5f, 0.2f, 0.2f);
                    renderer.sharedMaterial = mat;
                }
            }

            Debug.Log("Enemy visual placeholders created.");
        }

        [MenuItem("Shield Wall/Setup/1. Create Manager Hierarchy")]
        public static void CreateManagerHierarchy()
        {
            var gameManagerGO = CreateOrFind("GameManager");
            AddComponent<BattleBootstrapper>(gameManagerGO);
            _battleManager = AddComponent<BattleManager>(gameManagerGO);
            _turnManager = AddComponent<TurnManager>(gameManagerGO);
            _staminaManager = AddComponent<StaminaManager>(gameManagerGO);

            var diceManagerGO = CreateOrFind("DiceManager");
            _dicePoolManager = AddComponent<DicePoolManager>(diceManagerGO);
            _comboManager = AddComponent<ComboManager>(diceManagerGO);

            var combatManagerGO = CreateOrFind("CombatManager");
            _waveController = AddComponent<EnemyWaveController>(combatManagerGO);
            _combatResolver = AddComponent<CombatResolver>(combatManagerGO);
            _actionSelectionManager = AddComponent<ActionSelectionManager>(combatManagerGO);

            var shieldWallGO = CreateOrFind("ShieldWall");
            _shieldWallManager = AddComponent<ShieldWallManager>(shieldWallGO);
            _playerWarrior = AddComponent<PlayerWarrior>(shieldWallGO);

            Debug.Log("Manager hierarchy created.");
        }

        [MenuItem("Shield Wall/Setup/2. Create UI Prefabs")]
        public static void CreateUIPrefabs()
        {
            CreateDieVisualPrefab();
            CreateActionButtonPrefab();
            CreateHealthHeartPrefab();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("UI prefabs created in Assets/Prefabs/UI/");
        }

        private static void CreateDieVisualPrefab()
        {
            string path = "Assets/Prefabs/UI/DieVisual.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                Debug.Log("DieVisual prefab already exists.");
                return;
            }

            var go = new GameObject("DieVisual");
            
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80, 80);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var runeTextGO = new GameObject("RuneText");
            runeTextGO.transform.SetParent(go.transform, false);
            var runeRect = runeTextGO.AddComponent<RectTransform>();
            runeRect.anchorMin = Vector2.zero;
            runeRect.anchorMax = Vector2.one;
            runeRect.sizeDelta = Vector2.zero;
            var runeText = runeTextGO.AddComponent<TextMeshProUGUI>();
            runeText.text = "ᚦ";
            runeText.fontSize = 48;
            runeText.alignment = TextAlignmentOptions.Center;
            runeText.color = Color.white;

            var lockOverlayGO = new GameObject("LockOverlay");
            lockOverlayGO.transform.SetParent(go.transform, false);
            var lockRect = lockOverlayGO.AddComponent<RectTransform>();
            lockRect.anchorMin = Vector2.zero;
            lockRect.anchorMax = Vector2.one;
            lockRect.sizeDelta = Vector2.zero;
            var lockImage = lockOverlayGO.AddComponent<Image>();
            lockImage.color = new Color(1f, 0.8f, 0f, 0.3f);
            lockOverlayGO.SetActive(false);

            var dieVisual = go.AddComponent<DieVisual>();
            
            SetPrivateField(dieVisual, "_button", button);
            SetPrivateField(dieVisual, "_backgroundImage", image);
            SetPrivateField(dieVisual, "_runeText", runeText);
            SetPrivateField(dieVisual, "_lockOverlay", lockImage);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateActionButtonPrefab()
        {
            string path = "Assets/Prefabs/UI/ActionButton.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                Debug.Log("ActionButton prefab already exists.");
                return;
            }

            var go = new GameObject("ActionButton");
            
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(120, 60);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.4f, 0.3f, 0.2f);

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var nameTextGO = new GameObject("NameText");
            nameTextGO.transform.SetParent(go.transform, false);
            var nameRect = nameTextGO.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.5f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.sizeDelta = Vector2.zero;
            nameRect.offsetMin = new Vector2(5, 0);
            nameRect.offsetMax = new Vector2(-5, -5);
            var nameText = nameTextGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Action";
            nameText.fontSize = 16;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;

            var runeTextGO = new GameObject("RuneText");
            runeTextGO.transform.SetParent(go.transform, false);
            var runeRect = runeTextGO.AddComponent<RectTransform>();
            runeRect.anchorMin = new Vector2(0, 0);
            runeRect.anchorMax = new Vector2(1, 0.5f);
            runeRect.sizeDelta = Vector2.zero;
            runeRect.offsetMin = new Vector2(5, 5);
            runeRect.offsetMax = new Vector2(-5, 0);
            var runeText = runeTextGO.AddComponent<TextMeshProUGUI>();
            runeText.text = "ᚦᚦ";
            runeText.fontSize = 20;
            runeText.alignment = TextAlignmentOptions.Center;
            runeText.color = new Color(0.8f, 0.7f, 0.5f);

            var actionButton = go.AddComponent<ActionButton>();
            
            SetPrivateField(actionButton, "_button", button);
            SetPrivateField(actionButton, "_nameText", nameText);
            SetPrivateField(actionButton, "_runeRequirementText", runeText);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateHealthHeartPrefab()
        {
            string path = "Assets/Prefabs/UI/HealthHeart.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                Debug.Log("HealthHeart prefab already exists.");
                return;
            }

            var go = new GameObject("HealthHeart");
            
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(24, 24);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.55f, 0.13f, 0.13f);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        [MenuItem("Shield Wall/Setup/3. Create Canvas Hierarchy")]
        public static void CreateCanvasHierarchy()
        {
            var existingCanvas = Object.FindFirstObjectByType<Canvas>();
            if (existingCanvas != null)
            {
                Object.DestroyImmediate(existingCanvas.gameObject);
            }

            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var canvasScaler = canvasGO.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            CreateTopBar(canvasGO.transform);
            CreateDicePanel(canvasGO.transform);
            CreateActionsPanel(canvasGO.transform);
            CreateBottomBar(canvasGO.transform);
            CreateEndTurnButton(canvasGO.transform);
            CreateGameOverPanel(canvasGO.transform);

            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Debug.Log("Canvas hierarchy created.");
        }

        private static void CreateTopBar(Transform parent)
        {
            var topBar = CreateUIPanel("TopBar", parent);
            var topRect = topBar.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0, 1);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.pivot = new Vector2(0.5f, 1);
            topRect.sizeDelta = new Vector2(0, 60);
            topRect.anchoredPosition = Vector2.zero;

            var topImage = topBar.GetComponent<Image>();
            topImage.color = new Color(0.15f, 0.1f, 0.08f, 0.9f);

            var topLayout = topBar.AddComponent<HorizontalLayoutGroup>();
            topLayout.padding = new RectOffset(20, 20, 10, 10);
            topLayout.spacing = 20;
            topLayout.childAlignment = TextAnchor.MiddleCenter;
            topLayout.childControlWidth = false;
            topLayout.childControlHeight = true;

            var waveTextGO = new GameObject("WaveText");
            waveTextGO.transform.SetParent(topBar.transform, false);
            var waveRect = waveTextGO.AddComponent<RectTransform>();
            waveRect.sizeDelta = new Vector2(200, 40);
            var waveText = waveTextGO.AddComponent<TextMeshProUGUI>();
            waveText.text = "Wave 1/3";
            waveText.fontSize = 28;
            waveText.alignment = TextAlignmentOptions.Left;
            waveText.color = new Color(0.83f, 0.78f, 0.72f);
            _waveUI = waveTextGO.AddComponent<WaveUI>();
            SetPrivateField(_waveUI, "_waveText", waveText);

            var enemyPanel = CreateUIPanel("EnemyPanel", topBar.transform);
            var enemyRect = enemyPanel.GetComponent<RectTransform>();
            enemyRect.sizeDelta = new Vector2(400, 40);
            enemyPanel.GetComponent<Image>().color = new Color(0.3f, 0.15f, 0.15f, 0.8f);
            
            var enemyLayout = enemyPanel.AddComponent<HorizontalLayoutGroup>();
            enemyLayout.spacing = 10;
            enemyLayout.padding = new RectOffset(10, 10, 5, 5);
            enemyLayout.childAlignment = TextAnchor.MiddleCenter;
            enemyLayout.childControlWidth = false;
            enemyLayout.childControlHeight = false;
            
            var enemyUI = enemyPanel.AddComponent<EnemyUI>();
            SetPrivateField(enemyUI, "_enemyContainer", enemyPanel.transform);

            var spacer = new GameObject("Spacer");
            spacer.transform.SetParent(topBar.transform, false);
            var spacerRect = spacer.AddComponent<RectTransform>();
            spacerRect.sizeDelta = new Vector2(400, 40);
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            var staminaPanel = CreateUIPanel("StaminaPanel", topBar.transform);
            var staminaRect = staminaPanel.GetComponent<RectTransform>();
            staminaRect.sizeDelta = new Vector2(250, 40);
            staminaPanel.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var staminaTextGO = new GameObject("StaminaText");
            staminaTextGO.transform.SetParent(staminaPanel.transform, false);
            var staminaTextRect = staminaTextGO.AddComponent<RectTransform>();
            staminaTextRect.anchorMin = Vector2.zero;
            staminaTextRect.anchorMax = Vector2.one;
            staminaTextRect.sizeDelta = Vector2.zero;
            var staminaText = staminaTextGO.AddComponent<TextMeshProUGUI>();
            staminaText.text = "Stamina: 12/12";
            staminaText.fontSize = 22;
            staminaText.alignment = TextAlignmentOptions.Center;
            staminaText.color = new Color(0.24f, 0.35f, 0.43f);

            _staminaUI = staminaPanel.AddComponent<StaminaUI>();
            SetPrivateField(_staminaUI, "_staminaText", staminaText);
        }

        private static void CreateDicePanel(Transform parent)
        {
            var dicePanel = CreateUIPanel("DicePanel", parent);
            var diceRect = dicePanel.GetComponent<RectTransform>();
            diceRect.anchorMin = new Vector2(0.5f, 0);
            diceRect.anchorMax = new Vector2(0.5f, 0);
            diceRect.pivot = new Vector2(0.5f, 0);
            diceRect.sizeDelta = new Vector2(500, 120);
            diceRect.anchoredPosition = new Vector2(0, 180);

            dicePanel.GetComponent<Image>().color = new Color(0.2f, 0.15f, 0.1f, 0.9f);

            var diceContainer = new GameObject("DiceContainer");
            diceContainer.transform.SetParent(dicePanel.transform, false);
            var containerRect = diceContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0, 0.3f);
            containerRect.anchorMax = new Vector2(1, 1);
            containerRect.sizeDelta = Vector2.zero;
            containerRect.offsetMin = new Vector2(10, 0);
            containerRect.offsetMax = new Vector2(-10, -10);

            var layout = diceContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var rerollButton = CreateButton("RerollButton", dicePanel.transform, "Reroll (2)");
            var rerollRect = rerollButton.GetComponent<RectTransform>();
            rerollRect.anchorMin = new Vector2(0.5f, 0);
            rerollRect.anchorMax = new Vector2(0.5f, 0);
            rerollRect.pivot = new Vector2(0.5f, 0);
            rerollRect.sizeDelta = new Vector2(120, 30);
            rerollRect.anchoredPosition = new Vector2(0, 5);

            _diceUI = dicePanel.AddComponent<DiceUI>();
            
            var diePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/DieVisual.prefab");
            if (diePrefab != null)
            {
                var dieVisualComponent = diePrefab.GetComponent<DieVisual>();
                SetPrivateField(_diceUI, "_dieVisualPrefab", dieVisualComponent);
            }
            SetPrivateField(_diceUI, "_diceContainer", containerRect);
            SetPrivateField(_diceUI, "_rerollButton", rerollButton.GetComponent<Button>());
            
            var rerollText = rerollButton.GetComponentInChildren<TextMeshProUGUI>();
            SetPrivateField(_diceUI, "_rerollCountText", rerollText);
        }

        private static void CreateActionsPanel(Transform parent)
        {
            var actionsPanel = CreateUIPanel("ActionsPanel", parent);
            var actionsRect = actionsPanel.GetComponent<RectTransform>();
            actionsRect.anchorMin = new Vector2(0.5f, 0);
            actionsRect.anchorMax = new Vector2(0.5f, 0);
            actionsRect.pivot = new Vector2(0.5f, 0);
            actionsRect.sizeDelta = new Vector2(700, 80);
            actionsRect.anchoredPosition = new Vector2(0, 310);

            actionsPanel.GetComponent<Image>().color = new Color(0.25f, 0.2f, 0.15f, 0.9f);

            var actionsContainer = new GameObject("ActionsContainer");
            actionsContainer.transform.SetParent(actionsPanel.transform, false);
            var containerRect = actionsContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.sizeDelta = Vector2.zero;
            containerRect.offsetMin = new Vector2(10, 10);
            containerRect.offsetMax = new Vector2(-10, -10);

            var layout = actionsContainer.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            _actionUI = actionsPanel.AddComponent<ActionUI>();
            
            var actionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/ActionButton.prefab");
            if (actionPrefab != null)
            {
                var actionButtonComponent = actionPrefab.GetComponent<ActionButton>();
                SetPrivateField(_actionUI, "_actionButtonPrefab", actionButtonComponent);
            }
            SetPrivateField(_actionUI, "_actionContainer", containerRect);
        }

        private static void CreateBottomBar(Transform parent)
        {
            var bottomBar = CreateUIPanel("BottomBar", parent);
            var bottomRect = bottomBar.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0, 0);
            bottomRect.anchorMax = new Vector2(1, 0);
            bottomRect.pivot = new Vector2(0.5f, 0);
            bottomRect.sizeDelta = new Vector2(0, 100);
            bottomRect.anchoredPosition = Vector2.zero;

            bottomBar.GetComponent<Image>().color = new Color(0.12f, 0.08f, 0.06f, 0.95f);

            var bottomLayout = bottomBar.AddComponent<HorizontalLayoutGroup>();
            bottomLayout.padding = new RectOffset(20, 20, 10, 10);
            bottomLayout.spacing = 20;
            bottomLayout.childAlignment = TextAnchor.MiddleCenter;

            CreatePlayerHealthPanel(bottomBar.transform);
        }

        private static void CreatePlayerHealthPanel(Transform parent)
        {
            var healthPanel = CreateUIPanel("PlayerHealthPanel", parent);
            healthPanel.GetComponent<Image>().color = new Color(0.2f, 0.15f, 0.1f, 0.8f);
            
            var healthRect = healthPanel.GetComponent<RectTransform>();
            healthRect.sizeDelta = new Vector2(200, 60);

            var layoutElem = healthPanel.AddComponent<LayoutElement>();
            layoutElem.preferredWidth = 200;
            layoutElem.preferredHeight = 60;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(healthPanel.transform, false);
            var labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0, 1);
            labelRect.pivot = new Vector2(0, 0.5f);
            labelRect.sizeDelta = new Vector2(80, 0);
            labelRect.anchoredPosition = new Vector2(10, 0);
            var labelText = labelGO.AddComponent<TextMeshProUGUI>();
            labelText.text = "Health:";
            labelText.fontSize = 18;
            labelText.alignment = TextAlignmentOptions.Left;
            labelText.color = new Color(0.83f, 0.78f, 0.72f);

            var heartsContainer = new GameObject("HeartsContainer");
            heartsContainer.transform.SetParent(healthPanel.transform, false);
            var heartsRect = heartsContainer.AddComponent<RectTransform>();
            heartsRect.anchorMin = new Vector2(0, 0);
            heartsRect.anchorMax = new Vector2(1, 0.5f);
            heartsRect.sizeDelta = Vector2.zero;
            heartsRect.offsetMin = new Vector2(10, 5);
            heartsRect.offsetMax = new Vector2(-10, 0);

            var heartsLayout = heartsContainer.AddComponent<HorizontalLayoutGroup>();
            heartsLayout.spacing = 5;
            heartsLayout.childAlignment = TextAnchor.MiddleLeft;
            heartsLayout.childControlWidth = false;
            heartsLayout.childControlHeight = false;

            _healthUI = healthPanel.AddComponent<HealthUI>();
            SetPrivateField(_healthUI, "_heartsContainer", heartsRect);
            
            var heartPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/HealthHeart.prefab");
            if (heartPrefab != null)
            {
                var heartImage = heartPrefab.GetComponent<Image>();
                SetPrivateField(_healthUI, "_heartPrefab", heartImage);
            }
        }

        private static void CreateEndTurnButton(Transform parent)
        {
            var endTurnButton = CreateButton("EndTurnButton", parent, "END TURN");
            var endTurnRect = endTurnButton.GetComponent<RectTransform>();
            endTurnRect.anchorMin = new Vector2(1, 0);
            endTurnRect.anchorMax = new Vector2(1, 0);
            endTurnRect.pivot = new Vector2(1, 0);
            endTurnRect.sizeDelta = new Vector2(150, 50);
            endTurnRect.anchoredPosition = new Vector2(-20, 120);

            var buttonImage = endTurnButton.GetComponent<Image>();
            buttonImage.color = new Color(0.55f, 0.13f, 0.13f);

            if (_actionSelectionManager == null)
            {
                _actionSelectionManager = Object.FindFirstObjectByType<ActionSelectionManager>();
            }
            
            if (_actionSelectionManager != null)
            {
                SetPrivateField(_actionSelectionManager, "_endTurnButton", endTurnButton.GetComponent<Button>());
            }
        }

        private static void CreateGameOverPanel(Transform parent)
        {
            var gameOverPanel = CreateUIPanel("GameOverPanel", parent);
            var goRect = gameOverPanel.GetComponent<RectTransform>();
            goRect.anchorMin = Vector2.zero;
            goRect.anchorMax = Vector2.one;
            goRect.sizeDelta = Vector2.zero;

            gameOverPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
            gameOverPanel.SetActive(false);

            var victoryPanel = CreateUIPanel("VictoryPanel", gameOverPanel.transform);
            var victoryRect = victoryPanel.GetComponent<RectTransform>();
            victoryRect.anchorMin = new Vector2(0.5f, 0.5f);
            victoryRect.anchorMax = new Vector2(0.5f, 0.5f);
            victoryRect.sizeDelta = new Vector2(400, 200);
            victoryPanel.GetComponent<Image>().color = new Color(0.2f, 0.4f, 0.2f, 0.95f);

            var victoryTitleGO = new GameObject("VictoryTitle");
            victoryTitleGO.transform.SetParent(victoryPanel.transform, false);
            var vtRect = victoryTitleGO.AddComponent<RectTransform>();
            vtRect.anchorMin = new Vector2(0, 0.6f);
            vtRect.anchorMax = new Vector2(1, 1);
            vtRect.sizeDelta = Vector2.zero;
            var victoryTitle = victoryTitleGO.AddComponent<TextMeshProUGUI>();
            victoryTitle.text = "VICTORY!";
            victoryTitle.fontSize = 48;
            victoryTitle.alignment = TextAlignmentOptions.Center;
            victoryTitle.color = new Color(0.79f, 0.64f, 0.15f);

            var victoryRestartBtn = CreateButton("RestartButton", victoryPanel.transform, "Fight Again");
            var vrRect = victoryRestartBtn.GetComponent<RectTransform>();
            vrRect.anchorMin = new Vector2(0.5f, 0);
            vrRect.anchorMax = new Vector2(0.5f, 0);
            vrRect.pivot = new Vector2(0.5f, 0);
            vrRect.sizeDelta = new Vector2(150, 40);
            vrRect.anchoredPosition = new Vector2(0, 20);

            var defeatPanel = CreateUIPanel("DefeatPanel", gameOverPanel.transform);
            var defeatRect = defeatPanel.GetComponent<RectTransform>();
            defeatRect.anchorMin = new Vector2(0.5f, 0.5f);
            defeatRect.anchorMax = new Vector2(0.5f, 0.5f);
            defeatRect.sizeDelta = new Vector2(400, 200);
            defeatPanel.GetComponent<Image>().color = new Color(0.4f, 0.15f, 0.15f, 0.95f);
            defeatPanel.SetActive(false);

            var defeatTitleGO = new GameObject("DefeatTitle");
            defeatTitleGO.transform.SetParent(defeatPanel.transform, false);
            var dtRect = defeatTitleGO.AddComponent<RectTransform>();
            dtRect.anchorMin = new Vector2(0, 0.6f);
            dtRect.anchorMax = new Vector2(1, 1);
            dtRect.sizeDelta = Vector2.zero;
            var defeatTitle = defeatTitleGO.AddComponent<TextMeshProUGUI>();
            defeatTitle.text = "DEFEAT";
            defeatTitle.fontSize = 48;
            defeatTitle.alignment = TextAlignmentOptions.Center;
            defeatTitle.color = new Color(0.8f, 0.2f, 0.2f);

            var defeatCauseGO = new GameObject("DefeatCause");
            defeatCauseGO.transform.SetParent(defeatPanel.transform, false);
            var dcRect = defeatCauseGO.AddComponent<RectTransform>();
            dcRect.anchorMin = new Vector2(0, 0.3f);
            dcRect.anchorMax = new Vector2(1, 0.6f);
            dcRect.sizeDelta = Vector2.zero;
            var defeatCause = defeatCauseGO.AddComponent<TextMeshProUGUI>();
            defeatCause.text = "The shield wall has fallen.";
            defeatCause.fontSize = 20;
            defeatCause.alignment = TextAlignmentOptions.Center;
            defeatCause.color = Color.white;

            var defeatRestartBtn = CreateButton("RestartButton", defeatPanel.transform, "Try Again");
            var drRect = defeatRestartBtn.GetComponent<RectTransform>();
            drRect.anchorMin = new Vector2(0.5f, 0);
            drRect.anchorMax = new Vector2(0.5f, 0);
            drRect.pivot = new Vector2(0.5f, 0);
            drRect.sizeDelta = new Vector2(150, 40);
            drRect.anchoredPosition = new Vector2(0, 20);

            _gameOverUI = gameOverPanel.AddComponent<GameOverUI>();
            SetPrivateField(_gameOverUI, "_victoryPanel", victoryPanel);
            SetPrivateField(_gameOverUI, "_defeatPanel", defeatPanel);
            SetPrivateField(_gameOverUI, "_victoryTitleText", victoryTitle);
            SetPrivateField(_gameOverUI, "_defeatTitleText", defeatTitle);
            SetPrivateField(_gameOverUI, "_defeatCauseText", defeatCause);
            SetPrivateField(_gameOverUI, "_victoryRestartButton", victoryRestartBtn.GetComponent<Button>());
            SetPrivateField(_gameOverUI, "_defeatRestartButton", defeatRestartBtn.GetComponent<Button>());
        }

        [MenuItem("Shield Wall/Setup/4. Wire Manager References")]
        public static void WireManagerReferences()
        {
            _battleManager = Object.FindFirstObjectByType<BattleManager>();
            _turnManager = Object.FindFirstObjectByType<TurnManager>();
            _staminaManager = Object.FindFirstObjectByType<StaminaManager>();
            _dicePoolManager = Object.FindFirstObjectByType<DicePoolManager>();
            _comboManager = Object.FindFirstObjectByType<ComboManager>();
            _waveController = Object.FindFirstObjectByType<EnemyWaveController>();
            _combatResolver = Object.FindFirstObjectByType<CombatResolver>();
            _actionSelectionManager = Object.FindFirstObjectByType<ActionSelectionManager>();
            _shieldWallManager = Object.FindFirstObjectByType<ShieldWallManager>();
            _playerWarrior = Object.FindFirstObjectByType<PlayerWarrior>();
            _diceUI = Object.FindFirstObjectByType<DiceUI>();
            _waveUI = Object.FindFirstObjectByType<WaveUI>();
            _gameOverUI = Object.FindFirstObjectByType<GameOverUI>();

            if (_battleManager != null)
            {
                SetPrivateField(_battleManager, "_turnManager", _turnManager);
                SetPrivateField(_battleManager, "_staminaManager", _staminaManager);
            }

            if (_turnManager != null)
            {
                SetPrivateField(_turnManager, "_dicePoolManager", _dicePoolManager);
                SetPrivateField(_turnManager, "_comboManager", _comboManager);
                SetPrivateField(_turnManager, "_waveController", _waveController);
                SetPrivateField(_turnManager, "_combatResolver", _combatResolver);
                SetPrivateField(_turnManager, "_shieldWallManager", _shieldWallManager);
                SetPrivateField(_turnManager, "_staminaManager", _staminaManager);
                SetPrivateField(_turnManager, "_player", _playerWarrior);
            }

            if (_combatResolver != null)
            {
                SetPrivateField(_combatResolver, "_waveController", _waveController);
            }

            if (_shieldWallManager != null)
            {
                SetPrivateField(_shieldWallManager, "_player", _playerWarrior);
            }

            if (_comboManager != null)
            {
                SetPrivateField(_comboManager, "_dicePoolManager", _dicePoolManager);
            }

            if (_diceUI != null)
            {
                SetPrivateField(_diceUI, "_dicePoolManager", _dicePoolManager);
            }

            if (_waveUI != null)
            {
                SetPrivateField(_waveUI, "_waveController", _waveController);
            }

            if (_gameOverUI != null)
            {
                SetPrivateField(_gameOverUI, "_battleManager", _battleManager);
            }

            Debug.Log("Manager references wired.");
        }

        [MenuItem("Shield Wall/Setup/5. Assign ScriptableObjects")]
        public static void AssignScriptableObjects()
        {
            _comboManager = Object.FindFirstObjectByType<ComboManager>();
            _waveController = Object.FindFirstObjectByType<EnemyWaveController>();
            _shieldWallManager = Object.FindFirstObjectByType<ShieldWallManager>();
            _diceUI = Object.FindFirstObjectByType<DiceUI>();

            var actions = LoadAllAssets<ActionSO>("Assets/ScriptableObjects/Actions");
            var waves = LoadAllAssets<WaveConfigSO>("Assets/ScriptableObjects/Waves");
            var brothers = LoadAllAssets<ShieldBrotherSO>("Assets/ScriptableObjects/Brothers");
            var runes = LoadAllAssets<RuneSO>("Assets/ScriptableObjects/Runes");

            if (_comboManager != null && actions.Length > 0)
            {
                SetPrivateField(_comboManager, "_allActions", actions);
                Debug.Log($"Assigned {actions.Length} actions to ComboManager");
            }

            if (_waveController != null && waves.Length > 0)
            {
                SetPrivateField(_waveController, "_waveConfigs", waves);
                Debug.Log($"Assigned {waves.Length} waves to EnemyWaveController");
            }

            if (_shieldWallManager != null && brothers.Length > 0)
            {
                SetPrivateField(_shieldWallManager, "_brotherData", brothers);
                Debug.Log($"Assigned {brothers.Length} brothers to ShieldWallManager");
            }

            if (_diceUI != null && runes.Length > 0)
            {
                SetPrivateField(_diceUI, "_runeData", runes);
                Debug.Log($"Assigned {runes.Length} runes to DiceUI");
            }

            Debug.Log("ScriptableObject assets assigned.");
        }

        [MenuItem("Shield Wall/Scene Setup/Create Volume Profile")]
        public static void CreateVolumeProfile()
        {
            string profilePath = "Assets/Settings/BattleVolumeProfile.asset";
            
            var existingProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            if (existingProfile != null)
            {
                Debug.Log("BattleVolumeProfile already exists. Updating settings...");
                ConfigureVolumeProfile(existingProfile);
                EditorUtility.SetDirty(existingProfile);
                AssetDatabase.SaveAssets();
                return;
            }

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            ConfigureVolumeProfile(profile);
            
            AssetDatabase.CreateAsset(profile, profilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"Created BattleVolumeProfile at {profilePath}");
        }

        private static void ConfigureVolumeProfile(VolumeProfile profile)
        {
            if (!profile.Has<ColorAdjustments>())
                profile.Add<ColorAdjustments>();
            var colorGrading = profile.components.Find(c => c is ColorAdjustments) as ColorAdjustments;
            if (colorGrading != null)
            {
                colorGrading.active = true;
                colorGrading.saturation.Override(-15f);
                colorGrading.colorFilter.Override(new Color(0.95f, 0.97f, 1f));
                colorGrading.contrast.Override(10f);
            }

            if (!profile.Has<Vignette>())
                profile.Add<Vignette>();
            var vignette = profile.components.Find(c => c is Vignette) as Vignette;
            if (vignette != null)
            {
                vignette.active = true;
                vignette.intensity.Override(0.25f);
                vignette.smoothness.Override(0.4f);
                vignette.color.Override(new Color(0.16f, 0.09f, 0.06f));
            }

            if (!profile.Has<Bloom>())
                profile.Add<Bloom>();
            var bloom = profile.components.Find(c => c is Bloom) as Bloom;
            if (bloom != null)
            {
                bloom.active = true;
                bloom.intensity.Override(0.5f);
                bloom.threshold.Override(1.0f);
                bloom.scatter.Override(0.6f);
            }

            if (!profile.Has<FilmGrain>())
                profile.Add<FilmGrain>();
            var filmGrain = profile.components.Find(c => c is FilmGrain) as FilmGrain;
            if (filmGrain != null)
            {
                filmGrain.active = true;
                filmGrain.type.Override(FilmGrainLookup.Medium1);
                filmGrain.intensity.Override(0.1f);
            }
        }

        [MenuItem("Shield Wall/Scene Setup/Setup Battle Lighting")]
        public static void SetupBattleLighting()
        {
            var sun = Object.FindFirstObjectByType<Light>();
            if (sun == null || sun.type != LightType.Directional)
            {
                var sunGO = new GameObject("Directional Light");
                sun = sunGO.AddComponent<Light>();
                sun.type = LightType.Directional;
            }

            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            sun.intensity = 0.8f;
            sun.color = new Color(1f, 0.91f, 0.82f);
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = 0.6f;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f);

            Debug.Log("Battle lighting configured: Directional light and ambient settings applied.");
        }

        [MenuItem("Shield Wall/Scene Setup/Create Ground Plane")]
        public static void CreateGroundPlane()
        {
            string materialPath = "Assets/Art/Materials/Environment/Ground.mat";
            
            if (!System.IO.Directory.Exists("Assets/Art/Materials/Environment"))
            {
                System.IO.Directory.CreateDirectory("Assets/Art/Materials/Environment");
                AssetDatabase.Refresh();
            }

            Material groundMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (groundMat == null)
            {
                groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                groundMat.SetColor("_BaseColor", new Color(0.29f, 0.22f, 0.16f));
                groundMat.SetFloat("_Smoothness", 0.1f);
                groundMat.SetFloat("_Metallic", 0f);
                
                AssetDatabase.CreateAsset(groundMat, materialPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"Created Ground material at {materialPath}");
            }

            var existing = GameObject.Find("Ground");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(50f, 1f, 50f);
            ground.layer = 9;
            ground.isStatic = true;

            var renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = groundMat;
            }

            Debug.Log("Ground plane created with mud brown material.");
        }

        [MenuItem("Shield Wall/Scene Setup/Setup Layers")]
        public static void SetupLayers()
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            
            SerializedProperty layers = tagManager.FindProperty("layers");
            
            SetLayerName(layers, 6, "PlayerView");
            SetLayerName(layers, 7, "Brothers");
            SetLayerName(layers, 8, "Enemies");
            SetLayerName(layers, 9, "Environment");
            
            tagManager.ApplyModifiedProperties();
            
            Debug.Log("Layers configured: PlayerView(6), Brothers(7), Enemies(8), Environment(9)");
        }

        private static void SetLayerName(SerializedProperty layers, int index, string name)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(index);
            if (layer != null && string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = name;
            }
            else if (layer != null && layer.stringValue != name)
            {
                layer.stringValue = name;
            }
        }

        [MenuItem("Shield Wall/Scene Setup/Add Volume to Scene")]
        public static void AddVolumeToScene()
        {
            string profilePath = "Assets/Settings/BattleVolumeProfile.asset";
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            
            if (profile == null)
            {
                Debug.LogWarning("BattleVolumeProfile not found. Run 'Create Volume Profile' first.");
                CreateVolumeProfile();
                profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            }

            var existing = GameObject.Find("PostProcessVolume");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            var volumeGO = new GameObject("PostProcessVolume");
            var volume = volumeGO.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.profile = profile;

            Debug.Log("Post-process Volume added to scene with BattleVolumeProfile.");
        }

        [MenuItem("Shield Wall/Scene Setup/Apply Full Atmosphere")]
        public static void ApplyFullAtmosphere()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || scene.name != "Battle")
            {
                scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
                if (!scene.IsValid())
                {
                    Debug.LogError("Could not open Battle.unity!");
                    return;
                }
            }

            Debug.Log("Applying full atmosphere setup...");

            SetupLayers();
            CreateVolumeProfile();
            AddVolumeToScene();
            SetupBattleLighting();
            CreateGroundPlane();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("Full atmosphere setup complete! Scene saved.");
        }

        private static GameObject CreateOrFind(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;
            return new GameObject(name);
        }

        private static T AddComponent<T>(GameObject go) where T : Component
        {
            var existing = go.GetComponent<T>();
            if (existing != null) return existing;
            return go.AddComponent<T>();
        }

        private static GameObject CreateUIPanel(string name, Transform parent)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private static GameObject CreateButton(string name, Transform parent, string text)
        {
            var buttonGO = new GameObject(name);
            buttonGO.transform.SetParent(parent, false);
            var rect = buttonGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 40);
            
            var image = buttonGO.AddComponent<Image>();
            image.color = new Color(0.3f, 0.25f, 0.2f);
            
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
            tmp.fontSize = 18;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            return buttonGO;
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
                EditorUtility.SetDirty(obj as Object);
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

