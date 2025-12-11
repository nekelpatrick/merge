using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Data;
using ShieldWall.Formation;

namespace ShieldWall.Combat
{
    public static class EnemyTargetSelector
    {
        public static WallPosition SelectTarget(EnemySO enemy, IShieldWallState wallState)
        {
            if (enemy == null) return WallPosition.Center;

            return enemy.targeting switch
            {
                EnemyTargetingType.Player => WallPosition.Center,
                EnemyTargetingType.LowestHealth => FindLowestHealthTarget(wallState),
                EnemyTargetingType.Random => GetRandomOccupiedPosition(wallState),
                _ => WallPosition.Center
            };
        }

        private static WallPosition FindLowestHealthTarget(IShieldWallState wallState)
        {
            if (wallState == null) return WallPosition.Center;

            var occupiedPositions = wallState.GetOccupiedPositions();
            if (occupiedPositions.Count == 0) return WallPosition.Center;

            WallPosition lowestPos = WallPosition.Center;
            int lowestHealth = int.MaxValue;

            foreach (var pos in occupiedPositions)
            {
                int health = wallState.GetHealthAt(pos);
                if (health < lowestHealth)
                {
                    lowestHealth = health;
                    lowestPos = pos;
                }
            }

            return lowestPos;
        }

        private static WallPosition GetRandomOccupiedPosition(IShieldWallState wallState)
        {
            if (wallState == null) return WallPosition.Center;

            var occupiedPositions = wallState.GetOccupiedPositions();
            if (occupiedPositions.Count == 0) return WallPosition.Center;

            int randomIndex = Random.Range(0, occupiedPositions.Count);
            return occupiedPositions[randomIndex];
        }
    }

    public interface IShieldWallState
    {
        List<WallPosition> GetOccupiedPositions();
        int GetHealthAt(WallPosition position);
    }
}

