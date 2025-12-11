using System.Collections.Generic;
using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;
using ShieldWall.Formation;
using ShieldWall.Combat;
using ShieldWall.Visual;

namespace ShieldWall.ShieldWall
{
    public class ShieldWallManager : MonoBehaviour, IShieldWallState
    {
        [SerializeField] private ShieldBrotherSO[] _brotherData;
        [SerializeField] private PlayerWarrior _player;
        [SerializeField] private BrotherVisualController _brotherVisualController;

        private readonly Dictionary<WallPosition, ShieldBrother> _brothers = new Dictionary<WallPosition, ShieldBrother>();
        private readonly WallPosition[] _brotherPositions = 
        {
            WallPosition.FarLeft,
            WallPosition.Left,
            WallPosition.Right,
            WallPosition.FarRight
        };

        public PlayerWarrior Player => _player;
        public int AliveBrotherCount => GetAliveBrotherCount();

        private void Awake()
        {
            InitializeBrothers();
        }

        public void InitializeBrothers()
        {
            _brothers.Clear();

            var visualPositions = new Dictionary<WallPosition, ShieldBrotherSO>();

            for (int i = 0; i < _brotherPositions.Length && i < _brotherData.Length; i++)
            {
                if (_brotherData[i] != null)
                {
                    var brother = new ShieldBrother(_brotherData[i], _brotherPositions[i]);
                    _brothers[_brotherPositions[i]] = brother;
                    visualPositions[_brotherPositions[i]] = _brotherData[i];
                }
            }

            if (_brotherVisualController != null)
            {
                _brotherVisualController.InitializeBrothers(visualPositions);
            }

            RaiseWallIntegrityChanged();
        }

        public ShieldBrother GetBrotherAt(WallPosition position)
        {
            _brothers.TryGetValue(position, out var brother);
            return brother;
        }

        public int GetAliveBrotherCount()
        {
            int count = 0;
            foreach (var brother in _brothers.Values)
            {
                if (!brother.IsDead) count++;
            }
            return count;
        }

        public int GetBonusDice()
        {
            int alive = GetAliveBrotherCount();
            return alive switch
            {
                4 => 1,
                3 => 0,
                2 => 0,
                1 => -1,
                0 => -2,
                _ => 0
            };
        }

        public float GetPlayerTargetChance()
        {
            int alive = GetAliveBrotherCount();
            return alive switch
            {
                4 => 0.2f,
                3 => 0.2f,
                2 => 0.6f,
                1 => 0.8f,
                0 => 1.0f,
                _ => 0.2f
            };
        }

        public void ApplyDamageToBrother(WallPosition position, int damage)
        {
            var brother = GetBrotherAt(position);
            if (brother != null && !brother.IsDead)
            {
                brother.TakeDamage(damage);
                RaiseWallIntegrityChanged();
            }
        }

        public bool TryAutoDefense(WallPosition position)
        {
            var brother = GetBrotherAt(position);
            if (brother == null || brother.IsDead) return false;
            
            return brother.AttemptAutoDefense();
        }

        public void HealAllBrothers(int amount)
        {
            foreach (var brother in _brothers.Values)
            {
                if (!brother.IsDead)
                {
                    brother.Heal(amount);
                }
            }
        }

        private void RaiseWallIntegrityChanged()
        {
            GameEvents.RaiseWallIntegrityChanged(GetAliveBrotherCount());
        }

        public List<WallPosition> GetOccupiedPositions()
        {
            var positions = new List<WallPosition>();
            
            positions.Add(WallPosition.Center);
            
            foreach (var kvp in _brothers)
            {
                if (!kvp.Value.IsDead)
                {
                    positions.Add(kvp.Key);
                }
            }
            
            return positions;
        }

        public int GetHealthAt(WallPosition position)
        {
            if (position == WallPosition.Center)
            {
                return _player != null ? _player.CurrentHealth : 0;
            }
            
            var brother = GetBrotherAt(position);
            return brother?.CurrentHealth ?? 0;
        }

        public List<ShieldBrother> GetAllBrothers()
        {
            return new List<ShieldBrother>(_brothers.Values);
        }

        public List<ShieldBrother> GetAliveBrothers()
        {
            var alive = new List<ShieldBrother>();
            foreach (var brother in _brothers.Values)
            {
                if (!brother.IsDead)
                {
                    alive.Add(brother);
                }
            }
            return alive;
        }
    }
}

