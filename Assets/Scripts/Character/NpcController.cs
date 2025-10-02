using RPG.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Character
{
    public class NpcController : MonoBehaviour
    {
        private Canvas _canvasCmp;

        public TextAsset inkJson;

        private void Awake()
        {
            _canvasCmp = GetComponentInChildren<Canvas>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _canvasCmp.enabled = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _canvasCmp.enabled = false;
        }
        
        public void HandleInteract(InputAction.CallbackContext context)
        {
            if (!context.performed || !_canvasCmp.enabled) return;

            if (!inkJson)
            {
                Debug.LogWarning("Please add an ink file to the npc.");
                return;
            }
            
            EventManager.RaiseInitiateDialogue(inkJson);
        }
    }
}