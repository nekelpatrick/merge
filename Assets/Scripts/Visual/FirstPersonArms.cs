using UnityEngine;

namespace ShieldWall.Visual
{
    public class FirstPersonArms : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _leftArm;
        [SerializeField] private Transform _rightArm;
        [SerializeField] private Transform _shield;
        
        [Header("Idle Animation")]
        [SerializeField] private float _bobSpeed = 1f;
        [SerializeField] private float _bobAmount = 0.02f;
        
        [Header("Combat Animation")]
        [SerializeField] private float _blockRaiseAmount = 0.1f;
        [SerializeField] private float _animationSpeed = 5f;

        private Vector3 _shieldStartPos;
        private Quaternion _shieldStartRot;
        private bool _isBlocking;
        private float _bobTimer;

        private void Awake()
        {
            if (_shield != null)
            {
                _shieldStartPos = _shield.localPosition;
                _shieldStartRot = _shield.localRotation;
            }
        }

        private void OnEnable()
        {
            Core.GameEvents.OnAttackBlocked += HandleAttackBlocked;
            Core.GameEvents.OnPlayerWounded += HandlePlayerWounded;
        }

        private void OnDisable()
        {
            Core.GameEvents.OnAttackBlocked -= HandleAttackBlocked;
            Core.GameEvents.OnPlayerWounded -= HandlePlayerWounded;
        }

        private void Update()
        {
            UpdateIdleBob();
            UpdateShieldPosition();
        }

        private void UpdateIdleBob()
        {
            _bobTimer += Time.deltaTime * _bobSpeed;
            float bob = Mathf.Sin(_bobTimer) * _bobAmount;
            
            if (_leftArm != null)
            {
                var pos = _leftArm.localPosition;
                pos.y = bob;
                _leftArm.localPosition = pos;
            }
            
            if (_rightArm != null)
            {
                var pos = _rightArm.localPosition;
                pos.y = -bob * 0.5f;
                _rightArm.localPosition = pos;
            }
        }

        private void UpdateShieldPosition()
        {
            if (_shield == null) return;
            
            Vector3 targetPos = _isBlocking 
                ? _shieldStartPos + Vector3.up * _blockRaiseAmount 
                : _shieldStartPos;
            
            _shield.localPosition = Vector3.Lerp(_shield.localPosition, targetPos, Time.deltaTime * _animationSpeed);
        }

        private void HandleAttackBlocked(Core.Attack attack)
        {
            StartCoroutine(BlockAnimation());
        }

        private void HandlePlayerWounded(int damage)
        {
            StartCoroutine(HitReaction());
        }

        private System.Collections.IEnumerator BlockAnimation()
        {
            _isBlocking = true;
            yield return new WaitForSeconds(0.3f);
            _isBlocking = false;
        }

        private System.Collections.IEnumerator HitReaction()
        {
            if (_shield != null)
            {
                var originalPos = _shield.localPosition;
                _shield.localPosition = originalPos + Vector3.back * 0.05f;
                yield return new WaitForSeconds(0.1f);
                _shield.localPosition = originalPos;
            }
        }
    }
}

