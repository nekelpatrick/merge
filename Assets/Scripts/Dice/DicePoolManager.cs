using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.Dice
{
    public class DicePoolManager : MonoBehaviour
    {
        [SerializeField] private int _baseDiceCount = 4;
        
        private DicePool _dicePool;

        public DicePool Pool => _dicePool;
        public int CurrentDiceCount => _dicePool?.DiceCount ?? _baseDiceCount;

        private void Awake()
        {
            _dicePool = new DicePool(_baseDiceCount);
        }

        private void OnEnable()
        {
            GameEvents.OnWallIntegrityChanged += HandleWallIntegrityChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnWallIntegrityChanged -= HandleWallIntegrityChanged;
        }

        private void HandleWallIntegrityChanged(int aliveBrothers)
        {
            int bonus = aliveBrothers switch
            {
                4 => 1,
                3 => 0,
                2 => 0,
                1 => -1,
                0 => -2,
                _ => 0
            };
            
            _dicePool.SetBonusDice(bonus);
        }

        [ContextMenu("Roll Dice")]
        public RuneDie[] Roll()
        {
            return _dicePool.RollAll();
        }

        public void ToggleDieLock(int index)
        {
            _dicePool.ToggleLock(index);
        }

        public void ResetForNewTurn()
        {
            _dicePool.UnlockAll();
        }

        public RuneType[] GetLockedRunes()
        {
            return _dicePool.GetLockedRunes();
        }
    }
}

