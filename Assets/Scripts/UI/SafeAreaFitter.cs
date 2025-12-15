using UnityEngine;

namespace ShieldWall.UI
{
    /// <summary>
    /// MOB-040: Safe Area Fitter for mobile devices with notches/cutouts
    /// Adjusts RectTransform to stay within Screen.safeArea bounds
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class SafeAreaFitter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _conformX = true;
        [SerializeField] private bool _conformY = true;
        [SerializeField] private bool _updateOnScreenChange = true;
        
        [Header("Debug")]
        [SerializeField] private bool _logSafeArea = true;

        private RectTransform _rectTransform;
        private Rect _lastSafeArea = Rect.zero;
        private Vector2Int _lastScreenSize = Vector2Int.zero;
        private ScreenOrientation _lastOrientation = ScreenOrientation.Unknown;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            if (_updateOnScreenChange)
            {
                // Check if screen configuration changed
                if (HasScreenChanged())
                {
                    ApplySafeArea();
                }
            }
        }

        private bool HasScreenChanged()
        {
            // Check for screen size change
            if (Screen.width != _lastScreenSize.x || Screen.height != _lastScreenSize.y)
            {
                return true;
            }

            // Check for orientation change
            if (Screen.orientation != _lastOrientation)
            {
                return true;
            }

            // Check for safe area change (device rotation can change safe area)
            if (Screen.safeArea != _lastSafeArea)
            {
                return true;
            }

            return false;
        }

        [ContextMenu("Apply Safe Area")]
        public void ApplySafeArea()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            // Convert safe area to normalized coordinates (0-1 range)
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= screenSize.x;
            anchorMin.y /= screenSize.y;
            anchorMax.x /= screenSize.x;
            anchorMax.y /= screenSize.y;

            // Apply conformance flags
            if (!_conformX)
            {
                anchorMin.x = 0f;
                anchorMax.x = 1f;
            }

            if (!_conformY)
            {
                anchorMin.y = 0f;
                anchorMax.y = 1f;
            }

            // Apply anchors
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;

            // Reset offsets to match anchors exactly
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            // Cache values
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;

            if (_logSafeArea)
            {
                Debug.Log($"[SafeAreaFitter] Applied safe area on {gameObject.name}");
                Debug.Log($"  Screen: {screenSize}");
                Debug.Log($"  Safe Area: {safeArea}");
                Debug.Log($"  Anchors: Min={anchorMin}, Max={anchorMax}");
            }
        }

#if UNITY_EDITOR
        // Preview in editor
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                ApplySafeArea();
            }
        }
#endif
    }
}

