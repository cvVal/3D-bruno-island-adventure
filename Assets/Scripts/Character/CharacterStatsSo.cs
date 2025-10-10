using UnityEngine;

namespace RPG.Character
{
    [
        CreateAssetMenu(
            fileName = "Character Stats",
            menuName = "RPG/Character Stats SO",
            order = 0
        )
    ]
    public class CharacterStatsSo : ScriptableObject
    {
        [Header("Basic Stats")]
        public float health = 100f;
        public float damage = 10f;
        public float walkSpeed = 1f;
        public float runSpeed = 1.5f;
        public float attackSpeed = 1f;
        
        [Header("Dash Settings")]
        public float dashDistance = 2.5f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 0.5f;
        
        [Header("Recoil Settings")]
        [Tooltip("How far the character is knocked back when hit")]
        public float recoilDistance = 2f;
        [Tooltip("How long the knockback lasts")]
        public float recoilDuration = 0.3f;
        
        [Header("Invincibility Settings")]
        [Tooltip("How long invincibility lasts after taking damage")]
        public float invincibilityDuration = 1f;
    }
}