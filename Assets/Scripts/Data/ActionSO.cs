using UnityEngine;
using ShieldWall.Dice;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Action_", menuName = "ShieldWall/Action")]
    public class ActionSO : ScriptableObject
    {
        public string actionName;
        public RuneType[] requiredRunes;
        public ActionEffectType effectType;
        public int effectPower = 1;
        public Sprite icon;
        [TextArea] public string description;
    }
}

