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
    }
}