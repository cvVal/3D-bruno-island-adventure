using RPG.Core;
using RPG.Quest;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Character
{
    public class NpcController : MonoBehaviour
    {
        private Canvas _canvasCmp;
        private Inventory _playerInventoryCmp;
        private Reward _rewardCmp;

        public TextAsset inkJson;
        public QuestItemSo desiredQuestItem;
        public bool hasQuestItem;

        private void Awake()
        {
            _canvasCmp = GetComponentInChildren<Canvas>();
            _rewardCmp = GetComponent<Reward>();

            var playerGameObject = GameObject.FindGameObjectWithTag(Constants.PlayerTag);
            _playerInventoryCmp = playerGameObject.GetComponent<Inventory>();
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

            EventManager.RaiseInitiateDialogue(inkJson, gameObject);
        }

        public bool CheckPlayerForQuestItem()
        {
            if (hasQuestItem) return true;

            hasQuestItem = _playerInventoryCmp.HasItem(desiredQuestItem);

            if (_rewardCmp && hasQuestItem)
            {
                _rewardCmp.SendReward();
            }
            
            return _playerInventoryCmp && hasQuestItem;
        }
    }
}