using System.Collections;
using UnityEngine;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    public class EnemyVisualInstance : MonoBehaviour
    {
        private const float BODY_HEIGHT = 2f;
        private const float BODY_RADIUS = 0.3f;
        private const float HEAD_SIZE = 0.4f;
        private const float DEATH_DURATION = 0.3f;
        private const float HIT_FLASH_DURATION = 0.1f;

        public EnemySO EnemyData { get; private set; }

        private Transform _body;
        private Transform _head;
        private Material _bodyMaterial;
        private Material _headMaterial;
        private Color _baseColor;
        private BloodBurstVFX _bloodBurstPrefab;
        private bool _isDying;
        
        private ModularCharacterBuilder _modularBuilder;
        private DismembermentController _dismembermentController;

        public void Initialize(EnemySO enemy, Color color, BloodBurstVFX bloodBurstPrefab)
        {
            EnemyData = enemy;
            _baseColor = color;
            _bloodBurstPrefab = bloodBurstPrefab;

            _modularBuilder = GetComponent<ModularCharacterBuilder>();
            _dismembermentController = GetComponent<DismembermentController>();
            
            if (_modularBuilder != null)
            {
                return;
            }

            CreateBody(color);
            CreateHead(color);
        }

        private void CreateBody(Color color)
        {
            GameObject bodyGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bodyGO.name = "Body";
            bodyGO.transform.SetParent(transform);
            bodyGO.transform.localPosition = new Vector3(0, BODY_HEIGHT / 2f, 0);
            bodyGO.transform.localScale = new Vector3(BODY_RADIUS * 2f, BODY_HEIGHT / 2f, BODY_RADIUS * 2f);

            _body = bodyGO.transform;
            _bodyMaterial = CreateUnlitMaterial(color);
            bodyGO.GetComponent<Renderer>().material = _bodyMaterial;

            var collider = bodyGO.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }

        private void CreateHead(Color color)
        {
            GameObject headGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            headGO.name = "Head";
            headGO.transform.SetParent(transform);
            headGO.transform.localPosition = new Vector3(0, BODY_HEIGHT + HEAD_SIZE / 2f, 0);
            headGO.transform.localScale = Vector3.one * HEAD_SIZE;

            _head = headGO.transform;
            _headMaterial = CreateUnlitMaterial(color);
            headGO.GetComponent<Renderer>().material = _headMaterial;

            var collider = headGO.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }

        private Material CreateUnlitMaterial(Color color)
        {
            Shader unlitShader = Shader.Find("Unlit/Color");
            if (unlitShader == null)
            {
                unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
            }

            Material mat = new Material(unlitShader);
            mat.color = color;
            return mat;
        }

        public void PlayDeathAnimation()
        {
            if (_isDying) return;
            _isDying = true;
            StartCoroutine(DeathSequence());
        }
        
        public void PlayDeathAnimationWithDismemberment(DismembermentType type)
        {
            if (_isDying) return;
            _isDying = true;
            
            if (_dismembermentController != null)
            {
                _dismembermentController.TriggerDismemberment(type);
                StartCoroutine(DismemberedDeathSequence());
            }
            else
            {
                StartCoroutine(DeathSequence());
            }
        }

        public void PlayHitReaction()
        {
            if (_isDying) return;
            StartCoroutine(HitFlash());
        }

        private IEnumerator DeathSequence()
        {
            Vector3 startScale = transform.localScale;
            Quaternion startRotation = transform.localRotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(90f, 0f, 0f);
            
            float elapsed = 0f;
            while (elapsed < DEATH_DURATION)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / DEATH_DURATION;
                float easeT = t * t;
                
                transform.localScale = Vector3.Lerp(startScale, startScale * 0.1f, easeT);
                transform.localRotation = Quaternion.Slerp(startRotation, endRotation, easeT);
                
                yield return null;
            }

            SpawnBloodBurst();
            
            yield return new WaitForSeconds(0.2f);
            
            Destroy(gameObject);
        }

        private void SpawnBloodBurst()
        {
            if (_bloodBurstPrefab != null)
            {
                Vector3 burstPosition = transform.position + Vector3.up * (BODY_HEIGHT / 2f);
                var bloodBurst = Instantiate(_bloodBurstPrefab, burstPosition, Quaternion.identity);
                bloodBurst.Play();
            }
        }

        private IEnumerator HitFlash()
        {
            Color flashColor = Color.white;
            
            if (_bodyMaterial != null) _bodyMaterial.color = flashColor;
            if (_headMaterial != null) _headMaterial.color = flashColor;
            
            yield return new WaitForSeconds(HIT_FLASH_DURATION);
            
            if (_bodyMaterial != null) _bodyMaterial.color = _baseColor;
            if (_headMaterial != null) _headMaterial.color = _baseColor;
        }
        
        private IEnumerator DismemberedDeathSequence()
        {
            Quaternion startRotation = transform.localRotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(90f, 0f, 0f);
            
            float elapsed = 0f;
            while (elapsed < DEATH_DURATION)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / DEATH_DURATION;
                float easeT = t * t;
                
                transform.localRotation = Quaternion.Slerp(startRotation, endRotation, easeT);
                
                yield return null;
            }
            
            yield return new WaitForSeconds(0.2f);
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_bodyMaterial != null) Destroy(_bodyMaterial);
            if (_headMaterial != null) Destroy(_headMaterial);
        }
    }
}

