# Inventory Management

## Solution Architecture

### Event-Driven Design
The solution uses an established event-driven architecture to maintain clean separation of concerns.

#### 1. **EventManager** (Central Event Hub)
```csharp
public static event UnityAction<int> OnInventoryChanged;
public static void RaiseInventoryChanged(int itemCount);
```
- **Purpose**: Notifies UI when inventory state changes
- **Parameter**: `itemCount` - current number of items in inventory
- **Scalable**: Can be extended for more inventory features

#### 2. **Inventory** (Data Layer)
**`RemoveItem()` method:**
```csharp
public void RemoveItem(QuestItemSo itemToRemove)
{
    if (!items.Contains(itemToRemove)) return;
    
    items.Remove(itemToRemove);
    Debug.Log($"Removed {itemToRemove.itemName} from inventory.");
    
    // Notify UI about inventory change
    EventManager.RaiseInventoryChanged(items.Count);
}
```
- **When called**: After quest completion
- **What it does**: 
  - Removes item from list
  - Raises `OnInventoryChanged` event

**`HandleTreasureChestUnlocked()`:**
- Now raises `OnInventoryChanged` when items are added

#### 3. **NpcController** (Domain Logic)
**`CheckPlayerForQuestItem()`:**
```csharp
if (_rewardCmp && hasQuestItem)
{
    _playerInventoryCmp.RemoveItem(desiredQuestItem); // NEW: Remove item
    _rewardCmp.SendReward();
}
```
- **When**: Quest is verified as complete
- **What**: Removes quest item from inventory before giving reward

#### 4. **UIController** (Presentation Layer)
**Event subscription:**
```csharp
EventManager.OnInventoryChanged += HandleInventoryChanged;
```

**New handler:**
```csharp
private void HandleInventoryChanged(int itemCount)
{
    // Show icon only if there are items in inventory
    _questItemIcon.style.display = itemCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
}
```
- **When**: Any time inventory changes (add/remove)
- **What**: Shows/hides quest item icon based on inventory state

## Flow Diagram

### Quest Completion Flow
```
Player hands item to NPC
    ↓
NpcController.CheckPlayerForQuestItem()
    ↓
Inventory.RemoveItem(item)
    ↓
EventManager.RaiseInventoryChanged(0) ← itemCount = 0
    ↓
UIController.HandleInventoryChanged(0)
    ↓
Quest icon hidden ✅
```

### Treasure Chest Flow
```
Player opens chest
    ↓
Inventory.HandleTreasureChestUnlocked(item)
    ↓
EventManager.RaiseInventoryChanged(1) ← itemCount = 1
    ↓
UIController.HandleInventoryChanged(1)
    ↓
Quest icon shown ✅
```

## Architecture Benefits

### ✅ Separation of Concerns
- **Inventory**: Owns data, doesn't know about UI
- **UIController**: Displays data, doesn't know about game logic
- **EventManager**: Decouples components

### ✅ Scalability
- Easy to add more inventory features (e.g., multiple items)
- Can add more UI elements that react to inventory changes
- Event pattern supports unlimited subscribers

### ✅ Maintainability
- Changes to inventory logic don't affect UI
- Changes to UI don't affect game logic
- Clear, single responsibility for each component

### ✅ Reusability
- `RemoveItem()` can be used by any system (trading, dropping, etc.)
- `OnInventoryChanged` can notify multiple UI elements
- Pattern extends to other inventory-related features
