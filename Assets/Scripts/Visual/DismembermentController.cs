using UnityEngine;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class DismembermentController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private SeveredLimb _severedHeadPrefab;
        [SerializeField] private SeveredLimb _severedArmPrefab;
        [SerializeField] private SeveredLimb _severedLegPrefab;
        
        [Header("VFX")]
        [SerializeField] private BloodBurstVFX _bloodBurstPrefab;
        [SerializeField] private GameObject _bloodDecalPrefab;
        
        [Header("Settings")]
        [SerializeField] private float _severedLimbForce = 3f;
        
        private ModularCharacterBuilder _characterBuilder;
        
        private void Awake()
        {
            _characterBuilder = GetComponent<ModularCharacterBuilder>();
        }
        
        public void TriggerDismemberment(DismembermentType type, ActionSO actionUsed = null)
        {
            if (_characterBuilder == null) return;
            
            Vector3 limbPosition = transform.position;
            ModularCharacterBuilder.CharacterState targetState = ModularCharacterBuilder.CharacterState.Intact;
            SeveredLimb limbPrefab = null;
            
            switch (type)
            {
                case DismembermentType.Decapitation:
                    targetState = ModularCharacterBuilder.CharacterState.Headless;
                    limbPosition = _characterBuilder.GetLimbPosition(ModularCharacterBuilder.CharacterState.Headless);
                    limbPrefab = _severedHeadPrefab;
                    break;
                    
                case DismembermentType.ArmSword:
                    targetState = ModularCharacterBuilder.CharacterState.ArmlessSword;
                    limbPosition = _characterBuilder.GetLimbPosition(ModularCharacterBuilder.CharacterState.ArmlessSword);
                    limbPrefab = _severedArmPrefab;
                    break;
                    
                case DismembermentType.ArmShield:
                    targetState = ModularCharacterBuilder.CharacterState.ArmlessShield;
                    limbPosition = _characterBuilder.GetLimbPosition(ModularCharacterBuilder.CharacterState.ArmlessShield);
                    limbPrefab = _severedArmPrefab;
                    break;
                    
                case DismembermentType.Leg:
                    targetState = ModularCharacterBuilder.CharacterState.Legless;
                    limbPosition = transform.position + Vector3.down * 0.5f;
                    limbPrefab = _severedLegPrefab;
                    break;
                    
                case DismembermentType.Random:
                    type = (DismembermentType)Random.Range(0, 3);
                    TriggerDismemberment(type, actionUsed);
                    return;
            }
            
            _characterBuilder.SwapToState(targetState);
            
            if (limbPrefab != null)
            {
                SpawnSeveredLimb(limbPrefab, limbPosition);
            }
            
            SpawnBloodEffects(limbPosition);
        }
        
        private void SpawnSeveredLimb(SeveredLimb prefab, Vector3 position)
        {
            if (prefab == null) return;
            
            Vector3 direction = (transform.position - Camera.main.transform.position).normalized;
            direction.y = 0.5f;
            
            var severedLimb = Instantiate(prefab, position, Random.rotation);
            
            var renderer = GetComponentInChildren<MeshRenderer>();
            Material material = renderer != null ? renderer.sharedMaterial : null;
            
            var meshFilter = severedLimb.GetComponent<MeshFilter>();
            Mesh mesh = meshFilter != null ? meshFilter.sharedMesh : null;
            
            severedLimb.Initialize(direction, material, mesh);
        }
        
        private void SpawnBloodEffects(Vector3 position)
        {
            if (_bloodBurstPrefab != null)
            {
                var bloodBurst = Instantiate(_bloodBurstPrefab, position, Quaternion.identity);
                bloodBurst.Play();
            }
            
            if (_bloodDecalPrefab != null)
            {
                Vector3 decalPosition = position;
                decalPosition.y = 0.01f;
                
                Quaternion decalRotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
                
                var decal = Instantiate(_bloodDecalPrefab, decalPosition, decalRotation);
                
                float randomScale = Random.Range(0.5f, 2f);
                decal.transform.localScale = Vector3.one * randomScale;
                
                Destroy(decal, 30f);
            }
        }
    }
    
    public enum DismembermentType
    {
        Random,
        Decapitation,
        ArmSword,
        ArmShield,
        Leg
    }
}
