using UnityEngine;
using UnityEngine.Events;

namespace RPG.Utility
{
    /// <summary>
    /// Event handler component for bubble-related animation events.
    /// This class is typically attached to a GameObject and receives animation event callbacks,
    /// forwarding them as UnityAction events that other components can subscribe to.
    /// </summary>
    public class BubbleEvent : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when the bubble starts an attack animation.
        /// Subscribe to this event to handle attack initialization logic.
        /// </summary>
        public event UnityAction OnBubbleStartAttack = () => { };
        
        /// <summary>
        /// Event triggered when the bubble completes an attack animation.
        /// Subscribe to this event to handle post-attack cleanup or follow-up actions.
        /// </summary>
        public event UnityAction OnBubbleCompleteAttack = () => { };
        
        /// <summary>
        /// Event triggered when the bubble is hit.
        /// Subscribe to this event to handle damage, visual effects, or other hit reactions.
        /// </summary>
        public event UnityAction OnBubbleHit = () => { };
        
        /// <summary>
        /// Event triggered when the bubble completes its defeat animation.
        /// Subscribe to this event to handle cleanup, rewards, or other defeat-related logic.
        /// </summary>
        public event UnityAction OnBubbleCompleteDefeat = () => { };
        
        /// <summary>
        /// Animation event callback invoked when the attack animation starts.
        /// Raises the OnBubbleStartAttack event.
        /// </summary>
        private void OnStartAttack()
        {
            OnBubbleStartAttack.Invoke();
        }
        
        /// <summary>
        /// Animation event callback invoked when the attack animation completes.
        /// Raises the OnBubbleCompleteAttack event.
        /// </summary>
        private void OnCompleteAttack()
        {
            OnBubbleCompleteAttack.Invoke();
        }
        
        /// <summary>
        /// Animation event callback invoked when the bubble is hit.
        /// Raises the OnBubbleHit event.
        /// </summary>
        private void OnHit()
        {
            OnBubbleHit.Invoke();
        }
        
        /// <summary>
        /// Animation event callback invoked when the defeat animation completes.
        /// Raises the OnBubbleCompleteDefeat event.
        /// </summary>
        private void OnCompleteDefeat()
        {
            OnBubbleCompleteDefeat.Invoke();
        }
    }
}