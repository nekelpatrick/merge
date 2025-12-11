using System.Collections.Generic;
using ShieldWall.Core;

namespace ShieldWall.Dice
{
    public class DicePool
    {
        private readonly List<RuneDie> _dice = new List<RuneDie>();
        private int _baseDiceCount;
        private int _bonusDice;

        public int DiceCount => _baseDiceCount + _bonusDice;
        public IReadOnlyList<RuneDie> Dice => _dice;

        public DicePool(int baseDiceCount = 4)
        {
            _baseDiceCount = baseDiceCount;
            _bonusDice = 0;
            InitializeDice();
        }

        public void SetBonusDice(int bonus)
        {
            _bonusDice = bonus;
            InitializeDice();
        }

        private void InitializeDice()
        {
            int targetCount = System.Math.Max(2, DiceCount);
            
            while (_dice.Count < targetCount)
            {
                _dice.Add(new RuneDie());
            }
            
            while (_dice.Count > targetCount)
            {
                _dice.RemoveAt(_dice.Count - 1);
            }
        }

        public RuneDie[] RollAll()
        {
            foreach (var die in _dice)
            {
                die.Roll();
            }
            
            var diceArray = _dice.ToArray();
            GameEvents.RaiseDiceRolled(diceArray);
            return diceArray;
        }

        public void ToggleLock(int index)
        {
            if (index < 0 || index >= _dice.Count) return;
            
            _dice[index].IsLocked = !_dice[index].IsLocked;
            GameEvents.RaiseDieLockToggled(index, _dice[index].IsLocked);
        }

        public void SetLocked(int index, bool locked)
        {
            if (index < 0 || index >= _dice.Count) return;
            
            if (_dice[index].IsLocked != locked)
            {
                _dice[index].IsLocked = locked;
                GameEvents.RaiseDieLockToggled(index, locked);
            }
        }

        public RuneType[] GetLockedRunes()
        {
            var locked = new List<RuneType>();
            foreach (var die in _dice)
            {
                if (die.IsLocked)
                {
                    locked.Add(die.CurrentFace);
                }
            }
            return locked.ToArray();
        }

        public void ResetAll()
        {
            foreach (var die in _dice)
            {
                die.Reset();
            }
        }

        public void UnlockAll()
        {
            for (int i = 0; i < _dice.Count; i++)
            {
                if (_dice[i].IsLocked)
                {
                    _dice[i].IsLocked = false;
                    GameEvents.RaiseDieLockToggled(i, false);
                }
            }
        }
    }
}

