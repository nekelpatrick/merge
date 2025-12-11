using UnityEngine;

namespace ShieldWall.Visual
{
    public class BloodBurstVFX : MonoBehaviour
    {
        private const int MIN_PARTICLES = 50;
        private const int MAX_PARTICLES = 100;
        private const float CONE_ANGLE = 45f;
        private const float MIN_SIZE = 0.05f;
        private const float MAX_SIZE = 0.2f;
        private const float MIN_LIFETIME = 0.5f;
        private const float MAX_LIFETIME = 1.5f;
        private const float GRAVITY_MODIFIER = 1f;

        private static readonly Color BLOOD_COLOR_START = new Color(0.42f, 0.063f, 0.063f);
        private static readonly Color BLOOD_COLOR_END = new Color(0.165f, 0.02f, 0.02f);

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
            main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
            main.startSize = new ParticleSystem.MinMaxCurve(MIN_SIZE, MAX_SIZE);
            
            Gradient colorGradient = new Gradient();
            colorGradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(BLOOD_COLOR_START, 0.0f), 
                    new GradientColorKey(BLOOD_COLOR_END, 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1.0f, 0.0f), 
                    new GradientAlphaKey(0.0f, 1.0f) 
                }
            );
            main.startColor = new ParticleSystem.MinMaxGradient(colorGradient);
            
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
            
            Vector3 cameraDirection = Vector3.zero;
            if (Camera.main != null)
            {
                cameraDirection = (Camera.main.transform.position - transform.position).normalized;
            }
            
            float angleToCamera = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
            shape.rotation = new Vector3(-90f, angleToCamera, 0f);

            var collision = _particleSystem.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.dampen = 0.5f;
            collision.bounce = 0.1f;
            collision.lifetimeLoss = 0.3f;

            var renderer = GetComponent<ParticleSystemRenderer>();
            if (renderer != null)
            {
                renderer.material = CreateBloodMaterial();
                renderer.renderMode = ParticleSystemRenderMode.Billboard;
            }
        }

        private Material CreateBloodMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Particles/Standard Unlit");
            }
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            Material mat = new Material(shader);
            mat.color = BLOOD_COLOR_START;
            
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", BLOOD_COLOR_START);
            }
            
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

