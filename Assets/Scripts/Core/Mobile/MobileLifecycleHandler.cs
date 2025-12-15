using UnityEngine;
using ShieldWall.Core;
using ShieldWall.UI;

namespace ShieldWall.Mobile
{
    /// <summary>
    /// MOB-031: Application lifecycle handling for mobile platforms
    /// Detects app backgrounding/foregrounding and triggers pause/resume
    /// </summary>
    public class MobileLifecycleHandler : MonoBehaviour
    {
        [Header("Auto-Pause Settings")]
        [SerializeField] private bool _autoPauseOnBackground = true;
        [SerializeField] private bool _autoResumeOnForeground = false; // Keep paused for safety
        
        [Header("Debug")]
        [SerializeField] private bool _logLifecycleEvents = true;

        private bool _wasPausedBySystem = false;

        private void Awake()
        {
            // Only active on mobile platforms
            if (!IsMobilePlatform())
            {
                enabled = false;
                return;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // App is going to background
                HandleAppBackground();
            }
            else
            {
                // App is coming to foreground
                HandleAppForeground();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // iOS calls OnApplicationFocus instead of OnApplicationPause in some cases
            if (!hasFocus)
            {
                if (_logLifecycleEvents)
                {
                    Debug.Log("[MobileLifecycle] App lost focus");
                }
            }
            else
            {
                if (_logLifecycleEvents)
                {
                    Debug.Log("[MobileLifecycle] App gained focus");
                }
            }
        }

        private void HandleAppBackground()
        {
            if (_logLifecycleEvents)
            {
                Debug.Log("[MobileLifecycle] App going to background");
            }

            if (_autoPauseOnBackground)
            {
                // Fire event for pause menu to respond
                GameEvents.RaiseApplicationPauseRequested();
                _wasPausedBySystem = true;
                
                if (_logLifecycleEvents)
                {
                    Debug.Log("[MobileLifecycle] Auto-pause requested");
                }
            }

            // Save any critical state here if needed
            // PlayerPrefs.Save() is called automatically by Unity on mobile
        }

        private void HandleAppForeground()
        {
            if (_logLifecycleEvents)
            {
                Debug.Log("[MobileLifecycle] App returning to foreground");
            }

            if (_autoResumeOnForeground && _wasPausedBySystem)
            {
                // Optionally auto-resume (usually we keep paused for user safety)
                GameEvents.RaiseApplicationResumeRequested();
                _wasPausedBySystem = false;
                
                if (_logLifecycleEvents)
                {
                    Debug.Log("[MobileLifecycle] Auto-resume requested");
                }
            }
            else
            {
                if (_logLifecycleEvents)
                {
                    Debug.Log("[MobileLifecycle] Remaining paused (user must manually resume)");
                }
            }
        }

        private bool IsMobilePlatform()
        {
            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer;
        }

        // Called when app is about to quit (Android back button, iOS swipe-up)
        private void OnApplicationQuit()
        {
            if (_logLifecycleEvents)
            {
                Debug.Log("[MobileLifecycle] App quitting");
            }
            
            // Save critical state one final time
            PlayerPrefs.Save();
        }
    }
}

