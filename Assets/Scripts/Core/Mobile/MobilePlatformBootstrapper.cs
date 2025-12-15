using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Mobile
{
    /// <summary>
    /// MOB-030/MOB-053: Runtime mobile platform settings bootstrap
    /// Applies FPS targets, screen timeout, and quality profiles
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class MobilePlatformBootstrapper : MonoBehaviour
    {
        [Header("Quality Profiles")]
        [SerializeField] private MobileQualityProfileSO _androidDefaultProfile;
        [SerializeField] private MobileQualityProfileSO _iOSDefaultProfile;
        [SerializeField] private bool _allowProfileOverride = true;
        
        [Header("Frame Rate Settings (Fallback if no profile)")]
        [SerializeField] private int _targetFrameRate = 60;
        [SerializeField] private bool _disableVSync = true;
        
        [Header("Screen Settings")]
        [SerializeField] private bool _preventSleepDuringGameplay = true;
        [SerializeField] private int _sleepTimeout = -1; // Never sleep
        
        [Header("Debug")]
        [SerializeField] private bool _logSettings = true;

        private const string QUALITY_PROFILE_PREF_KEY = "MobileQualityProfile";
        
        public MobileQualityProfileSO CurrentProfile { get; private set; }

        private void Awake()
        {
            // Only apply on mobile platforms
            if (IsMobilePlatform())
            {
                ApplyPlatformSettings();
            }
            else if (_logSettings)
            {
                Debug.Log("[MobilePlatform] Not a mobile platform, skipping mobile-specific settings");
            }
        }

        private void ApplyPlatformSettings()
        {
            // Load and apply quality profile
            LoadAndApplyQualityProfile();
            
            // Apply screen settings (not in quality profile)
            if (_preventSleepDuringGameplay)
            {
                Screen.sleepTimeout = _sleepTimeout;
            }
            
            // Enforce portrait orientation at runtime (portrait-only mobile game)
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            
            if (_logSettings)
            {
                LogPlatformInfo();
            }
            
            // Fire optional event for other systems to respond
            GameEvents.RaisePlatformSettingsApplied();
        }

        private void LoadAndApplyQualityProfile()
        {
            MobileQualityProfileSO profileToApply = null;

            // Check for saved profile preference
            if (_allowProfileOverride && PlayerPrefs.HasKey(QUALITY_PROFILE_PREF_KEY))
            {
                string savedProfileName = PlayerPrefs.GetString(QUALITY_PROFILE_PREF_KEY);
                profileToApply = LoadProfileByName(savedProfileName);
                
                if (profileToApply != null && _logSettings)
                {
                    Debug.Log($"[MobilePlatform] Loaded saved profile: {savedProfileName}");
                }
            }

            // Fall back to platform default
            if (profileToApply == null)
            {
                profileToApply = GetDefaultProfileForPlatform();
            }

            // Apply profile if available
            if (profileToApply != null)
            {
                CurrentProfile = profileToApply;
                profileToApply.Apply();
            }
            else
            {
                // Fallback to manual settings if no profile
                Application.targetFrameRate = _targetFrameRate;
                if (_disableVSync)
                {
                    QualitySettings.vSyncCount = 0;
                }
                
                if (_logSettings)
                {
                    Debug.LogWarning("[MobilePlatform] No quality profile found, using fallback settings");
                }
            }
        }

        private MobileQualityProfileSO GetDefaultProfileForPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return _androidDefaultProfile;
                case RuntimePlatform.IPhonePlayer:
                    return _iOSDefaultProfile;
                default:
                    return null;
            }
        }

        private MobileQualityProfileSO LoadProfileByName(string profileName)
        {
            // Try to load from Resources (for runtime switching)
            var profile = Resources.Load<MobileQualityProfileSO>($"MobileQuality/{profileName}");
            return profile;
        }

        private void LogPlatformInfo()
        {
            Debug.Log($"[MobilePlatform] Platform: {Application.platform}");
            Debug.Log($"[MobilePlatform] Device Model: {SystemInfo.deviceModel}");
            Debug.Log($"[MobilePlatform] OS: {SystemInfo.operatingSystem}");
            Debug.Log($"[MobilePlatform] GPU: {SystemInfo.graphicsDeviceName}");
            Debug.Log($"[MobilePlatform] Memory: {SystemInfo.systemMemorySize} MB");
            Debug.Log($"[MobilePlatform] Screen: {Screen.width}x{Screen.height} @ {Screen.dpi} DPI");
            Debug.Log($"[MobilePlatform] Target FPS: {Application.targetFrameRate}");
            Debug.Log($"[MobilePlatform] VSync: {QualitySettings.vSyncCount}");
            Debug.Log($"[MobilePlatform] Sleep Timeout: {Screen.sleepTimeout}");
        }

        private bool IsMobilePlatform()
        {
            return Application.platform == RuntimePlatform.Android ||
                   Application.platform == RuntimePlatform.IPhonePlayer;
        }

        public void ApplyProfile(MobileQualityProfileSO profile)
        {
            if (profile == null)
            {
                Debug.LogError("[MobilePlatform] Cannot apply null quality profile");
                return;
            }

            CurrentProfile = profile;
            profile.Apply();

            // Save preference
            PlayerPrefs.SetString(QUALITY_PROFILE_PREF_KEY, profile.profileName);
            PlayerPrefs.Save();

            if (_logSettings)
            {
                Debug.Log($"[MobilePlatform] Applied and saved profile: {profile.profileName}");
            }
        }

        public void SetTargetFrameRate(int fps)
        {
            _targetFrameRate = fps;
            Application.targetFrameRate = fps;
            
            if (_logSettings)
            {
                Debug.Log($"[MobilePlatform] Frame rate changed to {fps} FPS");
            }
        }

        public void EnableSleep(bool enable)
        {
            if (enable)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
            
            if (_logSettings)
            {
                Debug.Log($"[MobilePlatform] Screen sleep {(enable ? "enabled" : "disabled")}");
            }
        }
    }
}

