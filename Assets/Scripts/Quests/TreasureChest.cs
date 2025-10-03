using RPG.Core;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Quest
{
    public class TreasureChest : MonoBehaviour
    {
        private bool _isInteractable;
        private bool _hasBeenOpened;
        
        [SerializeField] private QuestItemSo questItemSo;
        
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
            if (!_isInteractable || _hasBeenOpened || !context.performed) return;

            EventManager.RaiseTreasureChestUnlocked(questItemSo);
            
            animatorCmp.SetBool(Constants.IsShakingAnimatorParam, false);
            _hasBeenOpened = true;
        }
    }
}