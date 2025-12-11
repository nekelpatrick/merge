using System.Collections;
using UnityEngine;

namespace ShieldWall.Visual
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class SeveredLimb : MonoBehaviour
    {
        [Header("Physics Settings")]
        [SerializeField] private float _tumbleForce = 3f;
        [SerializeField] private float _tumbleTorque = 5f;
        [SerializeField] private float _lifeTime = 10f;
        
        [Header("Blood Trail")]
        [SerializeField] private ParticleSystem _bloodTrailPrefab;
        [SerializeField] private float _bloodTrailDuration = 2f;
        
        private Rigidbody _rigidbody;
        private ParticleSystem _bloodTrailInstance;
        private bool _isInitialized;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        public void Initialize(Vector3 direction, Material material, Mesh limbMesh)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null && limbMesh != null)
            {
                meshFilter.mesh = limbMesh;
            }
            
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null && material != null)
            {
                renderer.material = material;
            }
            
            if (_rigidbody != null)
            {
                _rigidbody.mass = 1f;
                _rigidbody.drag = 0.5f;
                _rigidbody.angularDrag = 0.5f;
                
                Vector3 randomDirection = (direction + Random.insideUnitSphere * 0.3f).normalized;
                _rigidbody.AddForce(randomDirection * _tumbleForce, ForceMode.Impulse);
                _rigidbody.AddTorque(Random.insideUnitSphere * _tumbleTorque, ForceMode.Impulse);
            }
            
            if (_bloodTrailPrefab != null)
            {
                _bloodTrailInstance = Instantiate(_bloodTrailPrefab, transform.position, Quaternion.identity, transform);
                _bloodTrailInstance.Play();
                StartCoroutine(StopBloodTrailAfterDelay());
            }
            
            StartCoroutine(DestroyAfterDelay());
        }
        
        private IEnumerator StopBloodTrailAfterDelay()
        {
            yield return new WaitForSeconds(_bloodTrailDuration);
            
            if (_bloodTrailInstance != null)
            {
                _bloodTrailInstance.Stop();
            }
        }
        
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(_lifeTime);
            
            if (_bloodTrailInstance != null)
            {
                Destroy(_bloodTrailInstance.gameObject);
            }
            
            Destroy(gameObject);
        }
    }
}
