using RPG.Quest;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    public static class EventManager
    {
        public static event UnityAction<float> OnChangePlayerHealth;
        public static event UnityAction<int> OnChangePlayerPotions;
        public static event UnityAction<TextAsset, GameObject> OnInitiateDialogue;
        public static event UnityAction<QuestItemSo, bool> OnTreasureChestUnlocked;
        public static event UnityAction<bool> OnToggleUI;
        public static event UnityAction<RewardSo> OnReward;
        public static event UnityAction<Collider, int> OnPortalEnter;
        public static event UnityAction<bool> OnCutSceneUpdated;
        public static event UnityAction OnVictory;
        public static event UnityAction OnGameOver;
        public static event UnityAction<int> OnInventoryChanged;

        public static void RaiseChangePlayerHealth(float newHealthPoints) =>
            OnChangePlayerHealth?.Invoke(newHealthPoints);

        public static void RaiseChangePlayerPotions(int newPotionCount) =>
            OnChangePlayerPotions?.Invoke(newPotionCount);

        public static void RaiseInitiateDialogue(
            TextAsset inkJson,
            GameObject npc
        ) => OnInitiateDialogue?.Invoke(inkJson, npc);

        public static void RaiseTreasureChestUnlocked(
            QuestItemSo itemSo,
            bool showUI
        ) => OnTreasureChestUnlocked?.Invoke(itemSo, showUI);

        public static void RaiseToggleUI(bool isOpened) =>
            OnToggleUI?.Invoke(isOpened);

        public static void RaiseReward(RewardSo rewardSo) =>
            OnReward?.Invoke(rewardSo);

        public static void RaisePortalEnter(
            Collider player,
            int nexSceneIndex
        ) => OnPortalEnter?.Invoke(player, nexSceneIndex);

        public static void RaiseCutSceneUpdated(bool isEnabled) =>
            OnCutSceneUpdated?.Invoke(isEnabled);

        public static void RaiseVictory() =>
            OnVictory?.Invoke();

        public static void RaiseGameOver() =>
            OnGameOver?.Invoke();

        public static void RaiseInventoryChanged(int itemCount) =>
            OnInventoryChanged?.Invoke(itemCount);
    }
}