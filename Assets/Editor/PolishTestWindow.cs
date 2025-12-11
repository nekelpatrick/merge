using UnityEngine;
using UnityEditor;
using ShieldWall.Visual;
using ShieldWall.Audio;
using ShieldWall.Core;

namespace ShieldWall.Editor
{
    public class PolishTestWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Shield Wall Builder/Polish/Test Window", false, 500)]
        public static void ShowWindow()
        {
            var window = GetWindow<PolishTestWindow>("Polish Test");
            window.minSize = new Vector2(300, 400);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Polish Effect Tester", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use these buttons to test individual polish effects during Play mode.", MessageType.Info);
            EditorGUILayout.Space(10);

            GUI.enabled = Application.isPlaying;

            DrawCameraEffectsSection();
            DrawTimeControlSection();
            DrawVFXSection();
            DrawGameEventsSection();

            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }

        private void DrawCameraEffectsSection()
        {
            EditorGUILayout.LabelField("Camera Effects", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("Shake - Light"))
            {
                var cam = FindFirstObjectByType<CameraEffects>();
                cam?.DirectionalShake(Vector3.back, 0.1f);
            }

            if (GUILayout.Button("Shake - Medium"))
            {
                var cam = FindFirstObjectByType<CameraEffects>();
                cam?.DirectionalShake(Vector3.back, 0.2f);
            }

            if (GUILayout.Button("Shake - Heavy"))
            {
                var cam = FindFirstObjectByType<CameraEffects>();
                cam?.DirectionalShake(Vector3.back, 0.4f);
            }

            if (GUILayout.Button("Camera Punch"))
            {
                var cam = FindFirstObjectByType<CameraEffects>();
                cam?.Punch(0.1f);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawTimeControlSection()
        {
            EditorGUILayout.LabelField("Time Control", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("Hit Stop (0.05s)"))
            {
                var time = FindFirstObjectByType<TimeController>();
                time?.HitStop(0.05f);
            }

            if (GUILayout.Button("Hit Stop (0.1s)"))
            {
                var time = FindFirstObjectByType<TimeController>();
                time?.HitStop(0.1f);
            }

            if (GUILayout.Button("Slow Motion (1s at 0.3x)"))
            {
                var time = FindFirstObjectByType<TimeController>();
                time?.SlowMotion(1f, 0.3f);
            }

            if (GUILayout.Button("Reset Time Scale"))
            {
                var time = FindFirstObjectByType<TimeController>();
                time?.ResetTimeScale();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawVFXSection()
        {
            EditorGUILayout.LabelField("VFX", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("Spawn Blood"))
            {
                var vfx = FindFirstObjectByType<ImpactVFXController>();
                vfx?.SpawnBlood(new Vector3(0, 1.2f, 0.5f), Vector3.back, 2);
            }

            if (GUILayout.Button("Spawn Block Effect"))
            {
                var vfx = FindFirstObjectByType<ImpactVFXController>();
                vfx?.SpawnBlockEffect(new Vector3(0, 1f, 0.6f));
            }

            if (GUILayout.Button("Post Process Damage"))
            {
                var pp = FindFirstObjectByType<PostProcessController>();
                pp?.TriggerDamageEffect(2);
            }

            if (GUILayout.Button("Post Process Kill"))
            {
                var pp = FindFirstObjectByType<PostProcessController>();
                pp?.TriggerKillEffect();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawGameEventsSection()
        {
            EditorGUILayout.LabelField("Simulate Game Events", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.HelpBox("These fire actual GameEvents, triggering all subscribed systems.", MessageType.Warning);

            if (GUILayout.Button("Player Wounded (2 damage)"))
            {
                GameEvents.RaisePlayerWounded(2);
            }

            if (GUILayout.Button("Enemy Killed"))
            {
                GameEvents.RaiseEnemyKilled(null);
            }

            if (GUILayout.Button("Attack Blocked"))
            {
                GameEvents.RaiseAttackBlocked(new Attack());
            }

            if (GUILayout.Button("Wave Cleared"))
            {
                GameEvents.RaiseWaveCleared();
            }

            EditorGUILayout.EndVertical();
        }

        private new T FindFirstObjectByType<T>() where T : Object => Object.FindFirstObjectByType<T>();
    }
}

