using UnityEngine;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class ModularCharacterBuilder : MonoBehaviour
    {
        public enum CharacterState
        {
            Intact,
            Headless,
            ArmlessSword,
            ArmlessShield,
            Legless
        }
        
        [Header("Configuration")]
        [SerializeField] private ModularCharacterData _characterData;
        
        [Header("Body Part Transforms")]
        [SerializeField] private Transform _torsoTransform;
        [SerializeField] private Transform _headTransform;
        [SerializeField] private Transform _rightArmTransform;
        [SerializeField] private Transform _leftArmTransform;
        [SerializeField] private Transform _rightLegTransform;
        [SerializeField] private Transform _leftLegTransform;
        
        private MeshFilter _torsoMeshFilter;
        private MeshRenderer _torsoRenderer;
        private CharacterState _currentState = CharacterState.Intact;
        
        private void Awake()
        {
            if (_torsoTransform != null)
            {
                _torsoMeshFilter = _torsoTransform.GetComponent<MeshFilter>();
                _torsoRenderer = _torsoTransform.GetComponent<MeshRenderer>();
            }
        }
        
        public void Initialize(ModularCharacterData data)
        {
            _characterData = data;
            BuildIntactCharacter();
        }
        
        public void BuildIntactCharacter()
        {
            _currentState = CharacterState.Intact;
            
            if (_characterData == null) return;
            
            SetBodyPart(_torsoTransform, _characterData.torsoMesh, true);
            SetBodyPart(_headTransform, _characterData.headMesh, true);
            SetBodyPart(_rightArmTransform, _characterData.rightArmMesh, true);
            SetBodyPart(_leftArmTransform, _characterData.leftArmMesh, true);
            SetBodyPart(_rightLegTransform, _characterData.rightLegMesh, true);
            SetBodyPart(_leftLegTransform, _characterData.leftLegMesh, true);
            
            ApplyMaterial();
        }
        
        public void SwapToState(CharacterState newState)
        {
            _currentState = newState;
            
            if (_characterData == null) return;
            
            switch (newState)
            {
                case CharacterState.Headless:
                    SetBodyPart(_headTransform, null, false);
                    if (_characterData.headlessTorsoMesh != null)
                    {
                        SetBodyPart(_torsoTransform, _characterData.headlessTorsoMesh, true);
                    }
                    break;
                    
                case CharacterState.ArmlessSword:
                    SetBodyPart(_rightArmTransform, null, false);
                    if (_characterData.armlessTorsoMesh != null)
                    {
                        SetBodyPart(_torsoTransform, _characterData.armlessTorsoMesh, true);
                    }
                    break;
                    
                case CharacterState.ArmlessShield:
                    SetBodyPart(_leftArmTransform, null, false);
                    if (_characterData.armlessTorsoMesh != null)
                    {
                        SetBodyPart(_torsoTransform, _characterData.armlessTorsoMesh, true);
                    }
                    break;
                    
                case CharacterState.Legless:
                    SetBodyPart(_rightLegTransform, null, false);
                    SetBodyPart(_leftLegTransform, null, false);
                    if (_characterData.leglessTorsoMesh != null)
                    {
                        SetBodyPart(_torsoTransform, _characterData.leglessTorsoMesh, true);
                    }
                    break;
                    
                case CharacterState.Intact:
                default:
                    BuildIntactCharacter();
                    break;
            }
        }
        
        public Vector3 GetLimbPosition(CharacterState limbType)
        {
            return limbType switch
            {
                CharacterState.Headless => _headTransform != null ? _headTransform.position : transform.position,
                CharacterState.ArmlessSword => _rightArmTransform != null ? _rightArmTransform.position : transform.position,
                CharacterState.ArmlessShield => _leftArmTransform != null ? _leftArmTransform.position : transform.position,
                _ => transform.position
            };
        }
        
        private void SetBodyPart(Transform partTransform, Mesh mesh, bool visible)
        {
            if (partTransform == null) return;
            
            var meshFilter = partTransform.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh = mesh;
            }
            
            var renderer = partTransform.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
            
            partTransform.gameObject.SetActive(visible);
        }
        
        private void ApplyMaterial()
        {
            if (_characterData == null || _characterData.bodyMaterial == null) return;
            
            ApplyMaterialToTransform(_torsoTransform);
            ApplyMaterialToTransform(_headTransform);
            ApplyMaterialToTransform(_rightArmTransform);
            ApplyMaterialToTransform(_leftArmTransform);
            ApplyMaterialToTransform(_rightLegTransform);
            ApplyMaterialToTransform(_leftLegTransform);
        }
        
        private void ApplyMaterialToTransform(Transform target)
        {
            if (target == null) return;
            
            var renderer = target.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = _characterData.bodyMaterial;
            }
        }
        
        public CharacterState GetCurrentState()
        {
            return _currentState;
        }
    }
}
