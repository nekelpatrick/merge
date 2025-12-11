using UnityEngine;

namespace ShieldWall.Visual
{
    public class ActionDismembermentMapper : MonoBehaviour
    {
        private static DismembermentType _lastActionDismembermentType = DismembermentType.Random;
        
        public static void SetDismembermentTypeForAction(string actionName)
        {
            if (actionName == null)
            {
                _lastActionDismembermentType = DismembermentType.Random;
                return;
            }
            
            string lowerName = actionName.ToLower();
            
            if (lowerName.Contains("strike") || lowerName.Contains("slash"))
            {
                _lastActionDismembermentType = Random.value < 0.5f 
                    ? DismembermentType.Decapitation 
                    : DismembermentType.ArmSword;
            }
            else if (lowerName.Contains("counter"))
            {
                _lastActionDismembermentType = DismembermentType.Decapitation;
            }
            else if (lowerName.Contains("berserker") || lowerName.Contains("rage"))
            {
                _lastActionDismembermentType = DismembermentType.Random;
            }
            else if (lowerName.Contains("spear") || lowerName.Contains("thrust"))
            {
                _lastActionDismembermentType = DismembermentType.Random;
            }
            else
            {
                _lastActionDismembermentType = DismembermentType.Random;
            }
        }
        
        public static DismembermentType GetDismembermentType()
        {
            return _lastActionDismembermentType;
        }
        
        public static void Reset()
        {
            _lastActionDismembermentType = DismembermentType.Random;
        }
    }
}
