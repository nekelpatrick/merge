using System.Collections;
using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.Visual
{
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("Spawn Animation")]
        [SerializeField] private float _spawnRiseHeight = 2f;
        [SerializeField] private float _spawnDuration = 0.5f;

        [Header("Idle Animation")]
        [SerializeField] private float _idleSwayAmount = 0.1f;
        [SerializeField] private float _idleSwaySpeed = 1f;

        [Header("Attack Telegraph")]
        [SerializeField] private float _telegraphPullBack = 0.3f;
        [SerializeField] private float _telegraphDuration = 0.3f;
        [SerializeField] private Color _telegraphColor = new Color(1f, 0.3f, 0.3f, 1f);

        [Header("Death Animation")]
        [SerializeField] private float _deathStaggerDistance = 0.5f;
        [SerializeField] private float _deathFallAngle = 90f;
        [SerializeField] private float _deathDuration = 0.6f;
        [SerializeField] private float _deathFadeDuration = 0.3f;

        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Renderer[] _renderers;
        private Coroutine _idleCoroutine;
        private Coroutine _animationCoroutine;
        private bool _isAlive = true;

        public void Initialize(Vector3 position)
        {
            _originalPosition = position;
            _originalRotation = transform.rotation;
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void PlaySpawnAnimation(System.Action onComplete = null)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(SpawnRoutine(onComplete));
        }

        public void PlayAttackTelegraph(System.Action onComplete = null)
        {
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(TelegraphRoutine(onComplete));
        }

        public void PlayDeathAnimation(System.Action onComplete = null)
        {
            _isAlive = false;
            StopIdle();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
            _animationCoroutine = StartCoroutine(DeathRoutine(onComplete));
        }

        public void StartIdle()
        {
            if (_idleCoroutine != null)
                StopCoroutine(_idleCoroutine);
            _idleCoroutine = StartCoroutine(IdleRoutine());
        }

        public void StopIdle()
        {
            if (_idleCoroutine != null)
            {
                StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine(System.Action onComplete)
        {
            Vector3 startPos = _originalPosition + Vector3.down * _spawnRiseHeight;
            transform.position = startPos;

            SetAlpha(0f);

            float elapsed = 0f;
            while (elapsed < _spawnDuration)
            {
                float t = Tweener.Evaluate(elapsed / _spawnDuration, EaseType.EaseOutBack);
                transform.position = Vector3.Lerp(startPos, _originalPosition, t);
                SetAlpha(t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = _originalPosition;
            SetAlpha(1f);
            
            StartIdle();
            onComplete?.Invoke();
            _animationCoroutine = null;
        }

        private IEnumerator TelegraphRoutine(System.Action onComplete)
        {
            StopIdle();

            Vector3 pullBackPos = _originalPosition + Vector3.back * _telegraphPullBack;

            yield return StartCoroutine(Tweener.TweenVector3(
                transform.position,
                pullBackPos,
                _telegraphDuration * 0.5f,
                EaseType.EaseOutQuad,
                v => transform.position = v
            ));

            SetTint(_telegraphColor);

            yield return new WaitForSeconds(_telegraphDuration * 0.3f);

            yield return StartCoroutine(Tweener.TweenVector3(
                transform.position,
                _originalPosition + Vector3.forward * 0.2f,
                _telegraphDuration * 0.2f,
                EaseType.EaseInQuad,
                v => transform.position = v
            ));

            SetTint(Color.white);
            transform.position = _originalPosition;
            
            StartIdle();
            onComplete?.Invoke();
            _animationCoroutine = null;
        }

        private IEnumerator DeathRoutine(System.Action onComplete)
        {
            Vector3 staggerPos = _originalPosition + Vector3.back * _deathStaggerDistance;
            Quaternion fallRotation = Quaternion.Euler(_deathFallAngle, 0, 0);

            float elapsed = 0f;
            while (elapsed < _deathDuration * 0.3f)
            {
                float t = elapsed / (_deathDuration * 0.3f);
                transform.position = Vector3.Lerp(_originalPosition, staggerPos, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            Vector3 currentPos = transform.position;
            Quaternion currentRot = transform.rotation;
            Vector3 fallPos = staggerPos + Vector3.down * 0.5f;

            while (elapsed < _deathDuration * 0.7f)
            {
                float t = Tweener.Evaluate(elapsed / (_deathDuration * 0.7f), EaseType.EaseInQuad);
                transform.position = Vector3.Lerp(currentPos, fallPos, t);
                transform.rotation = Quaternion.Slerp(currentRot, fallRotation, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < _deathFadeDuration)
            {
                float t = elapsed / _deathFadeDuration;
                SetAlpha(1f - t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetAlpha(0f);
            onComplete?.Invoke();
            _animationCoroutine = null;
        }

        private IEnumerator IdleRoutine()
        {
            float offset = Random.value * Mathf.PI * 2f;
            while (_isAlive)
            {
                float sway = Mathf.Sin(Time.time * _idleSwaySpeed + offset) * _idleSwayAmount;
                transform.position = _originalPosition + Vector3.right * sway;
                yield return null;
            }
        }

        private void SetAlpha(float alpha)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    var mat = renderer.material;
                    var color = mat.color;
                    color.a = alpha;
                    mat.color = color;
                }
            }
        }

        private void SetTint(Color tint)
        {
            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    renderer.material.color = tint;
                }
            }
        }

        private void OnDisable()
        {
            StopIdle();
            if (_animationCoroutine != null)
                StopCoroutine(_animationCoroutine);
        }
    }
}

