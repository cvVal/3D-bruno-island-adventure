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
        public float health = 100f;
        public float damage = 10f;
        public float walkSpeed = 1f;
        public float runSpeed = 1.5f;
        public float attackSpeed = 1f;
        public float dashDistance = 2.5f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 0.5f;
    }
}