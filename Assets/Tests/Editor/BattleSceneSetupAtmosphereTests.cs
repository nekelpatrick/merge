using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.IO;

namespace ShieldWall.Tests.Editor
{
    [TestFixture]
    public class BattleSceneSetupAtmosphereTests
    {
        private const string VolumeProfilePath = "Assets/Settings/BattleVolumeProfile.asset";
        private const string GroundMaterialPath = "Assets/Art/Materials/Environment/Ground.mat";

        [SetUp]
        public void SetUp()
        {
            CleanupTestArtifacts();
        }

        [TearDown]
        public void TearDown()
        {
            CleanupTestArtifacts();
        }

        private void CleanupTestArtifacts()
        {
            if (File.Exists(VolumeProfilePath))
            {
                AssetDatabase.DeleteAsset(VolumeProfilePath);
            }
            if (File.Exists(GroundMaterialPath))
            {
                AssetDatabase.DeleteAsset(GroundMaterialPath);
            }
            
            var ground = GameObject.Find("Ground");
            if (ground != null)
            {
                Object.DestroyImmediate(ground);
            }
            
            var volume = GameObject.Find("PostProcessVolume");
            if (volume != null)
            {
                Object.DestroyImmediate(volume);
            }
        }

        [Test]
        public void CreateVolumeProfile_CreatesAssetAtCorrectPath()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            
            Assert.IsNotNull(profile, "VolumeProfile should be created at expected path");
        }

        [Test]
        public void CreateVolumeProfile_HasColorAdjustments()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            
            Assert.IsTrue(profile.Has<ColorAdjustments>(), "Profile should have ColorAdjustments");
        }

        [Test]
        public void CreateVolumeProfile_HasVignette()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            
            Assert.IsTrue(profile.Has<Vignette>(), "Profile should have Vignette");
        }

        [Test]
        public void CreateVolumeProfile_HasBloom()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            
            Assert.IsTrue(profile.Has<Bloom>(), "Profile should have Bloom");
        }

        [Test]
        public void CreateVolumeProfile_HasFilmGrain()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            
            Assert.IsTrue(profile.Has<FilmGrain>(), "Profile should have FilmGrain");
        }

        [Test]
        public void CreateVolumeProfile_VignetteHasCorrectIntensity()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            profile.TryGet<Vignette>(out var vignette);
            
            Assert.AreEqual(0.25f, vignette.intensity.value, 0.01f, "Vignette intensity should be 0.25");
        }

        [Test]
        public void CreateVolumeProfile_ColorAdjustmentsHasCorrectSaturation()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            profile.TryGet<ColorAdjustments>(out var colorAdjustments);
            
            Assert.AreEqual(-15f, colorAdjustments.saturation.value, 0.01f, "Saturation should be -15");
        }

        [Test]
        public void CreateVolumeProfile_BloomHasCorrectThreshold()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            
            var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            profile.TryGet<Bloom>(out var bloom);
            
            Assert.AreEqual(1.0f, bloom.threshold.value, 0.01f, "Bloom threshold should be 1.0");
        }

        [Test]
        public void SetupBattleLighting_CreatesDirectionalLight()
        {
            var existingLights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var light in existingLights)
            {
                if (light.type == LightType.Directional)
                {
                    Object.DestroyImmediate(light.gameObject);
                }
            }

            ShieldWall.Editor.BattleSceneSetup.SetupBattleLighting();

            var sun = Object.FindFirstObjectByType<Light>();
            
            Assert.IsNotNull(sun, "Directional light should be created");
            Assert.AreEqual(LightType.Directional, sun.type, "Light should be directional");
        }

        [Test]
        public void SetupBattleLighting_HasCorrectIntensity()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupBattleLighting();

            var sun = Object.FindFirstObjectByType<Light>();
            
            Assert.AreEqual(0.8f, sun.intensity, 0.01f, "Light intensity should be 0.8");
        }

        [Test]
        public void SetupBattleLighting_HasSoftShadows()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupBattleLighting();

            var sun = Object.FindFirstObjectByType<Light>();
            
            Assert.AreEqual(LightShadows.Soft, sun.shadows, "Light should have soft shadows");
        }

        [Test]
        public void SetupBattleLighting_HasCorrectRotation()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupBattleLighting();

            var sun = Object.FindFirstObjectByType<Light>();
            var expectedRotation = Quaternion.Euler(50f, -30f, 0f);
            
            Assert.AreEqual(expectedRotation.eulerAngles.x, sun.transform.rotation.eulerAngles.x, 0.1f, "X rotation should be 50");
            Assert.AreEqual(expectedRotation.eulerAngles.y, sun.transform.rotation.eulerAngles.y, 0.1f, "Y rotation should be -30 (330)");
        }

        [Test]
        public void CreateGroundPlane_CreatesMaterial()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateGroundPlane();
            
            var material = AssetDatabase.LoadAssetAtPath<Material>(GroundMaterialPath);
            
            Assert.IsNotNull(material, "Ground material should be created");
        }

        [Test]
        public void CreateGroundPlane_CreatesGroundGameObject()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateGroundPlane();
            
            var ground = GameObject.Find("Ground");
            
            Assert.IsNotNull(ground, "Ground GameObject should be created");
        }

        [Test]
        public void CreateGroundPlane_HasCorrectScale()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateGroundPlane();
            
            var ground = GameObject.Find("Ground");
            
            Assert.AreEqual(50f, ground.transform.localScale.x, 0.01f, "Ground X scale should be 50");
            Assert.AreEqual(1f, ground.transform.localScale.y, 0.01f, "Ground Y scale should be 1");
            Assert.AreEqual(50f, ground.transform.localScale.z, 0.01f, "Ground Z scale should be 50");
        }

        [Test]
        public void CreateGroundPlane_HasCorrectLayer()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupLayers();
            ShieldWall.Editor.BattleSceneSetup.CreateGroundPlane();
            
            var ground = GameObject.Find("Ground");
            
            Assert.AreEqual(9, ground.layer, "Ground should be on layer 9 (Environment)");
        }

        [Test]
        public void CreateGroundPlane_IsStatic()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateGroundPlane();
            
            var ground = GameObject.Find("Ground");
            
            Assert.IsTrue(ground.isStatic, "Ground should be marked as static");
        }

        [Test]
        public void CreateGroundPlane_MaterialHasCorrectColor()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateGroundPlane();
            
            var material = AssetDatabase.LoadAssetAtPath<Material>(GroundMaterialPath);
            var expectedColor = new Color(0.29f, 0.22f, 0.16f);
            var actualColor = material.GetColor("_BaseColor");
            
            Assert.AreEqual(expectedColor.r, actualColor.r, 0.01f, "Material red should match mud brown");
            Assert.AreEqual(expectedColor.g, actualColor.g, 0.01f, "Material green should match mud brown");
            Assert.AreEqual(expectedColor.b, actualColor.b, 0.01f, "Material blue should match mud brown");
        }

        [Test]
        public void SetupLayers_ConfiguresPlayerViewLayer()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupLayers();
            
            string layerName = LayerMask.LayerToName(6);
            
            Assert.AreEqual("PlayerView", layerName, "Layer 6 should be named PlayerView");
        }

        [Test]
        public void SetupLayers_ConfiguresBrothersLayer()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupLayers();
            
            string layerName = LayerMask.LayerToName(7);
            
            Assert.AreEqual("Brothers", layerName, "Layer 7 should be named Brothers");
        }

        [Test]
        public void SetupLayers_ConfiguresEnemiesLayer()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupLayers();
            
            string layerName = LayerMask.LayerToName(8);
            
            Assert.AreEqual("Enemies", layerName, "Layer 8 should be named Enemies");
        }

        [Test]
        public void SetupLayers_ConfiguresEnvironmentLayer()
        {
            ShieldWall.Editor.BattleSceneSetup.SetupLayers();
            
            string layerName = LayerMask.LayerToName(9);
            
            Assert.AreEqual("Environment", layerName, "Layer 9 should be named Environment");
        }

        [Test]
        public void AddVolumeToScene_CreatesVolumeGameObject()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            ShieldWall.Editor.BattleSceneSetup.AddVolumeToScene();
            
            var volumeGO = GameObject.Find("PostProcessVolume");
            
            Assert.IsNotNull(volumeGO, "PostProcessVolume GameObject should be created");
        }

        [Test]
        public void AddVolumeToScene_VolumeIsGlobal()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            ShieldWall.Editor.BattleSceneSetup.AddVolumeToScene();
            
            var volume = Object.FindFirstObjectByType<Volume>();
            
            Assert.IsTrue(volume.isGlobal, "Volume should be global");
        }

        [Test]
        public void AddVolumeToScene_VolumeHasProfile()
        {
            ShieldWall.Editor.BattleSceneSetup.CreateVolumeProfile();
            ShieldWall.Editor.BattleSceneSetup.AddVolumeToScene();
            
            var volume = Object.FindFirstObjectByType<Volume>();
            
            Assert.IsNotNull(volume.profile, "Volume should have profile assigned");
        }

        [Test]
        public void AddVolumeToScene_CreatesProfileIfMissing()
        {
            Assert.IsFalse(File.Exists(VolumeProfilePath), "Profile should not exist before test");
            
            ShieldWall.Editor.BattleSceneSetup.AddVolumeToScene();
            
            Assert.IsTrue(File.Exists(VolumeProfilePath), "Profile should be created automatically");
        }
    }
}

