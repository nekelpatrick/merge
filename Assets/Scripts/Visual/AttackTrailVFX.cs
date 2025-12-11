using System.Collections;
using UnityEngine;

namespace ShieldWall.Visual
{
    public class AttackTrailVFX : MonoBehaviour
    {
        [Header("Trail Settings")]
        [SerializeField] private float _trailDuration = 0.3f;
        [SerializeField] private float _trailWidth = 0.1f;
        [SerializeField] private Color _trailColor = new Color(0.9f, 0.9f, 1f, 0.8f);
        [SerializeField] private int _segments = 20;

        [Header("Arc Settings")]
        [SerializeField] private float _arcAngle = 90f;
        [SerializeField] private float _arcRadius = 0.8f;

        private LineRenderer _lineRenderer;
        private Coroutine _trailCoroutine;

        private void Awake()
        {
            CreateLineRenderer();
        }

        private void CreateLineRenderer()
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = _segments;
            _lineRenderer.startWidth = _trailWidth;
            _lineRenderer.endWidth = _trailWidth * 0.1f;
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.material = CreateTrailMaterial();
            _lineRenderer.enabled = false;
        }

        private Material CreateTrailMaterial()
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");

            var mat = new Material(shader);
            mat.color = _trailColor;
            mat.SetFloat("_Surface", 1);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;

            return mat;
        }

        public void PlaySlash(Vector3 startDirection, bool rightToLeft = true)
        {
            if (_trailCoroutine != null)
                StopCoroutine(_trailCoroutine);
            _trailCoroutine = StartCoroutine(SlashRoutine(startDirection, rightToLeft));
        }

        private IEnumerator SlashRoutine(Vector3 startDirection, bool rightToLeft)
        {
            _lineRenderer.enabled = true;

            float startAngle = rightToLeft ? _arcAngle / 2f : -_arcAngle / 2f;
            float endAngle = rightToLeft ? -_arcAngle / 2f : _arcAngle / 2f;

            float elapsed = 0f;
            while (elapsed < _trailDuration)
            {
                float progress = elapsed / _trailDuration;
                float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);

                UpdateTrailPositions(currentAngle, progress);

                Color startColor = _trailColor;
                Color endColor = _trailColor;
                startColor.a = _trailColor.a * (1f - progress);
                endColor.a = 0f;
                _lineRenderer.startColor = startColor;
                _lineRenderer.endColor = endColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            _lineRenderer.enabled = false;
            _trailCoroutine = null;
        }

        private void UpdateTrailPositions(float currentAngle, float progress)
        {
            float trailLength = _arcAngle * progress;

            for (int i = 0; i < _segments; i++)
            {
                float t = (float)i / (_segments - 1);
                float angle = currentAngle - trailLength * t;
                float radians = angle * Mathf.Deg2Rad;

                Vector3 pos = new Vector3(
                    Mathf.Sin(radians) * _arcRadius,
                    Mathf.Cos(radians) * _arcRadius * 0.5f,
                    0.5f
                );

                _lineRenderer.SetPosition(i, pos);
            }
        }

        [ContextMenu("Test Slash Right to Left")]
        public void TestSlashRTL() => PlaySlash(Vector3.forward, true);

        [ContextMenu("Test Slash Left to Right")]
        public void TestSlashLTR() => PlaySlash(Vector3.forward, false);
    }
}

