using UnityEngine;

namespace ShieldWall.Data
{
    /// <summary>
    /// MOB-050: Mobile Quality Profile ScriptableObject
    /// Data-driven quality settings for different mobile device tiers
    /// </summary>
    [CreateAssetMenu(fileName = "MobileQualityProfile_", menuName = "ShieldWall/Mobile/Quality Profile")]
    public class MobileQualityProfileSO : ScriptableObject
    {
        [Header("Profile Info")]
        public string profileName = "Medium";
        [TextArea(2, 4)]
        public string description = "Balanced quality for mid-tier devices";
        public RuntimePlatform targetPlatform = RuntimePlatform.Android;

        [Header("Rendering")]
        [Range(0.5f, 1.0f)]
        public float renderScale = 0.85f;
        public int msaaQuality = 2; // 0, 2, 4, 8
        public bool enableHDR = false;
        
        [Header("Shadows")]
        public bool enableShadows = true;
        public UnityEngine.Rendering.Universal.ShadowResolution shadowResolution = 
            UnityEngine.Rendering.Universal.ShadowResolution._1024;
        [Range(10f, 50f)]
        public float shadowDistance = 20f;
        
        [Header("Post-Processing")]
        public bool enablePostProcessing = true;
        public bool enableBloom = false;
        public bool enableVignette = true;
        public bool enableChromaticAberration = true;
        public bool enableColorGrading = true;
        
        [Header("VFX")]
        [Range(0.25f, 1.0f)]
        public float particleMultiplier = 1.0f; // Scale particle emission rates
        public int maxParticlesPerEffect = 50;
        public bool enableBloodVFX = true;
        public bool enableDismemberment = true;
        
        [Header("Performance")]
        public int targetFrameRate = 60;
        public bool enableVSync = false;
        [Range(1, 4)]
        public int pixelLightCount = 1;
        
        [Header("Audio")]
        public bool forceMono = false;
        [Range(0.5f, 1.0f)]
        public float audioQualityScale = 1.0f;

        /// <summary>
        /// Apply this profile's settings to Unity's QualitySettings and URP
        /// </summary>
        public void Apply()
        {
            // Frame rate
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = enableVSync ? 1 : 0;
            
            // Lighting
            QualitySettings.pixelLightCount = pixelLightCount;
            
            // Shadows
            if (enableShadows)
            {
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.shadowResolution = ConvertShadowResolution(shadowResolution);
                QualitySettings.shadowDistance = shadowDistance;
            }
            else
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            
            // Anti-aliasing (applied at URP asset level, but we store preference)
            QualitySettings.antiAliasing = msaaQuality;
            
            Debug.Log($"[MobileQuality] Applied profile: {profileName}");
            Debug.Log($"  Render Scale: {renderScale}");
            Debug.Log($"  MSAA: {msaaQuality}x");
            Debug.Log($"  Shadows: {enableShadows} ({shadowResolution})");
            Debug.Log($"  Post-Processing: {enablePostProcessing}");
            Debug.Log($"  Target FPS: {targetFrameRate}");
        }

        private UnityEngine.ShadowResolution ConvertShadowResolution(
            UnityEngine.Rendering.Universal.ShadowResolution urpResolution)
        {
            switch (urpResolution)
            {
                case UnityEngine.Rendering.Universal.ShadowResolution._256:
                    return UnityEngine.ShadowResolution.Low;
                case UnityEngine.Rendering.Universal.ShadowResolution._512:
                case UnityEngine.Rendering.Universal.ShadowResolution._1024:
                    return UnityEngine.ShadowResolution.Medium;
                case UnityEngine.Rendering.Universal.ShadowResolution._2048:
                case UnityEngine.Rendering.Universal.ShadowResolution._4096:
                    return UnityEngine.ShadowResolution.High;
                default:
                    return UnityEngine.ShadowResolution.Medium;
            }
        }

        public string GetProfileSummary()
        {
            return $"{profileName} | {renderScale:F2}x | {msaaQuality}xMSAA | {targetFrameRate}fps | " +
                   $"Shadows:{(enableShadows ? shadowResolution.ToString() : "Off")} | " +
                   $"Post:{(enablePostProcessing ? "On" : "Off")}";
        }
    }
}

