using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShieldWall.Data
{
    [Serializable]
    public class EnemySpawn
    {
        public EnemySO enemy;
        public int count = 1;
    }

    [CreateAssetMenu(fileName = "Wave_", menuName = "ShieldWall/WaveConfig")]
    public class WaveConfigSO : ScriptableObject
    {
        public int waveNumber = 1;
        public List<EnemySpawn> enemies = new List<EnemySpawn>();
        public bool hasScriptedEvent;
        public string scriptedEventId;
    }
}

