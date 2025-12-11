using System.Collections.Generic;
using UnityEngine;

namespace ShieldWall.Data
{
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    [CreateAssetMenu(fileName = "Scenario_", menuName = "ShieldWall/BattleScenario")]
    public class BattleScenarioSO : ScriptableObject
    {
        public string scenarioName = "New Scenario";
        [TextArea(2, 4)]
        public string description = "";
        public Difficulty difficulty = Difficulty.Normal;
        public List<WaveConfigSO> waves = new List<WaveConfigSO>();

        [Header("Starting Conditions")]
        public int startingStamina = 12;
        public int startingPlayerHealth = 5;
        public int startingDiceCount = 4;

        [Header("Unlock")]
        public bool isUnlocked = true;
        public BattleScenarioSO prerequisite;

        [Header("Visuals")]
        public Sprite thumbnail;
    }
}

