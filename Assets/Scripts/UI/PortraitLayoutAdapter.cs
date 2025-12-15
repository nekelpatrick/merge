using UnityEngine;

namespace ShieldWall.UI
{
    /// <summary>
    /// Adjusts UI layout for different portrait aspect ratios (9:16 to 9:21)
    /// Handles extra vertical space on tall phones with notches
    /// </summary>
    [ExecuteAlways]
    public class PortraitLayoutAdapter : MonoBehaviour
    {
        [Header("Aspect Ratio Thresholds")]
        [SerializeField] private float _standardAspect = 16f / 9f;   // 1.78 (16:9 portrait)
        [SerializeField] private float _tallAspect = 19.5f / 9f;     // 2.17 (19.5:9 tall phones)
        
        [Header("Layout Adjustments")]
        [SerializeField] private RectTransform _battleViewArea;
        [SerializeField] private RectTransform _controlsArea;
        [SerializeField] private float _extraTallPadding = 50f;
        
        [Header("Debug")]
        [SerializeField] private bool _logAdjustments = false;
        
        private float _lastAspect = 0f;

        private void Start()
        {
            AdjustForAspectRatio();
        }

        private void Update()
        {
            // Recheck aspect ratio if screen size changes
            float currentAspect = GetCurrentAspectRatio();
            if (Mathf.Abs(currentAspect - _lastAspect) > 0.01f)
            {
                AdjustForAspectRatio();
            }
        }

        [ContextMenu("Adjust for Aspect Ratio")]
        public void AdjustForAspectRatio()
        {
            float currentAspect = GetCurrentAspectRatio();
            _lastAspect = currentAspect;
            
            if (currentAspect > _tallAspect)
            {
                // Extra tall phone (19.5:9 or taller) - add padding
                ApplyTallPhoneLayout();
                
                if (_logAdjustments)
                {
                    Debug.Log($"[PortraitLayoutAdapter] Applied tall phone layout (aspect: {currentAspect:F2})");
                }
            }
            else
            {
                // Standard portrait (16:9 to 18:9)
                ApplyStandardLayout();
                
                if (_logAdjustments)
                {
                    Debug.Log($"[PortraitLayoutAdapter] Applied standard layout (aspect: {currentAspect:F2})");
                }
            }
        }

        private float GetCurrentAspectRatio()
        {
            // For portrait, height/width gives us the aspect ratio
            return (float)Screen.height / Screen.width;
        }

        private void ApplyTallPhoneLayout()
        {
            // Expand battle view to use extra vertical space
            if (_battleViewArea != null)
            {
                var offset = _battleViewArea.offsetMin;
                offset.y += _extraTallPadding;
                _battleViewArea.offsetMin = offset;
            }

            // Optionally add padding to controls area
            if (_controlsArea != null)
            {
                // Keep controls at bottom but add slight breathing room
                var offset = _controlsArea.offsetMin;
                offset.y += _extraTallPadding * 0.5f;
                _controlsArea.offsetMin = offset;
            }
        }

        private void ApplyStandardLayout()
        {
            // Reset to default offsets
            if (_battleViewArea != null)
            {
                var offset = _battleViewArea.offsetMin;
                offset.y = 0;
                _battleViewArea.offsetMin = offset;
            }

            if (_controlsArea != null)
            {
                var offset = _controlsArea.offsetMin;
                offset.y = 0;
                _controlsArea.offsetMin = offset;
            }
        }

        public static bool IsExtraTallScreen()
        {
            float aspect = (float)Screen.height / Screen.width;
            return aspect > 2.1f; // Taller than 19:9
        }
    }
}
