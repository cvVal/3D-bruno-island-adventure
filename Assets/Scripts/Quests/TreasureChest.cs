using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Quest
{
    public class TreasureChest : MonoBehaviour
    {
        private static readonly int IsShaking = Animator.StringToHash("isShaking");
        
        private bool _isInteractable;
        private bool _hasBeenOpened;
        
        public Animator animatorCmp;

        private void OnTriggerEnter(Collider other)
        {
            _isInteractable = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _isInteractable = false;
        }

        public void HandleInteract(InputAction.CallbackContext context)
        {
            if (!_isInteractable || _hasBeenOpened) return;

            animatorCmp.SetBool(IsShaking, false);
            _hasBeenOpened = true;
        }
    }
}