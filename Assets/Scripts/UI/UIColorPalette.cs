using UnityEngine;

namespace ShieldWall.UI
{
    public static class UIColorPalette
    {
        public static readonly Color MudBrown = new Color(0.29f, 0.22f, 0.16f);
        public static readonly Color WornLeather = new Color(0.42f, 0.31f, 0.24f);
        public static readonly Color IronGray = new Color(0.36f, 0.36f, 0.36f);
        public static readonly Color BoneWhite = new Color(0.83f, 0.78f, 0.72f);
        
        public static readonly Color BloodRed = new Color(0.55f, 0.13f, 0.13f);
        public static readonly Color FireOrange = new Color(0.77f, 0.36f, 0.15f);
        public static readonly Color IronBlue = new Color(0.24f, 0.35f, 0.43f);
        public static readonly Color GoldHighlight = new Color(0.79f, 0.64f, 0.15f);
        
        public static readonly Color ThursGray = new Color(0.36f, 0.36f, 0.36f);
        public static readonly Color TyrRed = new Color(0.55f, 0.13f, 0.13f);
        public static readonly Color GeboBronze = new Color(0.55f, 0.41f, 0.08f);
        public static readonly Color BerkanaGreen = new Color(0.24f, 0.36f, 0.24f);
        public static readonly Color OthalaGold = new Color(0.79f, 0.64f, 0.15f);
        public static readonly Color LaguzPurple = new Color(0.36f, 0.24f, 0.43f);

        public static readonly Color PanelBackground = new Color(0.15f, 0.1f, 0.08f, 0.95f);
        public static readonly Color ButtonNormal = new Color(0.3f, 0.22f, 0.16f);
        public static readonly Color ButtonHover = new Color(0.4f, 0.3f, 0.2f);
        public static readonly Color ButtonPressed = new Color(0.2f, 0.15f, 0.1f);

        public static Color GetRuneColor(Dice.RuneType runeType)
        {
            return runeType switch
            {
                Dice.RuneType.Thurs => ThursGray,
                Dice.RuneType.Tyr => TyrRed,
                Dice.RuneType.Gebo => GeboBronze,
                Dice.RuneType.Berkana => BerkanaGreen,
                Dice.RuneType.Othala => OthalaGold,
                Dice.RuneType.Laguz => LaguzPurple,
                _ => BoneWhite
            };
        }
    }
}

