using System;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Visual
{
    /// <summary>
    /// Manages visual feedback for combat events - blood splatters, hit effects, etc.
    /// Listens to GameEvents and spawns appropriate VFX.
    /// </summary>
    public class CombatFeedbackController : MonoBehaviour
    {
        private const string DebugLogPath = @"c:\Users\PatrickLocal\merge\.cursor\debug.log";
        private const string DebugSessionId = "debug-session";
        private const string DebugRunId = "run1";

        [Header("Blood VFX")]
        [SerializeField] private BloodBurstVFX _bloodBurstPrefab;
        [SerializeField] private GameObject[] _bloodDecalPrefabs;
        [SerializeField] private float _bloodDecalLifetime = 30f;
        
        [Header("Hit VFX")]
        [SerializeField] private bool _enableBloodOnAttackLanded = true;
        [SerializeField] private float _bloodIntensityMultiplier = 1f;

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
        
        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            if (_enableBloodOnAttackLanded)
            {
                GameEvents.OnAttackLanded += HandleAttackLanded;
            }
        }
        
        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            if (_enableBloodOnAttackLanded)
            {
                GameEvents.OnAttackLanded -= HandleAttackLanded;
            }
        }
        
        private void HandleEnemyKilled(EnemySO enemy)
        {
            #region agent log
            try
            {
                File.AppendAllText(
                    DebugLogPath,
                    $"{{\"sessionId\":\"{DebugSessionId}\",\"runId\":\"{DebugRunId}\",\"hypothesisId\":\"H1\",\"location\":\"CombatFeedbackController.cs:HandleEnemyKilled\",\"message\":\"OnEnemyKilled\",\"data\":{{\"enemyName\":\"{Escape(enemy != null ? enemy.name : "null")}\",\"enemyInstanceId\":{(enemy != null ? enemy.GetInstanceID() : 0)}}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n"
                );
            }
            catch { }
            #endregion

            Vector3 enemyPosition = FindEnemyPosition(enemy);
            
            if (enemyPosition != Vector3.zero)
            {
                SpawnBloodBurst(enemyPosition);
                SpawnBloodDecal(enemyPosition);
            }
        }
        
        private void HandleAttackLanded(Attack attack)
        {
            Vector3 targetPosition = FindAttackTargetPosition(attack);
            
            if (targetPosition != Vector3.zero && attack.Damage > 0)
            {
                SpawnBloodBurst(targetPosition, 0.5f);
            }
        }
        
        private void SpawnBloodBurst(Vector3 position, float scale = 1f)
        {
            if (_bloodBurstPrefab == null)
            {
                #region agent log
                try
                {
                    File.AppendAllText(
                        DebugLogPath,
                        $"{{\"sessionId\":\"{DebugSessionId}\",\"runId\":\"{DebugRunId}\",\"hypothesisId\":\"H5\",\"location\":\"CombatFeedbackController.cs:SpawnBloodBurst\",\"message\":\"UsingRuntimeGeneratedBloodBurst\",\"data\":{{\"x\":{position.x},\"y\":{position.y},\"z\":{position.z},\"scale\":{scale},\"intensityMult\":{_bloodIntensityMultiplier}}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n"
                    );
                }
                catch { }
                #endregion

                GameObject bloodVFXGO = new GameObject("BloodBurst");
                bloodVFXGO.transform.position = position;
                BloodBurstVFX bloodVFX = bloodVFXGO.AddComponent<BloodBurstVFX>();
                bloodVFX.Play();
                Destroy(bloodVFXGO, 3f);
                return;
            }
            
            BloodBurstVFX bloodBurst = Instantiate(_bloodBurstPrefab, position, Quaternion.identity);
            bloodBurst.transform.localScale = Vector3.one * scale * _bloodIntensityMultiplier;
            bloodBurst.Play();
            Destroy(bloodBurst.gameObject, 3f);
        }
        
        private void SpawnBloodDecal(Vector3 position)
        {
            if (_bloodDecalPrefabs == null || _bloodDecalPrefabs.Length == 0)
            {
                return;
            }
            
            GameObject decalPrefab = _bloodDecalPrefabs[Random.Range(0, _bloodDecalPrefabs.Length)];
            if (decalPrefab == null) return;
            
            Vector3 decalPosition = position;
            decalPosition.y = 0.01f;
            
            Quaternion decalRotation = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);
            
            GameObject decal = Instantiate(decalPrefab, decalPosition, decalRotation);
            float randomScale = Random.Range(0.8f, 1.5f);
            decal.transform.localScale = Vector3.one * randomScale;
            
            Destroy(decal, _bloodDecalLifetime);
        }
        
        private Vector3 FindEnemyPosition(EnemySO enemy)
        {
            GameObject[] potentialEnemies;
            try
            {
                potentialEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            }
            catch (Exception ex)
            {
                #region agent log
                try
                {
                    File.AppendAllText(
                        DebugLogPath,
                        $"{{\"sessionId\":\"{DebugSessionId}\",\"runId\":\"{DebugRunId}\",\"hypothesisId\":\"H1\",\"location\":\"CombatFeedbackController.cs:FindEnemyPosition\",\"message\":\"FindGameObjectsWithTag threw\",\"data\":{{\"tag\":\"Enemy\",\"enemyName\":\"{Escape(enemy != null ? enemy.name : "null")}\",\"enemyInstanceId\":{(enemy != null ? enemy.GetInstanceID() : 0)},\"exceptionType\":\"{Escape(ex.GetType().Name)}\",\"exceptionMessage\":\"{Escape(ex.Message)}\"}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n"
                    );
                }
                catch { }
                #endregion

                // Tag may not exist in project settings. Fall back to name-based search instead of crashing.
                potentialEnemies = Array.Empty<GameObject>();
            }

            #region agent log
            try
            {
                File.AppendAllText(
                    DebugLogPath,
                    $"{{\"sessionId\":\"{DebugSessionId}\",\"runId\":\"{DebugRunId}\",\"hypothesisId\":\"H2\",\"location\":\"CombatFeedbackController.cs:FindEnemyPosition\",\"message\":\"FindGameObjectsWithTag ok\",\"data\":{{\"tag\":\"Enemy\",\"count\":{(potentialEnemies != null ? potentialEnemies.Length : -1)}}},\"timestamp\":{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}}}\n"
                );
            }
            catch { }
            #endregion

            if (potentialEnemies.Length > 0)
            {
                return potentialEnemies[Random.Range(0, potentialEnemies.Length)].transform.position;
            }
            
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in allObjects)
            {
                if (go.name.ToLower().Contains("enemy"))
                {
                    return go.transform.position;
                }
            }
            
            return new Vector3(0, 1, 5);
        }
        
        private Vector3 FindAttackTargetPosition(Attack attack)
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            string positionName = attack.Target.ToString().ToLower();
            
            foreach (GameObject go in allObjects)
            {
                if (go.name.ToLower().Contains(positionName) && 
                    (go.name.ToLower().Contains("brother") || go.name.ToLower().Contains("player")))
                {
                    return go.transform.position;
                }
            }
            
            return Vector3.zero;
        }
        
        [ContextMenu("Test Blood Burst")]
        private void TestBloodBurst()
        {
            SpawnBloodBurst(transform.position + Vector3.forward * 3f);
        }
    }
}
