using UnityEngine;

namespace ShieldWall.Visual
{
    [RequireComponent(typeof(MeshRenderer))]
    public class LODController : MonoBehaviour
    {
        [Header("LOD Settings")]
        [SerializeField] private bool _enableLOD = true;
        [SerializeField] private float _lodDistance0 = 10f;
        [SerializeField] private float _lodDistance1 = 20f;
        [SerializeField] private float _lodDistance2 = 30f;
        
        private MeshRenderer _renderer;
        private Transform _cameraTransform;
        private float _lastUpdateTime;
        private const float UPDATE_INTERVAL = 0.5f;
        
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }
        
        private void Start()
        {
            _cameraTransform = Camera.main != null ? Camera.main.transform : null;
        }
        
        private void Update()
        {
            if (!_enableLOD || _cameraTransform == null) return;
            
            if (Time.time - _lastUpdateTime < UPDATE_INTERVAL) return;
            _lastUpdateTime = Time.time;
            
            float distance = Vector3.Distance(transform.position, _cameraTransform.position);
            UpdateLOD(distance);
        }
        
        private void UpdateLOD(float distance)
        {
            if (_renderer == null) return;
            
            if (distance > _lodDistance2)
            {
                _renderer.enabled = false;
            }
            else
            {
                _renderer.enabled = true;
            }
        }
    }
}
