# State Pattern Implementation

## Overview

This project uses the **State Pattern** to manage complex behavior for both AI enemies and UI screens. The State Pattern allows an object to alter its behavior when its internal state changes, making the code more maintainable and easier to extend.

## Benefits of the State Pattern

✅ **Separation of Concerns** - Each state encapsulates its own behavior  
✅ **Easy to Extend** - Add new states without modifying existing ones  
✅ **Clear Transitions** - State changes are explicit and traceable  
✅ **Reduced Complexity** - Eliminates massive if/else or switch statements  
✅ **Testability** - Each state can be tested independently  

---

## AI State Machine (Enemy Behavior)

The AI state machine controls enemy behavior including patrol, chase, attack, return, and defeat states.

### Architecture

**Context Class**: `EnemyController`  
**Base State**: `AIBaseState`  
**Concrete States**: 
- `AIPatrolState`
- `AIChaseState`
- `AIAttackState`
- `AIReturnState`
- `AIDefeatedState`

### State Transition Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         AI STATE MACHINE                                │
└─────────────────────────────────────────────────────────────────────────┘

                              START (Non-Patrol Enemy)
                                        │
                                        ▼
                            ┌─────────────────────┐
                            │   Return State      │◄─────────┐
                            │   (Idle/Guard)      │          │
                            └─────────────────────┘          │
                                 │            ▲              │
                   Player enters │            │ Player exits │
                   chase range   │            │ chase range  │
                                 ▼            │              │
                            ┌─────────────────────┐          │
                ┌──────────►│   Chase State       │──────────┘
                │           │   (Pursue Player)   │
                │           └─────────────────────┘
                │                │            ▲
                │  Player exits  │            │ Player exits
                │  attack range  │            │ attack range
                │                ▼            │
                │           ┌─────────────────────┐
                └───────────│   Attack State      │
                            │   (Combat)          │
                            └─────────────────────┘


                              START (Patrol Enemy)
                                        │
                                        ▼
                            ┌─────────────────────┐
                    ┌──────►│   Patrol State      │◄─────────┐
                    │       │   (Walk Route)      │          │
                    │       └─────────────────────┘          │
                    │            │            ▲              │
                    │  Player    │            │ Reached      │
                    │  enters    │            │ patrol       │
                    │  chase     │            │ point        │
                    │  range     ▼            │              │
                    │       ┌─────────────────────┐          │
                    │       │   Return State      │──────────┘
                    │       │   (Back to Route)   │
                    │       └─────────────────────┘
                    │            │            ▲
                    │  Player    │            │ Player exits
                    │  still in  │            │ chase range
                    │  range     ▼            │
                    │       ┌─────────────────────┐
                    └───────│   Chase State       │
                            │   (Pursue Player)   │
                            └─────────────────────┘
                                 │            ▲
                   Player enters │            │ Player exits
                   attack range  │            │ attack range
                                 ▼            │
                            ┌─────────────────────┐
                            │   Attack State      │
                            │   (Combat)          │
                            └─────────────────────┘


                         ANY STATE (Health reaches 0)
                                        │
                                        ▼
                            ┌─────────────────────┐
                            │  Defeated State     │
                            │  (Death Animation)  │
                            └─────────────────────┘
                                        │
                                        ▼
                                  [DESTROYED]
```

### State Descriptions

#### 1. **Patrol State** (`AIPatrolState`)
**Purpose**: Enemy walks along a predefined patrol route (using Splines).

**Behavior**:
- Calculates next position in patrol route
- Moves agent toward next waypoint
- Rotates to face movement direction

**Transitions**:
- → **Chase State**: When player enters `chaseRange`

**Entry Conditions**: 
- Enemy has `Patrol` component
- Returned to patrol route from Return State

---

#### 2. **Chase State** (`AIChaseState`)
**Purpose**: Enemy pursues the player.

**Behavior**:
- Increases movement speed to `runSpeed`
- Moves directly toward player position
- Continuously rotates to face player
- NavMesh pathfinding to navigate obstacles

**Transitions**:
- → **Attack State**: When player is within `attackRange`
- → **Return State**: When player exits `chaseRange`

**Entry Conditions**: 
- Player enters chase range from any non-defeated state

---

#### 3. **Attack State** (`AIAttackState`)
**Purpose**: Enemy attacks the player.

**Behavior**:
- Stops all movement
- Faces player
- Initiates attack animation
- Deals damage via `Combat` component

**Transitions**:
- → **Chase State**: When player moves outside `attackRange`

**Entry Conditions**: 
- Player is within attack range
- UI is not open (`!HasOpenedUI`)

**Special Cases**:
- Cancels attack if player reference is lost
- Pauses when UI (dialogue) is opened

---

#### 4. **Return State** (`AIReturnState`)
**Purpose**: Enemy returns to original position or patrol route.

**Behavior**:
- Reduces speed to `walkSpeed`
- Navigates back to original position or next patrol point
- Rotates to face destination

**For Patrol Enemies**:
- Returns to nearest patrol waypoint
- → **Patrol State**: When waypoint is reached

**For Stationary Enemies**:
- Returns to spawn position
- Faces original direction when arrived
- Remains in Return State (acts as Idle)

**Transitions**:
- → **Chase State**: If player re-enters `chaseRange` during return
- → **Patrol State**: When patrol enemy reaches waypoint

**Entry Conditions**: 
- Player exits chase range while in Chase State

---

#### 5. **Defeated State** (`AIDefeatedState`)
**Purpose**: Handles enemy death.

**Behavior**:
- Plays death sound effect
- Triggers death animation (via Animator)
- No update logic (terminal state)
- Enemy GameObject is destroyed after animation completes

**Transitions**: 
- None (terminal state)

**Entry Conditions**: 
- `Health.HealthPoints` reaches 0
- `OnStartDefeated` event is triggered

---

### Implementation Details

#### EnemyController (Context)

```csharp
public class EnemyController : MonoBehaviour
{
    private AIBaseState _currentState;
    
    // State instances (created once, reused)
    public readonly AIReturnState ReturnState = new();
    public readonly AIChaseState ChaseState = new();
    public readonly AIAttackState AttackState = new();
    public readonly AIPatrolState PatrolState = new();
    public readonly AIDefeatedState DefeatedState = new();
    
    public float chaseRange = 2.5f;
    public float attackRange = 0.75f;
    
    public void SwitchState(AIBaseState newState)
    {
        _currentState = newState;
        _currentState.EnterState(this);
    }
}
```

#### Key Design Decisions

1. **State Instances are Readonly**: States are created once in `EnemyController` and reused, avoiding garbage collection overhead.

2. **States are Stateless**: AI states don't store data; they use the `EnemyController` context for all information.

3. **Distance-Based Transitions**: Most transitions are triggered by `DistanceFromPlayer` checks against `chaseRange` and `attackRange`.

4. **Two Starting States**: 
   - Enemies with `Patrol` component → Start in Patrol State
   - Enemies without → Start in Return State (acts as Idle)

---

## UI State Machine (Screen Management)

The UI state machine manages different UI screens and overlays including menus, dialogues, and game-over screens.

### Architecture

**Context Class**: `UIController`  
**Base State**: `UIBaseState`  
**Concrete States**: 
- `UIMainMenuState`
- `UIDialogueState`
- `UIQuestItemState`
- `UIVictoryState`
- `UIGameOverState`

### State Transition Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         UI STATE MACHINE                                │
└─────────────────────────────────────────────────────────────────────────┘

                          GAME START (Scene 0)
                                    │
                                    ▼
                        ┌─────────────────────┐
                        │  Main Menu State    │
                        │  (Start/Continue)   │
                        └─────────────────────┘
                                    │
                         Player selects option
                                    │
                                    ▼
                            [Scene Transition]
                                    │
                                    ▼
                          GAMEPLAY (Scene 1+)
                           [No Active State]
                     (Player HUD visible only)
                                    │
            ┌───────────────────────┼───────────────────────┐
            │                       │                       │
   Player talks to NPC    Treasure chest opened    Player dies / wins
            │                       │                       │
            ▼                       ▼                       ▼
┌─────────────────────┐ ┌─────────────────────┐ ┌─────────────────────┐
│  Dialogue State     │ │  Quest Item State   │ │  Game Over State    │
│  (NPC Conversation) │ │  (Item Acquired)    │ │  (Death Screen)     │
└─────────────────────┘ └─────────────────────┘ └─────────────────────┘
            │                       │                       │
   Dialogue ends          Player confirms         Player selects
            │                       │                "Go to Main Menu"
            ▼                       ▼                       │
    [Back to Gameplay]      [Back to Gameplay]             │
                                                            ▼
                                                  ┌─────────────────────┐
                                                  │  Main Menu State    │
                                                  │  (Scene 0)          │
                                                  └─────────────────────┘

                                                  ┌─────────────────────┐
                          Final Boss defeated ───►│  Victory State      │
                                                  │  (Win Screen)       │
                                                  └─────────────────────┘
                                                            │
                                                 Player selects 
                                                    "Go to Main Menu"
                                                            │
                                                            ▼
                                                  ┌─────────────────────┐
                                                  │  Main Menu State    │
                                                  │  (Scene 0)          │
                                                  └─────────────────────┘
```

### State Flow Chart (Detailed)

```
Event-Driven Transitions:

EventManager.OnInitiateDialogue
    │
    └──► UIDialogueState.EnterState()
              │
              │ (Dialogue continues via SelectButton)
              │
              └──► Story.canContinue == false
                        │
                        └──► ExitDialogue() → [Back to Gameplay]


EventManager.OnTreasureChestUnlocked (showUI: true)
    │
    └──► UIQuestItemState.EnterState()
              │
              │ (Player presses button)
              │
              └──► SelectButton() → [Back to Gameplay]


EventManager.OnGameOver
    │
    └──► UIGameOverState.EnterState()
              │
              │ (Player presses button)
              │
              └──► SelectButton() → SceneTransition.Initiate(0) → [Main Menu]


EventManager.OnVictory
    │
    └──► UIVictoryState.EnterState()
              │
              │ (Player presses button)
              │
              └──► SelectButton() → SceneTransition.Initiate(0) → [Main Menu]
```

### State Descriptions

#### 1. **Main Menu State** (`UIMainMenuState`)
**Purpose**: Display the main menu with Start/Continue options.

**Behavior**:
- Shows main menu container
- Checks for saved game (PlayerPrefs)
- Adds "Continue" button if save exists
- Handles button navigation
- Deletes save data on "Start" selection

**Transitions**:
- → **[Gameplay Scene]**: Via `SceneTransition.Initiate(sceneIndex)`
- No return to this state without scene transition

**Entry Conditions**: 
- Game starts (Scene 0 loaded)
- Player returns from Game Over or Victory states

---

#### 2. **Dialogue State** (`UIDialogueState`)
**Purpose**: Display NPC dialogue and player choices.

**Behavior**:
- Shows dialogue container
- Switches to UI action map (disables player movement)
- Reads Ink story text line by line
- Displays dialogue choices when available
- Handles button navigation for choices
- Binds external Ink functions (quest verification)

**Transitions**:
- → **[Back to Gameplay]**: When `Story.canContinue == false` (dialogue ends)

**Entry Conditions**: 
- `EventManager.OnInitiateDialogue` is raised
- Player interacts with NPC

**Special Features**:
- Integrates with Ink narrative system
- Supports branching dialogue with choices
- Can verify quest completion mid-dialogue

---

#### 3. **Quest Item State** (`UIQuestItemState`)
**Purpose**: Show acquired quest item notification.

**Behavior**:
- Displays quest item container with item name
- Switches to UI action map
- Pauses game (raises `OnToggleUI` event)
- Waits for player confirmation

**Transitions**:
- → **[Back to Gameplay]**: When player presses interact button

**Entry Conditions**: 
- `EventManager.OnTreasureChestUnlocked` is raised
- `showUI` parameter is `true`

**Note**: Quest item icon appears in HUD regardless of state, but the overlay only shows when `showUI` is true.

---

#### 4. **Victory State** (`UIVictoryState`)
**Purpose**: Display victory screen after defeating final boss.

**Behavior**:
- Shows victory container
- Switches to UI action map
- Plays victory sound effect
- Deletes all save data on confirmation

**Transitions**:
- → **[Main Menu Scene]**: Via `SceneTransition.Initiate(0)`

**Entry Conditions**: 
- `EventManager.OnVictory` is raised
- Final boss is defeated

---

#### 5. **Game Over State** (`UIGameOverState`)
**Purpose**: Display game over screen when player dies.

**Behavior**:
- Shows game over container
- Switches to UI action map
- Plays game over sound effect
- Deletes all save data on confirmation

**Transitions**:
- → **[Main Menu Scene]**: Via `SceneTransition.Initiate(0)`

**Entry Conditions**: 
- `EventManager.OnGameOver` is raised
- Player health reaches 0

---

### Implementation Details

#### UIController (Context)

```csharp
public class UIController : MonoBehaviour
{
    private UIBaseState _currentState;
    
    // State instances (created once in Awake)
    private UIMainMenuState _mainMenuState;
    private UIDialogueState _dialogueState;
    private UIQuestItemState _questItemState;
    private UIVictoryState _victoryState;
    private UIGameOverState _gameOverState;
    
    private void HandleInitiateDialogue(TextAsset inkJson, GameObject npc)
    {
        _currentState = _dialogueState;
        _currentState.EnterState();
        (_currentState as UIDialogueState)?.SetStory(inkJson, npc);
    }
}
```

#### Key Design Decisions

1. **Event-Driven**: UI state transitions are triggered by `EventManager` events, not by states themselves.

2. **No Active State During Gameplay**: Unlike AI, the UI state machine has no active state during normal gameplay (only HUD is visible).

3. **States Store References**: UI states cache references to UI elements in `EnterState()` to avoid repeated queries.

4. **Input Handling**: 
   - `EnterState()`: Switches input action map to "UI"
   - Exit logic: Switches back to "Gameplay" action map

5. **State-Specific Data**: Some states have specialized methods:
   - `UIDialogueState.SetStory()`: Initializes Ink story
   - `UIQuestItemState.SetQuestItemLabel()`: Sets item name

---

## Comparison: AI vs UI State Machines

| Aspect | AI State Machine | UI State Machine |
|--------|------------------|------------------|
| **Active During Gameplay** | Always active | Only during UI interactions |
| **Transition Logic** | Within states (distance checks) | External (EventManager) |
| **State Count** | 5 states | 5 states |
| **State Persistence** | States are readonly, reused | States created once in Awake |
| **Update Frequency** | Every frame (`Update()`) | Event-driven (no Update) |
| **Complexity** | High (spatial awareness, pathfinding) | Medium (UI element manipulation) |

---

## How to Add a New State

### For AI States:

1. **Create State Class**:
   ```csharp
   public class AIStunnedState : AIBaseState
   {
       public override void EnterState(EnemyController enemy)
       {
           // Initialization logic
       }
       
       public override void UpdateState(EnemyController enemy)
       {
           // Per-frame logic
           // Transition checks
       }
   }
   ```

2. **Add to EnemyController**:
   ```csharp
   public readonly AIStunnedState StunnedState = new();
   ```

3. **Implement Transitions**:
   - Add transition logic in relevant states
   - Consider all entry/exit conditions

### For UI States:

1. **Create State Class**:
   ```csharp
   public class UIPauseMenuState : UIBaseState
   {
       public UIPauseMenuState(UIController ui) : base(ui) { }
       
       public override void EnterState()
       {
           // Show pause menu UI
       }
       
       public override void SelectButton()
       {
           // Handle button selection
       }
   }
   ```

2. **Add to UIController**:
   ```csharp
   private UIPauseMenuState _pauseMenuState;
   
   private void Awake()
   {
       _pauseMenuState = new UIPauseMenuState(this);
   }
   ```

3. **Create Event Handler**:
   ```csharp
   private void HandlePauseGame()
   {
       _currentState = _pauseMenuState;
       _currentState.EnterState();
   }
   ```

4. **Subscribe to Event**:
   ```csharp
   EventManager.OnPauseGame += HandlePauseGame;
   ```

---
