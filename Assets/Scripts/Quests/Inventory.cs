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
        
        private void HandleTreasureChestUnlocked(QuestItemSo itemSo)
        {
            items.Add(itemSo);
            Debug.Log($"Added {itemSo.itemName} to inventory.");
        }
        
        public bool HasItem(QuestItemSo desiredItem)
        {
            return items.Contains(desiredItem);
        }
    }
}