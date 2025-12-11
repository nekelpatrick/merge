using System.Collections.Generic;
using System.Linq;
using ShieldWall.Data;

namespace ShieldWall.Dice
{
    public static class ComboResolver
    {
        public static List<ActionSO> Resolve(RuneType[] lockedDice, ActionSO[] allActions)
        {
            if (lockedDice == null || lockedDice.Length == 0 || allActions == null)
            {
                return new List<ActionSO>();
            }

            var runeCounts = CountRunes(lockedDice);
            var availableActions = new List<ActionSO>();
            
            var sortedActions = allActions
                .Where(a => a != null && a.requiredRunes != null)
                .OrderByDescending(a => a.requiredRunes.Length)
                .ToList();

            var remainingRunes = new Dictionary<RuneType, int>(runeCounts);

            foreach (var action in sortedActions)
            {
                if (CanAffordAction(action, remainingRunes))
                {
                    availableActions.Add(action);
                }
            }

            return availableActions;
        }

        public static List<ActionSO> ResolveGreedy(RuneType[] lockedDice, ActionSO[] allActions)
        {
            if (lockedDice == null || lockedDice.Length == 0 || allActions == null)
            {
                return new List<ActionSO>();
            }

            var runeCounts = CountRunes(lockedDice);
            var selectedActions = new List<ActionSO>();
            
            var sortedActions = allActions
                .Where(a => a != null && a.requiredRunes != null)
                .OrderByDescending(a => a.requiredRunes.Length)
                .ToList();

            var remainingRunes = new Dictionary<RuneType, int>(runeCounts);

            foreach (var action in sortedActions)
            {
                while (CanAffordAction(action, remainingRunes))
                {
                    selectedActions.Add(action);
                    SpendRunes(action, remainingRunes);
                }
            }

            return selectedActions;
        }

        public static bool CanAffordAction(ActionSO action, Dictionary<RuneType, int> availableRunes)
        {
            if (action == null || action.requiredRunes == null) return false;

            var requiredCounts = CountRunes(action.requiredRunes);

            foreach (var required in requiredCounts)
            {
                if (!availableRunes.TryGetValue(required.Key, out int available) || available < required.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public static void SpendRunes(ActionSO action, Dictionary<RuneType, int> availableRunes)
        {
            if (action == null || action.requiredRunes == null) return;

            var requiredCounts = CountRunes(action.requiredRunes);

            foreach (var required in requiredCounts)
            {
                if (availableRunes.ContainsKey(required.Key))
                {
                    availableRunes[required.Key] -= required.Value;
                }
            }
        }

        private static Dictionary<RuneType, int> CountRunes(RuneType[] runes)
        {
            var counts = new Dictionary<RuneType, int>();
            
            foreach (var rune in runes)
            {
                if (counts.ContainsKey(rune))
                {
                    counts[rune]++;
                }
                else
                {
                    counts[rune] = 1;
                }
            }

            return counts;
        }
    }
}

