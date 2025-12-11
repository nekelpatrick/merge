using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ShieldWall.Data;
using ShieldWall.Dice;
using ShieldWall.Core;
using ShieldWall.Combat;
using ShieldWall.ShieldWall;

namespace ShieldWall.Editor
{
    [InitializeOnLoad]
    public static class ShieldWallSetup
    {
        static ShieldWallSetup()
        {
            EditorApplication.delayCall += () =>
            {
                CreateMissingRuneAssets();
            };
        }

        private static void CreateMissingRuneAssets()
        {
            bool created = false;
            
            if (AssetDatabase.LoadAssetAtPath<RuneSO>("Assets/ScriptableObjects/Runes/Rune_Othala.asset") == null)
            {
                CreateRune(RuneType.Othala, "Odin", new Color(0.79f, 0.64f, 0.15f), "Wild card rune - can substitute any rune");
                created = true;
            }
            
            if (AssetDatabase.LoadAssetAtPath<RuneSO>("Assets/ScriptableObjects/Runes/Rune_Laguz.asset") == null)
            {
                CreateRune(RuneType.Laguz, "Loki", new Color(0.36f, 0.24f, 0.43f), "Chaos rune - unpredictable effects");
                created = true;
            }
            
            if (created)
            {
                AssetDatabase.SaveAssets();
                Debug.Log("Shield Wall: Created missing rune assets (Othala, Laguz)");
            }
        }

        [MenuItem("Shield Wall/Setup/Run Complete Setup")]
        public static void RunCompleteSetup()
        {
            CreateAllPlaceholderAssets();
            SetupBattleScene();
            SetupBattleManagers();
            Debug.Log("Shield Wall: Complete setup finished!");
        }

        [MenuItem("Shield Wall/Setup/Setup Battle Managers")]
        public static void SetupBattleManagers()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            if (!scene.IsValid())
            {
                Debug.LogError("Shield Wall: Could not open Battle.unity scene!");
                return;
            }

            var gameManager = CreateOrFindGameObject("GameManager");
            
            var battleManager = AddComponentIfMissing<BattleManager>(gameManager);
            var turnManager = AddComponentIfMissing<TurnManager>(gameManager);
            var staminaManager = AddComponentIfMissing<StaminaManager>(gameManager);

            var diceManager = CreateOrFindGameObject("DiceManager");
            var dicePoolManager = AddComponentIfMissing<DicePoolManager>(diceManager);
            var comboManager = AddComponentIfMissing<ComboManager>(diceManager);

            var combatManager = CreateOrFindGameObject("CombatManager");
            var waveController = AddComponentIfMissing<EnemyWaveController>(combatManager);
            var combatResolver = AddComponentIfMissing<CombatResolver>(combatManager);
            var actionSelectionManager = AddComponentIfMissing<ActionSelectionManager>(combatManager);

            var shieldWallGO = CreateOrFindGameObject("ShieldWall");
            var shieldWallManager = AddComponentIfMissing<ShieldWallManager>(shieldWallGO);
            var player = AddComponentIfMissing<PlayerWarrior>(shieldWallGO);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            Debug.Log("Shield Wall: Battle managers created! Please wire references in the Inspector.");
        }

        private static GameObject CreateOrFindGameObject(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;
            
            return new GameObject(name);
        }

        private static T AddComponentIfMissing<T>(GameObject go) where T : Component
        {
            var existing = go.GetComponent<T>();
            if (existing != null) return existing;
            
            return go.AddComponent<T>();
        }

        [MenuItem("Shield Wall/Setup/Create All Placeholder Assets")]
        public static void CreateAllPlaceholderAssets()
        {
            CreateRuneAssets();
            CreateBrotherAssets();
            CreateEnemyAssets();
            CreateActionAssets();
            CreateWaveAssets();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Shield Wall: All placeholder assets created successfully!");
        }

        [MenuItem("Shield Wall/Setup/Setup Battle Scene")]
        public static void SetupBattleScene()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Battle.unity");
            if (!scene.IsValid())
            {
                Debug.LogError("Shield Wall: Could not open Battle.unity scene!");
                return;
            }

            SetupCamera();
            SetupLighting();
            SetupGround();

            EditorSceneManager.SaveScene(scene);
            Debug.Log("Shield Wall: Battle scene configured!");
        }

        private static void SetupCamera()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var cameraGO = new GameObject("Main Camera");
                camera = cameraGO.AddComponent<Camera>();
                cameraGO.AddComponent<AudioListener>();
                cameraGO.tag = "MainCamera";
            }

            camera.transform.position = new Vector3(0f, 1.7f, 0f);
            camera.transform.rotation = Quaternion.identity;
            camera.fieldOfView = 70f;
            camera.nearClipPlane = 0.1f;
            camera.clearFlags = CameraClearFlags.Skybox;
        }

        private static void SetupLighting()
        {
            var existingLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            Light directionalLight = null;

            foreach (var light in existingLights)
            {
                if (light.type == LightType.Directional)
                {
                    directionalLight = light;
                    break;
                }
            }

            if (directionalLight == null)
            {
                var lightGO = new GameObject("Directional Light");
                directionalLight = lightGO.AddComponent<Light>();
                directionalLight.type = LightType.Directional;
            }

            directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            directionalLight.color = new Color(0.9f, 0.85f, 0.8f);
            directionalLight.intensity = 1f;
            directionalLight.shadows = LightShadows.Soft;
        }

        private static void SetupGround()
        {
            var existingGround = GameObject.Find("Ground");
            if (existingGround != null)
            {
                Debug.Log("Shield Wall: Ground already exists, skipping...");
                return;
            }

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
        }

        [MenuItem("Shield Wall/Setup/Create Rune Assets")]
        public static void CreateRuneAssets()
        {
            CreateRune(RuneType.Thurs, "Shield", new Color(0.36f, 0.36f, 0.36f), "Defense rune - protects against attacks");
            CreateRune(RuneType.Tyr, "Axe", new Color(0.55f, 0.13f, 0.13f), "Attack rune - strikes enemies");
            CreateRune(RuneType.Gebo, "Spear", new Color(0.55f, 0.41f, 0.08f), "Precision rune - targeted attacks");
            CreateRune(RuneType.Berkana, "Brace", new Color(0.24f, 0.36f, 0.24f), "Support rune - reinforcement");
            CreateRune(RuneType.Othala, "Odin", new Color(0.79f, 0.64f, 0.15f), "Wild card rune - can substitute any rune");
            CreateRune(RuneType.Laguz, "Loki", new Color(0.36f, 0.24f, 0.43f), "Chaos rune - unpredictable effects");
            
            Debug.Log("Shield Wall: Rune assets created");
        }

        [MenuItem("Shield Wall/Setup/Create Brother Assets")]
        public static void CreateBrotherAssets()
        {
            CreateBrother("Bjorn", "+block power");
            CreateBrother("Erik", "Better auto-defend");
            CreateBrother("Gunnar", "+strike power");
            CreateBrother("Olaf", "Extra wound absorption");
            
            Debug.Log("Shield Wall: Brother assets created");
        }

        [MenuItem("Shield Wall/Setup/Create Enemy Assets")]
        public static void CreateEnemyAssets()
        {
            CreateEnemy("Thrall", 1, 1, EnemyTargetingType.Random, false, false, new Color(0.5f, 0.5f, 0.5f));
            CreateEnemy("Warrior", 1, 1, EnemyTargetingType.LowestHealth, false, false, new Color(0.6f, 0.3f, 0.2f));
            CreateEnemy("Spearman", 1, 2, EnemyTargetingType.Player, false, false, new Color(0.4f, 0.35f, 0.2f));
            CreateEnemy("Berserker", 2, 2, EnemyTargetingType.Player, false, false, new Color(0.7f, 0.15f, 0.15f));
            CreateEnemy("Archer", 1, 1, EnemyTargetingType.Random, true, false, new Color(0.3f, 0.5f, 0.3f));
            CreateEnemy("ShieldBreaker", 1, 1, EnemyTargetingType.Player, false, true, new Color(0.3f, 0.3f, 0.5f));
            
            Debug.Log("Shield Wall: Enemy assets created");
        }

        [MenuItem("Shield Wall/Setup/Create Action Assets")]
        public static void CreateActionAssets()
        {
            CreateAction("Block", new[] { RuneType.Thurs, RuneType.Thurs }, ActionEffectType.Block, 1, "Negate 1 attack on you");
            CreateAction("Strike", new[] { RuneType.Tyr, RuneType.Gebo }, ActionEffectType.Strike, 1, "Kill 1 enemy");
            CreateAction("Cover", new[] { RuneType.Thurs, RuneType.Berkana }, ActionEffectType.Cover, 1, "Block attack on adjacent brother");
            CreateAction("Brace", new[] { RuneType.Berkana, RuneType.Berkana }, ActionEffectType.Brace, 1, "Reduce next wound to scratch");
            CreateAction("Counter", new[] { RuneType.Tyr, RuneType.Thurs }, ActionEffectType.Counter, 1, "Block + kill attacker");
            CreateAction("Testudo", new[] { RuneType.Thurs, RuneType.Thurs, RuneType.Thurs }, ActionEffectType.BlockAll, 1, "Block ALL attacks this turn");
            CreateAction("Berserker", new[] { RuneType.Tyr, RuneType.Tyr, RuneType.Tyr }, ActionEffectType.BerserkerRage, 3, "Kill 3 enemies, take 1 wound");
            CreateAction("Rally", new[] { RuneType.Berkana, RuneType.Berkana, RuneType.Berkana }, ActionEffectType.Heal, 1, "Heal 1 wound from each brother");
            CreateAction("WallPush", new[] { RuneType.Thurs, RuneType.Berkana, RuneType.Gebo }, ActionEffectType.Stun, 1, "Stun all enemies for 1 turn");
            CreateAction("SpearWall", new[] { RuneType.Gebo, RuneType.Gebo, RuneType.Gebo }, ActionEffectType.MultiStrike, 2, "Kill 2 enemies");
            
            Debug.Log("Shield Wall: Action assets created");
        }

        [MenuItem("Shield Wall/Setup/Create Wave Assets")]
        public static void CreateWaveAssets()
        {
            var thrall = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Thrall.asset");
            var warrior = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Warrior.asset");
            var spearman = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Spearman.asset");
            var berserker = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Berserker.asset");
            var archer = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_Archer.asset");
            var shieldBreaker = AssetDatabase.LoadAssetAtPath<EnemySO>("Assets/ScriptableObjects/Enemies/Enemy_ShieldBreaker.asset");

            if (thrall == null || warrior == null || spearman == null)
            {
                Debug.LogWarning("Shield Wall: Please create enemy assets first before creating waves!");
                return;
            }

            CreateWave(1, new[] { (thrall, 3) });
            CreateWave(2, new[] { (thrall, 2), (warrior, 2) });
            CreateWave(3, new[] { (warrior, 2), (spearman, 2), (thrall, 1) });
            
            if (berserker != null && archer != null && shieldBreaker != null)
            {
                CreateWave(4, new[] { (warrior, 3), (archer, 2), (shieldBreaker, 1) });
                CreateWave(5, new[] { (berserker, 2), (spearman, 2), (shieldBreaker, 1) });
            }
            
            Debug.Log("Shield Wall: Wave assets created");
        }

        private static void CreateRune(RuneType type, string displayName, Color color, string description)
        {
            string path = $"Assets/ScriptableObjects/Runes/Rune_{type}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<RuneSO>(path) != null)
            {
                Debug.Log($"Rune {type} already exists, skipping...");
                return;
            }

            var rune = ScriptableObject.CreateInstance<RuneSO>();
            rune.runeType = type;
            rune.runeName = displayName;
            rune.color = color;
            rune.description = description;

            AssetDatabase.CreateAsset(rune, path);
        }

        private static void CreateBrother(string name, string specialty)
        {
            string path = $"Assets/ScriptableObjects/Brothers/Brother_{name}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<ShieldBrotherSO>(path) != null)
            {
                Debug.Log($"Brother {name} already exists, skipping...");
                return;
            }

            var brother = ScriptableObject.CreateInstance<ShieldBrotherSO>();
            brother.brotherName = name;
            brother.maxHealth = 3;
            brother.autoDefendChance = 0.5f;
            brother.specialty = specialty;

            AssetDatabase.CreateAsset(brother, path);
        }

        private static void CreateEnemy(string name, int health, int damage, EnemyTargetingType targeting, 
            bool ignoresBlocks = false, bool destroysBlock = false, Color? tintColor = null)
        {
            string path = $"Assets/ScriptableObjects/Enemies/Enemy_{name}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<EnemySO>(path) != null)
            {
                Debug.Log($"Enemy {name} already exists, skipping...");
                return;
            }

            var enemy = ScriptableObject.CreateInstance<EnemySO>();
            enemy.enemyName = name;
            enemy.health = health;
            enemy.damage = damage;
            enemy.targeting = targeting;
            enemy.ignoresBlocks = ignoresBlocks;
            enemy.destroysBlock = destroysBlock;
            enemy.tintColor = tintColor ?? Color.white;

            AssetDatabase.CreateAsset(enemy, path);
        }

        private static void CreateAction(string name, RuneType[] runes, ActionEffectType effect, int power, string description)
        {
            string path = $"Assets/ScriptableObjects/Actions/Action_{name}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<ActionSO>(path) != null)
            {
                Debug.Log($"Action {name} already exists, skipping...");
                return;
            }

            var action = ScriptableObject.CreateInstance<ActionSO>();
            action.actionName = name;
            action.requiredRunes = runes;
            action.effectType = effect;
            action.effectPower = power;
            action.description = description;

            AssetDatabase.CreateAsset(action, path);
        }

        private static void CreateWave(int waveNumber, (EnemySO enemy, int count)[] enemies)
        {
            string path = $"Assets/ScriptableObjects/Waves/Wave_{waveNumber:D2}.asset";
            
            if (AssetDatabase.LoadAssetAtPath<WaveConfigSO>(path) != null)
            {
                Debug.Log($"Wave {waveNumber} already exists, skipping...");
                return;
            }

            var wave = ScriptableObject.CreateInstance<WaveConfigSO>();
            wave.waveNumber = waveNumber;
            
            foreach (var (enemy, count) in enemies)
            {
                wave.enemies.Add(new EnemySpawn { enemy = enemy, count = count });
            }

            AssetDatabase.CreateAsset(wave, path);
        }
    }
}

