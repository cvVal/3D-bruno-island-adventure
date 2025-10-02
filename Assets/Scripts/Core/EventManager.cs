using UnityEngine.Events;

namespace RPG.Core
{
    public static class EventManager
    {
        public static event UnityAction<float> OnChangePlayerHealth;
        public static event UnityAction<int> OnChangePlayerPotions;

        public static void RaiseChangePlayerHealth(float newHealthPoints) =>
            OnChangePlayerHealth?.Invoke(newHealthPoints);
        
        public static void RaiseChangePlayerPotions(int newPotionCount) =>
            OnChangePlayerPotions?.Invoke(newPotionCount);
    }
}