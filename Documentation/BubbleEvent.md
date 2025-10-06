# BubbleEvent System Documentation

## What is BubbleEvent?

`BubbleEvent` is an event handler component that bridges Unity's Animation Events with C# events. It acts as a communication layer between character animations (from the KayKit Animated Character asset) and your game's combat and health systems.

Instead of directly calling methods on other components from animation events, `BubbleEvent` receives these animation callbacks and forwards them as `UnityAction` events that multiple systems can subscribe to.

## Why Use BubbleEvent?

### Problems It Solves:
1. **Decoupling**: Animation clips shouldn't directly reference game logic components
2. **Flexibility**: Multiple systems can react to the same animation event
3. **Maintainability**: Easier to modify behavior without editing animation clips
4. **Clean Architecture**: Follows the Observer pattern for better code organization

### Without BubbleEvent:
```
Animation Clip → Direct Method Call on Combat Component
```
❌ Tight coupling, hard to extend

### With BubbleEvent:
```
Animation Clip → BubbleEvent → Event Published → Multiple Subscribers (Combat, Health, etc.)
```
✅ Loose coupling, easy to extend

## Where Is It Used?

The `BubbleEvent` component is attached to character GameObjects as a child component (KayKit Animated Character_v1.2 that contains the Animator). It's used by:

- **Combat.cs**: Subscribes to attack-related events
- **Health.cs**: Subscribes to defeat-related events
- Any character using the KayKit Animated Character model

## Available Events

| Event | Description | Typical Use Case |
|-------|-------------|------------------|
| `OnBubbleStartAttack` | Fired when attack animation begins | Set `IsAttacking = true`, lock movement |
| `OnBubbleCompleteAttack` | Fired when attack animation ends | Set `IsAttacking = false`, clear hit targets |
| `OnBubbleHit` | Fired at the exact frame the attack should deal damage | Perform hitbox detection, apply damage |
| `OnBubbleCompleteDefeat` | Fired when defeat animation finishes | Destroy enemy, spawn rewards, trigger game over |

## How to Use BubbleEvent

### Step 1: Add BubbleEvent Component

Add the `BubbleEvent` component to your character's model GameObject (the child that has the `Animator` component):

```
Player GameObject
└── Character Model (Animator here)
    └── BubbleEvent component ← Add here
```

### Step 2: Subscribe to Events

In your game logic components (like `Combat.cs` or `Health.cs`).

### Step 3: Configure Animation Events

In the Unity Editor, configure Animation Events on your animation clips:

1. Select the animation clip (e.g., `Attack.anim` from KayKit Animated Character)
2. Open the Animation window
3. Add Animation Events at specific keyframes
4. Set the Function name to match BubbleEvent's private methods:
   - `OnStartAttack` - At the beginning of the attack
   - `OnHit` - At the exact moment of impact (usually mid-animation)
   - `OnCompleteAttack` - At the end of the attack
   - `OnCompleteDefeat` - At the end of the defeat animation

## Configuration on KayKit Animated Character Clips

The KayKit Animated Character asset (v1.2) comes with pre-made animation clips. Here's how to configure Animation Events on them:

### Attack Animation Setup

**Animation Clip**: `Attack` (from KayKit Animated Character_v1.2.fbx)

1. **Open the Animation Window**:
   - Window → Animation → Animation
   - Select your character prefab with the Animator

2. **Add Animation Events**:
   - Click the Event button (looks like a marker) in the Animation window timeline
   - Add events at these approximate times:

   | Time (seconds) | Function Name | Purpose |
   |----------------|---------------|---------|
   | 0.0 | `OnStartAttack` | Mark attack as started |
   | 0.3-0.5 | `OnHit` | Actual damage dealing frame |
   | 0.8-1.0 | `OnCompleteAttack` | Mark attack as complete |

3. **Set the Function**:
   - Click on the event marker
   - In the Inspector, set the Function dropdown to the appropriate method name
   - The Object must be the GameObject with the `BubbleEvent` component

## Example Workflow

Here's a complete attack sequence:

1. **Player presses attack button** → `Combat.StartAttack()` called
2. **Animator plays Attack animation** → Animation begins
3. **Frame 1 of animation** → Animation Event calls `OnStartAttack()`
4. **BubbleEvent receives callback** → Invokes `OnBubbleStartAttack` event
5. **Combat.cs receives event** → Sets `IsAttacking = true`
6. **Frame 15 of animation** → Animation Event calls `OnHit()`
7. **BubbleEvent receives callback** → Invokes `OnBubbleHit` event
8. **Combat.cs receives event** → Performs raycast and applies damage
9. **Last frame of animation** → Animation Event calls `OnCompleteAttack()`
10. **BubbleEvent receives callback** → Invokes `OnBubbleCompleteAttack` event
11. **Combat.cs receives event** → Sets `IsAttacking = false`, clears hit targets

## Credits

Character animations provided by Kay Lousberg (KayKit Animated Character asset).