using UnityEngine;
using ShieldWall.Dice;

namespace ShieldWall.UI
{
    public static class RuneDisplay
    {
        public static string GetFullName(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "Shield",
                RuneType.Tyr => "Axe",
                RuneType.Gebo => "Spear",
                RuneType.Berkana => "Brace",
                RuneType.Othala => "Odin",
                RuneType.Laguz => "Loki",
                _ => "Unknown"
            };
        }

        public static string GetShortCode(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "SH",
                RuneType.Tyr => "AX",
                RuneType.Gebo => "SP",
                RuneType.Berkana => "BR",
                RuneType.Othala => "OD",
                RuneType.Laguz => "LO",
                _ => "?"
            };
        }

        public static string GetSymbol(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "ᚦ",
                RuneType.Tyr => "ᛏ",
                RuneType.Gebo => "ᚷ",
                RuneType.Berkana => "ᛒ",
                RuneType.Othala => "ᛟ",
                RuneType.Laguz => "ᛚ",
                _ => "�"
            };
        }

        public static Color GetDefaultColor(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => new Color(92f/255f, 92f/255f, 92f/255f),
                RuneType.Tyr => new Color(139f/255f, 32f/255f, 32f/255f),
                RuneType.Gebo => new Color(139f/255f, 105f/255f, 20f/255f),
                RuneType.Berkana => new Color(61f/255f, 92f/255f, 61f/255f),
                RuneType.Othala => new Color(201f/255f, 162f/255f, 39f/255f),
                RuneType.Laguz => new Color(92f/255f, 61f/255f, 110f/255f),
                _ => Color.gray
            };
        }

        public static string GetDomain(RuneType type)
        {
            return type switch
            {
                RuneType.Thurs => "Defense",
                RuneType.Tyr => "Attack",
                RuneType.Gebo => "Precision",
                RuneType.Berkana => "Support",
                RuneType.Othala => "Wild",
                RuneType.Laguz => "Chaos",
                _ => "Unknown"
            };
        }
    }
}
