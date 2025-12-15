using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Mobile;

namespace ShieldWall.UI.Mobile
{
    /// <summary>
    /// MOB-054/MOB-055: Mobile Settings UI
    /// Allows users to switch quality profiles and toggle performance settings
    /// </summary>
    public class MobileSettingsUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MobilePlatformBootstrapper _platformBootstrapper;
        
        [Header("Quality Profile Selection")]
        [SerializeField] private MobileQualityProfileSO[] _availableProfiles;
        [SerializeField] private TMP_Dropdown _profileDropdown;
        [SerializeField] private TextMeshProUGUI _profileDescriptionText;
        
        [Header("Individual Toggles")]
        [SerializeField] private Toggle _reduceVFXToggle;
        [SerializeField] private Toggle _disablePostProcessToggle;
        [SerializeField] private Toggle _reduceShadowsToggle;
        
        [Header("Frame Rate Selection")]
        [SerializeField] private TMP_Dropdown _fpsDropdown;
        
        [Header("Debug")]
        [SerializeField] private TextMeshProUGUI _currentProfileText;

        private const string VFX_TOGGLE_KEY = "MobileReduceVFX";
        private const string POST_TOGGLE_KEY = "MobileDisablePost";
        private const string SHADOWS_TOGGLE_KEY = "MobileReduceShadows";

        private void Start()
        {
            if (_platformBootstrapper == null)
            {
                _platformBootstrapper = FindFirstObjectByType<MobilePlatformBootstrapper>();
            }

            SetupProfileDropdown();
            SetupFPSDropdown();
            LoadSavedSettings();
            UpdateCurrentProfileDisplay();
        }

        private void OnEnable()
        {
            if (_profileDropdown != null)
                _profileDropdown.onValueChanged.AddListener(OnProfileSelected);
            if (_fpsDropdown != null)
                _fpsDropdown.onValueChanged.AddListener(OnFPSSelected);
            if (_reduceVFXToggle != null)
                _reduceVFXToggle.onValueChanged.AddListener(OnVFXToggleChanged);
            if (_disablePostProcessToggle != null)
                _disablePostProcessToggle.onValueChanged.AddListener(OnPostProcessToggleChanged);
            if (_reduceShadowsToggle != null)
                _reduceShadowsToggle.onValueChanged.AddListener(OnShadowsToggleChanged);
        }

        private void OnDisable()
        {
            if (_profileDropdown != null)
                _profileDropdown.onValueChanged.RemoveListener(OnProfileSelected);
            if (_fpsDropdown != null)
                _fpsDropdown.onValueChanged.RemoveListener(OnFPSSelected);
            if (_reduceVFXToggle != null)
                _reduceVFXToggle.onValueChanged.RemoveListener(OnVFXToggleChanged);
            if (_disablePostProcessToggle != null)
                _disablePostProcessToggle.onValueChanged.RemoveListener(OnPostProcessToggleChanged);
            if (_reduceShadowsToggle != null)
                _reduceShadowsToggle.onValueChanged.RemoveListener(OnShadowsToggleChanged);
        }

        private void SetupProfileDropdown()
        {
            if (_profileDropdown == null || _availableProfiles == null || _availableProfiles.Length == 0)
                return;

            _profileDropdown.ClearOptions();
            
            System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();
            foreach (var profile in _availableProfiles)
            {
                if (profile != null)
                {
                    options.Add(profile.profileName);
                }
            }
            
            _profileDropdown.AddOptions(options);
        }

        private void SetupFPSDropdown()
        {
            if (_fpsDropdown == null)
                return;

            _fpsDropdown.ClearOptions();
            _fpsDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "30 FPS",
                "60 FPS"
            });
        }

        private void LoadSavedSettings()
        {
            // Load toggle states
            if (_reduceVFXToggle != null)
            {
                _reduceVFXToggle.isOn = PlayerPrefs.GetInt(VFX_TOGGLE_KEY, 0) == 1;
            }
            
            if (_disablePostProcessToggle != null)
            {
                _disablePostProcessToggle.isOn = PlayerPrefs.GetInt(POST_TOGGLE_KEY, 0) == 1;
            }
            
            if (_reduceShadowsToggle != null)
            {
                _reduceShadowsToggle.isOn = PlayerPrefs.GetInt(SHADOWS_TOGGLE_KEY, 0) == 1;
            }
        }

        private void OnProfileSelected(int index)
        {
            if (_availableProfiles == null || index < 0 || index >= _availableProfiles.Length)
                return;

            MobileQualityProfileSO selectedProfile = _availableProfiles[index];
            if (selectedProfile != null && _platformBootstrapper != null)
            {
                _platformBootstrapper.ApplyProfile(selectedProfile);
                UpdateProfileDescription(selectedProfile);
                UpdateCurrentProfileDisplay();
                
                Debug.Log($"[MobileSettings] User selected profile: {selectedProfile.profileName}");
            }
        }

        private void OnFPSSelected(int index)
        {
            int targetFPS = index == 0 ? 30 : 60;
            
            if (_platformBootstrapper != null)
            {
                _platformBootstrapper.SetTargetFrameRate(targetFPS);
            }
            
            Debug.Log($"[MobileSettings] User selected {targetFPS} FPS");
        }

        private void OnVFXToggleChanged(bool isEnabled)
        {
            PlayerPrefs.SetInt(VFX_TOGGLE_KEY, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            
            // Fire event for VFX controllers to respond
            GameEvents.RaiseMobileSettingChanged("ReduceVFX", isEnabled);
            
            Debug.Log($"[MobileSettings] Reduce VFX: {isEnabled}");
        }

        private void OnPostProcessToggleChanged(bool isEnabled)
        {
            PlayerPrefs.SetInt(POST_TOGGLE_KEY, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            
            // Fire event for post-processing controller to respond
            GameEvents.RaiseMobileSettingChanged("DisablePost", isEnabled);
            
            Debug.Log($"[MobileSettings] Disable Post-Processing: {isEnabled}");
        }

        private void OnShadowsToggleChanged(bool isEnabled)
        {
            PlayerPrefs.SetInt(SHADOWS_TOGGLE_KEY, isEnabled ? 1 : 0);
            PlayerPrefs.Save();
            
            // Apply immediately
            QualitySettings.shadows = isEnabled ? ShadowQuality.Disable : ShadowQuality.All;
            
            Debug.Log($"[MobileSettings] Reduce Shadows: {isEnabled}");
        }

        private void UpdateProfileDescription(MobileQualityProfileSO profile)
        {
            if (_profileDescriptionText != null && profile != null)
            {
                _profileDescriptionText.text = profile.description;
            }
        }

        private void UpdateCurrentProfileDisplay()
        {
            if (_currentProfileText != null && _platformBootstrapper != null)
            {
                var currentProfile = _platformBootstrapper.CurrentProfile;
                if (currentProfile != null)
                {
                    _currentProfileText.text = $"Active: {currentProfile.GetProfileSummary()}";
                }
                else
                {
                    _currentProfileText.text = "Active: Default Settings";
                }
            }
        }

        public static bool GetReduceVFX()
        {
            return PlayerPrefs.GetInt(VFX_TOGGLE_KEY, 0) == 1;
        }

        public static bool GetDisablePostProcessing()
        {
            return PlayerPrefs.GetInt(POST_TOGGLE_KEY, 0) == 1;
        }

        public static bool GetReduceShadows()
        {
            return PlayerPrefs.GetInt(SHADOWS_TOGGLE_KEY, 0) == 1;
        }
    }
}

