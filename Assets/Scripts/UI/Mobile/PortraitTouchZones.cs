using UnityEngine;

namespace ShieldWall.UI.Mobile
{
    /// <summary>
    /// Validates and enforces touch-friendly zones for portrait play
    /// Optimizes UI placement for one-handed thumb reach
    /// </summary>
    public class PortraitTouchZones : MonoBehaviour
    {
        [Header("Zone Definitions (normalized 0-1)")]
        [Tooltip("Top boundary of primary thumb zone (bottom 40% of screen)")]
        [SerializeField] private float _primaryZoneTop = 0.4f;
        
        [Tooltip("Top boundary of secondary zone (middle 20% of screen)")]
        [SerializeField] private float _secondaryZoneTop = 0.6f;
        
        [Header("Minimum Touch Sizes (pixels at 1080x1920)")]
        [SerializeField] private Vector2 _minPrimaryButton = new Vector2(120, 100);
        [SerializeField] private Vector2 _minSecondaryButton = new Vector2(100, 80);
        [SerializeField] private Vector2 _minDieSize = new Vector2(100, 100);
        
        [Header("Debug Visualization")]
        [SerializeField] private bool _showZoneGizmos = false;
        [SerializeField] private Color _primaryZoneColor = new Color(0, 1, 0, 0.3f);
        [SerializeField] private Color _secondaryZoneColor = new Color(1, 1, 0, 0.2f);
        [SerializeField] private Color _viewOnlyZoneColor = new Color(1, 0, 0, 0.1f);

        /// <summary>
        /// Checks if a screen position is in the primary thumb zone (easiest to reach)
        /// </summary>
        public bool IsInPrimaryZone(Vector2 screenPosition)
        {
            float normalizedY = screenPosition.y / Screen.height;
            return normalizedY <= _primaryZoneTop;
        }

        /// <summary>
        /// Checks if a screen position is in the secondary zone (reachable but not ideal)
        /// </summary>
        public bool IsInSecondaryZone(Vector2 screenPosition)
        {
            float normalizedY = screenPosition.y / Screen.height;
            return normalizedY > _primaryZoneTop && normalizedY <= _secondaryZoneTop;
        }

        /// <summary>
        /// Checks if a screen position is in the view-only zone (hard to reach)
        /// </summary>
        public bool IsInViewOnlyZone(Vector2 screenPosition)
        {
            float normalizedY = screenPosition.y / Screen.height;
            return normalizedY > _secondaryZoneTop;
        }

        /// <summary>
        /// Validates if a UI element meets minimum touch target size for its zone
        /// </summary>
        public bool ValidateTouchTarget(RectTransform rectTransform, bool isPrimaryAction)
        {
            if (rectTransform == null) return false;

            Vector2 size = rectTransform.rect.size;
            Vector2 minSize = isPrimaryAction ? _minPrimaryButton : _minSecondaryButton;

            return size.x >= minSize.x && size.y >= minSize.y;
        }

        /// <summary>
        /// Gets the recommended minimum size for a button in a given zone
        /// </summary>
        public Vector2 GetRecommendedSize(Vector2 screenPosition)
        {
            if (IsInPrimaryZone(screenPosition))
                return _minPrimaryButton;
            else if (IsInSecondaryZone(screenPosition))
                return _minSecondaryButton;
            else
                return _minSecondaryButton; // View-only zone rarely has buttons
        }

        /// <summary>
        /// Returns a description of which zone a position is in (for debugging)
        /// </summary>
        public string GetZoneDescription(Vector2 screenPosition)
        {
            if (IsInPrimaryZone(screenPosition))
                return "Primary (Thumb-friendly)";
            else if (IsInSecondaryZone(screenPosition))
                return "Secondary (Reachable)";
            else
                return "View-only (Hard to reach)";
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!_showZoneGizmos) return;

            // Calculate zone heights in screen pixels
            float primaryY = Screen.height * _primaryZoneTop;
            float secondaryY = Screen.height * _secondaryZoneTop;

            // Draw primary zone (bottom)
            GUI.color = _primaryZoneColor;
            GUI.DrawTexture(new Rect(0, Screen.height - primaryY, Screen.width, primaryY), Texture2D.whiteTexture);

            // Draw secondary zone (middle)
            GUI.color = _secondaryZoneColor;
            GUI.DrawTexture(new Rect(0, Screen.height - secondaryY, Screen.width, secondaryY - primaryY), Texture2D.whiteTexture);

            // Draw view-only zone (top)
            GUI.color = _viewOnlyZoneColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height - secondaryY), Texture2D.whiteTexture);

            // Draw zone labels
            GUI.color = Color.white;
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 20;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(0, 50, Screen.width, 30), "VIEW ONLY ZONE (Top 40%)", labelStyle);
            GUI.Label(new Rect(0, Screen.height - secondaryY + 50, Screen.width, 30), "SECONDARY ZONE (Middle 20%)", labelStyle);
            GUI.Label(new Rect(0, Screen.height - primaryY / 2, Screen.width, 30), "PRIMARY ZONE (Bottom 40%)", labelStyle);
        }

        [ContextMenu("Log Touch Zone Info")]
        private void LogTouchZoneInfo()
        {
            Debug.Log($"[PortraitTouchZones] Screen: {Screen.width}x{Screen.height}");
            Debug.Log($"[PortraitTouchZones] Primary Zone: 0 - {Screen.height * _primaryZoneTop}px (bottom)");
            Debug.Log($"[PortraitTouchZones] Secondary Zone: {Screen.height * _primaryZoneTop}px - {Screen.height * _secondaryZoneTop}px");
            Debug.Log($"[PortraitTouchZones] View-Only Zone: {Screen.height * _secondaryZoneTop}px - {Screen.height}px (top)");
            Debug.Log($"[PortraitTouchZones] Min Primary Button: {_minPrimaryButton}");
            Debug.Log($"[PortraitTouchZones] Min Secondary Button: {_minSecondaryButton}");
            Debug.Log($"[PortraitTouchZones] Min Die Size: {_minDieSize}");
        }
#endif
    }
}
