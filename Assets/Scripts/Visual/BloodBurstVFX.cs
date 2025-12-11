using UnityEngine;

namespace ShieldWall.Visual
{
    public class BloodBurstVFX : MonoBehaviour
    {
        private const int MIN_PARTICLES = 30;
        private const int MAX_PARTICLES = 50;
        private const float CONE_ANGLE = 45f;
        private const float MIN_SIZE = 0.05f;
        private const float MAX_SIZE = 0.15f;
        private const float MIN_LIFETIME = 0.5f;
        private const float MAX_LIFETIME = 1f;
        private const float GRAVITY_MODIFIER = 1f;

        private static readonly Color BLOOD_COLOR = new Color(0.545f, 0.125f, 0.125f);

        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            if (_particleSystem == null)
            {
                _particleSystem = gameObject.AddComponent<ParticleSystem>();
                ConfigureParticleSystem();
            }
        }

        private void ConfigureParticleSystem()
        {
            var main = _particleSystem.main;
            main.duration = MAX_LIFETIME;
            main.loop = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(MIN_LIFETIME, MAX_LIFETIME);
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
            main.startSize = new ParticleSystem.MinMaxCurve(MIN_SIZE, MAX_SIZE);
            main.startColor = BLOOD_COLOR;
            main.gravityModifier = GRAVITY_MODIFIER;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = false;
            main.stopAction = ParticleSystemStopAction.Destroy;

            var emission = _particleSystem.emission;
            emission.enabled = true;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, MIN_PARTICLES, MAX_PARTICLES)
            });

            var shape = _particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = CONE_ANGLE;
            shape.radius = 0.1f;
            shape.rotation = new Vector3(-90f, 0f, 0f);

            var renderer = GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = CreateBloodMaterial();
            }
        }

        private Material CreateBloodMaterial()
        {
            Shader shader = Shader.Find("Particles/Standard Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            }
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            Material mat = new Material(shader);
            mat.color = BLOOD_COLOR;
            return mat;
        }

        public void Play()
        {
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
        }
    }
}

