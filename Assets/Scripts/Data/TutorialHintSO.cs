using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Hint_", menuName = "ShieldWall/TutorialHint")]
    public class TutorialHintSO : ScriptableObject
    {
        public string hintId;
        [TextArea(2, 4)] public string hintText;

        [Header("Trigger Conditions")]
        public TurnPhase triggerPhase;
        public int triggerWave = 1;
        public bool requiresDiceLocked;
        public bool requiresNoDiceLocked;

        [Header("Display")]
        public float displayDuration = 5f;
        public bool autoDismiss = true;
        public bool pauseGame;
    }
}

