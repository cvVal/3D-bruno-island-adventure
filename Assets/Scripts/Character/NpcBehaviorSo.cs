using UnityEngine;

namespace RPG.Character
{
    /// <summary>
    /// Defines what external functions an NPC's dialogue can call.
    /// Create different ScriptableObject assets for different NPC behaviors.
    /// </summary>
    [
        CreateAssetMenu(
            fileName = "NPC Behavior",
            menuName = "RPG/NPC Behavior",
            order = 0
        )
    ]
    public class NpcBehaviorSo : ScriptableObject
    {
        [Header("External Functions to Bind")] 
        [Tooltip("Bind the VerifyQuest external function for quest NPCs")]
        public bool hasQuestVerification;

        // Future: Add more behavioral flags as needed
        // public bool hasMerchantFunctions = false;
        // public bool hasCompanionFunctions = false;
        // public bool hasMinigameFunctions = false;
        // public bool hasCraftingFunctions = false;
    }
}