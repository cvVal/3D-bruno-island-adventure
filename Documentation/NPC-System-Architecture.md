# NPC System Architecture - Data-Driven Design

## Solution: Data-Driven Configuration

**Current Approach (ScriptableObject-based):**
- ✅ **One script (`NpcController`) for ALL NPCs**
- ✅ **Designers configure NPCs via Inspector**
- ✅ **No code changes needed for new NPCs**
- ✅ **Reusable behavior assets**

## How It Works

### 1. Create Behavior Assets

Create ScriptableObject assets that define NPC behaviors:

**In Unity:**
1. Right-click in Project → `Create → RPG → NPC Behavior`
2. Name it descriptively (e.g., "Quest NPC Behavior", "Merchant NPC Behavior")
3. Configure checkboxes for what functions to bind

**Example Assets:**
```
Assets/Scriptable Objects/NPC Behaviors/
├── Quest NPC Behavior.asset          (hasQuestVerification = true)
├── Merchant NPC Behavior.asset       (hasMerchantFunctions = true)
├── Dialogue Only NPC Behavior.asset  (all unchecked or null)
└── Companion NPC Behavior.asset      (hasCompanionFunctions = true)
```

### 2. Configure NPCs

**For Quest NPCs (like Pumpkin Jack):**
1. Add `NpcController` component
2. Assign fields in Inspector:
   - **Ink Json**: The dialogue file
   - **Npc Behavior**: Drag "Quest NPC Behavior" asset
   - **Desired Quest Item**: The item this NPC wants
   - **Reward** component: Assign RewardSo asset
3. Done!

**For Dialogue-Only NPCs:**
1. Add `NpcController` component
2. Assign:
   - **Ink Json**: The dialogue file
   - **Npc Behavior**: Leave empty or null
3. Done!

### 3. Architecture Diagram

```
┌─────────────────────────────────────┐
│         NpcController               │
│  (One script for ALL NPCs)          │
│                                     │
│  - Handles dialogue                 │
│  - Binds functions based on config  │
│  - Quest logic (if enabled)         │
└─────────────────────────────────────┘
              ↓ uses
┌─────────────────────────────────────┐
│      NpcBehaviorSo (Asset)          │
│  (Reusable configuration)           │
│                                     │
│  ☑ hasQuestVerification             │
│  ☐ hasMerchantFunctions             │
│  ☐ hasCompanionFunctions            │
└─────────────────────────────────────┘
```

## Adding New Behavior Types

**Example: Adding Merchant NPCs**

### Step 1: Update NpcBehaviorSo.cs
```csharp
[Tooltip("Bind merchant-related functions")]
public bool hasMerchantFunctions;
```

### Step 2: Update NpcController.BindExternalFunctions()
```csharp
private void BindExternalFunctions()
{
    if (npcBehavior == null) return;
    
    // Quest verification
    if (npcBehavior.hasQuestVerification)
    {
        if (desiredQuestItem != null)
        {
            _story.BindExternalFunction(Constants.InkStoryVerifyQuestFunc, VerifyQuest);
        }
    }
    
    // Merchant functions (NEW)
    if (npcBehavior.hasMerchantFunctions)
    {
        _story.BindExternalFunction("CheckGold", CheckPlayerGold);
        _story.BindExternalFunction("BuyItem", PurchaseItem);
    }
}
```

### Step 3: Implement methods in NpcController
```csharp
private void CheckPlayerGold()
{
    _story.variablesState["hasEnoughGold"] = /* check player gold */;
}

private void PurchaseItem()
{
    // Purchase logic here
}
```

### Step 4: Create Asset & Use
1. Create "Merchant NPC Behavior" asset
2. Check `hasMerchantFunctions` box
