using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Quest
{
    public class Inventory : MonoBehaviour
    {
        public List<QuestItemSo> items = new();

        private void OnEnable()
        {
            EventManager.OnTreasureChestUnlocked += HandleTreasureChestUnlocked;
        }

        private void OnDisable()
        {
            EventManager.OnTreasureChestUnlocked -= HandleTreasureChestUnlocked;
        }

        private void HandleTreasureChestUnlocked(QuestItemSo itemSo, bool showUI)
        {
            items.Add(itemSo);
            Debug.Log($"Added {itemSo.itemName} to inventory.");

            // Notify UI about inventory change
            EventManager.RaiseInventoryChanged(items.Count);
        }

        public bool HasItem(QuestItemSo desiredItem)
        {
            return items.Contains(desiredItem);
        }

        /// <summary>
        /// Removes an item from the inventory (e.g., when completing a quest).
        /// </summary>
        /// <param name="itemToRemove">The quest item to remove</param>
        public void RemoveItem(QuestItemSo itemToRemove)
        {
            if (!items.Contains(itemToRemove)) return;

            items.Remove(itemToRemove);
            Debug.Log($"Removed {itemToRemove.itemName} from inventory.");

            // Notify UI about inventory change
            EventManager.RaiseInventoryChanged(items.Count);
        }
    }
}