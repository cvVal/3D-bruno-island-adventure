# Dialogue System Architecture

## Overview
Reusable dialogue system supporting multiple NPCs with persistent conversation state across interactions.

## Key Architecture Decisions

### 1. **Story Instance Per NPC**
- Each `NpcController` owns a single `Story` instance (created once, reused forever)
- Variables (e.g., `hasMetJack`, `questCompleted`) **persist** between conversations
- Story **flow** resets to beginning via `ResetCallstack()` + `ChoosePathString("start")`

### 2. **External Function Binding**
- External functions bound **once** when Story is created in `BindExternalFunctions()`
- Prevents "Function already bound" exceptions
- Functions persist across story resets (callstack resets, not bindings)

### 3. **Separation of Concerns**

#### NpcController (Domain Layer)
- Owns the Ink Story instance
- Binds external functions via data-driven `NpcBehaviorSo` configuration
- Implements game logic (quest verification, inventory checks, rewards)

#### UIDialogueState (Presentation Layer)
- Renders dialogue UI
- Handles typewriter/fade effects
- Processes user input

## How It Works

### First Conversation
1. Player interacts → `GetOrCreateStory()` creates new Story
2. `BindExternalFunctions()` reads `NpcBehaviorSo` and binds functions accordingly
3. `hasMetJack = false` → First meeting dialogue
4. Conversation sets `hasMetJack = true`

### Subsequent Conversations
1. Player interacts → `GetOrCreateStory()` finds existing Story
2. `ResetCallstack()` clears execution stack
3. `ChoosePathString("start")` jumps to beginning
4. `hasMetJack = true` (preserved!) → "Already met" dialogue ✅

### Quest Verification Example

**In Ink file:**
```ink
EXTERNAL VerifyQuest()

VAR questCompleted = false

=== ask_if_found ===
~ VerifyQuest()
{ questCompleted:
    -> success
- else:
    -> still_searching
}
```

**In NpcController:**
```csharp
private void VerifyQuest()
{
    if (_story == null) return;
    _story.variablesState[Constants.InkStoryQuestCompletedVar] = 
        CheckPlayerForQuestItem();
}
```
