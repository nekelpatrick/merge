using System.Collections;
using UnityEngine;
using ShieldWall.Dice;
using ShieldWall.Data;

namespace ShieldWall.Core
{
    public class DebugBattleTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool _autoRunTest = true;
        [SerializeField] private float _stepDelay = 1f;

        [Header("Test Data")]
        [SerializeField] private ActionSO[] _testActions;
        [SerializeField] private WaveConfigSO[] _testWaves;
        [SerializeField] private ShieldBrotherSO[] _testBrothers;

        private DicePool _dicePool;
        private int _turnCount;

        private void Start()
        {
            if (_autoRunTest)
            {
                StartCoroutine(RunDebugTest());
            }
        }

        [ContextMenu("Run Debug Test")]
        public void RunTest()
        {
            StartCoroutine(RunDebugTest());
        }

        private IEnumerator RunDebugTest()
        {
            Debug.Log("=== SHIELD WALL DEBUG TEST ===");
            Debug.Log("Testing core systems...\n");

            yield return TestDiceSystem();
            yield return new WaitForSeconds(_stepDelay);

            yield return TestComboSystem();
            yield return new WaitForSeconds(_stepDelay);

            yield return TestGameEvents();
            yield return new WaitForSeconds(_stepDelay);

            yield return SimulateBattle();

            Debug.Log("\n=== DEBUG TEST COMPLETE ===");
        }

        private IEnumerator TestDiceSystem()
        {
            Debug.Log("--- DICE SYSTEM TEST ---");
            
            _dicePool = new DicePool(4);
            Debug.Log($"Created dice pool with {_dicePool.DiceCount} dice");

            var dice = _dicePool.RollAll();
            Debug.Log("Rolled dice:");
            for (int i = 0; i < dice.Length; i++)
            {
                Debug.Log($"  Die {i + 1}: {dice[i].CurrentFace} ({GetRuneSymbol(dice[i].CurrentFace)})");
            }

            _dicePool.ToggleLock(0);
            _dicePool.ToggleLock(1);
            Debug.Log($"Locked dice 1 and 2");

            var lockedRunes = _dicePool.GetLockedRunes();
            Debug.Log($"Locked runes: {string.Join(", ", lockedRunes)}");

            _dicePool.RollAll();
            Debug.Log("Re-rolled (locked dice should stay the same):");
            for (int i = 0; i < _dicePool.Dice.Count; i++)
            {
                var die = _dicePool.Dice[i];
                string lockStatus = die.IsLocked ? "[LOCKED]" : "";
                Debug.Log($"  Die {i + 1}: {die.CurrentFace} {lockStatus}");
            }

            yield return null;
        }

        private IEnumerator TestComboSystem()
        {
            Debug.Log("\n--- COMBO SYSTEM TEST ---");

            if (_testActions == null || _testActions.Length == 0)
            {
                Debug.LogWarning("No test actions assigned. Skipping combo test.");
                Debug.Log("To test combos, assign ActionSO assets in the Inspector.");
                yield break;
            }

            var testRunes = new[] { RuneType.Thurs, RuneType.Thurs, RuneType.Tyr, RuneType.Gebo };
            Debug.Log($"Testing with runes: {string.Join(", ", testRunes)}");

            var availableActions = ComboResolver.Resolve(testRunes, _testActions);
            Debug.Log($"Available actions ({availableActions.Count}):");
            foreach (var action in availableActions)
            {
                Debug.Log($"  - {action.actionName}: {action.description}");
            }

            yield return null;
        }

        private IEnumerator TestGameEvents()
        {
            Debug.Log("\n--- GAME EVENTS TEST ---");

            GameEvents.OnPhaseChanged += phase => Debug.Log($"EVENT: Phase changed to {phase}");
            GameEvents.OnStaminaChanged += stamina => Debug.Log($"EVENT: Stamina is now {stamina}");
            GameEvents.OnWaveStarted += wave => Debug.Log($"EVENT: Wave {wave} started!");

            Debug.Log("Firing test events...");
            GameEvents.RaisePhaseChanged(TurnPhase.WaveStart);
            yield return new WaitForSeconds(0.2f);
            
            GameEvents.RaiseStaminaChanged(12);
            yield return new WaitForSeconds(0.2f);
            
            GameEvents.RaiseWaveStarted(1);
            yield return new WaitForSeconds(0.2f);
            
            GameEvents.RaisePhaseChanged(TurnPhase.PlayerTurn);

            yield return null;
        }

        private IEnumerator SimulateBattle()
        {
            Debug.Log("\n--- SIMULATED BATTLE ---");
            
            int stamina = 12;
            int playerHealth = 5;
            int waveNumber = 1;

            for (int turn = 1; turn <= 5 && playerHealth > 0 && stamina > 0; turn++)
            {
                _turnCount = turn;
                Debug.Log($"\n=== TURN {turn} (Wave {waveNumber}) ===");
                Debug.Log($"Player HP: {playerHealth}/5 | Stamina: {stamina}/12");

                _dicePool.UnlockAll();
                var dice = _dicePool.RollAll();
                
                string diceResult = "";
                foreach (var die in dice)
                {
                    diceResult += GetRuneSymbol(die.CurrentFace) + " ";
                }
                Debug.Log($"Rolled: {diceResult}");

                yield return new WaitForSeconds(_stepDelay * 0.5f);

                int enemiesThisTurn = Random.Range(1, 4);
                Debug.Log($"Enemies attacking: {enemiesThisTurn}");

                int blocked = Random.Range(0, enemiesThisTurn + 1);
                int damage = enemiesThisTurn - blocked;
                
                Debug.Log($"Blocked {blocked} attacks, took {damage} damage");
                playerHealth -= damage;

                stamina--;
                Debug.Log($"Stamina tick: -{1}");

                if (turn % 2 == 0 && waveNumber < 3)
                {
                    waveNumber++;
                    stamina += 2;
                    Debug.Log($"Wave cleared! +2 Stamina. Starting Wave {waveNumber}");
                }

                yield return new WaitForSeconds(_stepDelay);
            }

            if (playerHealth <= 0)
            {
                Debug.Log("\n*** DEFEAT - Player died! ***");
            }
            else if (stamina <= 0)
            {
                Debug.Log("\n*** DEFEAT - Exhausted! ***");
            }
            else
            {
                Debug.Log("\n*** VICTORY - Battle complete! ***");
            }
        }

        private string GetRuneSymbol(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "[SH]",
                RuneType.Tyr => "[AX]",
                RuneType.Gebo => "[SP]",
                RuneType.Berkana => "[BR]",
                RuneType.Othala => "[OD]",
                RuneType.Laguz => "[LO]",
                _ => "[?]"
            };
        }
    }
}

