using UnityEngine;

namespace ShieldWall.Data
{
    [CreateAssetMenu(fileName = "ModularCharacter_", menuName = "ShieldWall/Modular Character Data")]
    public class ModularCharacterData : ScriptableObject
    {
        [Header("Body Parts")]
        public Mesh torsoMesh;
        public Mesh headMesh;
        public Mesh rightArmMesh;
        public Mesh leftArmMesh;
        public Mesh rightLegMesh;
        public Mesh leftLegMesh;
        
        [Header("Materials")]
        public Material bodyMaterial;
        
        [Header("Dismemberment Variants")]
        public Mesh headlessTorsoMesh;
        public Mesh armlessTorsoMesh;
        public Mesh leglessTorsoMesh;
        
        [Header("Colors")]
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;
    }
}
