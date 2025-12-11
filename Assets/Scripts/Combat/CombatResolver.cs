using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;
using ShieldWall.ShieldWall;

namespace ShieldWall.Combat
{
    public class CombatResolver : MonoBehaviour
    {
        [SerializeField] private EnemyWaveController _waveController;
        [SerializeField] private ShieldWallManager _shieldWallManager;
        [SerializeField] private PlayerWarrior _player;

        public struct ResolutionResult
        {
            public int EnemiesKilled;
            public int AttacksBlocked;
            public int DamageToPlayer;
            public Dictionary<WallPosition, int> DamageToBrothers;
            public int BrothersHealed;
            public int EnemiesStunned;
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
                DamageToBrothers = new Dictionary<WallPosition, int>(),
                BrothersHealed = 0,
                EnemiesStunned = 0
            };

            var remainingAttacks = new List<Attack>(enemyAttacks);

            result.EnemiesKilled = ExecuteStrikes(playerActions, ref result);

            ExecuteStunActions(playerActions, ref result);

            ExecuteHealActions(playerActions, ref result);

            result.AttacksBlocked = ExecuteBlocks(playerActions, remainingAttacks);

            ExecuteCoverActions(playerActions, remainingAttacks, wallState);

            ApplyRemainingDamage(remainingAttacks, result, wallState);

            return result;
        }

        private int ExecuteStrikes(List<ActionSO> playerActions, ref ResolutionResult result)
        {
            int killCount = 0;

            foreach (var action in playerActions)
            {
                if (action.effectType == ActionEffectType.Strike || action.effectType == ActionEffectType.MultiStrike)
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
                        Debug.Log($"CombatResolver: {action.actionName} killed {enemy.Data.enemyName}");
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
                else if (action.effectType == ActionEffectType.BerserkerRage)
                {
                    int enemiesToKill = action.effectPower;
                    var liveEnemies = _waveController?.GetLiveEnemies();
                    
                    if (liveEnemies != null)
                    {
                        for (int i = 0; i < enemiesToKill && liveEnemies.Count > 0; i++)
                        {
                            var enemy = liveEnemies[0];
                            _waveController.RemoveEnemy(enemy);
                            liveEnemies.RemoveAt(0);
                            killCount++;
                            Debug.Log($"CombatResolver: Berserker Rage killed {enemy.Data.enemyName}");
                        }
                    }
                    
                    if (_player != null)
                    {
                        _player.TakeDamage(1);
                        result.DamageToPlayer += 1;
                        Debug.Log("CombatResolver: Berserker Rage dealt 1 damage to player");
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
                if (action.effectType == ActionEffectType.BlockAll)
                {
                    int totalBlocked = attacks.Count;
                    foreach (var attack in attacks)
                    {
                        GameEvents.RaiseAttackBlocked(attack);
                    }
                    attacks.Clear();
                    blockedCount += totalBlocked;
                    Debug.Log($"CombatResolver: Testudo blocked ALL {totalBlocked} attacks!");
                }
                else if (action.effectType == ActionEffectType.Block || 
                    action.effectType == ActionEffectType.Counter)
                {
                    int blocksAvailable = action.effectPower;
                    
                    for (int i = attacks.Count - 1; i >= 0 && blocksAvailable > 0; i--)
                    {
                        if (attacks[i].Target == WallPosition.Center && !attacks[i].IgnoresBlocks)
                        {
                            GameEvents.RaiseAttackBlocked(attacks[i]);
                            attacks.RemoveAt(i);
                            blocksAvailable--;
                            blockedCount++;
                            Debug.Log("CombatResolver: Blocked attack on player");
                        }
                        else if (attacks[i].IgnoresBlocks && attacks[i].Target == WallPosition.Center)
                        {
                            Debug.Log($"CombatResolver: {attacks[i].Source.enemyName} ignores blocks!");
                        }
                    }
                }
            }

            return blockedCount;
        }

        private void ExecuteStunActions(List<ActionSO> playerActions, ref ResolutionResult result)
        {
            foreach (var action in playerActions)
            {
                if (action.effectType == ActionEffectType.Stun)
                {
                    var liveEnemies = _waveController?.GetLiveEnemies();
                    if (liveEnemies != null)
                    {
                        foreach (var enemy in liveEnemies)
                        {
                            enemy.ApplyStun();
                            result.EnemiesStunned++;
                        }
                        Debug.Log($"CombatResolver: Wall Push stunned {liveEnemies.Count} enemies!");
                    }
                }
            }
        }

        private void ExecuteHealActions(List<ActionSO> playerActions, ref ResolutionResult result)
        {
            foreach (var action in playerActions)
            {
                if (action.effectType == ActionEffectType.Heal)
                {
                    if (_shieldWallManager != null)
                    {
                        int healAmount = action.effectPower;
                        _shieldWallManager.HealAllBrothers(healAmount);
                        result.BrothersHealed = 4;
                        Debug.Log($"CombatResolver: Rally healed all brothers by {healAmount}!");
                    }
                }
            }
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
                
                if (enemy.IsStunned)
                {
                    enemy.ClearStun();
                    Debug.Log($"CombatResolver: {enemy.Data.enemyName} was stunned and skips attack");
                    continue;
                }

                var target = EnemyTargetSelector.SelectTarget(enemy.Data, wallState);
                attacks.Add(enemy.CreateAttack(target));
            }

            return attacks;
        }

        public void ClearAllStuns()
        {
            var enemies = _waveController?.GetLiveEnemies();
            if (enemies != null)
            {
                foreach (var enemy in enemies)
                {
                    enemy.ClearStun();
                }
            }
        }
    }
}

