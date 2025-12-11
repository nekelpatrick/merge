using UnityEngine;

namespace ShieldWall.Visual
{
    [CreateAssetMenu(fileName = "ToonMaterialPalette", menuName = "ShieldWall/Toon Material Palette")]
    public class ToonMaterialPalette : ScriptableObject
    {
        [Header("Character Materials")]
        public Material playerMaterial;
        public Material brotherMaterial;
        public Material enemyThrallMaterial;
        public Material enemyWarriorMaterial;
        public Material enemyBerserkerMaterial;
        public Material enemyArcherMaterial;
        
        [Header("Gore Materials")]
        public Material bloodMaterial;
        public Material goreMaterial;
        
        [Header("Palette Colors")]
        public Color mudBrown = new Color(0.29f, 0.22f, 0.16f);
        public Color wornLeather = new Color(0.42f, 0.31f, 0.24f);
        public Color ironGray = new Color(0.36f, 0.36f, 0.36f);
        public Color boneWhite = new Color(0.83f, 0.78f, 0.72f);
        public Color bloodRed = new Color(0.55f, 0.13f, 0.13f);
        
        public Material GetEnemyMaterial(string enemyType)
        {
            string lowerType = enemyType.ToLower();
            
            if (lowerType.Contains("thrall")) return enemyThrallMaterial;
            if (lowerType.Contains("warrior")) return enemyWarriorMaterial;
            if (lowerType.Contains("berserker")) return enemyBerserkerMaterial;
            if (lowerType.Contains("archer")) return enemyArcherMaterial;
            
            return enemyThrallMaterial;
        }
    }
}
