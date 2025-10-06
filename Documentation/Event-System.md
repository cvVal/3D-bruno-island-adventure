# Event System Documentation

## Overview

This project uses a centralized **Event Manager** with a **Publisher-Subscriber pattern** (Observer pattern) to enable decoupled communication between game systems. This architecture allows different components to communicate without direct references, improving maintainability and reducing coupling.

## Benefits of the Event System

✅ **Decoupling** - Publishers don't need references to subscribers  
✅ **Flexibility** - Multiple subscribers can react to the same event  
✅ **Maintainability** - Easy to add/remove event handlers  
✅ **Scalability** - New systems can subscribe without modifying existing code  
✅ **Debuggability** - Centralized location for all game events  

---

## Event Architecture

**Central Hub**: `EventManager` (static class)  
**Pattern**: Publisher-Subscriber (Observer)  
**Total Events**: 10 events

### Event List

| Event Name | Parameters | Purpose |
|------------|-----------|---------|
| `OnChangePlayerHealth` | `float newHealthPoints` | Update player health UI |
| `OnChangePlayerPotions` | `int newPotionCount` | Update potion count UI |
| `OnInitiateDialogue` | `TextAsset inkJson, GameObject npc` | Start NPC dialogue |
| `OnTreasureChestUnlocked` | `QuestItemSo itemSo, bool showUI` | Add item to inventory and optionally show UI |
| `OnToggleUI` | `bool isOpened` | Pause/unpause enemy AI when UI opens |
| `OnReward` | `RewardSo rewardSo` | Grant player bonuses (health, damage, weapon) |
| `OnPortalEnter` | `Collider player, int nextSceneIndex` | Save game state before scene transition |
| `OnCutSceneUpdated` | `bool isEnabled` | Enable/disable player input during cutscenes |
| `OnVictory` | none | Trigger victory screen |
| `OnGameOver` | none | Trigger game over screen |

---

## Event Flow Diagrams

### 1. OnChangePlayerHealth

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      OnChangePlayerHealth Event                         │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHERS (4):
┌──────────────────────┐
│ PlayerController     │ (Start) ───┐
│ • On game start/load │            │
└──────────────────────┘            │
                                    │
┌──────────────────────┐            │
│ PlayerController     │ (Reward)───┤
│ • After receiving    │            │
│   reward from NPC    │            │
└──────────────────────┘            │
                                    │         ┌─────────────────┐
┌──────────────────────┐            ├────────►│  EventManager   │
│ Health               │ (Damage)───┤         │  RaiseChange    │
│ • Player takes       │            │         │  PlayerHealth() │
│   damage             │            │         └─────────────────┘
└──────────────────────┘            │                  │
                                    │                  │
┌──────────────────────┐            │                  │
│ Health               │ (Heal)─────┘                  │
│ • Player uses potion │                               │
└──────────────────────┘                               │
                                                       │
                                                       ▼
SUBSCRIBER (1):                            ┌─────────────────────┐
                                           │   UIController      │
                                           │ • Updates health    │
                                           │   label in HUD      │
                                           └─────────────────────┘
```

### 2. OnChangePlayerPotions

```
┌─────────────────────────────────────────────────────────────────────────┐
│                     OnChangePlayerPotions Event                         │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHERS (3):
┌──────────────────────┐
│ Health               │ (Start) ───┐
│ • On game start      │            │
└──────────────────────┘            │
                                    │         ┌─────────────────┐
┌──────────────────────┐            ├────────►│  EventManager   │
│ Health               │ (Heal)─────┤         │  RaiseChange    │
│ • Player uses potion │            │         │  PlayerPotions()│
└──────────────────────┘            │         └─────────────────┘
                                    │                  │
┌──────────────────────┐            │                  │
│ PlayerController     │ (Reward)───┘                  │
│ • After receiving    │                               │
│   reward from NPC    │                               │
└──────────────────────┘                               │
                                                       ▼
SUBSCRIBER (1):                            ┌─────────────────────┐
                                           │   UIController      │
                                           │ • Updates potion    │
                                           │   count in HUD      │
                                           └─────────────────────┘
```

### 3. OnInitiateDialogue

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      OnInitiateDialogue Event                           │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ NpcController        │────────►│  EventManager   │
│ • Player interacts   │         │  RaiseInitiate  │
│   with NPC (E key)   │         │  Dialogue()     │
└──────────────────────┘         └─────────────────┘
                                          │
                                          │
                                          ▼
SUBSCRIBER (1):                 ┌─────────────────────┐
                                │   UIController      │
                                │ • Switches to       │
                                │   Dialogue State    │
                                │ • Shows dialogue UI │
                                │ • Passes Ink story  │
                                │   & NPC reference   │
                                └─────────────────────┘
```

### 4. OnTreasureChestUnlocked

```
┌─────────────────────────────────────────────────────────────────────────┐
│                   OnTreasureChestUnlocked Event                         │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────────┐
│ TreasureChest        │────────►│   EventManager      │
│ • Player opens chest │         │  RaiseTreasure      │
│   (showUI: true)     │         │  ChestUnlocked()    │
│                      │         └─────────────────────┘
│ • Chest opened in    │                  │
│   previous session   │                  │
│   (showUI: false)    │                  │
└──────────────────────┘                  │
                                          │
                    ┌─────────────────────┴─────────────────────┐
                    │                                           │
                    ▼                                           ▼
SUBSCRIBERS (2):                                   ┌─────────────────────┐
┌──────────────────────┐                           │   UIController      │
│ Inventory            │                           │ • Shows quest item  │
│ • Adds item to list  │                           │   acquired UI       │
│ • Logs acquisition   │                           │   (if showUI: true) │
└──────────────────────┘                           │ • Shows item icon   │
                                                   │   in HUD (always)   │
                                                   └─────────────────────┘
```

### 5. OnToggleUI

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         OnToggleUI Event                                │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ UIQuestItemState     │────────►│  EventManager   │
│ • EnterState()       │         │  RaiseToggleUI  │
│   (isOpened: true)   │         │  (true/false)   │
│                      │         └─────────────────┘
│ • SelectButton()     │                  │
│   (isOpened: false)  │                  │
└──────────────────────┘                  │
                                          ▼
SUBSCRIBER (1):                 ┌─────────────────────┐
                                │  EnemyController    │
                                │ • Sets HasOpenedUI  │
                                │   flag              │
                                │ • Prevents attack   │
                                │   state transitions │
                                │   during dialogue   │
                                └─────────────────────┘

FLOW:
Player opens chest → UIQuestItemState.EnterState() → RaiseToggleUI(true)
   → EnemyController.HasOpenedUI = true → Enemies pause attacks
   
Player closes UI → UIQuestItemState.SelectButton() → RaiseToggleUI(false)
   → EnemyController.HasOpenedUI = false → Enemies resume normal AI
```

### 6. OnReward

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           OnReward Event                                │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ Reward               │────────►│  EventManager   │
│ • SendReward()       │         │  RaiseReward()  │
│   called by NPC when │         └─────────────────┘
│   player completes   │                  │
│   quest              │                  │
└──────────────────────┘                  │
         ▲                                │
         │                                ▼
         │                     ┌─────────────────────┐
         │                     │  PlayerController   │
         │                     │ • Increases health  │
         │                     │ • Adds potions      │
         └─────────────────────│ • Boosts damage     │
                               │ • Swaps weapon      │
   ┌──────────────────────┐    │   (if forced)       │
   │ NpcController        │    └─────────────────────┘
   │ • CheckPlayerForQuest│              │
   │   Item() returns true│              │
   │ • Calls SendReward() │              ├─► RaiseChangePlayerHealth()
   └──────────────────────┘              └─► RaiseChangePlayerPotions()

FLOW:
Player completes dialogue with quest item → NpcController checks inventory
   → Reward.SendReward() → OnReward raised → PlayerController applies bonuses
   → Updates UI via health/potion events
```

### 7. OnPortalEnter

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        OnPortalEnter Event                              │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ Portal               │────────►│  EventManager   │
│ • Player collides    │         │  RaisePortal    │
│   with portal        │         │  Enter()        │
│ • Before scene       │         └─────────────────┘
│   transition         │                  │
└──────────────────────┘                  │
                                          ▼
SUBSCRIBER (1):                 ┌─────────────────────────┐
                                │   GameManager           │
                                │ • Saves player health   │
                                │ • Saves potion count    │
                                │ • Saves damage          │
                                │ • Saves weapon type     │
                                │ • Saves scene index     │
                                │ • Saves defeated enemies│
                                │ • Saves quest items     │
                                │ • Saves NPC quest states│
                                └─────────────────────────┘

FLOW:
Player enters portal → OnTriggerEnter → RaisePortalEnter(player, sceneIndex)
   → GameManager saves all progress to PlayerPrefs
   → SceneTransition.Initiate() loads next scene
```

### 8. OnCutSceneUpdated

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      OnCutSceneUpdated Event                            │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ CinematicController  │────────►│  EventManager   │
│ • Cutscene starts    │         │  RaiseCutScene  │
│   (isEnabled: false) │         │  Updated()      │
│                      │         └─────────────────┘
│ • Cutscene ends      │                  │
│   (isEnabled: true)  │                  │
└──────────────────────┘                  │
         ▲                                │
         │                                ▼
    PlayableDirector          ┌─────────────────────┐
    • played event            │   GameManager       │
    • stopped event           │ • Disables/enables  │
                              │   player input      │
                              │ • Prevents movement │
                              │   during cutscenes  │
                              └─────────────────────┘

FLOW:
Timeline starts → PlayableDirector.played → RaiseCutSceneUpdated(false)
   → GameManager disables player input

Timeline ends → PlayableDirector.stopped → RaiseCutSceneUpdated(true)
   → GameManager enables player input
```

### 9. OnVictory

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          OnVictory Event                                │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ Health               │────────►│  EventManager   │
│ • Final boss dies    │         │  RaiseVictory() │
│ • CompareTag         │         └─────────────────┘
│   "FinalBoss"        │                  │
│ • In defeat handler  │                  │
└──────────────────────┘                  │
         ▲                                │
         │                                ▼
    BubbleEvent                 ┌─────────────────────┐
    • OnBubbleComplete          │   UIController      │
      Defeat                    │ • Switches to       │
                                │   Victory State     │
                                │ • Shows victory UI  │
                                │ • Plays victory     │
                                │   sound             │
                                │ • Deletes save on   │
                                │   button press      │
                                └─────────────────────┘
```

### 10. OnGameOver

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         OnGameOver Event                                │
└─────────────────────────────────────────────────────────────────────────┘

PUBLISHER (1):
┌──────────────────────┐         ┌─────────────────┐
│ Health               │────────►│  EventManager   │
│ • Player dies        │         │  RaiseGameOver()│
│ • CompareTag         │         └─────────────────┘
│   "Player"           │                  │
│ • In defeat handler  │                  │
└──────────────────────┘                  │
         ▲                                │
         │                                ▼
    BubbleEvent                 ┌─────────────────────┐
    • OnBubbleComplete          │   UIController      │
      Defeat                    │ • Switches to       │
                                │   Game Over State   │
                                │ • Shows game over UI│
                                │ • Plays game over   │
                                │   sound             │
                                │ • Deletes save on   │
                                │   button press      │
                                └─────────────────────┘
```

---

## Complete Event Flow Map

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    COMPREHENSIVE EVENT FLOW MAP                            │
└────────────────────────────────────────────────────────────────────────────┘

GAMEPLAY SYSTEMS                EVENT MANAGER              UI SYSTEMS
═════════════════              ══════════════              ══════════

PlayerController ──────────┐
Health ─────────────────┐  │  OnChangePlayerHealth  ────────► UIController
                        └──┴──────────────────────────────► (Update HUD)

Health ──────────────┐
PlayerController ────┼──────  OnChangePlayerPotions ──────► UIController
                     └──────────────────────────────────► (Update HUD)

NpcController ─────────────── OnInitiateDialogue ─────────► UIController
                              ──────────────────────────► (Dialogue State)

TreasureChest ───────────┐    OnTreasureChestUnlocked ┌──► Inventory
                         ├────────────────────────────┤
                         └────────────────────────────┴──► UIController

UIQuestItemState ────────────  OnToggleUI ───────────────► EnemyController
                              ──────────────────────────► (Pause AI)

Reward ──────────────────────  OnReward ─────────────────► PlayerController
                              ──────────────────────────► (Apply Bonuses)

Portal ──────────────────────  OnPortalEnter ────────────► GameManager
                              ──────────────────────────► (Save Progress)

CinematicController ─────────  OnCutSceneUpdated ────────► GameManager
                              ──────────────────────────► (Toggle Input)

Health (FinalBoss) ──────────  OnVictory ────────────────► UIController
                              ──────────────────────────► (Victory State)

Health (Player) ─────────────  OnGameOver ───────────────► UIController
                              ──────────────────────────► (Game Over State)
```

---

## Event Details Reference

### Event 1: OnChangePlayerHealth

**Purpose**: Update the health display in the player's HUD.

**When Raised**:
- Game starts (initial health display)
- Player takes damage from enemy
- Player uses a healing potion
- Player receives a health reward from NPC

**Parameters**:
- `float newHealthPoints` - The new health value to display

**Publishers**:
1. `PlayerController.Start()` - Initial value
2. `PlayerController.HandleReward()` - After receiving reward
3. `Health.TakeDamage()` - After taking damage
4. `Health.HandleHeal()` - After using potion

**Subscribers**:
1. `UIController.HandleChangePlayerHealth()` - Updates `_healthLabel.text`

**Example Flow**:
```
Enemy attacks → Combat deals damage → Health.TakeDamage(15f)
   → RaiseChangePlayerHealth(85f) → UIController updates label to "85"
```

---

### Event 2: OnChangePlayerPotions

**Purpose**: Update the potion count display in the player's HUD.

**When Raised**:
- Game starts (initial potion count)
- Player uses a healing potion
- Player receives potion reward from NPC

**Parameters**:
- `int newPotionCount` - The new potion count to display

**Publishers**:
1. `Health.Start()` - Initial value
2. `Health.HandleHeal()` - After using potion
3. `PlayerController.HandleReward()` - After receiving reward

**Subscribers**:
1. `UIController.HandleChangePlayerPotions()` - Updates `_potionsLabel.text`

**Example Flow**:
```
Player presses H key → Health.HandleHeal() → potionCount--
   → RaiseChangePlayerPotions(2) → UIController updates label to "2"
```

---

### Event 3: OnInitiateDialogue

**Purpose**: Start a dialogue conversation with an NPC.

**When Raised**:
- Player presses interact button (E) while near NPC

**Parameters**:
- `TextAsset inkJson` - The Ink story file to display
- `GameObject npc` - Reference to the NPC GameObject

**Publishers**:
1. `NpcController.HandleInteract()` - When player interacts

**Subscribers**:
1. `UIController.HandleInitiateDialogue()` - Switches to Dialogue State

**Example Flow**:
```
Player near NPC → Presses E → NpcController.HandleInteract()
   → RaiseInitiateDialogue(inkJson, npcGameObject)
   → UIController switches to UIDialogueState
   → UIDialogueState.SetStory() initializes dialogue
```

---

### Event 4: OnTreasureChestUnlocked

**Purpose**: Notify systems that a quest item has been found.

**When Raised**:
- Player opens a treasure chest (first time)
- Chest is loaded that was previously opened (on scene load)

**Parameters**:
- `QuestItemSo itemSo` - The quest item that was found
- `bool showUI` - Whether to show the quest item UI overlay

**Publishers**:
1. `TreasureChest.HandleInteract()` - Player opens chest (`showUI: true`)
2. `TreasureChest.CheckItem()` - Previously opened chest (`showUI: false`)

**Subscribers**:
1. `Inventory.HandleTreasureChestUnlocked()` - Adds item to inventory
2. `UIController.HandleTreasureChestUnlocked()` - Shows UI and item icon

**Example Flow**:
```
Player opens chest → TreasureChest.HandleInteract()
   → RaiseTreasureChestUnlocked(itemSo, true)
   → Inventory adds item to list
   → UIController shows quest item UI overlay
   → Player presses button to close UI
```

**Note**: `showUI: false` is used when loading a previously opened chest to add the item to inventory without showing the UI again.

---

### Event 5: OnToggleUI

**Purpose**: Pause enemy AI when UI overlays are open.

**When Raised**:
- Quest item UI opens/closes

**Parameters**:
- `bool isOpened` - True if UI is opening, false if closing

**Publishers**:
1. `UIQuestItemState.EnterState()` - Raises `true`
2. `UIQuestItemState.SelectButton()` - Raises `false`

**Subscribers**:
1. `EnemyController.HandleToggleUI()` - Sets `HasOpenedUI` flag

**Example Flow**:
```
Quest item UI opens → UIQuestItemState.EnterState()
   → RaiseToggleUI(true) → EnemyController.HasOpenedUI = true
   → AIAttackState checks HasOpenedUI → Skips attack logic

Player closes UI → UIQuestItemState.SelectButton()
   → RaiseToggleUI(false) → EnemyController.HasOpenedUI = false
   → AI resumes normal behavior
```

**Why It's Needed**: Prevents enemies from attacking while the player is reading UI, which would feel unfair and break immersion.

---

### Event 6: OnReward

**Purpose**: Grant bonuses to the player after completing a quest.

**When Raised**:
- NPC verifies player has quest item during dialogue

**Parameters**:
- `RewardSo rewardSo` - ScriptableObject containing reward data

**Publishers**:
1. `Reward.SendReward()` - Called by NPC when quest is complete

**Subscribers**:
1. `PlayerController.HandleReward()` - Applies bonuses to player

**Example Flow**:
```
Player talks to NPC with quest item → NpcController.CheckPlayerForQuestItem()
   → Returns true → Reward.SendReward() → RaiseReward(rewardSo)
   → PlayerController receives reward:
      - HealthPoints += bonusHealth
      - potionCount += bonusPotion
      - Damage += bonusDamage
      - Weapon swap (if forced)
   → RaiseChangePlayerHealth() and RaiseChangePlayerPotions()
```

**Reward Data Includes**:
- `bonusHealth`: Health points to add
- `bonusPotion`: Potions to add
- `bonusDamage`: Damage increase
- `forceWeaponSwap`: Whether to change weapon
- `weapon`: New weapon type (if swapped)

---

### Event 7: OnPortalEnter

**Purpose**: Save all game progress before transitioning to next scene.

**When Raised**:
- Player enters a portal trigger

**Parameters**:
- `Collider player` - The player's collider component
- `int nextSceneIndex` - The build index of the next scene

**Publishers**:
1. `Portal.OnTriggerEnter()` - When player collides with portal

**Subscribers**:
1. `GameManager.HandlePortalEnter()` - Saves progress

**Example Flow**:
```
Player walks into portal → Portal.OnTriggerEnter()
   → RaisePortalEnter(playerCollider, 2)
   → GameManager.HandlePortalEnter() saves:
      - Player health, potions, damage, weapon
      - Scene index (for Continue button)
      - Defeated enemy IDs with timestamps
      - Quest items collected
      - NPC quest completion states
   → SceneTransition.Initiate(2) fades out and loads scene
```

**What Gets Saved**:
- `PlayerPrefsHealth`: Current health
- `PlayerPrefsPotions`: Potion count
- `PlayerPrefsDamage`: Attack damage
- `PlayerPrefsWeapon`: Current weapon
- `PlayerPrefsSceneIndex`: Next scene (for Continue)
- `PlayerPrefsQuestItems`: Items in inventory
- `PlayerPrefsNpcItems`: NPCs with completed quests
- Enemy defeat timestamps (for respawn system)

---

### Event 8: OnCutSceneUpdated

**Purpose**: Disable/enable player input during cinematic sequences.

**When Raised**:
- Cutscene starts or stops (via Timeline/PlayableDirector)

**Parameters**:
- `bool isEnabled` - False when cutscene starts, true when it ends

**Publishers**:
1. `CinematicController.HandlePlayedEvent()` - Raises `false`
2. `CinematicController.HandleStoppedEvent()` - Raises `true`

**Subscribers**:
1. `GameManager.HandleCutSceneUpdated()` - Toggles player input

**Example Flow**:
```
Player triggers cutscene → PlayableDirector.Play()
   → PlayableDirector.played event → HandlePlayedEvent()
   → RaiseCutSceneUpdated(false)
   → GameManager disables PlayerInput component
   → Cutscene plays (player can't move)

Cutscene ends → PlayableDirector.stopped event → HandleStoppedEvent()
   → RaiseCutSceneUpdated(true)
   → GameManager enables PlayerInput component
   → Player regains control
```

**Additional Behaviors**:
- Cutscene cameras are enabled/disabled automatically
- Cutscenes don't play if associated enemy is defeated
- One-time cutscenes use collider triggers

---

### Event 9: OnVictory

**Purpose**: Trigger the victory screen when the final boss is defeated.

**When Raised**:
- Final boss's defeat animation completes

**Parameters**: None

**Publishers**:
1. `Health.HandleBubbleCompleteDefeat()` - When FinalBoss tag is detected

**Subscribers**:
1. `UIController.HandleVictory()` - Switches to Victory State

**Example Flow**:
```
Player defeats final boss → Health reaches 0 → Defeated()
   → Animator plays defeat animation
   → Animation completes → BubbleEvent.OnCompleteDefeat fires
   → Health.HandleBubbleCompleteDefeat() checks tag
   → CompareTag("FinalBoss") == true
   → RaiseVictory()
   → UIController switches to UIVictoryState
   → Victory UI appears with celebration music
   → Player presses button → Returns to main menu
```

---

### Event 10: OnGameOver

**Purpose**: Trigger the game over screen when the player dies.

**When Raised**:
- Player's defeat animation completes

**Parameters**: None

**Publishers**:
1. `Health.HandleBubbleCompleteDefeat()` - When Player tag is detected

**Subscribers**:
1. `UIController.HandleGameOver()` - Switches to Game Over State

**Example Flow**:
```
Player takes fatal damage → Health reaches 0 → Defeated()
   → Animator plays defeat animation
   → Animation completes → BubbleEvent.OnCompleteDefeat fires
   → Health.HandleBubbleCompleteDefeat() checks tag
   → CompareTag("Player") == true
   → RaiseGameOver()
   → UIController switches to UIGameOverState
   → Game over UI appears with somber music
   → Player presses button → Returns to main menu
```

---

## Event Subscription Patterns

### Proper Subscription/Unsubscription

All event subscribers follow this pattern to prevent memory leaks:

```csharp
private void OnEnable()
{
    EventManager.OnEventName += HandleEventName;
}

private void OnDisable()
{
    EventManager.OnEventName -= HandleEventName;
}

private void HandleEventName(/* parameters */)
{
    // Handle event
}
```

**Why OnEnable/OnDisable?**
- Events are subscribed when component is enabled
- Events are unsubscribed when component is disabled
- Prevents null references when scenes change
- Avoids duplicate subscriptions

---

## Event Categories

### UI Update Events
- `OnChangePlayerHealth` - Real-time health updates
- `OnChangePlayerPotions` - Real-time potion updates

### Gameplay Events
- `OnInitiateDialogue` - Start NPC conversations
- `OnTreasureChestUnlocked` - Item acquisition
- `OnToggleUI` - Pause game systems
- `OnReward` - Apply player bonuses

### System Events
- `OnPortalEnter` - Save/load system
- `OnCutSceneUpdated` - Input management

### Game State Events
- `OnVictory` - Win condition
- `OnGameOver` - Lose condition

---

## How to Add a New Event

### Step 1: Add Event to EventManager

```csharp
public static class EventManager
{
    public static event UnityAction<YourDataType> OnYourEvent;
    
    public static void RaiseYourEvent(YourDataType data) =>
        OnYourEvent?.Invoke(data);
}
```

### Step 2: Raise Event from Publisher

```csharp
public class YourPublisher : MonoBehaviour
{
    private void SomeMethod()
    {
        // Your logic
        EventManager.RaiseYourEvent(yourData);
    }
}
```

### Step 3: Subscribe in Subscriber

```csharp
public class YourSubscriber : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnYourEvent += HandleYourEvent;
    }
    
    private void OnDisable()
    {
        EventManager.OnYourEvent -= HandleYourEvent;
    }
    
    private void HandleYourEvent(YourDataType data)
    {
        // Handle event
    }
}
```

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         EVENT SYSTEM ARCHITECTURE                       │
└─────────────────────────────────────────────────────────────────────────┘

LAYER 1: PUBLISHERS          LAYER 2: EVENT HUB       LAYER 3: SUBSCRIBERS
════════════════════         ══════════════════       ════════════════════

┌─────────────────┐
│ PlayerController│──┐
└─────────────────┘  │
┌─────────────────┐  │
│ Health          │──┤
└─────────────────┘  │       ┌──────────────┐       ┌─────────────────┐
┌─────────────────┐  ├──────►│              │──────►│ UIController    │
│ NpcController   │──┤       │ EventManager │       └─────────────────┘
└─────────────────┘  │       │  (Static)    │       ┌─────────────────┐
┌─────────────────┐  │       │              │──────►│ GameManager     │
│ TreasureChest   │──┤       │  10 Events   │       └─────────────────┘
└─────────────────┘  │       │              │       ┌─────────────────┐
┌─────────────────┐  │       │              │──────►│ EnemyController │
│ Portal          │──┤       └──────────────┘       └─────────────────┘
└─────────────────┘  │                              ┌─────────────────┐
┌─────────────────┐  │                              │ Inventory       │
│ Reward          │──┤                              └─────────────────┘
└─────────────────┘  │                              ┌─────────────────┐
┌─────────────────┐  │                              │ PlayerController│
│ Cinematic       │──┘                              └─────────────────┘
│ Controller      │
└─────────────────┘

         Loose Coupling - No Direct References Between Layers
```

---

## Summary

The Event System in this project provides a robust, decoupled architecture for game-wide communication. By centralizing all events in the `EventManager`, the system achieves:

- **Clear separation** between publishers and subscribers
- **Easy debugging** with a single source of truth
- **Scalability** for adding new features without modifying existing code
- **Maintainability** through consistent patterns and practices

All 10 events work together to create a cohesive gameplay experience, from UI updates to scene transitions, quest systems to game state management.

