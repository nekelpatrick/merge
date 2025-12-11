using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;

namespace ShieldWall.Combat
{
    public class CombatResolver : MonoBehaviour
    {
        [SerializeField] private EnemyWaveController _waveController;

        public struct ResolutionResult
        {
            public int EnemiesKilled;
            public int AttacksBlocked;
            public int DamageToPlayer;
            public Dictionary<WallPosition, int> DamageToBrothers;
        }

        public ResolutionResult Resolve(
            List<ActionSO> playerActions,
            List<Attack> enemyAttacks,
            IShieldWallState wallState)
        {
            var result = new ResolutionResult
            {
                EnemiesKilled = 0,
                AttacksBlocked = 0,
                DamageToPlayer = 0,
                DamageToBrothers = new Dictionary<WallPosition, int>()
            };

            var remainingAttacks = new List<Attack>(enemyAttacks);

            result.EnemiesKilled = ExecuteStrikes(playerActions);

            result.AttacksBlocked = ExecuteBlocks(playerActions, remainingAttacks);

            ExecuteCoverActions(playerActions, remainingAttacks, wallState);

            ApplyRemainingDamage(remainingAttacks, result, wallState);

            return result;
        }

        private int ExecuteStrikes(List<ActionSO> playerActions)
        {
            int killCount = 0;

            foreach (var action in playerActions)
            {
                if (action.effectType == ActionEffectType.Strike)
                {
                    int enemiesToKill = action.effectPower;
                    var liveEnemies = _waveController?.GetLiveEnemies();
                    
                    if (liveEnemies == null || liveEnemies.Count == 0) continue;

                    for (int i = 0; i < enemiesToKill && liveEnemies.Count > 0; i++)
                    {
                        var enemy = liveEnemies[0];
                        _waveController.RemoveEnemy(enemy);
                        liveEnemies.RemoveAt(0);
                        killCount++;
                        Debug.Log($"CombatResolver: Strike killed {enemy.Data.enemyName}");
                    }
                }
                else if (action.effectType == ActionEffectType.Counter)
                {
                    var liveEnemies = _waveController?.GetLiveEnemies();
                    if (liveEnemies != null && liveEnemies.Count > 0)
                    {
                        var enemy = liveEnemies[0];
                        _waveController.RemoveEnemy(enemy);
                        killCount++;
                        Debug.Log($"CombatResolver: Counter killed {enemy.Data.enemyName}");
                    }
                }
            }

            return killCount;
        }

        private int ExecuteBlocks(List<ActionSO> playerActions, List<Attack> attacks)
        {
            int blockedCount = 0;

            foreach (var action in playerActions)
            {
                if (action.effectType == ActionEffectType.Block || 
                    action.effectType == ActionEffectType.Counter)
                {
                    int blocksAvailable = action.effectPower;
                    
                    for (int i = attacks.Count - 1; i >= 0 && blocksAvailable > 0; i--)
                    {
                        if (attacks[i].Target == WallPosition.Center)
                        {
                            GameEvents.RaiseAttackBlocked(attacks[i]);
                            attacks.RemoveAt(i);
                            blocksAvailable--;
                            blockedCount++;
                            Debug.Log("CombatResolver: Blocked attack on player");
                        }
                    }
                }
            }

            return blockedCount;
        }

        private void ExecuteCoverActions(
            List<ActionSO> playerActions, 
            List<Attack> attacks, 
            IShieldWallState wallState)
        {
            foreach (var action in playerActions)
            {
                if (action.effectType == ActionEffectType.Cover)
                {
                    var adjacentPositions = new[] { WallPosition.Left, WallPosition.Right };
                    
                    foreach (var pos in adjacentPositions)
                    {
                        for (int i = attacks.Count - 1; i >= 0; i--)
                        {
                            if (attacks[i].Target == pos)
                            {
                                GameEvents.RaiseAttackBlocked(attacks[i]);
                                attacks.RemoveAt(i);
                                Debug.Log($"CombatResolver: Cover blocked attack on {pos}");
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void ApplyRemainingDamage(
            List<Attack> attacks, 
            ResolutionResult result, 
            IShieldWallState wallState)
        {
            foreach (var attack in attacks)
            {
                GameEvents.RaiseAttackLanded(attack);
                
                if (attack.Target == WallPosition.Center)
                {
                    result.DamageToPlayer += attack.Damage;
                    Debug.Log($"CombatResolver: {attack.Damage} damage to player");
                }
                else
                {
                    if (!result.DamageToBrothers.ContainsKey(attack.Target))
                    {
                        result.DamageToBrothers[attack.Target] = 0;
                    }
                    result.DamageToBrothers[attack.Target] += attack.Damage;
                    Debug.Log($"CombatResolver: {attack.Damage} damage to brother at {attack.Target}");
                }
            }
        }

        public List<Attack> GenerateEnemyAttacks(List<Enemy> enemies, IShieldWallState wallState)
        {
            var attacks = new List<Attack>();

            foreach (var enemy in enemies)
            {
                if (enemy.IsDead) continue;

                var target = EnemyTargetSelector.SelectTarget(enemy.Data, wallState);
                attacks.Add(enemy.CreateAttack(target));
            }

            return attacks;
        }
    }
}

