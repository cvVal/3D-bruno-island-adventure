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
        private AudioSource _audioSourceCmp;

        [SerializeField] private QuestItemSo questItemSo;

        public Animator animatorCmp;

        [Header("Audio Settings")] public AudioClip chestOpenClip;

        private void Awake()
        {
            _audioSourceCmp = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(Constants.PlayerPrefsQuestItems)) return;

            var playerQuestItems = PlayerPrefsUtility.GetString(Constants.PlayerPrefsQuestItems);
            playerQuestItems.ForEach(CheckItem);
        }

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

            EventManager.RaiseTreasureChestUnlocked(questItemSo, true);

            if (chestOpenClip)
                _audioSourceCmp.PlayOneShot(chestOpenClip);

            animatorCmp.SetBool(Constants.IsShakingAnimatorParam, false);
            _hasBeenOpened = true;
        }

        private void CheckItem(string itemName)
        {
            if (itemName != questItemSo.name) return;

            _hasBeenOpened = true;
            animatorCmp.SetBool(Constants.IsShakingAnimatorParam, false);

            EventManager.RaiseTreasureChestUnlocked(questItemSo, false);
        }
    }
}