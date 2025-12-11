using System.Collections;
using UnityEngine;

namespace ShieldWall.Visual
{
    public class ShieldBlockVFX : MonoBehaviour
    {
        [Header("Spark Settings")]
        [SerializeField] private int _sparkCount = 15;
        [SerializeField] private float _sparkSpeed = 5f;
        [SerializeField] private float _sparkLifetime = 0.5f;
        [SerializeField] private Color _sparkColor = new Color(1f, 0.9f, 0.6f, 1f);

        [Header("Ripple Settings")]
        [SerializeField] private float _rippleMaxScale = 2f;
        [SerializeField] private float _rippleDuration = 0.3f;
        [SerializeField] private Color _rippleColor = new Color(0.8f, 0.8f, 1f, 0.5f);

        private ParticleSystem _sparkSystem;
        private Transform _ripple;
        private Renderer _rippleRenderer;
        private Coroutine _rippleCoroutine;

        private void Awake()
        {
            CreateSparkSystem();
            CreateRipple();
        }

        private void CreateSparkSystem()
        {
            var sparkGO = new GameObject("Sparks");
            sparkGO.transform.SetParent(transform);
            sparkGO.transform.localPosition = Vector3.zero;

            _sparkSystem = sparkGO.AddComponent<ParticleSystem>();
            var main = _sparkSystem.main;
            main.duration = 0.5f;
            main.loop = false;
            main.startLifetime = _sparkLifetime;
            main.startSpeed = _sparkSpeed;
            main.startSize = 0.05f;
            main.startColor = _sparkColor;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 2f;

            var emission = _sparkSystem.emission;
            emission.enabled = false;

            var shape = _sparkSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Hemisphere;
            shape.radius = 0.1f;
            shape.rotation = new Vector3(-90, 0, 0);

            var colorOverLifetime = _sparkSystem.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_sparkColor, 0), new GradientColorKey(_sparkColor, 1) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) }
            );
            colorOverLifetime.color = gradient;

            var sizeOverLifetime = _sparkSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));

            var renderer = sparkGO.GetComponent<ParticleSystemRenderer>();
            renderer.material = CreateUnlitMaterial(_sparkColor);

            _sparkSystem.Stop();
        }

        private void CreateRipple()
        {
            _ripple = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
            _ripple.name = "Ripple";
            _ripple.SetParent(transform);
            _ripple.localPosition = Vector3.zero;
            _ripple.localRotation = Quaternion.Euler(0, 0, 0);
            _ripple.localScale = Vector3.zero;

            var collider = _ripple.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            _rippleRenderer = _ripple.GetComponent<Renderer>();
            _rippleRenderer.material = CreateUnlitMaterial(_rippleColor);
        }

        private Material CreateUnlitMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");

            var mat = new Material(shader);
            mat.color = color;

            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Blend", 0);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;

            return mat;
        }

        public void Play()
        {
            _sparkSystem.Emit(_sparkCount);

            if (_rippleCoroutine != null)
                StopCoroutine(_rippleCoroutine);
            _rippleCoroutine = StartCoroutine(RippleRoutine());
        }

        private IEnumerator RippleRoutine()
        {
            float elapsed = 0f;
            _ripple.localScale = Vector3.zero;
            _rippleRenderer.material.color = _rippleColor;

            while (elapsed < _rippleDuration)
            {
                float t = elapsed / _rippleDuration;
                float scale = Tweener.Evaluate(t, Core.EaseType.EaseOutQuad) * _rippleMaxScale;
                _ripple.localScale = Vector3.one * scale;

                Color c = _rippleColor;
                c.a = _rippleColor.a * (1f - t);
                _rippleRenderer.material.color = c;

                elapsed += Time.deltaTime;
                yield return null;
            }

            _ripple.localScale = Vector3.zero;
            _rippleCoroutine = null;
        }

        [ContextMenu("Test Play")]
        public void TestPlay() => Play();
    }
}

