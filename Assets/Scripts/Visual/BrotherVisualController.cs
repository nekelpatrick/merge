using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;

namespace ShieldWall.Visual
{
    public class BrotherVisualController : MonoBehaviour
    {
        private const float BODY_HEIGHT = 1.8f;
        private const float BODY_RADIUS = 0.25f;
        private const float HEAD_SIZE = 0.35f;
        private const float WOUNDED_LEAN_ANGLE = 15f;
        private const float DEATH_DURATION = 0.5f;
        private const float FADE_DURATION = 0.3f;

        [Header("Position Settings")]
        [SerializeField] private Vector3 _farLeftPosition = new Vector3(-3f, 0f, 1f);
        [SerializeField] private Vector3 _leftPosition = new Vector3(-1.5f, 0f, 1f);
        [SerializeField] private Vector3 _rightPosition = new Vector3(1.5f, 0f, 1f);
        [SerializeField] private Vector3 _farRightPosition = new Vector3(3f, 0f, 1f);

        [Header("Colors")]
        [SerializeField] private Color _healthyColor = new Color(0.3f, 0.3f, 0.4f);
        [SerializeField] private Color _woundedTint = new Color(0.6f, 0.2f, 0.2f);

        private readonly Dictionary<WallPosition, BrotherVisual> _brotherVisuals = new Dictionary<WallPosition, BrotherVisual>();

        private void OnEnable()
        {
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnBrotherDied += HandleBrotherDied;
        }

        private void OnDisable()
        {
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnBrotherDied -= HandleBrotherDied;
        }

        public void InitializeBrothers(Dictionary<WallPosition, ShieldBrotherSO> brotherPositions)
        {
            ClearExistingVisuals();

            foreach (var kvp in brotherPositions)
            {
                if (kvp.Key == WallPosition.Center) continue;
                if (kvp.Value == null) continue;

                CreateBrotherVisual(kvp.Key, kvp.Value);
            }
        }

        public void CreateBrotherAtPosition(WallPosition position, ShieldBrotherSO brother)
        {
            if (position == WallPosition.Center) return;
            if (brother == null) return;

            CreateBrotherVisual(position, brother);
        }

        private void CreateBrotherVisual(WallPosition position, ShieldBrotherSO brotherData)
        {
            Vector3 localPos = GetPositionForSlot(position);

            GameObject visualGO = new GameObject($"Brother_{brotherData.brotherName}");
            visualGO.transform.SetParent(transform);
            visualGO.transform.localPosition = localPos;

            var visual = new BrotherVisual
            {
                BrotherData = brotherData,
                Position = position,
                Root = visualGO.transform,
                IsWounded = false,
                IsDead = false
            };

            CreateBrotherPrimitives(visual);
            _brotherVisuals[position] = visual;
        }

        private void CreateBrotherPrimitives(BrotherVisual visual)
        {
            GameObject bodyGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bodyGO.name = "Body";
            bodyGO.transform.SetParent(visual.Root);
            bodyGO.transform.localPosition = new Vector3(0, BODY_HEIGHT / 2f, 0);
            bodyGO.transform.localScale = new Vector3(BODY_RADIUS * 2f, BODY_HEIGHT / 2f, BODY_RADIUS * 2f);

            visual.Body = bodyGO.transform;
            visual.BodyMaterial = CreateUnlitMaterial(_healthyColor);
            bodyGO.GetComponent<Renderer>().material = visual.BodyMaterial;

            var bodyCollider = bodyGO.GetComponent<Collider>();
            if (bodyCollider != null) Destroy(bodyCollider);

            GameObject headGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            headGO.name = "Head";
            headGO.transform.SetParent(visual.Root);
            headGO.transform.localPosition = new Vector3(0, BODY_HEIGHT + HEAD_SIZE / 2f, 0);
            headGO.transform.localScale = Vector3.one * HEAD_SIZE;

            visual.Head = headGO.transform;
            visual.HeadMaterial = CreateUnlitMaterial(_healthyColor);
            headGO.GetComponent<Renderer>().material = visual.HeadMaterial;

            var headCollider = headGO.GetComponent<Collider>();
            if (headCollider != null) Destroy(headCollider);
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

        private Vector3 GetPositionForSlot(WallPosition position)
        {
            return position switch
            {
                WallPosition.FarLeft => _farLeftPosition,
                WallPosition.Left => _leftPosition,
                WallPosition.Right => _rightPosition,
                WallPosition.FarRight => _farRightPosition,
                _ => Vector3.zero
            };
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            var visual = FindVisualForBrother(brother);
            if (visual == null || visual.IsDead) return;

            visual.IsWounded = true;
            ApplyWoundedState(visual);
        }

        private void HandleBrotherDied(ShieldBrotherSO brother)
        {
            var visual = FindVisualForBrother(brother);
            if (visual == null || visual.IsDead) return;

            visual.IsDead = true;
            StartCoroutine(PlayDeathAnimation(visual));
        }

        private BrotherVisual FindVisualForBrother(ShieldBrotherSO brother)
        {
            foreach (var kvp in _brotherVisuals)
            {
                if (kvp.Value.BrotherData == brother)
                {
                    return kvp.Value;
                }
            }
            return null;
        }

        private void ApplyWoundedState(BrotherVisual visual)
        {
            if (visual.BodyMaterial != null)
            {
                visual.BodyMaterial.color = _woundedTint;
            }
            if (visual.HeadMaterial != null)
            {
                visual.HeadMaterial.color = _woundedTint;
            }

            float leanDirection = visual.Position == WallPosition.FarLeft || visual.Position == WallPosition.Left ? 1f : -1f;
            visual.Root.localRotation = Quaternion.Euler(0, 0, WOUNDED_LEAN_ANGLE * leanDirection);
        }

        private IEnumerator PlayDeathAnimation(BrotherVisual visual)
        {
            Quaternion startRotation = visual.Root.localRotation;
            Quaternion endRotation = Quaternion.Euler(90f, 0f, 0f);

            float elapsed = 0f;
            while (elapsed < DEATH_DURATION)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / DEATH_DURATION;
                float easeT = t * t;

                visual.Root.localRotation = Quaternion.Slerp(startRotation, endRotation, easeT);
                yield return null;
            }

            yield return StartCoroutine(FadeOutVisual(visual));

            if (visual.Root != null)
            {
                Destroy(visual.Root.gameObject);
            }
            _brotherVisuals.Remove(visual.Position);
        }

        private IEnumerator FadeOutVisual(BrotherVisual visual)
        {
            Color bodyStartColor = visual.BodyMaterial != null ? visual.BodyMaterial.color : Color.clear;
            Color headStartColor = visual.HeadMaterial != null ? visual.HeadMaterial.color : Color.clear;

            float elapsed = 0f;
            while (elapsed < FADE_DURATION)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / FADE_DURATION;

                if (visual.BodyMaterial != null)
                {
                    visual.BodyMaterial.color = Color.Lerp(bodyStartColor, Color.clear, t);
                }
                if (visual.HeadMaterial != null)
                {
                    visual.HeadMaterial.color = Color.Lerp(headStartColor, Color.clear, t);
                }

                yield return null;
            }
        }

        private void ClearExistingVisuals()
        {
            foreach (var kvp in _brotherVisuals)
            {
                if (kvp.Value.Root != null)
                {
                    if (kvp.Value.BodyMaterial != null) Destroy(kvp.Value.BodyMaterial);
                    if (kvp.Value.HeadMaterial != null) Destroy(kvp.Value.HeadMaterial);
                    Destroy(kvp.Value.Root.gameObject);
                }
            }
            _brotherVisuals.Clear();
        }

        private void OnDestroy()
        {
            ClearExistingVisuals();
        }

        private class BrotherVisual
        {
            public ShieldBrotherSO BrotherData;
            public WallPosition Position;
            public Transform Root;
            public Transform Body;
            public Transform Head;
            public Material BodyMaterial;
            public Material HeadMaterial;
            public bool IsWounded;
            public bool IsDead;
        }
    }
}

