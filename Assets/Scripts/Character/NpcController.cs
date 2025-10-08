using RPG.Core;
using RPG.Quest;
using RPG.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using Ink.Runtime;

namespace RPG.Character
{
    public class NpcController : MonoBehaviour
    {
        private Canvas _canvasCmp;
        private Inventory _playerInventoryCmp;
        private Reward _rewardCmp;
        private Story _story;
        private bool _rewardPending;

        [Header("Dialogue")] public TextAsset inkJson;

        [Header("Behavior Configuration")]
        [Tooltip("Defines what external functions this NPC uses. Leave empty for dialogue-only NPCs.")]
        public NpcBehaviorSo npcBehavior;

        [Header("Quest Settings (Optional)")] public QuestItemSo desiredQuestItem;
        public bool hasQuestItem;

        private void Awake()
        {
            _canvasCmp = GetComponentInChildren<Canvas>();
            _rewardCmp = GetComponent<Reward>();

            var playerGameObject = GameObject.FindGameObjectWithTag(Constants.PlayerTag);
            _playerInventoryCmp = playerGameObject.GetComponent<Inventory>();
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(Constants.PlayerPrefsNpcItems)) return;

            var npcItems = PlayerPrefsUtility.GetString(Constants.PlayerPrefsNpcItems);
            npcItems.ForEach(CheckNpcQuestItem);
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

        public Story GetOrCreateStory()
        {
            if (_story == null)
            {
                _story = new Story(inkJson.text);
                BindExternalFunctions();
            }
            else
            {
                // Reset to the beginning without clearing variables
                _story.ResetCallstack();
                _story.ChoosePathString("start");
            }

            return _story;
        }

        /// <summary>
        /// Binds external functions based on the NPC's behavior configuration.
        /// Configure via NpcBehaviorSo.
        /// </summary>
        private void BindExternalFunctions()
        {
            // No behavior configuration = dialogue-only NPC
            if (!npcBehavior) return;

            if (!npcBehavior.hasQuestVerification) return;

            // Bind quest verification if configured
            if (desiredQuestItem)
            {
                _story.BindExternalFunction(Constants.InkStoryVerifyQuestFunc, VerifyQuest);
                _story.BindExternalFunction(Constants.InkStoryTriggerRewardFunc, TriggerReward);
            }
            else
            {
                Debug.LogWarning(
                    $"[{gameObject.name}] NPC has quest verification enabled but no desiredQuestItem assigned."
                );
            }
        }

        /// <summary>
        /// Called by Ink dialogue to verify if the player has the quest item.
        /// </summary>
        private void VerifyQuest()
        {
            if (_story == null) return;
            _story.variablesState[Constants.InkStoryQuestCompletedVar] = CheckPlayerForQuestItem();
        }

        private bool CheckPlayerForQuestItem()
        {
            if (!desiredQuestItem)
            {
                Debug.LogWarning(
                    $"[{gameObject.name}] CheckPlayerForQuestItem called but no desiredQuestItem is assigned."
                );
                return false;
            }

            if (hasQuestItem) return true;

            hasQuestItem = _playerInventoryCmp.HasItem(desiredQuestItem);

            if (_rewardCmp && hasQuestItem)
            {
                // Remove the quest item from inventory when completing the quest
                _playerInventoryCmp.RemoveItem(desiredQuestItem);

                _rewardPending = true;
            }

            return hasQuestItem;
        }

        private void TriggerReward()
        {
            if (!_rewardPending)
            {
                if (_rewardCmp && !_rewardCmp.HasRewardBeenClaimed)
                {
                    _rewardCmp.SendReward();
                }

                return;
            }

            if (!_rewardCmp)
            {
                Debug.LogWarning($"[{gameObject.name}] TriggerReward called but Reward component is missing.");
                _rewardPending = false;
                return;
            }

            _rewardPending = false;
            _rewardCmp.SendReward();
        }

        private void CheckNpcQuestItem(string itemName)
        {
            if (!desiredQuestItem) return;

            if (itemName == desiredQuestItem.itemName)
            {
                hasQuestItem = true;
            }
        }
    }
}