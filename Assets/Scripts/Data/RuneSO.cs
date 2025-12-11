using UnityEngine;
using ShieldWall.Dice;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "Rune_", menuName = "ShieldWall/Rune")]
    public class RuneSO : ScriptableObject
    {
        public RuneType runeType;
        public string runeName;
        public Sprite icon;
        public Color color = Color.white;
        [TextArea] public string description;
    }
}

